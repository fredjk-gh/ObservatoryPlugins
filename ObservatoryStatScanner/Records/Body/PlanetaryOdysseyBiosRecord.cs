using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class PlanetaryOdysseyBiosRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Odyssey Bio count")
    {
        private readonly Dictionary<string, int> BodyBioSignals = [];

        public override bool Enabled => Settings.EnableOdysseySurfaceBioRecord;

        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "bios"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || !BodyBioSignals.TryGetValue(scan.BodyName, out int bioCount))
                return [];
            if (!scan.Landable
                || scan.AtmosphereType?.Length == 0
                || scan.Atmosphere == "None"
                || bioCount == 0)
            {
                if (BodyBioSignals.ContainsKey(scan.BodyName)) BodyBioSignals.Remove(scan.BodyName);
                return [];
            }

            BodyBioSignals.Remove(scan.BodyName);
            return CheckMax(bioCount, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
        }

        public override List<Result> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            if (!Enabled || !isOdyssey) return [];

            var bioSignals = bodySignals.Signals.Where(s => s.Type == Constants.FSS_BODY_SIGNAL_BIOLOGICAL).ToList();
            if (bioSignals.Count == 1)
            {
                var bioSignal = bioSignals.First();
                if (bioSignal.Count > 0)
                    BodyBioSignals[bodySignals.BodyName] = bioSignal.Count;
            }

            return [];
        }
        public override List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, Dictionary<int, Scan> scans)
        {
            BodyBioSignals.Clear();

            return [];
        }
    }
}
