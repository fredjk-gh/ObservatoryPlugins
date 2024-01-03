using Observatory.Framework;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    internal class FleetCommanderSettings
    {
        [SettingDisplayName("Notify after jump")]
        public bool NotifyJumpComplete { get; set; }

        [SettingDisplayName("Notify after jump cooldown")]
        public bool NotifyJumpCooldown { get; set; }

        [SettingDisplayName("Notify when below 135 T remaining")]
        public bool NotifyLowFuel { get; set; }
    }
}