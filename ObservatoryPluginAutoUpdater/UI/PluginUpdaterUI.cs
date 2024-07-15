using System;
using System.Collections.Generic;
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
        Install,
        Update,
    }

    internal partial class PluginUpdaterUI : UserControl
    {
        private UIContext _context;

        public PluginUpdaterUI(UIContext context)
        {
            InitializeComponent();

            _context = context;

            // build a dictionary of controls reflecting the current state and populate the layout table.

            int gridRow = 0;
            foreach (var p in _context.KnownPlugins)
            {
                gridRow++; // pre-increment to skip header row.

                Label nameLabel = new()
                {
                    Text = p,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                };
                Label installedVersionLabel = new()
                {
                    Text = "",
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                };
                Label statusLabel = new()
                {
                    Text = "",
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                };
                Button actionButton = new()
                {
                    Text = "",
                    Visible = false,
                    Size = new Size(150, 45),
                    Tag = p,
                };
                actionButton.Click += InstallOrUpdatePlugin_Click;

                var controls = new PluginRowUI(p, gridRow, nameLabel, installedVersionLabel, statusLabel, actionButton, tblLayout);
                _context.AddRowControls(controls);
            }
        }

        public void UpdatePluginState(string pluginName, string installedVersion, string status, PluginAction action)
        {
            _context.Core.ExecuteOnUIThread(() =>
            {
                var controls = _context.GetRowControls(pluginName);
                controls.InstalledVersionLabel.Text = installedVersion;
                controls.StatusLabel.Text = status;
                switch (action)
                {
                    case PluginAction.Install:
                        controls.Action.Text = "Install";
                        controls.Action.Visible = true;
                        break;
                    case PluginAction.Update:
                        controls.Action.Text = "Update";
                        controls.Action.Visible = true;
                        break;
                    default:
                        controls.Action.Visible = false;
                        break;
                }
            });
        }

        public void ShowMessages(string messages)
        {
            _context.Core.ExecuteOnUIThread(() =>
            {
                txtMessages.Text = messages;
            });
        }

        private void CheckForUpdates_Click(object sender, EventArgs e)
        {
            _context.CheckForUpdates(true);
        }

        private void InstallOrUpdatePlugin_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            string pluginName = btn.Tag as string;
            if (string.IsNullOrWhiteSpace(pluginName)) return;
            if (!_context.LatestVersions.ContainsKey(pluginName)) return; // No info!

            PluginVersion latest = _context.LatestVersions[pluginName];
            VersionPair selectedVersion = null;
            if (!_context.LocalVersions.ContainsKey(pluginName))
            {
                // Install.
                selectedVersion = PluginVersion.SelectVersion(null, latest, _context.Settings.UseBeta, _context.Core.Version);
            }
            else
            {
                // Upgrade.
                selectedVersion = PluginVersion.SelectVersion(_context.LocalVersions[pluginName], latest, _context.Settings.UseBeta, _context.Core.Version);
            }

            if (selectedVersion != null)
            {
                if (_context.DownloadLatestPlugin(_context.HttpClient, selectedVersion.Latest.DownloadURL, latest))
                {
                    btn.Enabled = false;
                    _context.RequiresRestart = true;
                }
            }
            ShowMessages(_context.GetMessages());
        }

        private void llblGitHubWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _context.OpenUrl(UIContext.WIKI_URL);
        }
    }
}
