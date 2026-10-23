using com.github.fredjk_gh.PluginCommon.Data.Prospecting;
using Observatory.Framework.Interfaces;

namespace com.github.fredjk_gh.ObservatoryProspectorBasic.UI
{
    public partial class CommoditySelectorForm : Form
    {
        private readonly IObservatoryCore _core;
        private readonly IObservatoryWorker _plugin;
        private readonly ProspectorSettings _settings;
        private readonly HashSet<Commodities> _selectedCommodities;

        private bool _filterTaskPending = false;
        private bool _suppressRedraw = false;
        private bool _suppressListEvents = false;

        internal CommoditySelectorForm(IObservatoryCore core, IObservatoryWorker plugin, ProspectorSettings settings)
        {
            InitializeComponent();

            _core = core;
            _plugin = plugin;
            _settings = settings;

            _selectedCommodities = [.. _settings.ProspectingFor];

            foreach (MiningMethod mmItem in Enum.GetValues(typeof(MiningMethod)))
            {
                cboMiningMethod.Items.Add(mmItem);
            }

            DrawCommodityList();
        }

        private void DrawCommodityList()
        {
            if (_suppressRedraw) return;

            _suppressListEvents = true;
            try
            {
                lvCommodityDetails.Items.Clear();
                foreach (var c in CommodityDetails.ALL)
                {
                    // Check filters
                    if (!string.IsNullOrEmpty(txtNameFilter.Text) && !c.Value.DisplayName.Contains(txtNameFilter.Text, StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    if (!string.IsNullOrEmpty(txtRingBodyTypeFilter.Text)
                        && !c.Value.RingType.DisplayString().Contains(txtRingBodyTypeFilter.Text, StringComparison.CurrentCultureIgnoreCase)
                        && !c.Value.BodyType.DisplayString().Contains(txtRingBodyTypeFilter.Text, StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    if (cboMiningMethod.SelectedIndex >= 0
                        && Enum.TryParse<MiningMethod>(cboMiningMethod.SelectedItem?.ToString() ?? "", out MiningMethod mm)
                        && !c.Value.MiningMethod.HasFlag((MiningMethod)mm))
                        continue;

                    var cItem = lvCommodityDetails.Items.Add(c.Value.DisplayName);
                    cItem.Tag = c.Key;
                    cItem.Checked = _selectedCommodities.Contains(c.Key);

                    var typeItem = cItem.SubItems.Add(c.Value.CommodityType.DisplayString());
                    typeItem.Tag = c.Value.CommodityType;

                    var mineMethodItem = cItem.SubItems.Add(c.Value.MiningMethod.DisplayString());

                    var ringTypeItem = cItem.SubItems.Add(c.Value.RingType.DisplayString());

                    var bodyTypeItem = cItem.SubItems.Add(c.Value.BodyType.DisplayString());
                }
            }
            finally
            {
                _suppressListEvents = false;
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            if (_filterTaskPending) return;
            _filterTaskPending = true;

            Task.Run(async () =>
            {
                await Task.Delay(100);
                Invoke(() =>
                {
                    DrawCommodityList();
                    _filterTaskPending = false;
                });
            });
        }

        private void cboMiningMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawCommodityList();
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            _suppressRedraw = true;
            cboMiningMethod.SelectedIndex = -1;
            txtNameFilter.Text = string.Empty;
            txtRingBodyTypeFilter.Text = string.Empty;

            _suppressRedraw = false;
            DrawCommodityList();
        }

        private void lvCommodityDetails_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_suppressListEvents) return;

            Commodities c = (Commodities)e.Item.Tag;

            if (e.Item.Checked)
                _selectedCommodities.Add(c);
            else
                _selectedCommodities.Remove(c);

            lblItemsSelected.Text = $"{_selectedCommodities.Count} commodities selected";
        }

        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            _selectedCommodities.Clear();
            lblItemsSelected.Text = $"{_selectedCommodities.Count} commodities selected";
            DrawCommodityList();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CommoditySelectorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _settings.ProspectingFor.Clear();
            foreach (Commodities c in _selectedCommodities)
            {
                _settings.ProspectingFor.Add(c);
            }

            _core.SaveSettings(_plugin);
        }
    }
}
