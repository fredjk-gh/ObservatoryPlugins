using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using static System.Windows.Forms.AxHost;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    internal partial class ShipContent : HelmContentBase
    {
        private readonly ThemeableImageButton viewFleetButton;
        private readonly ScalableImageButton edsyButton;
        private readonly ScalableImageButton coriolisButton;

        private readonly ImageSpec ICON_WARNING_SIGN = new(Images.WarningSignImage)
        {
            Color = Color.Orange,
            Size = new(16, 16),
            Tag = "fuel",
            ToolTip = "Fuel level warning",
            Visible = false,
        };

        private readonly ImageSpec ICON_FUEL = new(Images.FuelImage)
        {
            Color = Color.Orange,
            Size = new(16, 16),
            Tag = "warning",
            ToolTip = "Fuel level warning",
            Visible = false,
        };

        private CommanderData _commanderData = null;
        private ShipData _lastShip = null;

        internal ShipContent(HelmContext context) : base(context)
        {
            InitializeComponent();

            ContentTitle = "Ship";

            SuspendLayout();

            viewFleetButton = new()
            {
                OriginalImage = Images.ShipFleetImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(viewFleetButton, "View your fleet");
            AddToolButton(viewFleetButton);

            edsyButton = new()
            {
                OriginalImage = Images.EdsyImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(edsyButton, "View loadout on edsy.org");
            AddToolButton(edsyButton);

            coriolisButton = new()
            {
                OriginalImage = Images.CoriolisImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(coriolisButton, "View loadout on coriolis.io");
            AddToolButton(coriolisButton);

            tlblFuel.ToolTipManager = ttManager;
            tlblFuel.AddImage(ICON_FUEL);
            tlblFuel.AddImage(ICON_WARNING_SIGN);

            ResumeLayout(false);
            PerformLayout();

            _c.UIMgr.PropertyChanged += UIState_PropertyChanged;

            InternalClear();
            _suppressEvents = false;
        }

        protected override void InternalDraw()
        {
            InternalClear();
            var state = _c.UIMgr.ForMode();

            if (!_c.Data.IsCommanderKnown(state.CommanderKey)
                || !state.ShipId.HasValue
                || !_c.Data.For(state.CommanderKey).Ships.IsKnown(state.ShipId.Value))
            {
                Debug.WriteLine($"Commander or ship was not found or do not agree.");
                return; // TODO: Spit out a message?
            }
            _commanderData ??= _c.Data.For(state.CommanderKey);

            _lastShip = _commanderData.Ships.GetShip(state.ShipId);

            if (_lastShip == null) return;

            lblShipName.Text = _lastShip.ShipName;
            lblShipIdentifier.Text = _lastShip.ShipIdent;
            lblShipType.Text = _lastShip.ShipType;
            if (FDevIDs.ShipyardBySymbol.ContainsKey(_lastShip.ShipType.ToLower()))
            {
                lblShipType.Text = FDevIDs.ShipyardBySymbol[_lastShip.ShipType.ToLower()].Name;
            }
            UpdateJumpRange();
            pbFuel.Maximum = (int)(_lastShip.FuelCapacity * 10);
            UpdateFuelRemaining(_lastShip.FuelRemaining);
        }

        public void UpdateFuelRemaining(double fuelRemaining)
        {
            _isDirty = true;
            pbFuel.Value = (int)(fuelRemaining * 10);
            ttManager.SetToolTip(pbFuel, $"{fuelRemaining} / {_lastShip.FuelCapacity} T");

            if (fuelRemaining < (_lastShip.MaxFuelPerJump * Constants.FuelWarningLevelFactor))
            {

                tlblFuel.SetVisibility(ICON_FUEL.Guid, true);
                tlblFuel.SetVisibility(ICON_WARNING_SIGN.Guid, true);
            }
            else
            {
                tlblFuel.SetVisibility(ICON_FUEL.Guid, false);
                tlblFuel.SetVisibility(ICON_WARNING_SIGN.Guid, false);
            }
        }

        public void UpdateJumpRange()
        {
            if (_lastShip == null) return;

            _isDirty = true;
            string boosted = _lastShip.JetConeBoostFactor > 1
                ? " boosted"
                : "";
            lblJumpRange.Text = $"{(_lastShip.MaxJumpRange):0.00} Ly (max{boosted})";
        }

        protected override void InternalClear()
        {
            lblShipName.Text = string.Empty;
            lblShipIdentifier.Text = string.Empty;
            lblShipType.Text = string.Empty;
            lblJumpRange.Text = string.Empty;
            pbFuel.Value = 0;
            pbFuel.Maximum = 1;
            ttManager.SetToolTip(pbFuel, string.Empty);
            ICON_FUEL.Visible = false;
            ICON_WARNING_SIGN.Visible = false;

            _commanderData = null;
            _lastShip = null;
        }

        private void UIState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case UIStateManager.PROP_COMMANDERKEY:
                    _commanderData = _c.Data.For(_c.UIMgr.ForMode().CommanderKey);
                    _lastShip = _commanderData?.Ships.GetShip(_c.UIMgr.ForMode().ShipId);
                    InternalDraw();
                    break;
                case UIStateManager.PROP_SHIPID:
                    InternalDraw();
                    break;
                case UIStateManager.PROP_FUELREMAINING:
                    if (_lastShip == null && _c.UIMgr.ForMode().ShipId.HasValue)
                        InternalDraw();
                    else
                        UpdateFuelRemaining(_c.UIMgr.ForMode().FuelRemaining);
                    break;
                case UIStateManager.PROP_JETCONEBOOST:
                    UpdateJumpRange();
                    break;
            }
        }
    }
}
