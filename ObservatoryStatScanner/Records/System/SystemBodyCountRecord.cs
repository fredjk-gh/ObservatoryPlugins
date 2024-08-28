using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class SystemBodyCountRecord : SystemRecord
    {
        public SystemBodyCountRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Bodies")
        { }

        public override bool Enabled => Settings.EnableSystemBodyCountRecords;

        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "bodies"; }

        public override List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans)
        {
            if (!Enabled) return new();

            return CheckMax(NotificationClass.PersonalBest, allBodiesFound.Count, allBodiesFound.TimestampDateTime, allBodiesFound.SystemName);
        }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled) return new();

            TrackIsSystemUndiscovered(scan, currentSystem);

            return new();
        }
    }
}
