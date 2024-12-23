using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class PlanetaryOdysseyBiosRecord : BodyRecord
    {
        private readonly Dictionary<string, int> BodyBioSignals = new();

        public PlanetaryOdysseyBiosRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Odyssey Bio count")
        { }

        public override bool Enabled => Settings.EnableOdysseySurfaceBioRecord;

        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "bios"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || !BodyBioSignals.ContainsKey(scan.BodyName))
                return new();
            int bioCount = BodyBioSignals[scan.BodyName];
            if (!scan.Landable
                || scan.AtmosphereType?.Length == 0
                || scan.Atmosphere == "None"
                || bioCount == 0)
            {
                if (BodyBioSignals.ContainsKey(scan.BodyName)) BodyBioSignals.Remove(scan.BodyName);
                return new();
            }

            BodyBioSignals.Remove(scan.BodyName);
            return CheckMax(bioCount, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
        }

        public override List<Result> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            if (!Enabled || !isOdyssey) return new();

            var bioSignals = bodySignals.Signals.Where(s => s.Type == Constants.FSS_BODY_SIGNAL_BIOLOGICAL).ToList();
            if (bioSignals.Count() == 1)
            {
                var bioSignal = bioSignals.First();
                if (bioSignal.Count > 0)
                    BodyBioSignals[bodySignals.BodyName] = bioSignal.Count;
            }

            return new();
        }
        public override List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, Dictionary<int, Scan> scans)
        {
            BodyBioSignals.Clear();

            return new();
        }
    }
}
