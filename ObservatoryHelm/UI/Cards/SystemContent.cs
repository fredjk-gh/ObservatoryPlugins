using System.ComponentModel;
using System.ComponentModel.Design;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.ObservatoryHelm.UI.Forms;
using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.Data.Id64;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Forms;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using com.github.fredjk_gh.PluginCommon.Utilities;
using static com.github.fredjk_gh.ObservatoryHelm.UI.UIStateManager;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    internal partial class SystemContent : HelmContentBase
    {
        private const string IMGLIST_FUEL = "fuel";

        private GetReferenceSystemForm _getReferenceSystemForm;
        private Id64Viewer _id64ViewerForm;
        private SuppressionZoneInfoForm _zoneHelpForm;

        private readonly ThemeableImageButton btnSystemNameCopy;
        private readonly ThemeableImageButton btnSystemBookmarkAdd;
        private readonly ThemeableImageButton btnSystemBookmarkView;
        private readonly ThemeableImageButton btnSystemId64;
        private readonly ScalableImageButton btnViewOnSpansh;
        private readonly ScalableImageButton btnViewOnInara;

        private readonly ImageSpec ICON_FIRST_DISCOVERY = new(Images.NewDiscoveryImage)
        {
            Size = new(20, 20),
            Tag = "first-discovery",
            ToolTip = "System was undiscovered when initially visited",
            Visible = true,
        };

        private readonly ImageSpec ICON_FULLY_SCANNED = new(Images.DoneAllImage)
        {
            Size = new(20, 20),
            Tag = "fully-scanned",
            ToolTip = "System has been fully scanned (FSS)",
            Visible = true,
        };

        private readonly ImageSpec ICON_POSITION = new(Images.PositionImage)
        {
            Size = new(16, 16),
            Tag = "position",
            ToolTip = "Coordinates",
            Visible = true,
        };

        private readonly ImageSpec ICON_ZONE_IN_BUBBLE = new(Images.BubbleImage)
        {
            Size = new(20, 20),
            Tag = "in-bubble",
            ToolTip = "This system is in or near the core 'bubble' of inhabited systems.",
            Visible = true,
        };

        private readonly ImageSpec ICON_ZONE_IN_SUPPRESSION = new(Images.BlockedImage)
        {
            Size = new(20, 20),
            Tag = "in-suppression",
            ToolTip = "This system may be in a star-type suppression zone.",
            Visible = true,
        };
        private readonly ImageSpec ICON_ZONE_IN_HESUPPRESSION = new(Images.HeSuppressionImage)
        {
            Size = new(20, 20),
            Tag = "in-he=suppression",
            ToolTip = "This system may be in a Helium suppression zone.",
            Visible = true,
        };

        private UIStateData _state;
        private CommanderData _displayedCmdr;
        private SystemData _displayedSys;

        internal SystemContent(HelmContext context) : base(context)
        {
            InitializeComponent();
            imgListForListView24.Images.Add(
                IMGLIST_FUEL,
                ImageCommon.RecolorAndSizeImage(
                    Images.FuelImage, ForeColor, imgListForListView24.ImageSize));

            ContentTitle = "System";

            SuspendLayout();
            btnSystemCoordsCopy.SetOriginalImage(Images.CopyImage, new(24, 24));
            btnSystemCoordsCopy.Enabled = false;

            btnSystemZoneHelp.SetOriginalImage(Images.HelpImage, new(24, 24));
            ttManager.SetToolTip(btnSystemZoneHelp, "More info about Zones.");

            btnSystemSetReference.SetOriginalImage(Images.EditLocationImage, new(24, 24));

            btnViewOnSpansh = new()
            {
                OriginalImage = Images.SpanshImage,
                ImageSize = new(32, 32),
                Enabled = false,
            };
            ttManager.SetToolTip(btnViewOnSpansh, "View system on Spansh");
            AddToolButton(btnViewOnSpansh);

            btnViewOnInara = new()
            {
                OriginalImage = Images.InaraImage,
                ImageSize = new(32, 32),
                Enabled = false,
            };
            ttManager.SetToolTip(btnViewOnInara, "View system on Inara");
            AddToolButton(btnViewOnInara);

            btnSystemBookmarkAdd = new()
            {
                OriginalImage = Images.BookmarkAddImage,
                ImageSize = new(32, 32),
                Enabled = false,
                Visible = false && _c.PluginTracker.IsActive(PluginType.fredjk_Archivist),
            };
            ttManager.SetToolTip(btnSystemBookmarkAdd, "Bookmark this system with Archivist");
            AddToolButton(btnSystemBookmarkAdd);

            btnSystemBookmarkView = new()
            {
                OriginalImage = Images.BookmarkViewImage,
                ImageSize = new(32, 32),
                Enabled = false,
                Visible = false && _c.PluginTracker.IsActive(PluginType.fredjk_Archivist),
            };
            ttManager.SetToolTip(btnSystemBookmarkView, "View bookmark info in Archivist");
            AddToolButton(btnSystemBookmarkView);

            btnSystemId64 = new()
            {
                OriginalImage = Images.Id64Image,
                ImageSize = new(32, 32),
                Enabled = false,
            };
            ttManager.SetToolTip(btnSystemId64, "View system id64 details");
            AddToolButton(btnSystemId64);

            btnSystemNameCopy = new()
            {
                OriginalImage = Images.CopyImage,
                ImageSize = new(32, 32),
                Enabled = false,
            };
            ttManager.SetToolTip(btnSystemNameCopy, "Copy name of system into system clipboard");
            AddToolButton(btnSystemNameCopy);

            tlblSystemAttributes.ToolTipManager = ttManager;
            tlblSystemAttributes.AddImage(ICON_FIRST_DISCOVERY);
            tlblSystemAttributes.AddImage(ICON_FULLY_SCANNED);
            tlblSystemAttributes.Text = string.Empty;

            tlblCoordinates.ToolTipManager = ttManager;
            tlblCoordinates.AddImage(ICON_POSITION);

            tlblZones.ToolTipManager = ttManager;
            tlblZones.AddImage(ICON_ZONE_IN_BUBBLE);
            tlblZones.AddImage(ICON_ZONE_IN_SUPPRESSION);
            tlblZones.AddImage(ICON_ZONE_IN_HESUPPRESSION);

            ResumeLayout(false);
            PerformLayout();

            // Wire up events...
            _state = _c.UIMgr.ForMode();
            _c.UIMgr.PropertyChanged += UIState_PropertyChanged;

            btnViewOnSpansh.Click += BtnViewOnSpansh_Click;
            btnViewOnInara.Click += BtnViewOnInara_Click;
            btnSystemNameCopy.Click += BtnSystemNameCopy_Click;
            btnSystemId64.Click += BtnSystemId64_Click;
            btnSystemBookmarkAdd.Click += BtnSystemBookmarkAdd_Click;
            btnSystemBookmarkView.Click += BtnSystemBookmarkView_Click;

            btnSystemCoordsCopy.Click += BtnSystemCoordsCopy_Click;
            btnSystemSetReference.Click += BtnSystemSetReference_Click;
            btnSystemZoneHelp.Click += BtnSystemZoneHelp_Click;

            InternalClear();
            _suppressEvents = false;
        }

        protected override void InternalDraw()
        {
            if (!_c.Data.IsCommanderKnown(_state.CommanderKey)) return;

            bool cmdrChanged = _displayedCmdr is null || !_displayedCmdr.Key.Equals(_state.CommanderKey);

            SuspendLayout();
            if (cmdrChanged)
            {
                InternalClear();
                _displayedCmdr = _c.Data.For(_state.CommanderKey);
                _displayedSys = _displayedCmdr.CurrentSystemData;
                UpdateSystemDropList(true);
            }
            else
            {
                UpdateSystemDropList();
            }

            if (this._displayedSys != null && this._displayedSys.SystemId64 != _state.SystemId64) this._displayedSys = null;

            bool systemChanged = this._displayedSys == null || _displayedCmdr.CurrentSystemData?.SystemId64 != _state.SystemId64;
            if (!systemChanged && !cmdrChanged) return;

            _displayedSys = cboSystem.SelectedItem as SystemData;
            if (_displayedSys == null && _displayedCmdr.CurrentSystemData != null)
                _displayedSys = _displayedCmdr.CurrentSystemData;

            if (_displayedSys == null) return;

            _suppressEvents = true;
            lblSystemBodyCount.Text = $"{(_displayedSys.BodyCount > 0 ? _displayedSys.BodyCount : "(unknown)")}";
            tlblZones.SetVisibility(ICON_ZONE_IN_BUBBLE.Guid, _displayedSys.IsInBubble ?? false);
            tlblZones.SetVisibility(ICON_ZONE_IN_SUPPRESSION.Guid, _displayedSys.IsInSuppressionZone ?? false);
            tlblZones.SetTooltip(ICON_ZONE_IN_SUPPRESSION.Guid, _displayedSys.SuppressionZoneDetails);
            tlblZones.SetVisibility(ICON_ZONE_IN_HESUPPRESSION.Guid, _displayedSys.IsInHeSuppressionBubble ?? false);
            tlblCoordinates.Text = UIFormatter.Coordinates(_displayedSys.Position.x, _displayedSys.Position.y, _displayedSys.Position.z);

            UpdateFlags();
            UpdateRefDistance();
            UpdateStars(true);

            SetButtonState(true);
            ResumeLayout();
            _suppressEvents = false;
        }

        private void SetButtonState(bool isEnabled)
        {
            btnSystemCoordsCopy.Enabled = isEnabled;
            btnViewOnInara.Enabled = isEnabled;
            btnViewOnSpansh.Enabled = isEnabled;
            btnSystemId64.Enabled = isEnabled;
            btnSystemNameCopy.Enabled = isEnabled;

            btnSystemBookmarkAdd.Enabled = false && _c.PluginTracker.IsActive(PluginType.fredjk_Archivist);
            btnSystemBookmarkView.Enabled = false && _c.PluginTracker.IsActive(PluginType.fredjk_Archivist);
        }

        private void UpdateFlags()
        {
            if (_displayedSys == null) return;

            _isDirty = true;
            lblSystemBodyCount.Text = $"{(_displayedSys.BodyCount > 0 ? _displayedSys.BodyCount : "(unknown)")}";
            tlblSystemAttributes.SetVisibility(ICON_FIRST_DISCOVERY.Guid, _displayedSys.IsFirstDiscovery);
            tlblSystemAttributes.SetVisibility(ICON_FULLY_SCANNED.Guid, _displayedSys.IsFullyDiscovered);
        }

        private void UpdateRefDistance()
        {
            if (_state.RefSystem == null)
            {
                lblRefDistance.Text = string.Empty;
                lblRefSystem.Text = "No reference system set";
                return;
            }

            _isDirty = true;
            lblRefSystem.Text = _state.RefSystem.SystemName;
            lblRefDistance.Text = string.Empty;

            if (_displayedSys != null)
            {
                lblRefDistance.Text = UIFormatter.DistanceLy(Id64CoordHelper.Distance(_displayedSys.Position, _state.RefSystem.Position), 2);
            }
        }

        private void UpdateStars(bool forceForFilterChange = false)
        {
            _isDirty = true;
            if (_displayedSys == null || (lvStars.Items.Count == _displayedSys.Stars.Count && !forceForFilterChange)) return;

            lvStars.Items.Clear();
            foreach (var s in _displayedSys.Stars.Values.OrderBy(s => s.BodyId))
            {
                if (cbShowOnlyScoopable.Checked && !s.IsScoopable) continue;

                var item = lvStars.Items.Add(new ListViewItem()
                {
                    ImageKey = s.IsScoopable ? "fuel" : null,
                    Text = s.TypeDescription,
                });
                item.SubItems.Add(s.ShortName);
                item.SubItems.Add(UIFormatter.DistanceLs(s.Scan.DistanceFromArrivalLS, 1));
            }
        }

        protected override void InternalClear()
        {
            cboSystem.DataSource = null;
            cboSystem.Items.Clear();
            tlblSystemAttributes.SetVisibility(ICON_FIRST_DISCOVERY.Guid, false);
            tlblSystemAttributes.SetVisibility(ICON_FULLY_SCANNED.Guid, false);
            tlblZones.SetVisibility(ICON_ZONE_IN_BUBBLE.Guid, false);
            tlblZones.SetVisibility(ICON_ZONE_IN_SUPPRESSION.Guid, false);
            tlblZones.SetTooltip(ICON_ZONE_IN_SUPPRESSION.Guid, string.Empty);
            tlblZones.SetVisibility(ICON_ZONE_IN_HESUPPRESSION.Guid, false);
            lblSystemBodyCount.Text = string.Empty;
            tlblCoordinates.Text = string.Empty;
            lblRefDistance.Text = string.Empty;
            lblRefSystem.Text = string.Empty;
            lvStars.Items.Clear();
            SetButtonState(false);

            _displayedCmdr = null;
            _displayedSys = null;
        }

        public void UpdateSystemDropList(bool forDiffCmdr = false)
        {
            SystemData selectedValue = null;

            // Nothing to do if the system is already in the system droplist.
            if (_displayedCmdr.RecentSystems.Count > 0 && !forDiffCmdr)
            {
                SystemData mostRecentSys = _displayedCmdr.RecentSystems[_displayedCmdr.RecentSystemIds.Last()];
                if (cboSystem.Items.Contains(mostRecentSys))
                {
                    return;
                }
            }

            _suppressEvents = true;
            if (cboSystem.SelectedIndex > 0 && !forDiffCmdr) selectedValue = (SystemData)cboSystem.SelectedItem;

            cboSystem.DisplayMember = "SystemName";
            cboSystem.ValueMember = "SystemId64";
            cboSystem.DataSource = _displayedCmdr.RecentSystemIds
                .Reverse<UInt64>()
                .Distinct()
                .Take(25)
                .Select(id => _displayedCmdr.RecentSystems[id])
                .ToList();

            if (selectedValue != null)
            {
                cboSystem.SelectedItem = selectedValue;
            }
            else if (cboSystem.Items.Count > 0)
            {
                if (_state.SystemId64.HasValue
                    && _displayedCmdr.RecentSystems.TryGetValue(_state.SystemId64.Value, out var system))
                {
                    cboSystem.SelectedItem = system;
                }
                else
                {
                    cboSystem.SelectedIndex = 0;
                }
            }
            _suppressEvents = false;
        }

        internal void SetTitle()
        {
            if (_displayedSys == null) return;
            if (_displayedSys.SystemId64 == _displayedCmdr.CurrentSystemAddress)
            {
                ContentTitle = "System";
            }
            else
            {
                ContentTitle = "Selected System";
            }
        }

        private void UIState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case UIStateManager.PROP_MODE:
                    _state = _c.UIMgr.ForMode();
                    if (_state.CommanderKey is null)
                    {
                        InternalClear();
                        return;
                    }
                    InternalDraw();
                    break;
                case UIStateManager.PROP_COMMANDERKEY:
                    InternalDraw();
                    break;
                case UIStateManager.PROP_SYSTEMID64:
                    InternalDraw();
                    break;
                case UIStateManager.PROP_BODYID:
                    // Refresh the star list, update flags
                    UpdateFlags();
                    UpdateStars();
                    break;
                case UIStateManager.PROP_ALLBODIESFOUND:
                    // Update flags
                    UpdateFlags();
                    break;
                case UIStateManager.PROP_REFSYSTEM:
                    // Update ref system details
                    UpdateRefDistance();
                    break;
            }
        }

        private void BtnViewOnSpansh_Click(object? sender, EventArgs e)
        {
            if (_displayedSys == null) return;

            string url = $"https://spansh.co.uk/system/{_displayedSys.SystemId64}";

            Misc.OpenUrl(url);
        }

        private void BtnViewOnInara_Click(object? sender, EventArgs e)
        {
            if (_displayedSys == null) return;

            string url = $"https://inara.cz/elite/starsystem/?search={Uri.EscapeDataString(_displayedSys.SystemName)}";

            Misc.OpenUrl(url);
        }

        private void cbShowOnlyScoopable_CheckedChanged(object sender, EventArgs e)
        {
            UpdateStars(true);
        }

        private void cboSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;

            if (cboSystem.SelectedItem is not SystemData sys) return;

            if (_state.CommanderKey.Equals(_c.UIMgr.Realtime.CommanderKey)
                && sys.SystemId64 == _c.UIMgr.Realtime.SystemId64)
            {
                // Switch back to realtime mode. Triggers re-draw.
                _c.UIMgr.Mode = UIMode.Realtime;
            }
            else
            {
                // Either the system switch or the mode switch triggers re-draw via property changed event.
                if (_c.UIMgr.Mode == UIMode.Realtime)
                {
                    // Use current realtime data as starting point.
                    var detached = _c.UIMgr.Detatched.CopyFrom(_c.UIMgr.Realtime);
                    detached.SwitchSystem(sys.SystemId64);
                    _c.UIMgr.Mode = UIMode.Detached;
                }
                else
                {
                    // Already in detached mode, just switch system.
                    _state.SwitchSystem(sys.SystemId64);
                }
            }
        }

        protected override void OnBoxStateChanged(object sender, CollapsibleGroupBox.BoxStateEventArgs e)
        {
            _isExpanded = e.Expanded;
            SetTitle();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            imgListForListView24.Images.Clear();
            imgListForListView24.Images.Add(
                IMGLIST_FUEL,
                ImageCommon.RecolorAndSizeImage(
                    Images.FuelImage, ForeColor, imgListForListView24.ImageSize));
        }

        private void BtnSystemSetReference_Click(object sender, EventArgs e)
        {
            if (_getReferenceSystemForm != null) return;

            // Open a new form to enter System Name and id64 or X/y/z
            // Request from Archivist if no x/y/z
            _getReferenceSystemForm = new(_c);
            _c.Core.RegisterControl(_getReferenceSystemForm);

            btnSystemSetReference.Enabled = false;
            _getReferenceSystemForm.FormClosed += SetReference_FormClosed;
            _getReferenceSystemForm.StartPosition = FormStartPosition.CenterScreen;
            _getReferenceSystemForm.Show();
        }

        private void SetReference_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_getReferenceSystemForm.ReferenceSystem != null)
            {
                _state.RefSystem = _getReferenceSystemForm.ReferenceSystem;
                _displayedCmdr.ReferenceSystem = _getReferenceSystemForm.ReferenceSystem;
                _c.Data.SaveToCache();
            }

            try
            {
                _c.Core.UnregisterControl(_getReferenceSystemForm);
                _getReferenceSystemForm.Dispose();
            }
            finally
            {
                _getReferenceSystemForm = null;
                btnSystemSetReference.Enabled = true;
            }
        }

        private void BtnSystemCoordsCopy_Click(object sender, EventArgs e)
        {
            Misc.SetTextToClipboard(tlblCoordinates.Text);
        }

        private void BtnSystemId64_Click(object sender, EventArgs e)
        {

            if (_id64ViewerForm is not null && !_id64ViewerForm.IsDisposed)
            {
                _id64ViewerForm.Activate();
                return;
            }

            _id64ViewerForm = new(_displayedSys.Id64Details, _displayedSys.SystemName);
            _c.Core.RegisterControl(_id64ViewerForm);
            _id64ViewerForm.FormClosed += Id64Viewer_FormClosed;
            _id64ViewerForm.Show();
        }

        private void Id64Viewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                _c.Core.UnregisterControl(_id64ViewerForm);
                _id64ViewerForm.Dispose();
            }
            catch { }
            finally
            {
                _id64ViewerForm = null;
            }
        }

        private void BtnSystemZoneHelp_Click(object sender, EventArgs e)
        {
            if (_zoneHelpForm is not null && !_zoneHelpForm.IsDisposed)
            {
                _zoneHelpForm.Activate();
                return;
            }

            _zoneHelpForm = new();
            _c.Core.RegisterControl(_zoneHelpForm);
            _zoneHelpForm.FormClosed += ZoneHelpForm_FormClosed;
            _zoneHelpForm.Show();
        }

        private void ZoneHelpForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                _c.Core.UnregisterControl(_zoneHelpForm);
                _zoneHelpForm.Dispose();
            }
            catch { }
            finally
            {
                _zoneHelpForm = null;
            }
        }

        private void BtnSystemNameCopy_Click(object sender, EventArgs e)
        {
            Misc.SetTextToClipboard(_displayedSys.SystemName);
        }

        private void BtnSystemBookmarkView_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnSystemBookmarkAdd_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
