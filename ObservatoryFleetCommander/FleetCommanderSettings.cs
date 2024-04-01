using Observatory.Framework;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    internal class FleetCommanderSettings
    {
        public static readonly FleetCommanderSettings DEFAULT = new ()
        {
            NotifyJumpComplete = false,
            NotifyJumpCooldown = true,
            NotifyLowFuel = true,
        };

        [SettingNewGroup("Notifications")]
        [SettingDisplayName("Notify jump")]
        public bool NotifyJumpComplete { get; set; }

        [SettingDisplayName("Notify after jump cooldown")]
        public bool NotifyJumpCooldown { get; set; }

        [SettingDisplayName("Notify when below 135 T fuel remaining")]
        public bool NotifyLowFuel { get; set; }

        [SettingNewGroup("Fleet Carrier Tools")]
        [SettingDisplayName("Enable Jump/Cooldown Countdown Popout")]
        public bool EnableRealtimeCountdown { get; set; }

        [SettingDisplayName("Plot route via Spansh")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action PlotCarrierRoute { get; internal set; }

        [SettingDisplayName("Fix routing problem")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action FixCarrierRoute { get; internal set; }
    }
}