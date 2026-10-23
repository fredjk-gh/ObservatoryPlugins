using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    public enum TransitModeType
    {
        Unknown,
        OwnShip,
        ShipLaunchedVessel, // SRV, Fighter? or Nomad.
        OnFoot, // Other than Carrier or station.
        FleetCarrier,
        Station,
        TaxiOrOtherShip,
        Multicrew
    }

    internal class TransitMode
    {
        internal TransitModeType PreviousMode { get; set; } = TransitModeType.Unknown;
        internal bool Docked { get; set; } = false;
        internal bool Taxi { get; set; } = false;
        internal bool Multicrew { get; set; } = false;
        internal bool InSRV { get; set; } = false;
        internal bool OnFoot { get; set; } = false;
        internal string StationType { get; set; } = "";

        internal TransitModeType ToEnum()
        {
            TransitModeType mode = TransitModeType.OwnShip; // Early days of Elite only had this mode. Start here.
            if (InSRV) mode = TransitModeType.ShipLaunchedVessel;
            else if (OnFoot)
            {
                if (PreviousMode == TransitModeType.FleetCarrier) mode = TransitModeType.FleetCarrier;
                mode = TransitModeType.OnFoot;
            }
            else if (Docked)
            {
                if (StationType == "FleetCarrier")
                    mode = TransitModeType.FleetCarrier;
                else
                    mode = TransitModeType.Station;
            }
            else if (Taxi) mode = TransitModeType.TaxiOrOtherShip;
            else if (Multicrew) mode = TransitModeType.Multicrew;

            return mode;
        }

        internal TransitMode WithPrevious(TransitModeType mode)
        {
            PreviousMode = mode;
            return this;
        }

        internal static TransitMode FromJournal(Location location)
        {
            return new()
            {
                Docked = location.Docked,
                Taxi = location.Taxi,
                InSRV = location.InSRV,
                OnFoot = location.OnFoot,
                StationType = location.StationType,
            };
        }

        internal static TransitMode FromJournal(Docked docked)
        {
            return new()
            {
                Docked = true,
                Taxi = docked.Taxi,
                StationType = docked.StationType,
            };
        }

        internal static TransitMode FromJournal(Undocked undocked)
        {
            return new()
            {
                Docked = false,
                Taxi = undocked.Taxi,
            };
        }

        internal static TransitMode FromJournal(Disembark embarkOrDisembark)
        {
            if (embarkOrDisembark is Embark)
            {
                return new()
                {
                    Taxi = embarkOrDisembark.Taxi,
                    InSRV = embarkOrDisembark.SRV,
                    OnFoot = embarkOrDisembark.OnPlanet,
                };
            }
            // FWIW, dis/embarking on a FleetCarrier is incorrectly represented in journals (represented as OnPlanet = true with no station info :/).
            // We'd need to know that the previous state was docked on FleetCarrier to correctly deduce this.
            return new()
            {
                Taxi = embarkOrDisembark.Taxi,
                InSRV = embarkOrDisembark.SRV,
                OnFoot = embarkOrDisembark.OnPlanet,
                StationType = embarkOrDisembark.StationType,
            };
        }
    }
}
