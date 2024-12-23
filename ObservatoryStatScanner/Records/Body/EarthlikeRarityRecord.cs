using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class EarthlikeRarityRecord : BodyRecord
    {
        private List<Scan> _elws = new();

        public EarthlikeRarityRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData csvData)
            : base(settings, recordKind, csvData, "ELW rarity score")
        {
        }

        public override bool Enabled => true; // TODO use setting.
        public override string ValueFormat { get => "{0:0.##}"; }
        public override string Units { get => "score"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || scan.PlanetClass != Constants.SCAN_EARTHLIKE) return new();

            _elws.Add(scan);

            return new();
        }

        public override List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, Dictionary<int, Scan> scans)
        {
            if (!Enabled || _elws.Count == 0) return new();

            List<Result> results = new();
            foreach (var scan in _elws)
            {
                var otherBody = new BodyData(scan, scans);
                //var score = otherBody.GetRarityScore();
                //results.AddRange(CheckMax(score, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
