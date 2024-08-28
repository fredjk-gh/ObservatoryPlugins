using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class BodyTypeTally : BodyRecord
    {
        // This is reset on every app restart -- so this value can drift after a while if you somehow re-scan bodies you've
        // seen before. Note that this also suppresses the duplicate you get if you FSS and DSS in the same session.
        private HashSet<string> _alreadySeen = new();

        public BodyTypeTally(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Scanned")
        {
            _disallowedStates = new() { LogMonitorState.PreRead };
        }

        public override bool Enabled => Settings.EnablePersonalBests;

        public override string ValueFormat { get => "{0:n0}"; }
        public override string Units { get => "bodies"; }
        public override Function MaxFunction { get => Function.Count; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (_alreadySeen.Contains(scan.BodyName)) return new();
            _alreadySeen.Add(scan.BodyName);

            var newValue = (Data.HasMax ? Data.MaxValue + 1 : 1);

            CheckMax(newValue, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));

            return new(); // Don't emit these to the grid -- cuz they're too verbose. Summary only.
        }

        public override void Reset()
        {
            base.Reset();
            _alreadySeen.Clear();
        }
    }
}
