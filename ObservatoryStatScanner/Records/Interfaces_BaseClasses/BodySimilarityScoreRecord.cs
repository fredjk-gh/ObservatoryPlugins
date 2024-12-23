using com.github.fredjk_gh.ObservatoryStatScanner.Records.Body;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses
{
    internal class BodySimilarityScoreRecord : BodyRecord
    {
        private BodyData _refBody;

        public BodySimilarityScoreRecord(BodyData refBody, StatScannerSettings settings, RecordKind recordKind, IRecordData csvData)
            : base(settings, recordKind, csvData, $"Similarity Score: {refBody.Name}")
        {
            _refBody = refBody;
        }

        public override bool Enabled => Settings.EnableElwSimilarityRecords;
        public override string ValueFormat { get => "{0:0.##}"; }
        public override string Units { get => ""; }

        public override List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, Dictionary<int, Scan> scans)
        {
            List<Scan> elws = scans.Values.Where(s => Constants.SCAN_EARTHLIKE.Equals(s.PlanetClass)).ToList();

            if (!Enabled || elws.Count == 0) return new();

            List<Result> results = new();

            foreach (var scan in elws)
            {
                var otherBody = new BodyData(scan, scans);
                var score = _refBody.GetSimilarityScore(otherBody);
                Debug.WriteLine($"{_refBody.Name} Similarity Score: {otherBody.Name}\t{score}");
                // NOTE: LOWER is better, so CheckMin here.
                results.AddRange(CheckMin(score, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
