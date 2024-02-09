using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    internal class AutoUpdaterSettings
    {
        public static readonly AutoUpdaterSettings DEFAULT = new()
        {
            UseBeta = false,
            Aggregator = false,
            Archivist = false,
            FleetCommander = false,
            Helm = false,
            ProspectorBasic = false,
            StatScanner = false,
        };

        [SettingDisplayName("Use beta versions")]
        public bool UseBeta { get; set; }

        [SettingNewGroup("Available plugins (requires restart to install; not yet implemented)")]
        [SettingDisplayName("Install Aggregator")]
        public bool Aggregator { get; set; }

        [SettingDisplayName("Install Archivist")]
        public bool Archivist { get; set; }

        [SettingDisplayName("Install FleetCommander")]
        public bool FleetCommander { get; set; }

        [SettingDisplayName("Install Helm")]
        public bool Helm { get; set; }

        [SettingDisplayName("Install Prospector Basic")]
        public bool ProspectorBasic { get; set; }

        [SettingDisplayName("Install StatScanner")]
        public bool StatScanner { get; set; }
        
        [SettingDisplayName("Download updates now")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action UpdateNow { get; internal set; }

        [SettingDisplayName("Open GitHub Wiki")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action OpenGitHub { get; internal set; }
    }
}
