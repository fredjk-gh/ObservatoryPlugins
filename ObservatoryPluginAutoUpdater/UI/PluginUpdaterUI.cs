using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI
{
    public enum PluginAction
    {
        None,
        InstallStable,
        InstallBeta,
        UpdateStable,
        UpdateBeta,
    }

    internal partial class PluginUpdaterUI : UserControl
    {
        // Properties of PluginRowUI which are not listed here will be hidden.
        private readonly Dictionary<string, string> ColumnHeaderText = new()
        {
            { "PluginName" , "Plugin Name" },
            { "InstalledVersion" , "Installed Version" },
            { "StableVersion" , "Stable Version" },
            { "BetaVersion" , "Beta Version" },
            { "Status" , "Status" },
        };
        private const string COL_ACTION = "Action";

        private readonly UIContext _context;
        private readonly Dictionary<string, PluginRowUI> _pluginUI = new();
        private readonly BindingList<PluginRowUI> _rows = new();

        public PluginUpdaterUI(UIContext context)
        {
            InitializeComponent();

            _context = context;
            _context.Settings.PropertyChanged += Settings_PropertyChanged;

            // From: https://stackoverflow.com/questions/3339300/net-tablelayoutpanel-clearing-controls-is-very-slow
            typeof(TableLayoutPanel)
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(tblLayout, true, null);

            chkAllowBeta.Checked = _context.Settings.UseBeta;
            chkAllowBeta.CheckedChanged += chkAllowBeta_CheckedChanged;

            dgvPlugins.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvPlugins.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvPlugins.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPlugins.EnableHeadersVisualStyles = false;
            dgvPlugins.BackgroundColorChanged += Grid_BackgroundColorChanged;
            dgvPlugins.ForeColorChanged += Grid_ForeColorChanged;
            dgvPlugins.CellClick += Grid_CellClick;

            // build a dictionary of plugins to grid rows.
            foreach (var p in _context.KnownPlugins)
            {
                var rowUi = new PluginRowUI(p.Key);
                _rows.Add(rowUi);
                _pluginUI.Add(rowUi.PluginName, rowUi);
            }
            ApplyCellStyle();
            dgvPlugins.DataSource = _rows;

            foreach (DataGridViewColumn c in dgvPlugins.Columns)
            {
                if (ColumnHeaderText.ContainsKey(c.Name))
                {
                    c.HeaderText = ColumnHeaderText[c.Name];
                }
                else
                {
                    c.Visible = false;
                }
            }
            int colIndex = dgvPlugins.Columns.Add(new DataGridViewButtonColumn()
            {
                Name = COL_ACTION,
                HeaderText = "Action",
                FlatStyle = FlatStyle.Flat,
            });
        }

        public void UpdatePluginState(string pluginName, string installedVersion, string status, PluginVersion latest, PluginAction action)
        {
            _context.Core.ExecuteOnUIThread(() =>
            {
                var controls = _pluginUI[pluginName];
                if (controls.PendingRestart) return; // Do nothing, we've already downloaded this one.

                controls.InstalledVersion = installedVersion;
                controls.StableVersion = latest.Production?.Version ?? "";
                controls.BetaVersion = latest.Beta?.Version ?? "";
                controls.Status = status;
                controls.PluginAction = action;

                var row = dgvPlugins.Rows.Cast<DataGridViewRow>().Where(r => r.DataBoundItem == controls).FirstOrDefault();
                if (row == null)
                {
                    return; // This is unexpected.
                }
                DataGridViewButtonCell buttonCell = (DataGridViewButtonCell)row.Cells[COL_ACTION];

                switch (action)
                {
                    case PluginAction.InstallStable:
                        buttonCell.Value = "Install Stable";
                        break;
                    case PluginAction.InstallBeta:
                        buttonCell.Value = "Install Beta";
                        break;
                    case PluginAction.UpdateStable:
                        buttonCell.Value = "Update Stable";
                        break;
                    case PluginAction.UpdateBeta:
                        buttonCell.Value = "Update Beta";
                        break;
                    default:
                        buttonCell.Value = "";
                        break;
                }
                dgvPlugins.AutoResizeColumns();
            });
        }

        public void ShowMessages(string messages)
        {
            _context.Core.ExecuteOnUIThread(() =>
            {
                txtMessages.Text = messages;
                txtMessages.Select(txtMessages.Text.Length, 0);
                txtMessages.ScrollToCaret();
            });
        }

        public override void Refresh()
        {
            _context.Core.ExecuteOnUIThread(() =>
            {
                base.Refresh();
            });
        }

        private void ApplyCellStyle()
        {
            dgvPlugins.DefaultCellStyle = new()
            {
                BackColor = dgvPlugins.BackgroundColor,
            };
        }
        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _context.Core.ExecuteOnUIThread(() =>
            {
                switch (e.PropertyName)
                {
                    case "UseBeta":
                        chkAllowBeta.Checked = _context.Settings.UseBeta;
                        break;
                }
            });
        }

        private void chkAllowBeta_CheckedChanged(object sender, EventArgs e)
        {
            _context.Settings.UseBeta = chkAllowBeta.Checked;
            _context.Core.SaveSettings(_context.Worker);
        }

        private void Grid_BackgroundColorChanged(object sender, EventArgs e)
        {
            dgvPlugins.SuspendLayout();
            dgvPlugins.ColumnHeadersDefaultCellStyle.BackColor = dgvPlugins.BackgroundColor;
            dgvPlugins.RowHeadersDefaultCellStyle.BackColor = dgvPlugins.BackgroundColor;

            // Propagate to Default cell style.
            ApplyCellStyle();
            dgvPlugins.ResumeLayout();
        }

        private void Grid_ForeColorChanged(object sender, EventArgs e)
        {
            dgvPlugins.ColumnHeadersDefaultCellStyle.ForeColor = dgvPlugins.ForeColor;
            dgvPlugins.RowHeadersDefaultCellStyle.ForeColor = dgvPlugins.ForeColor;
        }

        private void CheckForUpdates_Click(object sender, EventArgs e)
        {
            _context.CheckForUpdates(true);
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridViewRow row = dgvPlugins.Rows[e.RowIndex];
            DataGridViewButtonCell buttonCell = row.Cells[e.ColumnIndex] as DataGridViewButtonCell;

            if (buttonCell == null) return; // Not a button cell.
            PluginRowUI data = row.DataBoundItem as PluginRowUI;
            if (data == null || data.PluginAction == PluginAction.None) return;

            string pluginName = data.PluginName;
            if (!_context.LatestVersions.ContainsKey(pluginName))
            {
                _context.AddMessage("No applicable version found to download!");
                return; // No info!
            }

            PluginVersion latest = _context.LatestVersions[pluginName];
            VersionDetail selectedVersion = null;

            switch (data.PluginAction)
            {
                case PluginAction.InstallStable:
                case PluginAction.UpdateStable:
                    selectedVersion = latest.Production;
                    break;
                case PluginAction.InstallBeta:
                case PluginAction.UpdateBeta:
                    selectedVersion = latest.Beta;
                    break;
            }

            if (selectedVersion != null)
            {
                if (_context.DownloadLatestPlugin(_context.HttpClient, selectedVersion.DownloadURL, latest))
                {
                    _context.RequiresRestart = true;
                    buttonCell.Value = "";
                    data.PluginAction = PluginAction.None;
                    data.Status = "Pending restart";
                    data.PendingRestart = true; // Prevent this information from being clobbered.
                    dgvPlugins.AutoResizeColumns();
                }
            }
            else
            {
                _context.AddMessage("No applicable version found to download!");
            }
            ShowMessages(_context.GetMessages());
        }

        private void llblGitHubWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _context.OpenUrl(UIContext.WIKI_URL);
        }
    }
}
