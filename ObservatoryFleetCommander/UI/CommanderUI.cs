using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    internal class CommanderUI : Panel
    {
        private IObservatoryCore _core;
        private Commander _worker;
        private CarrierManager _manager;
        private FlowLayoutPanel _flowLayoutPanel;
        private Dictionary<string /* callsign */, CarrierUI> _uiByCallsign = new();

        private bool _initialized = false;

        public CommanderUI(IObservatoryCore core, Commander worker, CarrierManager manager)
        {
            _core = core;
            _worker = worker;
            _manager = manager;

            AutoScroll = true;
            DoubleBuffered = true;
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            Controls.Add(_flowLayoutPanel = new FlowLayoutPanel());
            _flowLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            _flowLayoutPanel.AutoScroll = true;
        }

        public bool IsReadAll { get => _core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch); }

        public void Init()
        {
            if (IsReadAll) return;

            Clear(); // Remove placeholders label or existing ui controls, if present.
            if (_manager.Count > 0)
            {
                _initialized = true; // Avoid infinite recursion.
                foreach (var c in _manager.Carriers)
                {
                    Add(c.CarrierCallsign);
                }
            }
            else
            {
                var lblNoCarriers = new Label();
                lblNoCarriers.Text = "No carriers detected";

                _flowLayoutPanel.Controls.Add(lblNoCarriers);
            }
        }

        public void Repaint()
        {
            if (IsReadAll) return;

            if (!_initialized) Init();
            foreach (var ui in _uiByCallsign.Values)
            {
                ui.Draw();
            }
        }

        public void Clear()
        {
            foreach (Control control in _flowLayoutPanel.Controls)
            {
                control.Visible = false;
                control.Dispose();
            }
            _uiByCallsign.Clear();
            _flowLayoutPanel.Controls.Clear();
            _initialized = false;
        }

        public CarrierUI Get(string callsign)
        {
            if (IsReadAll) return null;
            if (!_initialized) Init();

            return _uiByCallsign[callsign];
        }

        public CarrierUI Get(CarrierData carrierData)
        {
            if (IsReadAll) return null;
            if (!_initialized) Init();

            return _uiByCallsign[carrierData.CarrierCallsign];
        }

        public CarrierUI Add(string callsign)
        {
            if (IsReadAll) return null;

            if (!_initialized)
            {
                Init();
                return Get(callsign);
            }

            var data = _manager.GetByCallsign(callsign);
            var ui = new CarrierUI(_core, _worker, _manager, data);
            ui.Width = Convert.ToInt32(600 * (ui.DeviceDpi / 96.0));
            ui.Height = Convert.ToInt32(350 * (ui.DeviceDpi / 96.0));
            ui.Margin = new(4);

            _uiByCallsign.Add(callsign, ui);

            var expander = new ExpanderTile(ui, ui.ShortName, _worker.settings.UICardsAreDefaultExpanded);

            _flowLayoutPanel.Controls.Add(expander);

            return ui;
        }
    }
}
