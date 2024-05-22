using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI
{
    internal class PluginRowUI
    {
        private TableLayoutPanel _panel;

        public PluginRowUI(string name, int row, Label nameLabel, Label versionLabel, Label statusLabel, Button action, TableLayoutPanel panel)
        {
            PluginName = name;
            GridRow = row;
            NameLabel = nameLabel;
            InstalledVersionLabel = versionLabel;
            StatusLabel = statusLabel;
            Action = action;
            _panel = panel;

            _panel.Controls.Add(NameLabel, 0, row);
            _panel.Controls.Add(InstalledVersionLabel, 1, row);
            _panel.Controls.Add(StatusLabel, 2, row);
            _panel.Controls.Add(Action, 3, row);
            
            IsInstalled = false;
            HasUpdate = false;
        }

        public string PluginName { get; set; }
        public int GridRow { get; set; }

        public Label NameLabel { get; set; }
        public Label InstalledVersionLabel { get; set; }
        public Label StatusLabel { get; set; }
        public Button Action { get; set; }

        public bool IsInstalled { get; set; }
        public bool HasUpdate { get; set; }
    }
}
