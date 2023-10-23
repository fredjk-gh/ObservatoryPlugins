using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ObservatoryStatScanner.StatScannerSettings;

namespace ObservatoryStatScanner.Records
{
    internal class SystemBodyCountRecord : SystemRecord
    {
        public SystemBodyCountRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Body count")
        { }

        public override bool Enabled => Settings.EnableSystemBodyCountRecords;

        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "bodies"; }

        public override List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans)
        {
            if (!Enabled) return new();

            return CheckMax(NotificationClass.PersonalBest, allBodiesFound.Count, allBodiesFound.Timestamp, allBodiesFound.SystemName);
        }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled) return new();

            TrackIsSystemUndiscovered(scan, currentSystem);

            return new();
        }
    }
}
