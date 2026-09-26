using Observatory.Framework;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class ArchivistSettings
    {
        public const string FALLBACK_SHARE_METHOD = "No sharing";
        public const string DEFAULT_SHARE_METHOD = "Plugin Interop Message (selected fredjk-gh plugin support only)";

        private static readonly Dictionary<string, object> SHARE_METHOD_VALUES = new()
        {
            { FALLBACK_SHARE_METHOD , ShareMethod.None },
            { DEFAULT_SHARE_METHOD , ShareMethod.InteropMessage },
            { "Journal Replay (affects all plugins)" , ShareMethod.JournalReplay },
        };

        public enum ShareMethod
        {
            None,
            InteropMessage,
            JournalReplay,
        }

        public ArchivistSettings()
        {
            // Defaults go here.
            ShareSystemData = true;
            DataShareMethodEnum = ShareMethod.InteropMessage;
            AutoFetchWellKnownSystemsFromSpansh = false;
            JsonViewerFontSize = -1;
            EnableAutoUpdates = true;
        }

        [SettingNewGroup("Plugin Interop")]
        [SettingDisplayName("Share data for known systems (Replaced by Data Sharing Method)")]
        [SettingIgnore] // Keep for backwards compatibility and migration.
        public bool ShareSystemData { get; set; }

        [SettingDisplayName("Data Sharing Method")]
        [SettingBackingValue("DataShareMethod")]
        public Dictionary<string, object> DataShareMethodOptions
        {
            get => SHARE_METHOD_VALUES;
        }

        [SettingIgnore]
        public string DataShareMethod { get; set; }

        [SettingIgnore]
        public ShareMethod DataShareMethodEnum
        {
            get => (ArchivistSettings.ShareMethod)DataShareMethodOptions[DataShareMethod];
            set
            {
                DataShareMethod = DataShareMethodOptions.Where(e => (ShareMethod)e.Value == value).Select(e => e.Key).First();
            }
        }

        [SettingDisplayName("Auto-fetch data for \"well-known\" bubble systems from Spansh")]
        public bool AutoFetchWellKnownSystemsFromSpansh { get; set; }

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
