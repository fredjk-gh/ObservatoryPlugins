using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    internal class AutoUpdaterSettings
    {
        public AutoUpdaterSettings()
        {
            Enabled = false;
            UseBeta = false;
        }

        [SettingDisplayName("Enabled (requires restart when enabling)")]
        public bool Enabled { get; set; }

        [SettingDisplayName("Use beta versions")]
        public bool UseBeta { get; set; }

    }
}
