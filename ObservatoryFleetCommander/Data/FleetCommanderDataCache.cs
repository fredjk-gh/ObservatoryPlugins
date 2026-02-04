namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    internal class FleetCommanderDataCache
    {
        internal const long CURRENT_FILE_VERSION = 2;

        public static FleetCommanderDataCache FromManager(Manager manager)
        {
            FleetCommanderDataCache serializeable = new()
            {
                FileVersion = CURRENT_FILE_VERSION,
                KnownCommanders = manager.Commanders,
                Squadrons = manager.Squadrons,
            };

            return serializeable;
        }

        public long FileVersion { get; set; }
        public List<CommanderData> KnownCommanders { get; set; }
        public List<SquadronData> Squadrons { get; set; }
    }
}
