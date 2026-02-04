using Observatory.Framework;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    internal class FleetCommanderSettings
    {
        public FleetCommanderSettings()
        {
            // Defaults go here
            CooldownNotificationSoundFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}{System.IO.Path.DirectorySeparatorChar}example.mp3";
            NotifyJumpCooldown = true;
            NotifyLowFuel = true;
            UICardsAreDefaultExpanded = true;
            EnableAutoUpdates = true;
        }

        [SettingNewGroup("Notifications")]
        [SettingDisplayName("Notify jump")]
        public bool NotifyJumpComplete { get; set; }

        [SettingDisplayName("Notify after jump cooldown")]
        public bool NotifyJumpCooldown { get; set; }

        [SettingDisplayName("Cooldown notification sound file (optional)")]
        [JsonIgnore]
        public FileInfo CooldownNotificationSound
        {
            get => new(CooldownNotificationSoundFile);
            set => CooldownNotificationSoundFile = value.FullName;
        }

        [SettingIgnore]
        public string CooldownNotificationSoundFile { get; set; }

        [SettingDisplayName("Notify when below 135 T fuel remaining")]
        public bool NotifyLowFuel { get; set; }

        [SettingNewGroup("UI")]
        [SettingDisplayName("Expand Carrier cards by default")]
        public bool UICardsAreDefaultExpanded { get; set; }

        [SettingDisplayName("Use In-game times (not local)")]
        public bool UIDateTimesUseInGameTime { get; set; }

        [SettingDisplayName("Reset Countdown window size/position")]
        [JsonIgnore]
        public Action ClearCountdownWindowSizePosition { get; internal set; }

        [SettingNewGroup("Updates")]
        [SettingDisplayName("Enable automatic updates")]
        public bool EnableAutoUpdates { get; set; }

        [SettingDisplayName("Enable Beta versions (warning: things may break)")]
        public bool EnableBetaUpdates { get; set; }


        #region Hidden settings
        [SettingIgnore]
        public int CountdownWindowX { get; set; }

        [SettingIgnore]
        public int CountdownWindowY { get; set; }

        [SettingIgnore]
        public int CountdownWindowWidth { get; set; }

        [SettingIgnore]
        public int CountdownWindowHeight { get; set; }
        #endregion
    }
}
