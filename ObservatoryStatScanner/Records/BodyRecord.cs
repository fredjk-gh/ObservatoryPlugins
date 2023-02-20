using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ObservatoryStatScanner.StatScannerSettings;

namespace ObservatoryStatScanner.Records
{
    internal abstract class BodyRecord : IRecord
    {
        private const string PROCGEN_NAME_RE = @"\s+[A-Z][A-Z]-[A-Z]\s+[a-z]\d+(-\d+)?";
        private static readonly Regex RE = new Regex(PROCGEN_NAME_RE);

        protected StatScannerSettings Settings;
        protected CSVData data;
        protected string format = "{0:0.##0000}";

        protected BodyRecord(StatScannerSettings settings, RecordKind recordKind, CSVData csvData, string displayName)
        {
            Settings = settings;
            RecordKind = recordKind;
            data = csvData;
            DisplayName = displayName;
        }

        public abstract bool Enabled { get; }
        public RecordTable Table { get => data.Table; }
        public string DisplayName { get; }
        public string VariableName { get => data.Variable; }
        public string JournalObjectName { get => data.JournalObjectName; }
        public string EDAstroObjectName { get => data.EDAstroObjectName; }

        public long MinCount { get => data.MinCount; }
        public double MinValue { get => (Settings.DevMode ? data.MinValue * Settings.DevModeMinScaleFactor : data.MinValue); }
        public string MinBody { get => data.MinBody; }

        public long MaxCount { get => data.MaxCount; }
        public double MaxValue { get => (Settings.DevMode ? data.MaxValue * Settings.DevModeMaxScaleFactor : data.MaxValue); }
        public string MaxBody { get => data.MaxBody; }
        public RecordKind RecordKind { get; }

        public string ValueFormat { get => format; set => format = value; }
 
        public virtual List<StatScannerGrid> CheckScan(Scan scan)
        {
            return new();
        }

        public virtual List<StatScannerGrid> CheckFSSBodySignals(FSSBodySignals bodySignals, FileHeader header)
        {
            return new();
        }
        public virtual List<StatScannerGrid> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans)
        {
            return new();
        }

        public virtual List<StatScannerGrid> CheckFSSDiscoveryScan(FSSDiscoveryScan fssDiscoveryScan)
        {
            return new();
        }

        protected List<StatScannerGrid> CheckMax(double observedValue, string timestamp, string bodyName, bool wasDiscovered)
        {
            List<StatScannerGrid> results = new();
            double thresholdFactor = 1.0 - (Settings.MaxNearRecordThreshold / 100.0);
            var observedValueRounded = Math.Round(observedValue, Constants.EDASTRO_PRECISION);
            
            var outcome = (observedValueRounded > MaxValue ? Outcome.PotentialNew :
                (observedValueRounded == MaxValue && MaxCount < Settings.HighCardinalityTieSuppression ? Outcome.Tie :
                    (observedValueRounded >= Math.Round(MaxValue * thresholdFactor, Constants.EDASTRO_PRECISION) && observedValueRounded < MaxValue ? Outcome.NearRecord : Outcome.None)));

            if (outcome != Outcome.None)
            {
                var gridItem = MakeGridItem(outcome, Function.Max, observedValue, timestamp, bodyName, wasDiscovered);
                if (gridItem != null) results.Add(gridItem);
            }
            return results;
        }

        protected List<StatScannerGrid> CheckMin(double observedValue, string timestamp, string bodyName, bool isUndiscovered)
        {
            List<StatScannerGrid> results = new();
            var thresholdFactor = 1.0 + (Settings.MinNearRecordThreshold / 100.0);
            var observedValueRounded = Math.Round(observedValue, Constants.EDASTRO_PRECISION);

            var outcome = (observedValueRounded < MinValue ? Outcome.PotentialNew :
                (observedValueRounded == MinValue && MinCount < Settings.HighCardinalityTieSuppression ? Outcome.Tie :
                    (observedValueRounded <= Math.Round(MinValue * thresholdFactor, Constants.EDASTRO_PRECISION) && observedValueRounded > MinValue ? Outcome.NearRecord : Outcome.None)));

            if (outcome != Outcome.None)
            {
                var gridItem = MakeGridItem(outcome, Function.Min, observedValue, timestamp, bodyName, isUndiscovered);
                if (gridItem != null) results.Add(gridItem);
            }
            return results;
        }

        protected StatScannerGrid MakeGridItem(
            Outcome outcome, Function function, double observedValue, string timestamp, string bodyName, bool isUndiscovered)
        {
            double recordValue;
            long recordTieCount;
            string recordHolder;
            int threshold;

            switch (function)
            {
                case Function.Min:
                    recordValue = MinValue;
                    recordTieCount = MinCount;
                    recordHolder = MinBody;
                    threshold = Settings.MinNearRecordThreshold;
                    break;
                case Function.Max:
                    recordValue = MaxValue;
                    recordTieCount = MaxCount;
                    recordHolder = MaxBody;
                    threshold = Settings.MaxNearRecordThreshold;
                    break;
                default:
                    return null; /// Should never happen
            }
            var details = "";
            switch (outcome)
            {
                case Outcome.PotentialNew:
                    details = "Potential new record" + (Settings.DevMode ? " (dev mode)" : "");
                    break;
                case Outcome.Tie:
                    details = $"Tied record (with ~{recordTieCount} others)";
                    break;
                case Outcome.NearRecord:
                    details = $"Near-record (within {threshold}%)";
                    break;
            }
            // Override above if this was actually the record holder (corrects potential rounding differences)
            if (bodyName == recordHolder) details = "Record holder";

            // This is not a galactic record holder, and we're showing procgen only records (except for visited galactic record holders).
            // OR: FDs only is enabled, and this is not first discovered and not a record holder.
            var procGenHandling = (ProcGenHandlingMode)Settings.ProcGenHandlingOptions[Settings.ProcGenHandling];
            if ((RecordKind == RecordKind.Galactic && procGenHandling == ProcGenHandlingMode.ProcGenOnly && bodyName != recordHolder)
                    || (RecordKind == RecordKind.GalacticProcGen && procGenHandling == ProcGenHandlingMode.ProcGenIgnore)
                    || (Settings.FirstDiscoveriesOnly && !isUndiscovered && bodyName != recordHolder))
                return null;
    
            StatScannerGrid gridRow = new StatScannerGrid()
            {
                Timestamp = timestamp,
                Body = bodyName,
                ObjectClass = EDAstroObjectName,
                Variable = DisplayName,
                Function = function.ToString(),
                RecordValue = String.Format(ValueFormat, recordValue),
                ObservedValue = String.Format(ValueFormat, observedValue),
                Details = details,
                RecordHolder = recordHolder,
                DiscoveryStatus = (isUndiscovered ? "First Discovery" : "Already discovered"),
                RecordKind = RecordKind.ToString(),
            };
            return gridRow;
        }

        static protected bool IsNonProcGenOrTerraformedELW(Scan scan)
        {
            if (!string.IsNullOrEmpty(scan.PlanetClass) && scan.PlanetClass == Constants.SCAN_EARTHLIKE)
            {
                return !RE.IsMatch(scan.BodyName) || !string.IsNullOrEmpty(scan.TerraformState);
            }
            return false;
        }

        static protected bool IsUndiscovered(Scan scan)
        {
            // Nav beacons scans are definitely not undiscovered.
            if (scan.ScanType == Constants.SCAN_TYPE_NAV_BEACON) return false;
            // Exclude barycentres; planetary bodies must be both undiscovered AND unmapped.
            if (!string.IsNullOrEmpty(scan.PlanetClass) && scan.PlanetClass != Constants.SCAN_BARYCENTRE && !scan.WasDiscovered && !scan.WasMapped) return true;
            // For stars, they just need to be undiscovered. The NavBeaconDetail takes care of the bulk of the known ones.
            if (!string.IsNullOrEmpty(scan.StarType) && !scan.WasDiscovered) return true;
            return false;
        }
    }
}