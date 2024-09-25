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
        public JsonViewer()
        {
            InitializeComponent();

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

        private void btnClose_Click(object sender, EventArgs e)
        {
            txtJson.Text = string.Empty;
            Close();
        }
    }
}
