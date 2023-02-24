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

        public override List<StatScannerGrid> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans)
        {
            if (!Enabled) return new();

            return CheckMax(allBodiesFound.Count, allBodiesFound.Timestamp, allBodiesFound.SystemName);
        }

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled) return new();

            TrackIsSystemUndiscovered(scan);

            return new();
        }
    }
}
