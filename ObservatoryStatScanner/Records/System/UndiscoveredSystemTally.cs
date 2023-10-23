using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class UndiscoveredSystemTally : SystemRecord
    {
        private readonly HashSet<string> _visitedSystems = new();

        public UndiscoveredSystemTally(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Undiscovered")
        {
            _disallowedStates = new() { LogMonitorState.PreRead };
        }

        public override bool Enabled => Settings.EnableUndiscoveredSystemCountRecord;

        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "systems"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled) return new();

            TrackIsSystemUndiscovered(scan, currentSystem);
            if (scan.DistanceFromArrivalLS == 0 && scan.PlanetClass != Constants.SCAN_BARYCENTRE
                && IsUndiscoveredSystem.GetValueOrDefault(currentSystem) && !_visitedSystems.Contains(currentSystem))
            {
                _visitedSystems.Add(currentSystem);
                var newValue = (Data.HasMax ? Data.MaxValue + 1 : 1);
                return CheckMax(NotificationClass.Tally, newValue, scan.Timestamp, currentSystem);
            }
            return new();
        }
    }
}
