using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    internal partial class PlanetContent : HelmContentBase
    {
        private readonly ImageSpec ICON_BODY_VALUABLE = new(PluginCommon.Images.BodyValuableImage)
        {
            Color = Color.Green,
            Size = new(20, 20),
            Tag = "valuable",
            ToolTip = "Body is valuable",
            Visible = true,
        };
        private readonly ImageSpec ICON_BODY_MAPPED = new(PluginCommon.Images.BodyMappedImage)
        {
            Color = Color.LightBlue,
            Size = new(20, 20),
            Tag = "mapped",
            ToolTip = "Current commander has mapped this body",
            Visible = true,
        };
        private readonly ImageSpec ICON_BODY_LANDABLE = new(PluginCommon.Images.BodyLandableImage)
        {
            Size = new(20, 20),
            Tag = "landable",
            ToolTip = "Body is landable",
            Visible = true,
        };
        private readonly ImageSpec ICON_WARNING_SIGN = new(PluginCommon.Images.WarningSignImage)
        {
            Color = Color.Orange,
            Size = new(16, 16),
            Tag = "warning",
            ToolTip = "Use caution! Gravity is above warning threshold",
            Visible = true,
        };
        private readonly ImageSpec ICON_WARNING_GRAVITY_NO_ONFOOT = new(PluginCommon.Images.NoOnfootImage)
        {
            Color = Color.Orange,
            Size = new(16, 16),
            Tag = "no-onfoot",
            ToolTip = "Warning: Gravity exceeds suit limits",
            Visible = true,
        };
        private readonly ImageSpec ICON_WARNING_TEMP_COLD = new(PluginCommon.Images.WarningColdImage)
        {
            Color = Color.Blue,
            Size = new(16, 16),
            Tag = "cold",
            ToolTip = "Warning: Extreme cold temperatures",
            Visible = true,
        };
        private readonly ImageSpec ICON_WARNING_TEMP_HEAT = new(PluginCommon.Images.WarningHeatImage)
        {
            Color = Color.Red,
            Size = new(16, 16),
            Tag = "heat",
            ToolTip = "Warning: Temperature exceeds thermal limits and could kill you",
            Visible = true,
        };
        private readonly ImageSpec ICON_WARNING_TEMP_NO_ONFOOT = new(PluginCommon.Images.NoOnfootImage)
        {
            Color = Color.Orange,
            Size = new(16, 16),
            Tag = "no-onfoot",
            ToolTip = "Warning: Extreme temperatures; unable to disembark",
            Visible = true,
        };
        private readonly ImageSpec ICON_POSITION = new(PluginCommon.Images.PositionImage)
        {
            Size = new(16, 16),
            Tag = "position",
            ToolTip = "Coordinates",
            Visible = true,
        };
        private readonly ImageSpec ICON_BODY_GEOSIGNALS = new(PluginCommon.Images.BodyGeoSignalsImage)
        {
            Size = new(16, 16),
            Tag = "geos",
            ToolTip = "Geological signals",
            Visible = true,
        };
        private readonly ImageSpec ICON_BODY_BIOSIGNALS = new(PluginCommon.Images.BodyBioSignalsImage)
        {
            Size = new(16, 16),
            Tag = "bios",
            ToolTip = "Biological signals",
            Visible = true,
        };
        private readonly ImageSpec ICON_BODY_RINGS = new(PluginCommon.Images.BodyRingsImage)
        {
            Size = new(16, 16),
            Tag = "rings",
            ToolTip = "Ring count",
            Visible = true,
        };

        private CommanderData _currCommander = null;
        private SystemData _displayedSystem = null;
        private PlanetData _displayedBody = null;
        private UIStateData _state;

        internal PlanetContent(HelmContext context) : base(context)
        {
            InitializeComponent();

            ContentTitle = "Planet";

            tlblBodyAttributes.ToolTipManager = ttManager;
            tlblGravityValue.ToolTipManager = ttManager;
            tlblTempValue.ToolTipManager = ttManager;
            tlblCoordinates.ToolTipManager = ttManager;
            tlblGeoSignals.ToolTipManager = ttManager;
            tlblBioSignals.ToolTipManager = ttManager;
            tlblBodyRings.ToolTipManager = ttManager;

            this.SuspendLayout();
            btnBodyCoordsCopy.SetOriginalImage(PluginCommon.Images.CopyImage, new(24, 24));
            tlblBodyAttributes.AddImage(ICON_BODY_VALUABLE);
            tlblBodyAttributes.AddImage(ICON_BODY_MAPPED);
            tlblBodyAttributes.AddImage(ICON_BODY_LANDABLE);
            tlblBodyAttributes.Text = string.Empty;
            tlblGravityValue.AddImage(ICON_WARNING_SIGN);
            tlblGravityValue.AddImage(ICON_WARNING_GRAVITY_NO_ONFOOT);
            tlblTempValue.AddImage(ICON_WARNING_TEMP_COLD);
            tlblTempValue.AddImage(ICON_WARNING_TEMP_HEAT);
            tlblTempValue.AddImage(ICON_WARNING_TEMP_NO_ONFOOT);
            tlblCoordinates.AddImage(ICON_POSITION);
            tlblGeoSignals.AddImage(ICON_BODY_GEOSIGNALS);
            tlblBioSignals.AddImage(ICON_BODY_BIOSIGNALS);
            tlblBodyRings.AddImage(ICON_BODY_RINGS);

            this.ResumeLayout();
            _suppressEvents = false;

            _state = _c.UIMgr.ForMode();
            _c.UIMgr.PropertyChanged += UIState_PropertyChanged;
            cboBody.SelectedIndexChanged += CboBody_SelectedIndexChanged;

            lblBodyType.FontChanged += TraceFontChanged;
            InternalClear();
        }

        private void TraceFontChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Font Size changed: {Font}");
        }

        protected override void InternalDraw() // Assumes internal thread.
        {
            _currCommander ??= _c.Data.For(_state.CommanderKey);

            if (_displayedSystem is not null
                && _displayedBody is not null
                && _displayedSystem.SystemId64 == _state.SystemId64
                && _displayedBody.BodyId == _state.BodyId)
            {
                // Something ping, but the currently displayed body hasn't actually changed.
                // Just refresh the body list and flags.
                UpdateBodyDropList(true);
                UpdateFlags();
                return;
            }

            bool systemChanged = _displayedSystem is null || _displayedSystem.SystemId64 != _state.SystemId64;
            if (systemChanged &&
                (_currCommander is null || !_state.SystemId64.HasValue
                    || !_currCommander.RecentSystems.TryGetValue(_state.SystemId64.Value, out _displayedSystem)))
                return;

            if (_displayedSystem.Planets.Count > 0 && _displayedSystem.Planets.Count != cboBody.Items.Count)
                UpdateBodyDropList();
            else if (_displayedSystem.Planets.Count == 0)
            {
                InternalClear();
                return;
            }

            if ((systemChanged || _displayedBody?.BodyId != _state.BodyId)
                    && !_displayedSystem.Planets.TryGetValue(_state.BodyId, out this._displayedBody))
                return;

            SuspendLayout();


            _suppressEvents = true;
            lblBodyType.Text = _displayedBody.TypeDescription;
            lblRadiusValue.Text = UIFormatter.DistanceBelowLs(_displayedBody.Scan.Radius);
            tlblGravityValue.Text = UIFormatter.GravityG(_displayedBody.Scan.SurfaceGravity);
            float g = Conversions.Mpers2ToG(_displayedBody.Scan.SurfaceGravity);
            tlblGravityValue.SetVisibility(ICON_WARNING_SIGN.Guid, g > _c.Settings.GravityAdvisoryThreshold);
            tlblGravityValue.SetVisibility(ICON_WARNING_GRAVITY_NO_ONFOOT.Guid, g >= 2.7f); // Per various internet sources.

            tlblTempValue.Text = UIFormatter.TemperatureK(_displayedBody.Scan.SurfaceTemperature);
            float t = _displayedBody.Scan.SurfaceTemperature;
            tlblTempValue.SetVisibility(ICON_WARNING_TEMP_COLD.Guid, t <= 182); // Per wiki
            tlblTempValue.SetVisibility(ICON_WARNING_TEMP_HEAT.Guid, t >= 700); // Per wiki
            tlblTempValue.SetVisibility(ICON_WARNING_TEMP_NO_ONFOOT.Guid, t >= 800); // Double check disembark limit?

            lblBodyPressure.Text = UIFormatter.PressureAtm(_displayedBody.Scan.SurfacePressure);
            lblAtmosphereTypeValue.Text = $"{(!string.IsNullOrEmpty(_displayedBody.Scan.Atmosphere) ? _displayedBody.Scan.Atmosphere : "(none)")}";

            tlblGeoSignals.Text = $"{_displayedBody.SignalCountByType(PlanetData.SignalType.Geological)}";
            tlblBioSignals.Text = $"{_displayedBody.SignalCountByType(PlanetData.SignalType.Biological)}";
            tlblBodyRings.Text = $"{_displayedBody.Scan.Rings?.Count ?? 0}";

            lblBodyDistance.Text = UIFormatter.DistanceLs(_displayedBody.Scan.DistanceFromArrivalLS, 1);

            UpdateSurfacePosition();
            UpdateFlags();
            ResumeLayout();

            _suppressEvents = false;
        }

        private void UpdateSurfacePosition()
        {
            if (_displayedBody == null)
            {
                InternalDraw();
                return;
            }
            _isDirty = true;
            if (_state.SurfacePosition != null && _displayedBody.Scan.BodyName == _state.SurfacePosition.BodyName)
            {
                lblBodyAltitude.Text = UIFormatter.DistanceBelowLs((float)_state.SurfacePosition.Altitude);
                tlblCoordinates.Text = UIFormatter.SurfaceCoordinates(_state.SurfacePosition.Latitude, _state.SurfacePosition.Longitude);
                SetButtonState(true);
            }
            else
            {
                lblBodyAltitude.Text = "-";
                tlblCoordinates.Text = "-";
                SetButtonState(false);
            }
        }

        private void UpdateBodyDropList(bool preserveSelected = false)
        {
            _suppressEvents = true;
            _isDirty = true;
            PlanetData selected = null;

            if (_displayedSystem == null
                && _state.SystemId64.HasValue
                && _currCommander.RecentSystems.TryGetValue(_state.SystemId64.Value, out var system))
            {
                _displayedSystem = system;
            }
            if (_displayedSystem == null) return;

            if (preserveSelected && cboBody.SelectedIndex >= 0)
            {
                selected = (PlanetData)cboBody.SelectedItem;
            }
            else
            {
                _displayedSystem.Planets
                    .TryGetValue(_state.BodyId, out selected);
            }

            cboBody.DisplayMember = "ShortName";
            cboBody.ValueMember = "BodyId";
            cboBody.DataSource = _displayedSystem.Planets.Values
                .OrderBy(b => b.BodyId)
                .ToList();

            if (selected != null)
            {
                cboBody.SelectedItem = selected;
                _displayedBody = selected;
            }
            else
            {
                cboBody.SelectedIndex = 0;
                var body = cboBody.SelectedItem as PlanetData;
                _c.UIMgr.ReplayMode = true;
                _state.BodyId = body.BodyId;
                _c.UIMgr.ReplayMode = false;
                _displayedBody = body;
            }

            _suppressEvents = false;
        }

        private void UpdateFlags()
        {
            if (_displayedBody == null) return;
            _isDirty = true;

            tlblBodyAttributes.SetVisibility(ICON_BODY_VALUABLE.Guid, _displayedBody.IsHighValue);
            tlblBodyAttributes.SetVisibility(ICON_BODY_MAPPED.Guid, _displayedBody.WasMapped);
            tlblBodyAttributes.SetVisibility(ICON_BODY_LANDABLE.Guid, _displayedBody.IsLandable);
        }

        protected override void InternalClear()
        {
            cboBody.DataSource = new List<PlanetData>();
            lblBodyType.Text = string.Empty;
            tlblBodyAttributes.SetVisibility(ICON_BODY_VALUABLE.Guid, false);
            tlblBodyAttributes.SetVisibility(ICON_BODY_MAPPED.Guid, false);
            tlblBodyAttributes.SetVisibility(ICON_BODY_LANDABLE.Guid, false);
            lblRadiusValue.Text = string.Empty;
            tlblGravityValue.Text = string.Empty;
            tlblGravityValue.SetVisibility(ICON_WARNING_SIGN.Guid, false);
            tlblGravityValue.SetVisibility(ICON_WARNING_GRAVITY_NO_ONFOOT.Guid, false);
            tlblTempValue.Text = string.Empty;
            tlblTempValue.SetVisibility(ICON_WARNING_TEMP_COLD.Guid, false);
            tlblTempValue.SetVisibility(ICON_WARNING_TEMP_HEAT.Guid, false);
            tlblTempValue.SetVisibility(ICON_WARNING_TEMP_NO_ONFOOT.Guid, false);
            lblBodyPressure.Text = string.Empty;
            lblAtmosphereTypeValue.Text = string.Empty;
            tlblGeoSignals.Text = string.Empty;
            tlblBioSignals.Text = string.Empty;
            tlblBodyRings.Text = string.Empty;
            lblBodyDistance.Text = string.Empty;

            lblBodyAltitude.Text = string.Empty;
            tlblCoordinates.Text = string.Empty;

            SetButtonState(false);
            _currCommander = null;
            _displayedBody = null;
            _displayedSystem = null;
        }

        internal void SetTitle()
        {
            if (_displayedBody == null) return;

            if (_c.UIMgr.Mode == UIStateManager.UIMode.Realtime
                && _displayedBody?.BodyId != _c.UIMgr.ForMode(UIStateManager.UIMode.Realtime)?.BodyId)
            {
                ContentTitle = "Current Planet";
            }
            else
            {
                ContentTitle = "Selected Planet";
            }
        }

        private void SetButtonState(bool isEnabled)
        {
            btnBodyCoordsCopy.Enabled = isEnabled;
        }

        private void CboBody_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;

            if (cboBody.SelectedItem is not PlanetData body) return;

            _state.BodyId = body.BodyId;
        }

        private void UIState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                switch (e.PropertyName)
                {
                    case UIStateManager.PROP_MODE:
                    case UIStateManager.PROP_COMMANDERKEY:
                        _state = _c.UIMgr.ForMode();
                        if (_state.CommanderKey is null)
                        {
                            InternalClear();
                            return;
                        }
                        _currCommander = _c.Data.For(_state.CommanderKey);
                        if (_state.SystemId64.HasValue)
                            _displayedSystem = _currCommander?.RecentSystems[_state.SystemId64.Value];

                        _displayedBody = null; // Wait for a property update to come in or pick a default.
                        InternalDraw();
                        break;
                    case UIStateManager.PROP_SYSTEMID64:
                    case UIStateManager.PROP_BODYID:
                        _currCommander = _c.Data.For(_state.CommanderKey);
                        if (!_state.SystemId64.HasValue
                            || !_currCommander.RecentSystems.TryGetValue(_state.SystemId64.Value, out var system)
                            || system.Planets.Count == 0)
                        {
                            InternalClear();
                        }
                        else if (_state.BodyId < 0
                            || system.Planets.ContainsKey(_state.BodyId))
                        {
                            InternalDraw();
                        }
                        break;
                    case UIStateManager.PROP_SURFACEPOSITION:
                        UpdateSurfacePosition();
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void OnBoxStateChanged(object sender, CollapsibleGroupBox.BoxStateEventArgs e)
        {
            _isExpanded = e.Expanded;
            SetTitle();
        }

        private void BodySignalsLabelClick(object sender, EventArgs e)
        {
            string targetPlugin = "";
            if (sender == tlblGeoSignals)
            {
                targetPlugin = "GeoPredictor";
            }
            else if (sender == tlblBioSignals
                && _c.PluginTracker.IsActive(PluginType.mattg_BioInsights))
            {
                targetPlugin = "BioInsights";
            }
            else if (sender == tlblBioSignals
                && _c.PluginTracker.IsActive(PluginType.vithigar_Botanist))
            {
                targetPlugin = "Botanist";
            }
            else if (sender == tlblBodyRings)
            {
                targetPlugin = "Prospector";
            }

            if (!string.IsNullOrEmpty(targetPlugin))
            {
                _c.Core.FocusPlugin(targetPlugin);
            }
        }
    }
}
