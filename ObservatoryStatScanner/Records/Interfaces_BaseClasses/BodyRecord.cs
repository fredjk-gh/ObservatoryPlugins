using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal abstract class BodyRecord : IRecord
    {
        protected StatScannerSettings Settings;
        protected IRecordData Data;
        protected List<LogMonitorState> _disallowedStates = new();

        protected BodyRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData csvData, string displayName)
        {
            Settings = settings;
            RecordKind = recordKind;
            Data = csvData;
            DisplayName = displayName;
        }

        public abstract bool Enabled { get; }

        public RecordTable Table { get => Data.Table; }
        public RecordKind RecordKind { get; }
        public List<LogMonitorState> DisallowedLogMonitorStates => _disallowedStates;

        public string VariableName { get => Data.Variable; }
        public string DisplayName { get; }
        public string JournalObjectName { get => Data.JournalObjectName; }
        public string EDAstroObjectName { get => Data.EDAstroObjectName; }
        public virtual string ValueFormat { get => "{0:0.0000##}"; }
        public virtual string Units { get => "";  }

        public bool HasMax => Data.HasMax;
        public long MaxCount { get => Data.MaxCount; }
        public double MaxValue { get => (Settings.DevMode ? Data.MaxValue * Settings.DevModeMaxScaleFactor : Data.MaxValue); }
        public string MaxHolder { get => Data.MaxHolder; }
        public virtual Function MaxFunction { get => Function.Maximum; }

        public bool HasMin => Data.HasMin;
        public long MinCount { get => Data.MinCount; }
        public double MinValue { get => (Settings.DevMode ? Data.MinValue * Settings.DevModeMinScaleFactor : Data.MinValue); }
        public string MinHolder { get => Data.MinHolder; }
        public virtual Function MinFunction { get => Function.Minimum; }
 
        public virtual List<Result> CheckScan(Scan scan, string currentSystem)
        {
            return new();
        }
        public virtual List<Result> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            return new();
        }
        public virtual List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans)
        {
            return new();
        }
        public virtual List<Result> CheckCodexEntry(CodexEntry codexEntry)
        {
            return new();
        }
        
        public void MaybeInitForPersonalBest(DB.PersonalBestManager manager)
        {
            Data.Init(manager);
        }

        public virtual void Reset()
        {
            Data.ResetMutable();
        }

        public List<Result> Summary()
        {
            var results = new List<Result>();

            if (HasMax)
            {
                results.Add(
                    new(NotificationClass.None,
                        new()
                        {
                            Timestamp = "Summary",
                            ObjectClass = EDAstroObjectName,
                            Variable = DisplayName,
                            Function = MaxFunction.ToString(),
                            RecordValue = String.Format(ValueFormat, MaxValue),
                            Units = Units,
                            RecordHolder = (MaxCount > 1 ? $"{MaxHolder} (and {MaxCount} more)" : MaxHolder),
                            Details = Constants.UI_CURRENT_PERSONAL_BEST,
                            RecordKind = RecordKind.ToString(),
                        },
                        Constants.SUMMARY_COALESCING_ID));
            }
            if (HasMin)
            {
                results.Add(
                    new(NotificationClass.None,
                        new()
                        {
                            Timestamp = "Summary",
                            ObjectClass = EDAstroObjectName,
                            Variable = DisplayName,
                            Function = MinFunction.ToString(),
                            RecordValue = String.Format(ValueFormat, MinValue),
                            Units = Units,
                            RecordHolder = (MinCount > 1 ? $"{MinHolder} (and {MinCount} more)" : MinHolder),
                            Details = Constants.UI_CURRENT_PERSONAL_BEST,
                            RecordKind = RecordKind.ToString(),
                        },
                        Constants.SUMMARY_COALESCING_ID));
            }
            return results;
        }

        protected List<Result> CheckMax(double observedValue, string timestamp, string bodyName, int bodyId, bool isUndiscovered)
        {
            List<Result> results = new();

            if (RecordKind == RecordKind.Personal)
            {
                if (!IncludeBodyForPersonalRecord(bodyName, isUndiscovered)) return results;
                if (!Data.HasMax || observedValue > Data.MaxValue)
                {
                    var gridItem = MakeGridItem(Outcome.PersonalNew, MaxFunction, observedValue, timestamp, bodyName, bodyId, isUndiscovered);

                    if (gridItem != null) results.Add(gridItem);
                }
                // Setting or tying a new personal best. Set *after* making the grid item to preserve previous.
                Data.SetOrUpdateMax(bodyName, observedValue);
                return results;
            }

            var observedValueRounded = Math.Round(observedValue, Constants.EDASTRO_PRECISION);
            double thresholdFactor = 1.0 - (Settings.MaxNearRecordThreshold / 100.0);
            var outcome = (observedValueRounded > MaxValue ? Outcome.PotentialNew :
                (observedValueRounded == MaxValue && MaxCount < Settings.HighCardinalityTieSuppression ? Outcome.Tie :
                    (observedValueRounded >= Math.Round(MaxValue * thresholdFactor, Constants.EDASTRO_PRECISION) && observedValueRounded < MaxValue ? Outcome.NearRecord : Outcome.None)));

            if (outcome != Outcome.None)
            {
                var gridItem = MakeGridItem(outcome, MaxFunction, observedValue, timestamp, bodyName, bodyId, isUndiscovered);
                if (gridItem != null) results.Add(gridItem);
            }
            return results;
        }

        protected List<Result> CheckMin(double observedValue, string timestamp, string bodyName, int bodyId, bool isUndiscovered)
        {
            List<Result> results = new();

            if (RecordKind == RecordKind.Personal)
            {
                if (!IncludeBodyForPersonalRecord(bodyName, isUndiscovered)) return results;
                if (!Data.HasMin || observedValue < Data.MinValue)
                {
                    var gridItem = MakeGridItem(Outcome.PersonalNew, MinFunction, observedValue, timestamp, bodyName, bodyId, isUndiscovered);

                    if (gridItem != null) results.Add(gridItem);
                }
                // Setting a new personal best. Set *after* making the grid item to preserve previous.
                Data.SetOrUpdateMin(bodyName, observedValue);
                return results; // Done with personal records.
            }

            var observedValueRounded = Math.Round(observedValue, Constants.EDASTRO_PRECISION);
            var thresholdFactor = 1.0 + (Settings.MinNearRecordThreshold / 100.0);

            var outcome = (observedValueRounded < MinValue ? Outcome.PotentialNew :
                (observedValueRounded == MinValue && MinCount < Settings.HighCardinalityTieSuppression ? Outcome.Tie :
                    (observedValueRounded <= Math.Round(MinValue * thresholdFactor, Constants.EDASTRO_PRECISION) && observedValueRounded > MinValue ? Outcome.NearRecord : Outcome.None)));

            if (outcome != Outcome.None)
            {
                var gridItem = MakeGridItem(outcome, MinFunction, observedValue, timestamp, bodyName, bodyId, isUndiscovered);
                if (gridItem != null) results.Add(gridItem);
            }
            return results;
        }

        protected Result MakeGridItem(
            Outcome outcome, Function function, double observedValue, string timestamp, string bodyName, int bodyId, bool isUndiscovered)
        {
            string recordValueStr;
            double recordTieCount;
            string recordTieCountStr;
            string recordHolder;
            int threshold;

            switch (function)
            {
                case Function.Minimum:
                    recordValueStr = (HasMin ? String.Format(ValueFormat, MinValue) : "-");
                    recordTieCount = MinCount;
                    recordTieCountStr = (HasMin ? $"{MinCount}" : "");
                    recordHolder = (HasMin ? MinHolder : "");
                    threshold = Settings.MinNearRecordThreshold;
                    break;
                case Function.Sum:
                case Function.Count:
                case Function.Maximum:
                    recordValueStr = (HasMax ? String.Format(ValueFormat, MaxValue) : "-");
                    recordTieCount = MaxCount;
                    recordTieCountStr = (HasMax ? $"{MaxCount}" : "");
                    recordHolder = (HasMax ? MaxHolder : "");
                    threshold = Settings.MaxNearRecordThreshold;
                    break;
                default:
                    return null; // Should never happen
            }
            var details = "";
            NotificationClass notificationClass = NotificationClass.None;
            switch (outcome)
            {
                case Outcome.PersonalNew:
                    details = Constants.UI_NEW_PERSONAL_BEST;
                    notificationClass = NotificationClass.PersonalBest;
                    break;
                case Outcome.PotentialNew:
                    details = Constants.UI_POTENTIAL_NEW_RECORD + (Settings.DevMode ? " (dev mode)" : "");
                    notificationClass = NotificationClass.PossibleNewGalacticRecord;
                    break;
                case Outcome.Tie:
                    details = string.Format(Constants.UI_FS_TIED_RECORD_COUNT, recordTieCountStr);
                    notificationClass = NotificationClass.MatchedGalacticRecord;
                    break;
                case Outcome.NearRecord:
                    details = string.Format(Constants.UI_FS_NEAR_RECORD_COUNT, threshold);
                    notificationClass = NotificationClass.NearGalacticRecord;
                    break;
            }
            // Override above if this was actually the record holder (corrects potential rounding differences)
            if (bodyName == recordHolder && recordHolder.Length > 0)
            {
                details = Constants.UI_RECORD_HOLDER_VISITED;
                notificationClass = NotificationClass.VisitedGalacticRecord;
            }

            // This is not a galactic record holder, and we're showing procgen only records (except for visited galactic record holders).
            // OR: FDs only is enabled, and this is not first discovered and not a record holder.
            var procGenHandling = (StatScannerSettings.ProcGenHandlingMode)Settings.ProcGenHandlingOptions[Settings.ProcGenHandling];
            if ((RecordKind == RecordKind.Galactic && procGenHandling == StatScannerSettings.ProcGenHandlingMode.ProcGenOnly && bodyName != recordHolder)
                    || (RecordKind == RecordKind.GalacticProcGen && procGenHandling == StatScannerSettings.ProcGenHandlingMode.ProcGenIgnore)
                    || (Settings.FirstDiscoveriesOnly && !isUndiscovered && bodyName != recordHolder))
                return null;

            StatScannerGrid gridRow = new()
            {
                Timestamp = timestamp,
                BodyOrItem = bodyName,
                ObjectClass = EDAstroObjectName,
                Variable = DisplayName,
                Function = function.ToString(),
                ObservedValue = String.Format(ValueFormat, observedValue),
                RecordValue = recordValueStr,
                Units = Units,
                RecordHolder = (recordTieCount > 1 ? $"{recordHolder} (and {recordTieCount - 1} more)" : recordHolder),
                Details = details,
                DiscoveryStatus = (isUndiscovered ? Constants.UI_FIRST_DISCOVERY : Constants.UI_ALREADY_DISCOVERED),
                RecordKind = RecordKind.ToString(),
            };
            return new(notificationClass, gridRow, bodyId);
        }

        private bool IncludeBodyForPersonalRecord(string bodyName, bool isUndiscovered)
        {
            var procGenHandling = (StatScannerSettings.ProcGenHandlingMode)Settings.ProcGenHandlingOptions[Settings.ProcGenHandling];
            if ((procGenHandling == StatScannerSettings.ProcGenHandlingMode.ProcGenOnly && !Constants.PROCGEN_NAME_RE.IsMatch(bodyName))
                    || (Settings.FirstDiscoveriesOnly && !isUndiscovered))
                return false;
            return true;
        }

        static protected bool IsNonProcGenOrTerraformedELW(Scan scan)
        {
            if (!string.IsNullOrEmpty(scan.PlanetClass) && scan.PlanetClass == Constants.SCAN_EARTHLIKE)
            {
                return !Constants.PROCGEN_NAME_RE.IsMatch(scan.BodyName) || !string.IsNullOrEmpty(scan.TerraformState);
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