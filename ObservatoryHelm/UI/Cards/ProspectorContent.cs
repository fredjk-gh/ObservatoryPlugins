using System.ComponentModel;
using System.ComponentModel.Design;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    internal partial class ProspectorContent : HelmContentBase
    {
        private readonly ImageSpec ICON_WARNING_SIGN = new(PluginCommon.Images.WarningSignImage)
        {
            Color = Color.Orange,
            Size = new(16, 16),
            Tag = "limpet",
            ToolTip = "Don't forget limpets! The current ship has mining modules equipped, but no limpets in Cargo!",
            Visible = false,
        };

        // TODO: Auto-hide if no prospectors are alive or ship enters supercruise?
        internal ProspectorContent(HelmContext context, ToolTip sharedTtip) : base(context, sharedTtip)
        {
            InitializeComponent();

            dgvProspectorHistory.ColumnHeadersDefaultCellStyle.BackColor = dgvProspectorHistory.BackgroundColor;
            dgvProspectorHistory.ColumnHeadersDefaultCellStyle.ForeColor = dgvProspectorHistory.ForeColor;
            dgvProspectorHistory.RowHeadersDefaultCellStyle.BackColor = dgvProspectorHistory.BackgroundColor;
            dgvProspectorHistory.RowHeadersDefaultCellStyle.ForeColor = dgvProspectorHistory.ForeColor;
            dgvProspectorHistory.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
            dgvProspectorHistory.DefaultCellStyle.BackColor = dgvProspectorHistory.BackgroundColor;
            dgvProspectorHistory.DefaultCellStyle.ForeColor = dgvProspectorHistory.ForeColor;
            dgvProspectorHistory.BackgroundColorChanged += Grid_BackgroundColorChanged;
            dgvProspectorHistory.ForeColorChanged += Grid_ForeColorChanged;
            colMaterials.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            ContentTitle = "Prospectors";
            EnablePopout = true;

            tlblLimpetStatus.ToolTipManager = _toolTip;
            tlblLimpetStatus.AddImage(ICON_WARNING_SIGN);

            // TODO add buttons for statistics.

            InternalClear();

            _c.UIMgr.PropertyChanged += UIState_PropertyChanged;
            _suppressEvents = false;
        }

        protected override void InternalDraw()
        {
            // The list is ordered most recent first.
            List<ProspectorDetails> items = [.. _c.UIMgr.ForMode().ProspectedEvents
                .OrderByDescending(pd => pd.Event.TimestampDateTime)
                .Take(8)];

            dgvProspectorHistory.Rows.Clear();
            foreach (var item in items)
            {
                DataGridViewRow newRow = new()
                {
                    Tag = ProspectorHelper.GetEventFingerprint(item.Event)
                };
                newRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = item.Event.TimestampDateTime.ToLongTimeString(),
                });
                if (item.Event.Remaining == 0)
                {
                    newRow.Cells.Add(DGVImageLabelCell.New("Depleted"));
                }
                else
                {
                    newRow.Cells.Add(DGVImageLabelCell.New(
                        string.Join(
                            Environment.NewLine,
                            item.Event.Materials.Select(m => $"{CargoHelper.CommodityName(m.Name, m.Name_Localised)}: {m.Proportion:#.##}%")),
                        item.CommoditiesImage is not null ? [item.CommoditiesImage] : []));
                }
                newRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = GetMatContentDisplayName(item.Event.Content),
                });
                if (!string.IsNullOrEmpty(item.Event.MotherlodeMaterial))
                {
                    newRow.Cells.Add(
                        DGVImageLabelCell.New(CargoHelper.CommodityName(item.Event.MotherlodeMaterial, item.Event.MotherlodeMaterial_Localised),
                        item.CoreImage is not null ? [item.CoreImage] : []));
                }
                else
                {
                    newRow.Cells.Add(DGVImageLabelCell.New(string.Empty));
                }
                dgvProspectorHistory.Rows.Add(newRow);
            }

            UpdateLimpets();
        }

        private object GetMatContentDisplayName(string content)
        {
            switch (content)
            {
                case JournalConstants.MATERIAL_CONTENT_LOW:
                    return "Low";
                case JournalConstants.MATERIAL_CONTENT_MED:
                    return "Medium";
                case JournalConstants.MATERIAL_CONTENT_HIGH:
                    return "High";
                default:
                    return string.Empty;
            }
        }

        protected override void InternalClear()
        {
            dgvProspectorHistory.Rows.Clear();
        }

        private void UpdateLimpets()
        {
            var commanderData = _c.Data.For(_c.UIMgr.ForMode().CommanderKey);
            int limpetsCount = 0;
            bool enableLimpetsWarning = false;
            commanderData?.Ships.Cargo.TryGetValue(CargoHelper.LimpetDronesKey, out limpetsCount);
            var loadout = commanderData?.Ships.CurrentShip?.Loadout;
            if (loadout is not null && limpetsCount == 0)
                enableLimpetsWarning = Constants.MiningModuleCount(loadout) > Math.Max(1, _c.Settings.LimpetNotificationMinMiningModules);

            tlblLimpetStatus.Text = $"Limpets aboard: {limpetsCount}";
            tlblLimpetStatus.SetVisibility(ICON_WARNING_SIGN.Guid, enableLimpetsWarning);

            _isDirty = true;
        }

        private void Grid_ForeColorChanged(object sender, EventArgs e)
        {
            dgvProspectorHistory.ColumnHeadersDefaultCellStyle.ForeColor = dgvProspectorHistory.ForeColor;
            dgvProspectorHistory.RowHeadersDefaultCellStyle.ForeColor = dgvProspectorHistory.ForeColor;
            dgvProspectorHistory.DefaultCellStyle.ForeColor = dgvProspectorHistory.ForeColor;
        }

        private void Grid_BackgroundColorChanged(object sender, EventArgs e)
        {
            dgvProspectorHistory.ColumnHeadersDefaultCellStyle.BackColor = dgvProspectorHistory.BackgroundColor;
            dgvProspectorHistory.RowHeadersDefaultCellStyle.BackColor = dgvProspectorHistory.BackgroundColor;
            dgvProspectorHistory.DefaultCellStyle.BackColor = dgvProspectorHistory.BackgroundColor;
        }

        private void UIState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case UIStateManager.PROP_COMMANDERKEY:
                    InternalClear();
                    break;
                case UIStateManager.PROP_PROSPECTEDEVENTS:
                    Draw();
                    break;
                case UIStateManager.PROP_SHIP_CARGO:
                    UpdateLimpets();
                    break;
                case UIStateManager.PROP_SHIPID:
                    UpdateLimpets();
                    break;
            }
        }
    }
}
