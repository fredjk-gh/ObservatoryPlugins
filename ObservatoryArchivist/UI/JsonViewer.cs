using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.github.fredjk_gh.ObservatoryArchivist.UI
{
    public partial class JsonViewer : Form
    {
        private ArchivistContext _context;
        internal JsonViewer(ArchivistContext context)
        {
            InitializeComponent();
            btnFontSizeIcon.SetIcon(Properties.Resources.FontSizeIcon.ToBitmap(), new(32, 32));

            _context = context;

            tbFontSize.Value = Convert.ToInt32(context.Settings.JsonViewerFontSize);
            SetTextFontSize(context.Settings.JsonViewerFontSize);
            txtJson.Text = string.Empty;
        }

        public void ViewJson(string json)
        {
            txtJson.Text = PrettyPrintJson(json);
            txtJson.Select(0, 0);
        }

        private string PrettyPrintJson(string rawJson)
        {
            JsonSerializerOptions opts = new()
            {
                WriteIndented = true,
                AllowTrailingCommas = true,
            };
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(rawJson);

            return JsonSerializer.Serialize(jsonElement, opts);
        }

        private void SetTextFontSize(int jsonViewerFontSize)
        {
            txtJson.Font = new Font(txtJson.Font.FontFamily, (float)tbFontSize.Value);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            txtJson.Text = string.Empty;
            Close();
        }

        private void tbFontSize_Scroll(object sender, EventArgs e)
        {
            try
            {
                SetTextFontSize(tbFontSize.Value);
                ttip.SetToolTip(tbFontSize, $"Font Size: {tbFontSize.Value}");
                _context.Settings.JsonViewerFontSize = tbFontSize.Value;
                _context.Core.SaveSettings(_context.PluginWorker);
            }
            catch (Exception ex)
            {
                _context.Core.GetPluginErrorLogger(_context.PluginWorker)(ex, "Saving new font size setting");
            }
        }
    }
}
