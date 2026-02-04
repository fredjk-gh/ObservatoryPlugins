using com.github.fredjk_gh.PluginCommon.Data.Id64;

namespace com.github.fredjk_gh.PluginCommon.UI.Forms
{
    public partial class Id64Viewer : Form
    {
        private readonly Id64Details _id64Details;
        private readonly string _commonSystemName;

        public Id64Viewer(Id64Details details, string commonSystemName)
        {
            InitializeComponent();
            _id64Details = details;
            _commonSystemName = commonSystemName;

            dgvId64.BackgroundColorChanged += DataGridView_AColorChanged;
            dgvId64.ForeColorChanged += DataGridView_AColorChanged;
        }

        private void DataGridView_AColorChanged(object sender, EventArgs e)
        {
            dgvId64.ColumnHeadersDefaultCellStyle.BackColor = dgvId64.BackgroundColor;
            dgvId64.ColumnHeadersDefaultCellStyle.ForeColor = dgvId64.ForeColor;
            dgvId64.RowHeadersDefaultCellStyle.BackColor = dgvId64.BackgroundColor;
            dgvId64.RowHeadersDefaultCellStyle.ForeColor = dgvId64.ForeColor;
            ShowData();
        }

        private void ShowData()
        {
            dgvId64.Rows.Clear();
            lblTitle.Text = $"Id64 details for {_commonSystemName}";

            var allValues = _id64Details.Values();
            foreach (var v in allValues.AllKeys)
            {
                AddRow(v, allValues[v]);
            }
        }

        private DataGridViewRow AddRow(string name, string value)
        {
            DataGridViewRow row = new()
            {
                DefaultCellStyle = new()
                {
                    BackColor = dgvId64.BackgroundColor,
                    ForeColor = dgvId64.ForeColor,
                }
            };

            row.Cells.Add(new DataGridViewTextBoxCell() { Value = name });
            row.Cells.Add(new DataGridViewTextBoxCell() { Value = value });

            dgvId64.Rows.Add(row);
            return row;
        }
    }
}
