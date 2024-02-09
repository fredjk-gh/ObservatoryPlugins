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
        };

        [SettingNewGroup("Plugin Interop")]
        [SettingDisplayName("Share data for known systems")]
        public bool ShareSystemData { get; set; }
    }
}
