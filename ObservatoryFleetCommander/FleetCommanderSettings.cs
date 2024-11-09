using Observatory.Framework;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    internal class FleetCommanderSettings
    {
        public static readonly FleetCommanderSettings DEFAULT = new()
        {
            NotifyJumpComplete = false,
            NotifyJumpCooldown = true,
            NotifyLowFuel = true,
            UICardsAreDefaultExpanded = true,
        };

        [SettingNewGroup("Notifications")]
        [SettingDisplayName("Notify jump")]
        public bool NotifyJumpComplete { get; set; }

        [SettingDisplayName("Notify after jump cooldown")]
        public bool NotifyJumpCooldown { get; set; }

        [SettingDisplayName("Notify when below 135 T fuel remaining")]
        public bool NotifyLowFuel { get; set; }

        [SettingNewGroup("UI")]
        [SettingDisplayName("Expand Carrier cards by default")]
        public bool UICardsAreDefaultExpanded { get; set; }

        [SettingDisplayName("Reset Countdown window size/position")]
        [JsonIgnore]
        public Action ClearCountdownWindowSizePosition { get; internal set; }


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
