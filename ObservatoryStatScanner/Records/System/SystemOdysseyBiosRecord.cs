using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.System
{
    internal class SystemOdysseyBiosRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : SystemRecord(settings, recordKind, data, "Odyssey Bios"), IRecord
    {
        private readonly Dictionary<string, int> BodyBioSignals = [];

        public override bool Enabled => Settings.EnableOdysseySurfaceBioRecord;

        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "bios"; }
        public override Function MaxFunction { get => Function.Sum; }

        public override List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, Dictionary<int, Scan> scans)
        {
            if (!Enabled) return [];

            int systemBioCount = 0;
            foreach (var bodyName in BodyBioSignals.Keys)
            {
                if (bodyName.StartsWith(allBodiesFound.SystemName))
                {
                    systemBioCount += BodyBioSignals[bodyName];
                }
            }
            BodyBioSignals.Clear();

            return CheckMax(NotificationClass.PersonalBest, systemBioCount, allBodiesFound.TimestampDateTime, allBodiesFound.SystemName);
        }

        public override List<Result> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            if (!Enabled || !isOdyssey) return [];

            List<Signal> bodiesWithBioSignals = [.. bodySignals.Signals.Where(s => s.Type == Constants.FSS_BODY_SIGNAL_BIOLOGICAL)];
            if (bodiesWithBioSignals.Count == 1)
            {
                Signal bioSignal = bodiesWithBioSignals.First();
                if (bioSignal.Count > 0)
                    BodyBioSignals[bodySignals.BodyName] = bioSignal.Count;
            }

            return [];
        }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled) return [];

            TrackIsSystemUndiscovered(scan, currentSystem);

            // Check for atmosphere of any bodies that have bio signals.
            if (BodyBioSignals.ContainsKey(scan.BodyName)
                && (!scan.Landable
                    || scan.AtmosphereType?.Length == 0
                    || scan.Atmosphere == "None"))
            {
                BodyBioSignals.Remove(scan.BodyName);
            }

            return [];
        }
    }
}
