using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    internal class AutoUpdaterSettings : INotifyPropertyChanged
    {
        public static readonly AutoUpdaterSettings DEFAULT = new()
        {
            UseBeta = false,
        };

        private bool _useBeta;

        [SettingDisplayName("Use beta versions")]
        public bool UseBeta
        {
            get => _useBeta;
            set
            {
                _useBeta = value;
                OnPropertyChanged();
            }
        }

        internal void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
