using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryHelm
{
    class TrackedData
    {
        public TrackedData()
        {
            Scans = new();
        }

        public bool IsOdyssey { get; set; }
        public double FuelCapacity { get; set; }
        public double FuelRemaining { get; set; }
        public double MaxFuelPerJump { get; set; }
        public string? CurrentSystem { get; set; }
        public double DistanceTravelled { get; set; }
        public int JumpsRemainingInRoute { get; set; }
        public string? NeutronPrimarySystem { get; set; }
        public Scan? ScoopableSecondaryCandidateScan { get; set; }
        public string? NeutronPrimarySystemNotified { get; set; }
        public string? FuelWarningNotifiedSystem { get; set; }
        public Dictionary<string, Scan> Scans { get; private set; }

        public void SystemReset(string systemName, double fuelLevel, double jumpDist)
        {
            Scans.Clear();
            CurrentSystem = systemName;
            FuelRemaining = fuelLevel;
            DistanceTravelled += jumpDist;
        }

        public void SessionReset(bool isOdyssey, double fuelCapacity, double fuelLevel)
        {
            IsOdyssey = isOdyssey;
            FuelCapacity = fuelCapacity;
            FuelRemaining = fuelLevel;
            JumpsRemainingInRoute = 0;
            DistanceTravelled = 0;
        }
    }
}
