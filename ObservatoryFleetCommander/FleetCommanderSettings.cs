using Observatory.Framework;

namespace ObservatoryFleetCommander.Worker
{
    internal class FleetCommanderSettings
    {
        [SettingDisplayName("Notify after jump cooldown")]
        public bool NotifyJumpCooldown { get; set; }

        [SettingDisplayName("Notify when below 135 T remaining")]
        public bool NotifyLowFuel { get; set; }
    }
}