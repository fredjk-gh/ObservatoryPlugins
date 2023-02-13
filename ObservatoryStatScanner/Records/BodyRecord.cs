using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal abstract class BodyRecord : IRecord
    {
        private const string PROCGEN_NAME_RE = @"\s+[A-Z][A-Z]-[A-Z]\s+[a-z]\d+(-\d+)?";
        private readonly Regex RE = new Regex(PROCGEN_NAME_RE);

        protected StatScannerSettings Settings;
        private double _minValue;
        private double _maxValue;
        private string _format = "{0:0.##0000}";

        protected BodyRecord(
            StatScannerSettings settings,
            RecordTable recordTable,
            string displayName,
            string variableName,
            string journalObjectName,
            string eDAstroObjectName,
            long minCount,
            double minValue,
            string minBody,
            long maxCount,
            double maxValue,
            string maxBody)
        {
            Table = recordTable;
            DisplayName = displayName;
            VariableName = variableName;
            JournalObjectName = journalObjectName;
            EDAstroObjectName = eDAstroObjectName;
            MinCount = minCount;
            _minValue = minValue;
            MinBody = minBody;
            MaxCount = maxCount;
            _maxValue = maxValue;
            MaxBody = maxBody;
            Settings = settings;
        }

        public abstract bool Enabled { get; }
        public RecordTable Table { get; }
        public string DisplayName { get; }
        public string VariableName { get; }
        public string JournalObjectName { get; }
        public string EDAstroObjectName { get; }

        public long MinCount { get; }
        public double MinValue { get => (Settings.DevMode ? _minValue * 1.2 : _minValue); set => _minValue = value; }
        public string MinBody { get; }

        public long MaxCount { get; }
        public double MaxValue { get => (Settings.DevMode ? _maxValue * 0.8 : _maxValue); set => _maxValue = value; }
        public string MaxBody { get; }

        public virtual string ValueFormat { get => _format; set => _format = value; }
 
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

        protected List<StatScannerGrid> CheckMax(double observedValue, string timestamp, string bodyName)
        {
            List<StatScannerGrid> results = new();
            double thresholdFactor = 1.0 - (Settings.MaxNearRecordThreshold / 100.0);
            var observedValueRounded = Math.Round(observedValue, Constants.EDASTRO_PRECISION);
            
            var outcome = (observedValueRounded > MaxValue ? Outcome.PotentialNew :
                (observedValueRounded == MaxValue && MaxCount < Settings.HighCardinalityTieSuppression ? Outcome.Tie :
                    (observedValueRounded >= Math.Round(MaxValue * thresholdFactor, Constants.EDASTRO_PRECISION) && observedValueRounded < MaxValue ? Outcome.NearRecord : Outcome.None)));

            if (outcome != Outcome.None)
            {
                results.Add(MakeGridItem(
                    outcome,
                    MaxValue,
                    observedValue,
                    timestamp,
                    EDAstroObjectName,
                    DisplayName,
                    Function.Max,
                    bodyName,
                    MaxBody,
                    MaxCount,
                    Settings.MaxNearRecordThreshold));
            }
            return results;
        }

        protected List<StatScannerGrid> CheckMin(double observedValue, string timestamp, string bodyName)
        {
            List<StatScannerGrid> results = new();
            var thresholdFactor = 1.0 + (Settings.MinNearRecordThreshold / 100.0);
            var observedValueRounded = Math.Round(observedValue, Constants.EDASTRO_PRECISION);

            var outcome = (observedValueRounded < MinValue ? Outcome.PotentialNew :
                (observedValueRounded == MinValue && MinCount < Settings.HighCardinalityTieSuppression ? Outcome.Tie :
                    (observedValueRounded <= Math.Round(MinValue * thresholdFactor, Constants.EDASTRO_PRECISION) && observedValueRounded > MinValue ? Outcome.NearRecord : Outcome.None)));

            if (outcome != Outcome.None)
            {
                results.Add(MakeGridItem(
                    outcome,
                    MinValue,
                    observedValue,
                    timestamp,
                    EDAstroObjectName,
                    DisplayName,
                    Function.Min,
                    bodyName,
                    MinBody,
                    MinCount,
                    Settings.MinNearRecordThreshold));
            }
            return results;
        }

        protected StatScannerGrid MakeGridItem(
            Outcome outcome,
            double recordValue,
            double observedValue,
            string timestamp,
            string objectClass,
            string displayName,
            Function function,
            string bodyName,
            string recordHolder,
            long recordTieCount,
            int threshold)
        {
            var details = "";
            switch (outcome)
            {
                case Outcome.PotentialNew:
                    details = "Potential new record";
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

            StatScannerGrid gridRow = new StatScannerGrid()
            {
                Timestamp = timestamp,
                Body = bodyName,
                ObjectClass = objectClass,
                Variable = displayName,
                Function = function.ToString(),
                RecordValue = String.Format(ValueFormat, recordValue),
                ObservedValue = String.Format(ValueFormat, observedValue),
                Details = details,
                RecordHolder = recordHolder,
            };
            return gridRow;
        }

        protected bool IsNonProcGenOrTerraformedELW(Scan scan)
        {
            if (!string.IsNullOrEmpty(scan.PlanetClass) && scan.PlanetClass == "Earthlike body")
            {
                return !RE.IsMatch(scan.BodyName) || !string.IsNullOrEmpty(scan.TerraformState);
            }
            return false;
        }
    }
}