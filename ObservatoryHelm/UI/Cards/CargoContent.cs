using System.ComponentModel;
using System.ComponentModel.Design;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    internal partial class CargoContent : HelmContentBase
    {
        private CommanderData _commanderData = null;

        internal CargoContent(HelmContext context) : base(context)
        {
            InitializeComponent();

            // Auto-collapse if cargo is empty.
            ContentTitle = "Cargo";

            _c.UIMgr.PropertyChanged += UIState_PropertyChanged;

            InternalClear();
            _suppressEvents = false;
        }

        protected override void InternalDraw()
        {
            var uiData = _c.UIMgr.ForMode();
            pbCargoGauge.Value = Math.Clamp(uiData.Cargo, pbCargoGauge.Minimum, pbCargoGauge.Maximum);
            ttManager.SetToolTip(pbCargoGauge, $"{uiData.Cargo} T of cargo");
            ContentTitle = uiData.Cargo > 0 ? $"Cargo - ${uiData.Cargo} T" : "Cargo";

            if (uiData.Cargo == 0)
            {
                lvCargo.Items.Clear();
                return;
            }

            _commanderData ??= _c.Data.For(uiData.CommanderKey);
            if (_commanderData is null || !uiData.ShipId.HasValue || !_commanderData.Ships.IsKnown(uiData.ShipId.Value))
                return;

            var shipData = _commanderData.Ships.GetShip(uiData.ShipId);

            string usageString = $"{uiData.Cargo} / {shipData.CargoCapacity} T";
            ttManager.SetToolTip(pbCargoGauge, $"{usageString} cargo space used");
            ContentTitle = uiData.Cargo > 0 ? $"Cargo - {usageString}" : "Cargo";

            var cargo = _commanderData.Ships.Cargo;

            HashSet<string> knownItems = [];
            HashSet<string> removeItems = [];
            foreach (ListViewItem row in lvCargo.Items)
            {
                if (cargo.TryGetValue(row.Name, out var cargoCount))
                {
                    row.Text = $"{cargoCount}";
                    knownItems.Add(row.Name);
                }
                else
                {
                    removeItems.Add(row.Name);
                }
            }

            // Backfill new items.
            foreach (var item in cargo)
            {
                if (knownItems.Contains(item.Key)) continue;
                var lvItem = lvCargo.Items.Add($"{item.Value}");
                lvItem.Name = item.Key;
                lvItem.SubItems.Add($"{CargoHelper.CommodityName(item.Key)}");
            }

            // Remove items that are no longer present.
            foreach (var removed in removeItems)
            {
                lvCargo.Items.RemoveByKey(removed);
            }
        }

        protected override void InternalClear()
        {
            lvCargo.Items.Clear();
            pbCargoGauge.Value = 0;
            ttManager.SetToolTip(pbCargoGauge, "");

            _commanderData = null;
        }

        private void UIState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case UIStateManager.PROP_CARGO:
                    // When the items in the cargo hold change.
                    InternalDraw();
                    break;
                case UIStateManager.PROP_SHIPID:
                    // Detect max cargo changes.
                    _commanderData = _c.Data.For(_c.UIMgr.ForMode().CommanderKey);
                    if (_commanderData is not null
                        && _commanderData.Ships.CurrentShipID.HasValue
                        && _commanderData.Ships.CurrentShip.CargoCapacity.HasValue)
                        pbCargoGauge.Maximum = _commanderData.Ships.CurrentShip.CargoCapacity.Value;
                    break;
            }
        }
    }
}
