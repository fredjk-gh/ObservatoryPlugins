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
        public static readonly ArchivistSettings DEFAULT = new()
        {
            ShareSystemData = true,
            JsonViewerFontSize = -1,
        };

        [SettingNewGroup("Plugin Interop")]
        [SettingDisplayName("Share data for known systems (WIP)")]
        public bool ShareSystemData { get; set; }

        [SettingNewGroup("UI")]
        [SettingDisplayName("Json viewer default font size")]
        [SettingNumericBounds(5, 24, 1, 1)]
        public int JsonViewerFontSize { get; set; }
    }
}
