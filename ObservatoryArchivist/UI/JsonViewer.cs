using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.Data;

namespace com.github.fredjk_gh.ObservatoryArchivist.UI
{
    public partial class JsonViewer : Form
    {
        private readonly ArchivistContext _context;

        internal JsonViewer(ArchivistContext context)
        {
            InitializeComponent();
            btnFontSizeIcon.SetOriginalImage(Images.TextSizeImage, new(32, 32));

            _context = context;

            tbFontSize.Value = Convert.ToInt32(context.Settings.JsonViewerFontSize);
            SetTextFontSize(context.Settings.JsonViewerFontSize);
            txtJson.Text = string.Empty;
        }

        public void ViewJson(string json)
        {
            txtJson.Text = JsonHelper.PrettyPrintJson(json);
            txtJson.Select(0, 0);
        }

        private void SetTextFontSize(int jsonViewerFontSize)
        {
            txtJson.Font = new Font(txtJson.Font.FontFamily, (float)jsonViewerFontSize);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            txtJson.Text = string.Empty;
            Close();
        }

        private void TbFontSize_Scroll(object sender, EventArgs e)
        {
            try
            {
                SetTextFontSize(tbFontSize.Value);
                ttip.SetToolTip(tbFontSize, $"Font Size: {tbFontSize.Value}");
                _context.Settings.JsonViewerFontSize = tbFontSize.Value;
                _context.Core.SaveSettings(_context.Worker);
            }
            catch (Exception ex)
            {
                _context.Core.GetPluginErrorLogger(_context.Worker)(ex, "Saving new font size setting");
            }
        }
    }
}
