using com.github.fredjk_gh.ObservatoryHelm.Data;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Forms
{
    public partial class SuppressionZoneInfoForm : Form
    {
        private readonly List<SuppressionSpec> _allSpecs = [];

        public SuppressionZoneInfoForm()
        {
            InitializeComponent();
            dgvZoneInfo.BackgroundColorChanged += DataGridView_AColorChanged;
            dgvZoneInfo.ForeColorChanged += DataGridView_AColorChanged;

            _allSpecs.AddRange(SuppressionSpec.StarType);
            _allSpecs.Add(SuppressionSpec.Helium);
            _allSpecs.Add(SuppressionSpec.Bubble);

            ShowData();
        }

        private void DataGridView_AColorChanged(object sender, EventArgs e)
        {
            dgvZoneInfo.ColumnHeadersDefaultCellStyle.BackColor = dgvZoneInfo.BackgroundColor;
            dgvZoneInfo.ColumnHeadersDefaultCellStyle.ForeColor = dgvZoneInfo.ForeColor;
            dgvZoneInfo.RowHeadersDefaultCellStyle.BackColor = dgvZoneInfo.BackgroundColor;
            dgvZoneInfo.RowHeadersDefaultCellStyle.ForeColor = dgvZoneInfo.ForeColor;
            ShowData();
        }

        private void ShowData()
        {
            dgvZoneInfo.Rows.Clear();

            foreach (var spec in _allSpecs)
            {
                AddRow(spec.Name, spec.Description);
            }
        }

        private DataGridViewRow AddRow(string name, string value)
        {
            DataGridViewRow row = new()
            {
                DefaultCellStyle = new()
                {
                    BackColor = dgvZoneInfo.BackgroundColor,
                    ForeColor = dgvZoneInfo.ForeColor,
                    WrapMode = DataGridViewTriState.True,
                }
            };

            row.Cells.Add(new DataGridViewTextBoxCell() { Value = name });
            row.Cells.Add(new DataGridViewTextBoxCell() { Value = value });

            dgvZoneInfo.Rows.Add(row);
            return row;
        }

    }
}
