using Observatory.Framework;

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

        [SettingNewGroup("")]
        [SettingDisplayName("Expand Carrier cards by default")]
        public bool UICardsAreDefaultExpanded { get; set; }
    }
}