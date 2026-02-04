using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.System
{
    internal class UndiscoveredSystemTally : SystemRecord
    {
        private readonly HashSet<string> _visitedSystems = [];

        public UndiscoveredSystemTally(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Undiscovered")
        {
            _disallowedStates = [LogMonitorState.PreRead];
        }

        public override bool Enabled => Settings.EnableUndiscoveredSystemCountRecord;

        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "systems"; }
        public override Function MaxFunction { get => Function.Count; }


        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled) return [];

            TrackIsSystemUndiscovered(scan, currentSystem);
            if (scan.DistanceFromArrivalLS == 0 && scan.PlanetClass != Constants.SCAN_BARYCENTRE
                && IsUndiscoveredSystem.GetValueOrDefault(currentSystem) && !_visitedSystems.Contains(currentSystem))
            {
                _visitedSystems.Add(currentSystem);
                var newValue = Data.HasMax ? Data.MaxValue + 1 : 1;
                return CheckMax(NotificationClass.Tally, newValue, scan.TimestampDateTime, currentSystem);
            }
            return [];
        }

        public override void Reset()
        {
            base.Reset();
            _visitedSystems.Clear();
        }
    }
}
