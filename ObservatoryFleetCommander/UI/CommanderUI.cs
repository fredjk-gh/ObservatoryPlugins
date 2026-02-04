using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using com.github.fredjk_gh.PluginCommon.UI;
using Observatory.Framework;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    internal class CommanderUI : Panel
    {
        private readonly CommanderContext _c;

        private readonly TableLayoutPanel _tablePanel;
        private readonly FlowLayoutPanel _flowLayoutPanel;
        private readonly Dictionary<ulong /* carrierId*/, CarrierUI> _uiByCarrierID = [];

        private bool _initialized = false;

        internal CommanderUI(CommanderContext context)
        {
            _c = context;

            AutoScroll = true;
            DoubleBuffered = true;
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            Controls.Add(_tablePanel = new TableLayoutPanel());
            _tablePanel.Dock = DockStyle.Fill;
            _tablePanel.ColumnStyles.Clear();
            _tablePanel.ColumnStyles.Add(new()
            {
                SizeType = SizeType.Percent,
                Width = 100,
            });

            _tablePanel.RowStyles.Clear();
            _tablePanel.RowStyles.Add(new()
            {
                SizeType = SizeType.Percent,
                Height = 100,
            });


            // First row: Controls for each carrier.
            _tablePanel.Controls.Add(_flowLayoutPanel = new FlowLayoutPanel()
            {
                AllowDrop = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
            });

            _flowLayoutPanel.DragEnter += CardPanel_DragEnter;
            _flowLayoutPanel.DragDrop += CardPanel_DragDrop;
        }

        public bool IsReadAll { get => _c.Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch); }

        public void Init()
        {
            if (IsReadAll) return;

            Clear(); // Remove placeholder label or existing ui controls, if present.

            if (_c.Manager.CarrierCount > 0)
            {
                _initialized = true; // Avoid infinite recursion.
                foreach (var c in _c.Manager.Carriers)
                {
                    Add(c.CarrierId);
                }

                // TODO: Restore order from settings.
                foreach (CollapsibleGroupBox e in _flowLayoutPanel.Controls)
                {
                    e.Expanded = ((FleetCommanderSettings)_c.Worker.Settings).UICardsAreDefaultExpanded;
                }
            }
            else
            {
                Label lblNoCarriers = new()
                {
                    Text = "No carriers detected; If this isn't right, run a Read All.",
                    AutoSize = true
                };

                _flowLayoutPanel.Controls.Add(lblNoCarriers);
            }
        }

        public void Repaint()
        {
            if (IsReadAll) return;

            if (!_initialized) Init();
            foreach (var ui in _uiByCarrierID.Values)
            {
                ui.Draw();
            }
        }

        public void UpdateCommanderOnboardState()
        {
            if (IsReadAll) return;

            if (!_initialized) Init();
            foreach (var ui in _uiByCarrierID.Values)
            {
                ui.UpdateCommanderState();
            }
        }

        public void Clear()
        {
            foreach (Control control in _flowLayoutPanel.Controls)
            {
                control.Visible = false;
                control.Dispose();
            }
            _uiByCarrierID.Clear();
            _flowLayoutPanel.Controls.Clear();
            _initialized = false;
        }

        public CarrierUI Get(ulong carrierID)
        {
            if (IsReadAll) return null;
            if (!_initialized) Init();

            return _uiByCarrierID[carrierID];
        }

        public CarrierUI Get(CarrierData carrierData)
        {
            if (IsReadAll) return null;
            if (!_initialized) Init();

            return _uiByCarrierID[carrierData.CarrierId];
        }

        public CarrierUI Add(ulong carrierID)
        {
            if (IsReadAll) return null;

            if (!_initialized)
            {
                Init();
                return Get(carrierID);
            }

            SuspendLayout();


            var data = _c.Manager.GetById(carrierID);
            CarrierUI ui = new(_c, data)
            {
                Margin = new(4),
                BorderStyle = BorderStyle.FixedSingle
            };

            Size uiSize = new(
                Convert.ToInt32(750 * (ui.DeviceDpi / 96.0)),
                Convert.ToInt32(450 * (ui.DeviceDpi / 96.0)));

            _uiByCarrierID.Add(carrierID, ui);

            CollapsibleGroupBox expander = new()
            {
                ContentControl = ui,
                Padding = new(3),
            };
            expander.MouseDown += Card_MouseDown;
            expander.BoxStateChanged += Card_BoxStateChanged;

            _flowLayoutPanel.Controls.Add(expander);
            expander.Width = uiSize.Width;
            expander.Height = uiSize.Height;

            _c.Core.RegisterControl(expander);
            ResumeLayout();

            return ui;
        }

        private void Card_BoxStateChanged(object sender, CollapsibleGroupBox.BoxStateEventArgs e)
        {
            // Save state of card?
            //if (_isLoading) return; // Avoid re-entrant behavior

            //CollapsibleGroupBox cardCtrl = sender as CollapsibleGroupBox;
            //if (cardCtrl == null) return;

            //if (!Enum.TryParse(cardCtrl.ContentControl.Name, out HelmContext.Card cardType)) return;

            //if (e.Expanded)
            //{
            //    _context.Settings.CollapsedCards.RemoveAll(c => c == cardType);
            //}
            //else if (!_context.Settings.CollapsedCards.Contains(cardType))
            //{
            //    _context.Settings.CollapsedCards.Add(cardType);
            //}
            //_context.Core.SaveSettings(_context.Worker);
        }


        private void Card_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (sender is not CollapsibleGroupBox card) return;

                card.DoDragDrop(card, DragDropEffects.Move);
            }
        }

        private void CardPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(typeof(CollapsibleGroupBox)))
            {
                if (e.Data.GetData(typeof(CollapsibleGroupBox)) is not CollapsibleGroupBox card) return;

                Point p = _flowLayoutPanel.PointToClient(new(e.X, e.Y));
                var target = _flowLayoutPanel.GetChildAtPoint(p);
                if (target is not CollapsibleGroupBox)
                {
                    _flowLayoutPanel.Controls.SetChildIndex(card, _flowLayoutPanel.Controls.Count - 1);
                }
                else
                {
                    _flowLayoutPanel.Controls.SetChildIndex(card, _flowLayoutPanel.Controls.GetChildIndex(target, false));
                }
                _flowLayoutPanel.Invalidate();

                // Save order of cards as a setting.
                //SaveCardOrder();
            }
        }

        private void CardPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(typeof(CollapsibleGroupBox)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
