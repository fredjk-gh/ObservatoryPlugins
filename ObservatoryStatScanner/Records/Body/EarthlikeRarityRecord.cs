using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class EarthlikeRarityRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData csvData)
        : BodyRecord(settings, recordKind, csvData, "ELW rarity score")
    {
        private readonly List<Scan> _elws = [];

        public override bool Enabled => false; // TODO: Add Settings.EnableElwRarityRecord;
        public override string ValueFormat { get => "{0:0.##}"; }
        public override string Units { get => "score"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || scan.PlanetClass != Constants.SCAN_EARTHLIKE) return [];

            _elws.Add(scan);

            return [];
        }

        // TODO: Implement ELW Rarity Score.
        public override List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, Dictionary<int, Scan> scans)
        {
            if (!Enabled || _elws.Count == 0) return [];

            List<Result> results = [];
            foreach (Scan scan in _elws)
            {
                //var elwBody = new BodyData(scan, scans);
                //var score = elwBody.GetRarityScore();
                //results.AddRange(CheckMax(score, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
