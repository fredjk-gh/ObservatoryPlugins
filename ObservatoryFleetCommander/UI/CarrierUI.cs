using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public partial class CarrierUI : UserControl
    {
        private const string TIMER_JUMP_TITLE = "Jump timer:";
        private const string TIMER_JUMP_COOLDOWN = "Cooldown timer:";

        private const int COOLDOWN_MINUTES = 5;
        private IObservatoryCore _core;
        private Commander _commanderPlugin;
        private CarrierManager _manager;
        private CarrierData _data;

        // Countdown UI info.
        private System.Timers.Timer _countdownTicker;
        private DateTime _departureDateTime;
        private DateTime _cooldownDateTime;

        private string _selectedSystemName = "";

        public CarrierUI(IObservatoryCore core, Commander caller, CarrierManager manager, CarrierData data)
        {
            InitializeComponent();

            _core = core;
            _commanderPlugin = caller;
            _manager = manager;
            _data = data;

            _countdownTicker = new System.Timers.Timer();
            _countdownTicker.Interval = 1000;
            _countdownTicker.Elapsed += Countdown_Tick;

            if (!core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch))
            {
                Draw();
            }
        }

        public string ShortName { get => _data.CarrierName; }

        private bool IsReadAll { get => _core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch); }

        #region Public methods
        public void Draw(string msg = "") // UI thread.
        {
            lblNameAndCallsign.Text = $"{_data.CarrierName} ({_data.CarrierCallsign})";
            lblOwningCommander.Text = $"Owned by: {_data.OwningCommander}";
            lblPositionValue.Text = "(unknown)";
            if (_data.IsPositionKnown) lblPositionValue.Text = _data.Position.BodyName; // Don't use UpdatePosition here.
            UpdateStats();
            UpdateFuel();
            UpdateCommanderState();

            if (_data.LastCarrierJumpRequest != null)
            {
                lblNextJumpValue.Text = _data.LastCarrierJumpRequest.SystemName;
                InitCountdown(_data.LastCarrierJumpRequest);
            }
            else
            {
                JumpCanceled();
            }

            PopulateRoute();

            txtMessages.Text = string.Empty;
            SetMessage(msg);
        }

        public void InitCountdown(CarrierJumpRequest jumpRequest)
        {
            if (IsReadAll || jumpRequest == null || string.IsNullOrWhiteSpace(jumpRequest.DepartureTime)) return;

            _departureDateTime = jumpRequest.DepartureTimeDateTime;
            _cooldownDateTime = _departureDateTime.AddMinutes(COOLDOWN_MINUTES); ;

            var nextSystem = new CarrierPositionData(_data.LastCarrierJumpRequest);
            int estFuelUsage = _data.EstimateFuelForJumpFromCurrentPosition(nextSystem, _core);
            if (estFuelUsage > 0)
            {
                lblNextJumpValue.Text = $"{_data.LastCarrierJumpRequest.SystemName} @ {_departureDateTime.ToShortTimeString()}{Environment.NewLine}Estimated fuel usage: {estFuelUsage} T";
            }
            else
            {
                lblNextJumpValue.Text = $"{_data.LastCarrierJumpRequest.SystemName} @ {_departureDateTime.ToShortTimeString()}";
            }

            UpdateCountdown();
            _countdownTicker.Start();
        }

        public void SetMessage(string message)
        {
            if (IsReadAll) return;

            if (!string.IsNullOrWhiteSpace(message))
                txtMessages.Text = message;
        }

        public void JumpCanceled(string msg = "")
        {
            if (IsReadAll) return;

            _countdownTicker.Stop();
            lblNextJumpValue.Text = "(none scheduled)";
            lblTimerTitle.Visible = false;
            lblTimerValue.Visible = false;

            SetMessage(msg);
        }

        public void JumpScheduled(bool initTimersToo = false)
        {
            if (IsReadAll || _data.LastCarrierJumpRequest == null) return;

            var jumpTargetBody = (!string.IsNullOrEmpty(_data.LastCarrierJumpRequest.Body)) ? _data.LastCarrierJumpRequest.Body : _data.LastCarrierJumpRequest.SystemName;
            double departureTimeMins = _departureDateTime.Subtract(DateTime.Now).TotalMinutes;
            if (initTimersToo) InitCountdown(_data.LastCarrierJumpRequest);
            if (departureTimeMins > 0)
                SetMessage($"Requested a jump to {jumpTargetBody}. Jump in {departureTimeMins:#.0} minutes (@ {_departureDateTime.ToShortTimeString()}).");
        }

        public void UpdatePosition(CarrierPositionData position, string msg = "")
        {
            if (IsReadAll) return;

            lblPositionValue.Text = position.BodyName;

            if (_data.HasRoute)
            {
                var currentSys = position.SystemName;
                var indexOfCurrent = _data.Route.IndexOf(currentSys);

                for (var j = 0; j < _data.Route.Jumps.Count; j++)
                {
                    var jump = _data.Route.Jumps[j];
                    clbRoute.SetItemChecked(j, j <= indexOfCurrent);
                }

                if (_data.Route.IsDestination(position.SystemName))
                {
                    ClearRoute("Your carrier has reached the end of its route.");
                }
            }
            SetMessage(msg);
        }

        public void UpdateFuel(string msg = "")
        {
            if (IsReadAll) return;

            pbFuelLevel.Value = _data.CarrierFuel;

            SetMessage(msg);
        }

        public void UpdateStats(string msg = "")
        {
            if (IsReadAll) return;

            if (_data.LastCarrierStats == null)
            {
                lblBalanceValue.Text = "(unknown)";
                return;
            }

            lblBalanceValue.Text = $"{_data.LastCarrierStats.Finance.CarrierBalance:n0} Cr";

            SetMessage(msg);
        }

        public void UpdateCommanderState()
        {
            if (IsReadAll) return;

            lblCommanderStateValue.Text = _data.CommanderIsDockedOrOnFoot ? "Docked or on-foot" : "Away";
        }

        #endregion

        #region Private methods

        private void UpdateCountdown() // UI Thread
        {
            if (IsReadAll) return;

            if (DateTime.Now.CompareTo(_departureDateTime) < 0)
            {
                if (!lblTimerTitle.Visible) lblTimerTitle.Visible = true;
                if (!lblTimerValue.Visible) lblTimerValue.Visible = true;

                lblTimerTitle.Text = TIMER_JUMP_TITLE;
                lblTimerValue.Text = $"{_departureDateTime.Subtract(DateTime.Now).ToString(@"h\:mm\:ss")}";
            }
            else if (DateTime.Now.CompareTo(_cooldownDateTime) <= 0)
            {
                if (!lblTimerTitle.Visible) lblTimerTitle.Visible = true;
                if (!lblTimerValue.Visible) lblTimerValue.Visible = true;

                lblNextJumpValue.Text = $"(completed)";
                lblTimerTitle.Text = TIMER_JUMP_COOLDOWN;
                lblTimerValue.Text = $"{_cooldownDateTime.Subtract(DateTime.Now).ToString(@"h\:mm\:ss")}";
            }
            else
            {
                JumpCanceled();
                return;
            }
        }

        private void PopulateRoute()
        {
            clbRoute.Items.Clear();
            if (_data.HasRoute)
            {
                var currentSys = _data.Position.SystemName;
                var indexOfCurrent = _data.Route.IndexOf(currentSys);

                for (var j = 0; j < _data.Route.Jumps.Count; j++)
                {
                    var jump = _data.Route.Jumps[j];
                    clbRoute.Items.Add(jump.SystemName, j <= indexOfCurrent);

                    // While we're here:
                    CarrierPositionData.CachePosition(jump.SystemName, jump.Position);
                }
                btnClearRoute.Enabled = true;
                lblLinkToSpansh.Enabled = !string.IsNullOrWhiteSpace(_data.Route.JobID);
            }
            else
            {
                btnClearRoute.Enabled = false;
                lblLinkToSpansh.Enabled = false;
            }
        }

        private void PlotCarrierRoute()
        {
            SpanshCarrierRouterForm dlgSpanshOptions = new(_core, _commanderPlugin, _data.OwningCommander, _manager);

            _core.RegisterControl(dlgSpanshOptions);

            DialogResult result = dlgSpanshOptions.ShowDialog();

            if (result == DialogResult.OK)
            {
                // The result is stored on the CarrierData for the selected Commander (indicated on the "SelectedCommander" property).
                CarrierData data = _manager.GetByCommander(dlgSpanshOptions.SelectedCommander);
                if (data != null)
                {
                    // Freshen the contents of the cache for next run of the application.
                    _commanderPlugin.SerializeDataCache();

                    // Pre-cache the positions of systems in the route.
                    foreach (var jump in data.Route.Jumps)
                    {
                        CarrierPositionData.CachePosition(jump.SystemName, jump.Position);
                    }

                    // Stuff the first waypoint in the route into the clipboard.
                    var firstJump = data.Route.GetNextJump(data.Position?.SystemName ?? "");
                    if (firstJump != null)
                    {
                        Clipboard.SetText(firstJump.SystemName);

                        // Spit out an update to the grid indicating a route is set.
                        SetMessage($"Route plotted via Spansh. First jump system name ({firstJump.SystemName}) is in the clipboard.");
                    }

                    PopulateRoute();
                }
            }
            else if (result == DialogResult.Abort) // Cleared route.
            {
                dlgSpanshOptions.CarrierDataForSelectedCommander.Route = null;
                _commanderPlugin.SerializeDataCache();

                ClearRoute("Route cleared.");
            }

            _core.UnregisterControl(dlgSpanshOptions);
            dlgSpanshOptions.Dispose();
        }

        private void ClearRoute(string msg = "")
        {
            clbRoute.Items.Clear();
            btnClearRoute.Enabled = false;
            lblLinkToSpansh.Enabled = false;

            SetMessage(msg);
        }
        #endregion

        #region Event Handlers

        private void Label_DoubleClickToCopy(object sender, EventArgs e)
        {
            var label = sender as System.Windows.Forms.Label;
            if (label == null || string.IsNullOrWhiteSpace(label.Text)) return;

            Clipboard.SetText(label.Text);
        }

        private void lblLinkToSpansh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!_data.HasRoute || string.IsNullOrWhiteSpace(_data.Route.JobID)) return;

            var url = $"https://spansh.co.uk/fleet-carrier/results/{_data.Route.JobID}";

            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void ctxRouteMenu_SetCurrentPosition_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedSystemName) || !_data.HasRoute) return;

            // Find _selectedSystemName in the route, and copy the necessary info into 
            // Position. Mark jumps up-to and including this system as checked, the rest
            // unchecked.
            var index = _data.Route.IndexOf(_selectedSystemName);
            if (index == -1) return;

            var jumpInfo = _data.Route.Jumps[index];
            CarrierPositionData pos = new(jumpInfo);
            _data.MaybeUpdateLocation(pos);
            lblPositionValue.Text = _selectedSystemName;

            for (int i = 0; i < _data.Route.Jumps.Count; i++)
            {
                clbRoute.SetItemChecked(i, i <= index);
            }
        }

        private void ctxRouteMenu_CopySystemName_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedSystemName)) return;

            Clipboard.SetText(_selectedSystemName);

            SetMessage($"Copied `{_selectedSystemName}` to clipboard.");
        }

        private void clbRoute_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                if (ctxRouteMenu.Visible) ctxRouteMenu.Visible = false;
                return;
            }
            var index = clbRoute.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                _selectedSystemName = clbRoute.Items[index].ToString();
                ctxRouteMenu.Show(Cursor.Position);
                ctxRouteMenu.Visible = true;
            }
            else
            {
                ctxRouteMenu.Visible = false;
            }
        }

        private void btnClearRoute_Click(object sender, EventArgs e)
        {
            _data.Route = null;
            _commanderPlugin.SerializeDataCache();

            ClearRoute("Route cleared.");
        }

        private void btnNewRoute_Click(object sender, EventArgs e)
        {
            PlotCarrierRoute();
        }
        private void Countdown_Tick(object sender, ElapsedEventArgs e)
        {
            UpdateCountdown();
        }

        #endregion
    }
}
