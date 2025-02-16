﻿using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using com.github.fredjk_gh.ObservatoryFleetCommander.UI;
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
using System.Reflection;
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

        private IObservatoryCore _core;
        private Commander _commanderPlugin;
        private CarrierManager _manager;
        private CarrierData _data;
        private CountdownTimerForm _timerForm;
        private string _selectedSystemName = "";

        public CarrierUI(IObservatoryCore core, Commander caller, CarrierManager manager, CarrierData data)
        {
            InitializeComponent();
            btnPopOutTimer.SetIcon(Properties.Resources.OpenInNewIcon.ToBitmap(), new(32, 32));
            btnNewRoute.SetIcon(Properties.Resources.RouteAddIcon.ToBitmap(), new(32, 32));
            btnOpenFromSpansh.SetIcon(Properties.Resources.OpenFromLinkIcon.ToBitmap(), new(32, 32));
            btnClearRoute.SetIcon(Properties.Resources.RouteClearIcon.ToBitmap(), new(32, 32));
            btnOpenSpansh.SetIcon(Properties.Resources.OpenInBrowserIcon.ToBitmap(), new(32, 32));

            _core = core;
            _commanderPlugin = caller;
            _manager = manager;
            _data = data;

            _data.Ticker.Elapsed += Countdown_Tick;

            if (!core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch))
            {
                JumpCanceled();
#if DEBUG
                btnPopOutTimer.Visible = true;
#endif
                Draw();
            }
        }

        public string ShortName { get => _data.CarrierName; }

        private bool IsReadAll { get => _core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch); }

        #region Public methods
        public void Draw(string msg = "") // UI thread.
        {
            txtMessages.Text = string.Empty;
            lblNameAndCallsign.Text = $"{_data.CarrierName} ({_data.CarrierCallsign})";
            lblOwningCommander.Text = $"Owned by: {_data.OwningCommander}";
            DisplayPosition(_data.Position);
            UpdateStats();
            UpdateFuel();
            UpdateCommanderState();

            if (_data.LastCarrierJumpRequest != null)
            {
                lblNextJumpValue.Text = _data.LastCarrierJumpRequest.SystemName;
                InitCountdown();
            }

            PopulateRoute();

            SetMessage(msg);
        }

        public void InitCountdown()
        {
            if (IsReadAll || !(_data.CarrierJumpTimerScheduled || _data.CooldownNotifyScheduled)) return;

            var nextSystem = new CarrierPositionData(_data.LastCarrierJumpRequest);
            int estFuelUsage = _data.EstimateFuelForJumpFromCurrentPosition(nextSystem, _core, _commanderPlugin);
            if (estFuelUsage > 0)
            {
                lblNextJumpValue.Text = $"{_data.LastCarrierJumpRequest.SystemName} @ {_data.DepartureDateTime.ToShortTimeString()}{Environment.NewLine}Estimated fuel usage: {estFuelUsage} T";
            }
            else
            {
                lblNextJumpValue.Text = $"{_data.LastCarrierJumpRequest.SystemName} @ {_data.DepartureDateTime.ToShortTimeString()}";
            }

            UpdateCountdown();
            _data.Ticker.Start();
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

            _data.Ticker.Stop();
            lblNextJumpValue.Text = "(none scheduled)";
            lblTimerTitle.Visible = false;
            lblTimerValue.Visible = false;
            btnPopOutTimer.Visible = false;

            if (_timerForm != null)
            {
                _timerForm.RefreshDisplay();
            }
            SetMessage(msg);
        }

        public void JumpScheduled(bool initTimersToo = false)
        {
            if (IsReadAll || !_data.CarrierJumpTimerScheduled) return;

            var jumpTargetBody = (!string.IsNullOrEmpty(_data.LastCarrierJumpRequest.Body)) ? _data.LastCarrierJumpRequest.Body : _data.LastCarrierJumpRequest.SystemName;
            double departureTimeMins = _data.DepartureDateTime.Subtract(DateTime.Now).TotalMinutes;
            if (initTimersToo) InitCountdown();
            if (departureTimeMins > 0)
            {
                var jumpsRemaining = "";

                if (_data.HasRoute && _data.IsPositionKnown)
                {
                    var index = _data.Route.IndexOf(_data.Position.SystemName);
                    if (index >= 0)
                        jumpsRemaining = $"{Environment.NewLine}{_data.Route.Jumps.Count - index - 1} jumps remaining in route.";
                }

                SetMessage($"Requested a jump to {jumpTargetBody}. Jump in {departureTimeMins:#.0} minutes (@ {_data.DepartureDateTime.ToShortTimeString()}).{jumpsRemaining}");
            }
        }

        public void UpdatePosition(CarrierPositionData position, string msg = "")
        {
            if (IsReadAll) return;

            DisplayPosition(position);

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

        public void DisplayPosition(CarrierPositionData position)
        {
            if (position == null || string.IsNullOrWhiteSpace(position.SystemName))
            {
                lblPositionValue.Text = "(unknown)";
                ttipCarrierUI.SetToolTip(lblPositionValue, "");
                return;
            }

            lblPositionValue.Text = position.BodyName;
            if (_data.HasRoute)
            {
                var indexOfCurrent = _data.Route.IndexOf(position.SystemName);
                if (indexOfCurrent >= 0 && indexOfCurrent < _data.Route.Jumps.Count)
                {
                    ttipCarrierUI.SetToolTip(lblPositionValue, $"{_data.Route.Jumps.Count - indexOfCurrent - 1} jumps remaining");
                }
            }
        }

        public void UpdateFuel(string msg = "")
        {
            if (IsReadAll) return;

            pbFuelLevel.Value = Math.Clamp(_data.CarrierFuel, pbFuelLevel.Minimum, pbFuelLevel.Maximum);
            ttipCarrierUI.SetToolTip(pbFuelLevel, $"{_data.CarrierFuel} T / {pbFuelLevel.Maximum - _data.CarrierFuel} T to refill");

            SetMessage(msg);
        }

        public void UpdateStats(string msg = "")
        {
            if (IsReadAll) return;

            var asOfDate = _data.StatsAsOfDate;

            if (_data.CarrierBalance == 0)
            {
                lblBalanceValue.Text = "(unknown)";
            }
            else
            {
                var upkeepWeeks = Math.Floor(_data.CarrierBalance / 34950000.0);
                var upkeepUntilDate = asOfDate.AddDays(upkeepWeeks * 7);
                if (upkeepUntilDate.Subtract(DateTime.Now).Days <= 30)
                {
                    msg = msg + Environment.NewLine + "WARNING: Carrier may have low funds remaining! Add funds to the carrier budget to avoid it being decommissioned.";
                }
                lblBalanceValue.Text = $"{_data.CarrierBalance:n0} Cr";
                ttipCarrierUI.SetToolTip(
                    lblBalanceValue,
                    $"Balance as of {asOfDate}; upkeep until {upkeepUntilDate.ToString("MMM yyyy")}");
            }

            if (_data.CapacityUsed == 0)
            {
                lblCapacityValue.Text = "(unknown)";
            }
            else
            {
                lblCapacityValue.Text = $"{_data.CapacityUsed:#,###} T";
                ttipCarrierUI.SetToolTip(lblCapacityValue, $"Capacity as of {asOfDate}");
            }

            SetMessage(msg);
        }

        public void UpdateCommanderState()
        {
            if (IsReadAll) return;

            lblCommanderStateValue.Text = _data.CommanderIsDockedOrOnFoot ? "Aboard" : "Away";
        }

        #endregion

        #region Private methods

        private void UpdateCountdown() // UI Thread
        {
            if (IsReadAll || !(_data.CarrierJumpTimerScheduled || _data.CooldownNotifyScheduled)) return;

            if (_data.CarrierJumpTimerScheduled && DateTime.Now.CompareTo(_data.DepartureDateTime) < 0)
            {
                if (!lblTimerTitle.Visible) lblTimerTitle.Visible = true;
                if (!lblTimerValue.Visible) lblTimerValue.Visible = true;
                if (!btnPopOutTimer.Visible) btnPopOutTimer.Visible = true;

                lblTimerTitle.Text = TIMER_JUMP_TITLE;
                lblTimerValue.Text = $"{_data.DepartureDateTime.Subtract(DateTime.Now).ToString(@"h\:mm\:ss")}";
            }
            else if (_data.CooldownNotifyScheduled && DateTime.Now.CompareTo(_data.CooldownDateTime) <= 0)
            {
                if (!lblTimerTitle.Visible) lblTimerTitle.Visible = true;
                if (!lblTimerValue.Visible) lblTimerValue.Visible = true;
                if (!btnPopOutTimer.Visible) btnPopOutTimer.Visible = true;

                lblNextJumpValue.Text = $"(completed)";
                lblTimerTitle.Text = TIMER_JUMP_COOLDOWN;
                lblTimerValue.Text = $"{_data.CooldownDateTime.Subtract(DateTime.Now).ToString(@"h\:mm\:ss")}";
            }
            else
            {
                JumpCanceled();
            }
        }

        private void PopulateRoute()
        {
            clbRoute.Items.Clear();
            if (_data.HasRoute)
            {
                int indexOfCurrent = -1;
                if (_data.IsPositionKnown)
                {
                    var currentSys = _data.Position.SystemName;
                    indexOfCurrent = _data.Route.IndexOf(currentSys);
                }

                for (var j = 0; j < _data.Route.Jumps.Count; j++)
                {
                    var jump = _data.Route.Jumps[j];
                    clbRoute.Items.Add(jump.SystemName, j <= indexOfCurrent);

                    // While we're here:
                    CarrierPositionData.CachePosition(jump.SystemName, jump.Position);
                }
                btnClearRoute.Enabled = true;
                btnOpenSpansh.Enabled = !string.IsNullOrWhiteSpace(_data.Route.JobID);
            }
            else
            {
                btnClearRoute.Enabled = false;
                btnOpenSpansh.Enabled = false;
            }
        }

        private void CreateCarrierRoute(Form theUI, string purpose)
        {
            try
            {
                ICarrierRouteCreator creatorForm = theUI as ICarrierRouteCreator;
                if (creatorForm == null)
                {
                    theUI.Dispose();
                    return;
                }

                _core.RegisterControl(theUI);

                DialogResult result = theUI.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // The result is stored on the CarrierData for the selected Commander (indicated on the "SelectedCommander" property).
                    CarrierData data = _manager.GetByCommander(creatorForm.SelectedCommander);
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
                            SetMessage($"Route {purpose} via Spansh. Next jump system name ({firstJump.SystemName}) is in the clipboard.");
                        }

                        PopulateRoute();
                    }
                }
                else if (result == DialogResult.Abort) // Cleared route.
                {
                    creatorForm.CarrierDataForSelectedCommander.Route = null;
                    _commanderPlugin.SerializeDataCache();

                    ClearRoute("Route cleared.");
                }

                _core.UnregisterControl(theUI);
                theUI.Dispose();
            }
            catch (Exception ex)
            {
                _core.GetPluginErrorLogger(_commanderPlugin).Invoke(ex, $"CreateCarrierRoute-{purpose}");
            }
        }

        private void ClearRoute(string msg = "")
        {
            clbRoute.Items.Clear();
            btnClearRoute.Enabled = false;
            btnOpenSpansh.Enabled = false;

            SetMessage(msg);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            // Custom: If countdown timer window is open, close it and clean up.
            if (_timerForm != null && _timerForm.Visible)
            {
                var theTimerForm = _timerForm; // _timerForm can get set to null between now and the .Dispose() call.
                theTimerForm.Close();
                @theTimerForm.Dispose();
                _timerForm = null;
            }
        }

        private void SaveCountdownWindowSizeAndPosition()
        {
            if (_timerForm == null) return;

            _commanderPlugin.settings.CountdownWindowX = _timerForm.Location.X;
            _commanderPlugin.settings.CountdownWindowY = _timerForm.Location.Y;

            _commanderPlugin.settings.CountdownWindowWidth = _timerForm.Width;
            _commanderPlugin.settings.CountdownWindowHeight = _timerForm.Height;

            _core.SaveSettings(_commanderPlugin);
        }
        #endregion

        #region Event Handlers

        private void Label_DoubleClickToCopy(object sender, EventArgs e)
        {
            var label = sender as System.Windows.Forms.Label;
            if (label == null || string.IsNullOrWhiteSpace(label.Text)) return;

            Clipboard.SetText(label.Text);
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
            DisplayPosition(pos);
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
                clbRoute.SelectedIndex = index;
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
            SpanshCarrierRouterForm dlgSpanshCreate = new(_core, _commanderPlugin, _data.OwningCommander, _manager);

            CreateCarrierRoute(dlgSpanshCreate, "plotted");
        }

        private void btnOpenFromSpansh_Click(object sender, EventArgs e)
        {
            SpanshImportCarrierRouteForm dlgSpanshImport = new(_core, _commanderPlugin, _data.OwningCommander, _manager);

            CreateCarrierRoute(dlgSpanshImport, "imported");
        }

        private void Countdown_Tick(object sender, ElapsedEventArgs e)
        {
            _core.ExecuteOnUIThread(() =>
            {
                UpdateCountdown();
            });
        }

        #endregion

        private void btnPopOutTimer_Click(object sender, EventArgs e)
        {
            if (_timerForm != null) return;

            _timerForm = new CountdownTimerForm(_core, _data);
            _core.RegisterControl(_timerForm);

            if (_commanderPlugin.settings.CountdownWindowWidth > 0 && _commanderPlugin.settings.CountdownWindowHeight > 0)
            {
                _timerForm.Width = _commanderPlugin.settings.CountdownWindowWidth;
                _timerForm.Height = _commanderPlugin.settings.CountdownWindowHeight;
            }
            _timerForm.Show();
            if (_commanderPlugin.settings.CountdownWindowX != 0 && _commanderPlugin.settings.CountdownWindowY != 0) // "default", use start-up position
            {
                // TODO: Ensure the form is in bound?
                var location = new Point(_commanderPlugin.settings.CountdownWindowX, _commanderPlugin.settings.CountdownWindowY);

                _timerForm.Location = location;
            }

            _timerForm.FormClosed += TimerFormClosed;
            _timerForm.Move += TimerFormMove;
            _timerForm.ResizeEnd += TimerFormResized;
        }

        private void TimerFormResized(object sender, EventArgs e)
        {
            SaveCountdownWindowSizeAndPosition();
        }

        private void TimerFormMove(object sender, EventArgs e)
        {
            SaveCountdownWindowSizeAndPosition();
        }

        private void TimerFormClosed(object sender, FormClosedEventArgs e)
        {
            _core.UnregisterControl(_timerForm);
            _timerForm = null;
        }

        private void btnOpenSpansh_Click(object sender, EventArgs e)
        {
            if (!_data.HasRoute || string.IsNullOrWhiteSpace(_data.Route.JobID)) return;

            var url = $"https://spansh.co.uk/fleet-carrier/results/{_data.Route.JobID}";

            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

    }
}
