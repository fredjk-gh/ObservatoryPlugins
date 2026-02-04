using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using com.github.fredjk_gh.ObservatoryFleetCommander.UI;
using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages.Data;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework.Files.ParameterTypes;
using System.Data;
using System.Timers;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public partial class CarrierUI : CollapsibleGroupBoxContent
    {
        private const string TIMER_JUMP_TITLE = "Jump timer:";
        private const string TIMER_JUMP_COOLDOWN = "Cooldown timer:";

        private readonly CommanderContext _c;
        private readonly CarrierData _data;
        private CountdownTimerForm _timerForm;
        private string _selectedSystemName = "";

        internal CarrierUI(CommanderContext context, CarrierData data)
        {
            InitializeComponent();
            DoubleBuffered = true;

            SuspendLayout();
            btnPopOutTimer.SetOriginalImage(Images.OpenInNewImage);
            btnNewRoute.SetOriginalImage(Images.RouteAddImage);
            btnClearRoute.SetOriginalImage(Images.RouteClearImage);
            btnOpenInventory.SetOriginalImage(Images.CommoditiesImage);

            _c = context;
            _data = data;

            _data.Ticker.Elapsed += Countdown_Tick;

            InternalDraw();
            JumpCanceled();
#if DEBUG
            btnPopOutTimer.Visible = true;
#endif

            ResumeLayout();
        }

        private bool IsReadAll { get => _c.IsReadAll; }
        internal InventoryForm InventoryForm { get; private set; }

        #region Public methods
        public override void Draw()
        {
            _c.Core.ExecuteOnUIThread(() =>
            {
                InternalDraw();
            });
        }

        public void Draw(string msg)
        {
            _c.Core.ExecuteOnUIThread(() =>
            {
                InternalDraw(msg);
            });
        }

        public void InitCountdown()
        {
            if (IsReadAll || !(_data.CarrierJumpTimerScheduled || _data.CooldownNotifyScheduled)) return;

            if (_data.LastCarrierJumpRequest != null)
            {
                var nextSystem = new CarrierPositionData(_data.LastCarrierJumpRequest);
                var departureTime = UIFormatter.DateTimeAsShortTime(_data.DepartureDateTime, _c.Settings.UIDateTimesUseInGameTime);

                // TODO: Spawn this off in its own task that can block on fetching data externally.
                int estFuelUsage = _data.EstimateFuelForJumpFromCurrentPosition(_c, nextSystem);
                var usageStr = string.Empty;
                if (estFuelUsage > 0)
                {
                    usageStr = $"{Environment.NewLine}Estimated fuel usage: {UIFormatter.Tonnage(estFuelUsage)}";
                }
                lblNextJumpValue.Text = $"{_data.LastCarrierJumpRequest.SystemName} @ {departureTime}{usageStr}";
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

            lblNextJumpValue.Text = "(none scheduled)";
            if (_data.LastCarrierJumpCancelled == null || !_data.CooldownNotifyScheduled)
            {
                _data.Ticker.Stop();
                lblTimerTitle.Visible = false;
                lblTimerValue.Visible = false;
                btnPopOutTimer.Visible = false;
            }

            _timerForm?.RefreshDisplay();
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

                string departureTime = _data.DepartureDateTime.ToShortTimeString();
                if (_c.Settings.UIDateTimesUseInGameTime)
                {
                    departureTime = _data.DepartureDateTime.ToUniversalTime().ToShortTimeString();
                }
                SetMessage($"Requested a jump to {jumpTargetBody}. Jump in {UIFormatter.Minutes(departureTimeMins)} (@ {departureTime}).{jumpsRemaining}");
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
            ttipCarrierUI.SetToolTip(pbFuelLevel, $"{UIFormatter.Tonnage(_data.CarrierFuel)} / {UIFormatter.Tonnage(pbFuelLevel.Maximum - _data.CarrierFuel)} to refill");

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
                lblBalanceValue.Text = UIFormatter.Credits(_data.CarrierBalance);
                ttipCarrierUI.SetToolTip(
                    lblBalanceValue,
                    $"Balance as of {asOfDate}; upkeep until {UIFormatter.DateMMMyyyy(upkeepUntilDate)}");
            }

            if (_data.LastCarrierStats is null)
            {
                lblUsedCapacityValue.Text = "(unknown)";
            }
            else
            {
                lblUsedCapacityValue.Text = UIFormatter.Tonnage(_data.CarrierCapacity - _data.LastCarrierStats.SpaceUsage.FreeSpace);
                ttipCarrierUI.SetToolTip(lblUsedCapacityValue, $"Capacity as of {asOfDate}");
            }

            if (_data.DistanceTravelledLy == 0)
            {
                lblDistanceTravelledValue.Text = "(unknown)";
                ttipCarrierUI.SetToolTip(lblDistanceTravelledValue, "");
            }
            else
            {
                lblDistanceTravelledValue.Text = UIFormatter.DistanceLy(_data.DistanceTravelledLy);
                ttipCarrierUI.SetToolTip(lblDistanceTravelledValue, $"{_data.TotalJumps} jumps");
            }

            SetMessage(msg);
        }

        public void UpdateCommanderState()
        {
            if (IsReadAll) return;

            if (_data.CommandersOnBoard.Count > 0)
            {
                lblCommanderStateValue.Text = string.Join(", ", _data.CommandersOnBoard);
            }
            else
            {
                lblCommanderStateValue.Text = "(none)";
            }
        }

        #endregion

        #region Private methods

        internal void InternalDraw(string msg = "") // UI thread.
        {
            txtMessages.Text = string.Empty;
            lblNameAndCallsign.Text = $"{_data.CarrierName} ({_data.CarrierCallsign})";
            ContentTitle = _data.CarrierName;

            if (_data.CarrierType == CarrierType.SquadronCarrier)
            {
                if (_data.Owner == Manager.SQUADRON_CARRIER_OWNER)
                    lblOwningCommander.Text = Manager.SQUADRON_CARRIER_OWNER;
                else
                    lblOwningCommander.Text = $"Squadron: {_data.Owner}";
            }
            else
            {
                lblOwningCommander.Text = $"Owner: {_data.Owner}";
            }
            ttipCarrierUI.SetToolTip(lblOwningCommander, _data.Owner);
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

        private void UpdateCountdown() // UI Thread
        {
            if (IsReadAll || !(_data.CarrierJumpTimerScheduled || _data.CooldownNotifyScheduled)) return;

            if (_data.CarrierJumpTimerScheduled && DateTime.Now.CompareTo(_data.DepartureDateTime) < 0)
            {
                if (!lblTimerTitle.Visible) lblTimerTitle.Visible = true;
                if (!lblTimerValue.Visible) lblTimerValue.Visible = true;
                if (!btnPopOutTimer.Visible) btnPopOutTimer.Visible = true;

                lblTimerTitle.Text = TIMER_JUMP_TITLE;
                lblTimerValue.Text = UIFormatter.Timehmmss(_data.DepartureDateTime.Subtract(DateTime.Now));
            }
            else if (_data.CooldownNotifyScheduled && DateTime.Now.CompareTo(_data.CooldownDateTime) <= 0)
            {
                if (!lblTimerTitle.Visible) lblTimerTitle.Visible = true;
                if (!lblTimerValue.Visible) lblTimerValue.Visible = true;
                if (!btnPopOutTimer.Visible) btnPopOutTimer.Visible = true;

                if (_data.LastCarrierJumpRequest != null)
                    lblNextJumpValue.Text = $"(completed)";
                else
                    lblNextJumpTitle.Text = $"(cancelled)";
                lblTimerTitle.Text = TIMER_JUMP_COOLDOWN;
                lblTimerValue.Text = UIFormatter.Timehmmss(_data.CooldownDateTime.Subtract(DateTime.Now));
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

                    MaybeCacheViaArchivist(_data);

                }
                btnClearRoute.Enabled = true;
            }
            else
            {
                btnClearRoute.Enabled = false;
            }
        }

        private void CreateCarrierRoute(Form theUI, string purpose)
        {
            try
            {
                if (theUI is not ICarrierRouteCreator creatorForm)
                {
                    theUI.Dispose();
                    return;
                }

                _c.Core.RegisterControl(theUI);

                DialogResult result = theUI.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // The result is stored on the CarrierData for the selected CarrierId (indicated on the "SelectedCarrierId" property).
                    CarrierData data = creatorForm.CarrierDataForSelectedId;
                    if (data != null)
                    {
                        // Freshen the contents of the cache for next run of the application.
                        _c.SerializeDataCacheV2();

                        // Pre-cache the positions of systems in the route.
                        int tritiumUsage = 0;
                        foreach (var jump in data.Route.Jumps)
                        {
                            CarrierPositionData.CachePosition(jump.SystemName, jump.Position);
                            tritiumUsage += jump.FuelUsed;
                        }

                        MaybeCacheViaArchivist(data);

                        // Stuff the first waypoint in the route into the clipboard.
                        var firstJump = data.Route.GetNextJump(data.Position?.SystemName ?? "");
                        if (firstJump is not null)
                        {
                            Misc.SetTextToClipboard(firstJump.SystemName);

                            // Spit out an update to the grid indicating a route is set.
                            SetMessage($"Route {purpose} via Spansh. Next jump system name ({firstJump.SystemName}) is in the clipboard. Estimated tritium required is {tritiumUsage} T.");
                        }

                        PopulateRoute();
                    }
                }
                else if (result == DialogResult.Abort) // Cleared route.
                {
                    creatorForm.CarrierDataForSelectedId.Route = null;
                    _c.SerializeDataCacheV2();

                    ClearRoute("Route cleared.");
                }

                _c.Core.UnregisterControl(theUI);
                theUI.Dispose();
            }
            catch (Exception ex)
            {
                _c.Core.GetPluginErrorLogger(_c.Worker).Invoke(ex, $"CreateCarrierRoute-{purpose}");
            }
        }

        private void MaybeCacheViaArchivist(CarrierData data)
        {
            if (!_c.PluginTracker.IsActive(PluginType.fredjk_Archivist)) return;

            // We're running Archivist: Batch cache these addresses.
            List<ArchivistPositionCacheItem> toCache =
                [.. data.Route.Jumps.Select(j => ArchivistPositionCacheItem.New(j.SystemName, j.Id64, j.Position))];

            var msg = ArchivistPositionCacheBatch.NewAddToCache(toCache);

            _c.Dispatcher.SendMessage(msg, PluginType.fredjk_Archivist);
        }

        private void ClearRoute(string msg = "")
        {
            clbRoute.Items.Clear();
            btnClearRoute.Enabled = false;

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

            _c.Settings.CountdownWindowX = _timerForm.Location.X;
            _c.Settings.CountdownWindowY = _timerForm.Location.Y;

            _c.Settings.CountdownWindowWidth = _timerForm.Width;
            _c.Settings.CountdownWindowHeight = _timerForm.Height;

            _c.Core.SaveSettings(_c.Worker);
        }
        #endregion

        #region Event Handlers

        private void Label_DoubleClickToCopy(object sender, EventArgs e)
        {
            if (sender is not Label label || string.IsNullOrWhiteSpace(label.Text)) return;

            Misc.SetTextToClipboard(label.Text);

        }

        private void CtxRouteMenu_SetCurrentPosition_Click(object sender, EventArgs e)
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

        private void CtxRouteMenu_CopySystemName_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedSystemName)) return;

            Misc.SetTextToClipboard(_selectedSystemName);

            SetMessage($"Copied `{_selectedSystemName}` to clipboard.");
        }

        private void ClbRoute_MouseDown(object sender, MouseEventArgs e)
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

        private void BtnClearRoute_Click(object sender, EventArgs e)
        {
            _data.Route = null;
            _c.SerializeDataCacheV2();

            ClearRoute("Route cleared.");
        }

        private void BtnNewRoute_Click(object sender, EventArgs e)
        {
            SpanshCarrierRouterForm dlgSpanshCreate = new(_c, _data.Owner);

            CreateCarrierRoute(dlgSpanshCreate, "plotted");
        }

        private void Countdown_Tick(object sender, ElapsedEventArgs e)
        {
            _c.Core.ExecuteOnUIThread(() =>
            {
                UpdateCountdown();
            });
        }

        private void BtnPopOutTimer_Click(object sender, EventArgs e)
        {
            if (_timerForm != null)
            {
                _timerForm.Close();
                return;
            }

            _timerForm = new CountdownTimerForm(_c.Core, _data);
            _c.Core.RegisterControl(_timerForm);

            if (_c.Settings.CountdownWindowWidth > 0 && _c.Settings.CountdownWindowHeight > 0)
            {
                _timerForm.Width = _c.Settings.CountdownWindowWidth;
                _timerForm.Height = _c.Settings.CountdownWindowHeight;
            }
            _timerForm.Show();
            if (_c.Settings.CountdownWindowX != 0 && _c.Settings.CountdownWindowY != 0) // "default", use start-up position
            {
                var location = new Point(_c.Settings.CountdownWindowX, _c.Settings.CountdownWindowY);
                var inboundScreen = Screen.AllScreens.Where(s => s.WorkingArea.Contains(location)).FirstOrDefault();
                if (inboundScreen != null)
                {
                    // Should be safe...?
                    _timerForm.Location = location;
                }
            }

            _timerForm.FormClosed += TimerFormClosed;
            _timerForm.Move += TimerFormMove;
            _timerForm.ResizeEnd += TimerFormResized;
            btnPopOutTimer.SetOriginalImage(Images.CloseImage);
            ttipCarrierUI.SetToolTip(btnPopOutTimer, "Close pop-out timer window for this carrier");
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
            _c.Core.UnregisterControl(_timerForm);
            _timerForm = null;
            btnPopOutTimer.SetOriginalImage(Images.OpenInNewImage);
            ttipCarrierUI.SetToolTip(btnPopOutTimer, "Open Timer in pop-out window");
        }

        private void BtnOpenInventory_Click(object sender, EventArgs e)
        {
            if (InventoryForm is not null && !InventoryForm.IsDisposed)
            {
                InventoryForm.Activate();
                return;
            }
            
            InventoryForm = new (_c, _data);
            InventoryForm.FormClosed += InventoryForm_FormClosed;
            _c.Core.RegisterControl(InventoryForm);

            InventoryForm.Show();
        }

        private void InventoryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                _c.Core.UnregisterControl(InventoryForm);
                InventoryForm?.Dispose();
                InventoryForm = null;
            }
            catch { }
        }
        #endregion

    }
}
