using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.System
{
    internal class SystemBodyCountRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : SystemRecord(settings, recordKind, data, "Bodies")
    {
        public override bool Enabled => Settings.EnableSystemBodyCountRecords;

        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "bodies"; }

        public override List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, Dictionary<int, Scan> scans)
        {
            if (!Enabled) return [];

            return CheckMax(NotificationClass.PersonalBest, allBodiesFound.Count, allBodiesFound.TimestampDateTime, allBodiesFound.SystemName);
        }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled) return [];

            TrackIsSystemUndiscovered(scan, currentSystem);

            return [];
        }
    }
}
