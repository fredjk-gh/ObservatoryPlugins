using System.ComponentModel;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.ObservatoryHelm.UI.Cards;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using Observatory.Framework.Files;

namespace com.github.fredjk_gh.ObservatoryHelm.UI
{
    public partial class HelmUI : UserControl
    {
        private HelmContext _c;
        private Dictionary<HelmContext.Card, CollapsibleGroupBox> _cards = new();
        private int _cardWidth;
        private float _originalFontSize;
        private bool _isLoading = true;

        internal HelmUI(HelmContext context)
        {
            _c = context;
            InitializeComponent();
            DoubleBuffered = true;
            _originalFontSize = this.Font.Size;

            flpCardPanel.DragEnter += CardPanel_DragEnter;
            flpCardPanel.DragDrop += CardPanel_DragDrop;

            ForeColorChanged += Helm_UpdateForeColor;

            SuspendLayout(); // Won't be re-triggered until _isLoading is false or manually done.

            // TODO: Refactor so individual cards can be created/removed at runtime.
            List<CollapsibleGroupBoxContent> contentControls = new();
            contentControls.Add(new CommanderContent(_c) { Name = HelmContext.Card.Commander.ToString() });
            if (!_c.Settings.ShipPanelHidden)
                contentControls.Add(new ShipContent(_c) { Name = HelmContext.Card.Ship.ToString() });
            if (!_c.Settings.RoutePanelHidden)
                contentControls.Add(new RouteContent(_c) { Name = HelmContext.Card.Route.ToString() });
            if (!_c.Settings.SystemPanelHidden)
                contentControls.Add(new SystemContent(_c) { Name = HelmContext.Card.System.ToString() });
            if (!_c.Settings.BodyPanelHidden)
                contentControls.Add(new PlanetContent(_c) { Name = HelmContext.Card.Body.ToString() });
            //if (!_c.Settings.StationPanelHidden)
            //    contentControls.Add(new StationContent(_c) { Name = HelmContext.Card.Station.ToString() });
            if (!_c.Settings.CargoPanelHidden)
                contentControls.Add(new CargoContent(_c) { Name = HelmContext.Card.Cargo.ToString() });
            if (!_c.Settings.ProspectorPanelHidden)
                contentControls.Add(new ProspectorContent(_c) { Name = HelmContext.Card.Prospector.ToString() });
            if (!_c.Settings.MessagesPanelHidden)
                contentControls.Add(new MessagesContent(_c) { Name = HelmContext.Card.Messages.ToString() });

            int maxCardWidth = -1;
            foreach (var c in contentControls)
            {
                maxCardWidth = Math.Max(maxCardWidth, c.Width);
                CollapsibleGroupBox card = new()
                {
                    ContentControl = c,
                    Expanded = !(c is CommanderContent),
                    Padding = new(3),
                    Width = c.Width,
                };
                AddCard(card);
            }

            SetCardWidthAndState(maxCardWidth);
            SetCardOrder();
            UpdateForeColor();
            _c.Settings.PropertyChanged += Settings_PropertyChanged;
            ResumeLayout();
            _isLoading = false;
        }

        public void Draw()
        {
            foreach (var c in _cards.Values)
            {
                c.Draw();
            }
        }

        public void Clear()
        {
            _c.UIMgr.Clear();
            foreach (var c in _cards.Values)
            {
                c.Clear();
            }
        }

        public void ChangeCommander(CommanderKey key)
        {
            _c.UIMgr.Realtime.CommanderKey = key;
        }

        public void ChangeShip(UInt64 shipId)
        {
            _c.UIMgr.Realtime.ShipId= shipId;
        }

        public void ChangeFuelLevel(double newFuelLevel)
        {
            _c.UIMgr.Realtime.FuelRemaining = newFuelLevel;
        }

        public void ChangeJetConeBoost(double newJetConeBoost)
        {
            _c.UIMgr.Realtime.JetConeBoost = newJetConeBoost;
        }

        public void ChangeCargo(int cargo)
        {
            _c.UIMgr.Realtime.Cargo = cargo;
        }

        public void ChangeSystem(ulong systemAddress)
        {
            _c.UIMgr.Realtime.SystemId64 = systemAddress;
        }

        public void ChangeRoute(NavRouteFile route)
        {
            if (route != null)
            {
                _c.UIMgr.Realtime.JumpsRemaining = route.Route.Count() - 1; // Includes origin.
            }
            else
            {
                _c.UIMgr.Realtime.JumpsRemaining = 0;
            }
        }

        public void ChangeBody(int bodyId)
        {
            _c.UIMgr.Realtime.BodyId = bodyId;
        }

        public void ChangeAllBodiesFound(bool found)
        {
            _c.UIMgr.Realtime.AllBodiesFound = found;
        }

        public void MaybeAdjustFontSize()
        {
            AdjustFont();
        }

        public void AddCard(CollapsibleGroupBox card)
        {
            _c.Core.RegisterControl(card);
            if (!Enum.TryParse(card.ContentControl.Name, out HelmContext.Card cardType)) return;

            _cards.Add(cardType, card);

            if (!_isLoading) SuspendLayout();
            flpCardPanel.Controls.Add(card);
            if (!_isLoading) ResumeLayout();

            // Do this after adding the controls to avoid re-entrance when applying settings.
            card.MouseDown += Card_MouseDown;
            card.BoxStateChanged += Card_BoxStateChanged;
        }

        internal void SetCardWidthAndState(int newWidth)
        {
            if (_cardWidth == newWidth) return; // nothing to do!?

            _cardWidth = Math.Clamp(newWidth, 400, 1500);

            if (!_isLoading) SuspendLayout();
            foreach (var card in _cards.Values)
            {
                card.Width = _cardWidth;

                if (Enum.TryParse(card.ContentControl.Name, out HelmContext.Card cardType))
                {
                    card.Expanded = !_c.Settings.CollapsedCards.Contains(cardType);
                }
                card.Invalidate();
            }
            if (!_isLoading) ResumeLayout();
        }

        internal void SetCardOrder()
        {
            if (_c.Settings.CardOrdering == null || _c.Settings.CardOrdering.Count == 0) return;

            for (int i = 0; i < _c.Settings.CardOrdering.Count; i++)
            {
                if (i >= flpCardPanel.Controls.Count) break;

                HelmContext.Card type = _c.Settings.CardOrdering[i];
                if (!_cards.ContainsKey(type)) continue;
                var card = _cards[type];
                flpCardPanel.Controls.SetChildIndex(card, i);
            }
            flpCardPanel.Invalidate();
        }

        private void SaveCardOrder()
        {
            List<HelmContext.Card> order = new();

            foreach (var ctrl in flpCardPanel.Controls)
            {
                HelmContext.Card cardType;
                CollapsibleGroupBox cardCtrl = ctrl as CollapsibleGroupBox;

                if (cardCtrl == null) continue;
                if (!Enum.TryParse<HelmContext.Card>(cardCtrl.ContentControl.Name, out cardType)) continue;

                order.Add(cardType);
            }

            _c.Settings.CardOrdering = order;
            _c.Core.SaveSettings(_c.Worker);
        }

        private void AdjustFont()
        {
            float adjustment = (float)_c.Settings.FontSizeAdjustment;
            if (adjustment == 0.0f) return;

            // Always use _originalFontSize otherwise your font size always increases or always decreases.
            float newFontSize = Math.Clamp(_originalFontSize + adjustment, 2.0f, 32.0f);
            SuspendLayout();
            this.Font = new Font(this.Font.FontFamily, newFontSize);
            if (_cards.Count > 0 && _cards[0].Width != _cardWidth)
            {
                SetCardWidthAndState(_cards[0].Width);
            }
            ResumeLayout();
        }

        private void UpdateWrapping()
        {
            SuspendLayout();
            flpCardPanel.WrapContents = !flpCardPanel.WrapContents;
            flpCardPanel.AutoScroll = false; // force re-layout
            ResumeLayout();
            flpCardPanel.AutoScroll = true;
            UpdateWrapContentImage();
        }

        private void UpdateForeColor()
        {
            tsbAbout.Image = ImageCommon.RecolorAndSizeImage(PluginCommon.Images.InfoBubbleImage, ForeColor);
            tsbFontShrink.Image = ImageCommon.RecolorAndSizeImage(PluginCommon.Images.TextSizeSmallerImage, ForeColor);
            tsbFontEnlarge.Image = ImageCommon.RecolorAndSizeImage(PluginCommon.Images.TextSizeLargerImage, ForeColor);
            tsbSettings.Image = ImageCommon.RecolorAndSizeImage(PluginCommon.Images.SettingsCogImage, ForeColor);
            UpdateWrapContentImage();
        }

        private void UpdateWrapContentImage()
        {
            if (flpCardPanel.WrapContents)
                tsbWrap.Image = ImageCommon.RecolorAndSizeImage(PluginCommon.Images.WrapOffImage, ForeColor);
            else
                tsbWrap.Image = ImageCommon.RecolorAndSizeImage(PluginCommon.Images.WrapImage, ForeColor);
        }

        private void Card_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var card = sender as CollapsibleGroupBox;
                if (card == null) return;

                var effect = card.DoDragDrop(card, DragDropEffects.Move);
            }
        }

        private void CardPanel_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(typeof(CollapsibleGroupBox)))
            {
                var card = e.Data.GetData(typeof(CollapsibleGroupBox)) as CollapsibleGroupBox;
                if (card == null) return;

                Point p = flpCardPanel.PointToClient(new Point(e.X, e.Y));
                var target = flpCardPanel.GetChildAtPoint(p);
                var targetCard = target as CollapsibleGroupBox;
                if (targetCard == null)
                {
                    flpCardPanel.Controls.SetChildIndex(card, flpCardPanel.Controls.Count - 1);
                }
                else
                {
                    flpCardPanel.Controls.SetChildIndex(card, flpCardPanel.Controls.GetChildIndex(target, false));
                }
                flpCardPanel.Invalidate();

                // Save order of cards as a setting.
                SaveCardOrder();
            }
        }

        private void CardPanel_DragEnter(object? sender, DragEventArgs e)
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

        private void Card_BoxStateChanged(object sender, CollapsibleGroupBox.BoxStateEventArgs e)
        {
            if (_isLoading) return; // Avoid re-entrant behavior

            CollapsibleGroupBox cardCtrl = sender as CollapsibleGroupBox;
            if (cardCtrl == null) return;

            if (!Enum.TryParse(cardCtrl.ContentControl.Name, out HelmContext.Card cardType)) return;

            if (e.Expanded)
            {
                _c.Settings.CollapsedCards.RemoveAll(c => c == cardType);
            }
            else if (!_c.Settings.CollapsedCards.Contains(cardType))
            {
                _c.Settings.CollapsedCards.Add(cardType);
            }
            _c.Core.SaveSettings(_c.Worker);
        }

        private void Helm_UpdateForeColor(object sender, EventArgs e)
        {
            if (sender != this) return;
            UpdateForeColor();
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FontSizeAdjustment":
                    AdjustFont();
                    break;
                case "EnableWrappedLayout":
                    UpdateWrapping();
                    break;
            }
        }

        private void tsbFontShrink_Click(object sender, EventArgs e)
        {
            _c.Settings.FontSizeAdjustment -= 1;
            _c.Core.SaveSettings(_c.Worker);
            // The PropertyChanged event handles refreshing the UI.
        }

        private void tsbFontEnlarge_Click(object sender, EventArgs e)
        {
            _c.Settings.FontSizeAdjustment += 1;
            _c.Core.SaveSettings(_c.Worker);
            // The PropertyChanged event handles refreshing the UI.
        }

        private void tsbWrap_Click(object sender, EventArgs e)
        {
            _c.Settings.EnableWrappedLayout = !_c.Settings.EnableWrappedLayout;
            _c.Core.SaveSettings(_c.Worker);
            // The PropertyChanged event handles updating the button and refreshing the UI.
        }

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            _c.Core.OpenAbout(_c.Worker);
        }

        private void tsbSettings_Click(object sender, EventArgs e)
        {
            _c.Core.OpenSettings(_c.Worker);
        }
    }
}
