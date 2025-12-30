using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class ArchivistSettings
    {
        public ArchivistSettings()
        {
            // Defaults go here.
            ShareSystemData = true;
            JsonViewerFontSize = -1;
            EnableAutoUpdates = true;
        }

        [SettingNewGroup("Plugin Interop")]
        [SettingDisplayName("Share data for known systems")]
        public bool ShareSystemData { get; set; }

        [SettingNewGroup("UI")]
        [SettingDisplayName("Json viewer default font size")]
        [SettingNumericBounds(5, 24, 1, 1)]
        public int JsonViewerFontSize { get; set; }

        [SettingNewGroup("Updates")]
        [SettingDisplayName("Enable automatic updates")]
        public bool EnableAutoUpdates { get; set; }

        [SettingDisplayName("Enable Beta versions (warning: things may break)")]
        public bool EnableBetaUpdates { get; set; }
    }
}
