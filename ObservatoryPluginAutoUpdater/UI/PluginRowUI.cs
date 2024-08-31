using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI
{
    internal class PluginRowUI : INotifyPropertyChanged
    {
        private string _pluginName;
        private string _installedVersion;
        private string _stableVersion;
        private string _betaVersion;
        private string _status;

        public PluginRowUI(string name)
        {
            PluginName = name;
            InstalledVersion = "unknown";
            StableVersion = "";
            BetaVersion = "";
            Status = "unknown";
            PluginAction = PluginAction.None;
        }

        public string PluginName {
            get => _pluginName;
            set {
                _pluginName = value;
                OnPropertyChanged();
            }
        }

        public string InstalledVersion
        {
            get => _installedVersion;
            set { 
                _installedVersion = value;
                OnPropertyChanged();
            }
        }

        public string StableVersion
        { 
            get => _stableVersion;
            set
            {
                _stableVersion = value;
                OnPropertyChanged();
            }
        }
        public string BetaVersion
        {
            get => _betaVersion;
            set
            {
                _betaVersion = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        internal PluginAction PluginAction { get; set; }
        internal bool PendingRestart { get; set; }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
