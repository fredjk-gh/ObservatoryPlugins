using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.UI;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    internal partial class CargoContent : HelmContentBase
    {
        private CommanderData _commanderData = null;

        internal CargoContent(HelmContext context, ToolTip sharedTtip) : base(context, sharedTtip)
        {
            InitializeComponent();

            // Auto-collapse if cargo is empty.
            ContentTitle = "Cargo";
            EnablePopout = true;

            _toolTip.SetToolTip(lvCargoShip, "Cargo hold contents");
            _toolTip.SetToolTip(pbCargoGaugeShip, "5 / 20 T used");
            _toolTip.SetToolTip(lvCargoSRV, "Cargo hold contents");
            _toolTip.SetToolTip(pbCargoGaugeSRV, "5 / 20 T used");

            _c.UIMgr.PropertyChanged += UIState_PropertyChanged;

            InternalClear();
            _suppressEvents = false;
        }

        protected override void InternalDraw()
        {
            var uiData = _c.UIMgr.ForMode();
            _commanderData ??= _c.Data.For(uiData.CommanderKey);

            DrawCargoContent(pbCargoGaugeShip, lvCargoShip, uiData.ShipCargo, GetShipCargoCapacity(uiData), _commanderData.Ships.Cargo);
            DrawCargoContent(pbCargoGaugeSRV, lvCargoSRV, uiData.SRVCargo, GetSRVCargoCapacity(), _commanderData.SRV.Cargo);
            UpdateForTransitMode();
        }

        protected override void InternalClear()
        {
            ClearCargoContent(pbCargoGaugeShip, lvCargoShip);
            ClearCargoContent(pbCargoGaugeSRV, lvCargoSRV);
            tabCargos.SelectedTab = tabCargoShip;

            tabCargoShip.Text = "Ship";
            tabCargoSRV.Text = "SRV";

            _commanderData = null;
        }

        private void UpdateForTransitMode()
        {
            var uiData = _c.UIMgr.ForMode();

            if (_commanderData.Ships.CurrentShipID is not null)
                tabCargoShip.Text = $"Ship - {_commanderData.Ships.CurrentShip?.ShipName}";
            else
                tabCargoShip.Text = "Ship";

            if (_commanderData.SRV.ActiveSRVType is not null)
                tabCargoSRV.Text = $"SRV - {_commanderData.SRV.ActiveSRVDetails.Name}";
            else
                tabCargoSRV.Text = "SRV";


            switch (uiData.TransitMode)
            {
                case TransitModeType.OwnShip:
                    tabCargos.SelectedTab = tabCargoShip;
                    break;
                case TransitModeType.ShipLaunchedVessel:
                    tabCargos.SelectedTab = tabCargoSRV;
                    break;
            }
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            lvCargoShip.ScaleColumns(factor);
            lvCargoSRV.ScaleColumns(factor);
        }

        private void DrawCargoContent(ThemeableProgressBar pb, ListView lv, int cargoCount, int maxCargo, Dictionary<string, int> cargo = null)
        {
            if (maxCargo != pb.Maximum) pb.Maximum = maxCargo;
            pb.Value = Math.Clamp(cargoCount, pb.Minimum, pb.Maximum);
            _toolTip.SetToolTip(pb, $"{cargoCount} T of cargo");

            Debug.WriteLineIf(
                (cargo is null && cargoCount > 0)
                || (cargo is not null && cargo.Values.Sum() != cargoCount),
                $"In DrawCargoContent, cargoCount ({cargoCount}) does not agree with inventory count: {(cargo is null ? 0 : cargo.Values.Sum())}");

            if (cargoCount == 0 || cargo is null)
            {
                lv.Items.Clear();
                return;
            }

            string usageString = $"{cargoCount} / {maxCargo} T";
            _toolTip.SetToolTip(pb, $"{usageString} cargo space used");

            // Experiment: Just clear and re-draw the listview.
            lv.Items.Clear();
            foreach (var item in cargo)
            {
                var lvItem = lv.Items.Add($"{item.Value}");
                lvItem.Name = item.Key;
                lvItem.SubItems.Add($"{CargoHelper.CommodityName(item.Key)}");
            }
        }

        private void ClearCargoContent(ThemeableProgressBar pb, ListView lv)
        {
            lv.Items.Clear();
            pb.Value = 0;
            _toolTip.SetToolTip(pb, "");
        }

        private void UIState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case UIStateManager.PROP_TRANSITMODE:
                    UpdateForTransitMode();
                    break;
                case UIStateManager.PROP_SHIP_CARGO:
                    InternalDraw();
                    break;
                case UIStateManager.PROP_SRV_CARGO:
                    InternalDraw();
                    break;
                case UIStateManager.PROP_SHIPID:
                    // Detect max cargo changes.
                    _commanderData = _c.Data.For(_c.UIMgr.ForMode().CommanderKey);
                    if (_commanderData is not null
                        && _commanderData.Ships.CurrentShipID.HasValue
                        && _commanderData.Ships.CurrentShip.CargoCapacity.HasValue)
                        pbCargoGaugeShip.Maximum = _commanderData.Ships.CurrentShip.CargoCapacity.Value;
                    UpdateForTransitMode();
                    break;
            }
        }

        private int GetShipCargoCapacity(UIStateData uiData)
        {
            if (_commanderData is null || !uiData.ShipId.HasValue || !_commanderData.Ships.IsKnown(uiData.ShipId.Value))
                return pbCargoGaugeShip.Maximum; // Preserve current value.

            var shipData = _commanderData.Ships.GetShip(uiData.ShipId);
            return shipData?.CargoCapacity ?? pbCargoGaugeShip.Maximum; // Preserve current value.
        }

        private int GetSRVCargoCapacity()
        {
            if (_commanderData is null || !_commanderData.SRV.ActiveSRVType.HasValue)
                return pbCargoGaugeSRV.Maximum; // Preserve current value.

            return _commanderData.SRV.ActiveSRVDetails?.MaxCargo ?? pbCargoGaugeSRV.Maximum; // Preserve current value.
        }
    }
}
