using System.ComponentModel;
using System.ComponentModel.Design;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using Observatory.Framework.Files.Journal;

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
            ToolTip = "Limpet warning",
            Visible = false,
        };

        // TODO: Auto-hide if no prospectors are alive or ship enters supercruise?
        internal ProspectorContent(HelmContext context) : base(context)
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

            tlblLimpetStatus.ToolTipManager = ttManager;
            tlblLimpetStatus.AddImage(ICON_WARNING_SIGN);

            // TODO add buttons for statistics.

            InternalClear();

            _c.UIMgr.PropertyChanged += UIState_PropertyChanged;
            _suppressEvents = false;
        }

        protected override void InternalDraw()
        {
            // The list is ordered most recent first.
            List<ProspectedAsteroid> items = [.. _c.UIMgr.ForMode().ProspectedEvents
                .OrderByDescending(pa => pa.TimestampDateTime)
                .Take(8)];

            dgvProspectorHistory.Rows.Clear();
            foreach (var item in items)
            {
                DataGridViewRow newRow = new()
                {
                    Tag = ProspectorHelper.GetEventFingerprint(item)
                };
                newRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = item.Timestamp,
                });
                if (item.Remaining == 0)
                {
                    newRow.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = "Depleted",
                    });
                }
                else
                {
                    newRow.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = string.Join(
                            Environment.NewLine,
                            item.Materials.Select(m => $"{CargoHelper.CommodityName(m.Name, m.Name_Localised)}: {m.Proportion:#.##}%")),
                    });
                }
                newRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = item.MotherlodeMaterial,
                });
                dgvProspectorHistory.Rows.Add(newRow);
            }

            UpdateLimpets();
        }

        protected override void InternalClear()
        {
            dgvProspectorHistory.Rows.Clear();
        }

        private void UpdateLimpets()
        {
            var commanderData = _c.Data.For(_c.UIMgr.ForMode().CommanderKey);

            if (commanderData is not null &&
                commanderData.Ships.Cargo.TryGetValue(CargoHelper.LimpetDronesKey, out var limpetsCount))
            {
                tlblLimpetStatus.Text = $"Limpets aboard: {limpetsCount}";
            }
            else
            {
                tlblLimpetStatus.Text = "Limpets aboard: 0";
            }
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
                case UIStateManager.PROP_CARGO:
                    UpdateLimpets();
                    break;
            }
        }
    }
}
