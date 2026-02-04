using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses
{
    internal abstract class BodyRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData csvData, string displayName)
        : IRecord
    {
        protected StatScannerSettings Settings = settings;
        protected IRecordData Data = csvData;
        protected List<LogMonitorState> _disallowedStates = [];

        public abstract bool Enabled { get; }

        public RecordTable Table { get => Data.Table; }
        public RecordKind RecordKind { get; } = recordKind;
        public List<LogMonitorState> DisallowedLogMonitorStates => _disallowedStates;

        public string VariableName { get => Data.Variable; }
        public string DisplayName { get; } = displayName;
        public string JournalObjectName { get => Data.JournalObjectName; }
        public string EDAstroObjectName { get => Data.EDAstroObjectName; }
        public virtual string ValueFormat { get => "{0:0.0000##}"; }
        public virtual string Units { get => "";  }

        public bool HasMax => Data.HasMax;
        public long MaxCount { get => Data.MaxCount; }
        public double MaxValue { get => Settings.DevMode ? Data.MaxValue * Settings.DevModeMaxScaleFactor : Data.MaxValue; }
        public string MaxHolder { get => Data.MaxHolder; }
        public DateTime MaxRecordDateTime => Data.MaxRecordDateTime;
        public virtual Function MaxFunction { get => Function.Maximum; }

        public bool HasMin => Data.HasMin;
        public long MinCount { get => Data.MinCount; }
        public double MinValue { get => Settings.DevMode ? Data.MinValue * Settings.DevModeMinScaleFactor : Data.MinValue; }
        public string MinHolder { get => Data.MinHolder; }
        public DateTime MinRecordDateTime => Data.MinRecordDateTime;
        public virtual Function MinFunction { get => Function.Minimum; }
 
        public virtual List<Result> CheckScan(Scan scan, string currentSystem)
        {
            return [];
        }
        public virtual List<Result> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            return [];
        }
        public virtual List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, Dictionary<int, Scan> scans)
        {
            return [];
        }
        public virtual List<Result> CheckCodexEntry(CodexEntry codexEntry)
        {
            return [];
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
                            Timestamp = Data.MaxRecordDateTime.ToString(),
                            ObjectClass = EDAstroObjectName,
                            Variable = DisplayName,
                            Function = MaxFunction.ToString(),
                            RecordValue = string.Format(ValueFormat, MaxValue),
                            Units = Units,
                            RecordHolder = MaxCount > 1 ? $"{MaxHolder} (and {MaxCount} more)" : MaxHolder,
                            Details = Constants.UI_CURRENT_PERSONAL_BEST,
                            DiscoveryStatus = Settings.FirstDiscoveriesOnly ? Constants.UI_FIRST_DISCOVERY : Constants.UI_DISCOVERY_STATE_ANY,
                            RecordKind = RecordKind.ToString(),
                        },
                        CoalescingIDs.SUMMARY_COALESCING_ID));
            }
            if (HasMin)
            {
                results.Add(
                    new(NotificationClass.None,
                        new()
                        {
                            Timestamp = Data.MinRecordDateTime.ToString(),
                            ObjectClass = EDAstroObjectName,
                            Variable = DisplayName,
                            Function = MinFunction.ToString(),
                            RecordValue = string.Format(ValueFormat, MinValue),
                            Units = Units,
                            RecordHolder = MinCount > 1 ? $"{MinHolder} (and {MinCount} more)" : MinHolder,
                            Details = Constants.UI_CURRENT_PERSONAL_BEST,
                            DiscoveryStatus = Settings.FirstDiscoveriesOnly ? Constants.UI_FIRST_DISCOVERY : Constants.UI_DISCOVERY_STATE_ANY,
                            RecordKind = RecordKind.ToString(),
                        },
                        CoalescingIDs.SUMMARY_COALESCING_ID));
            }
            return results;
        }

        protected List<Result> CheckMax(double observedValue, DateTime timestamp, string bodyName, int bodyId, bool isUndiscovered)
        {
            List<Result> results = [];

            if (RecordKind == RecordKind.Personal)
            {
                if (!IncludeBodyForPersonalRecord(bodyName, isUndiscovered)) return results;
                if (!Data.HasMax || observedValue > Data.MaxValue)
                {
                    var gridItem = MakeGridItem(Outcome.PersonalNew, MaxFunction, observedValue, timestamp, bodyName, bodyId, isUndiscovered);

                    if (gridItem != null) results.Add(gridItem);
                }
                // Setting or tying a new personal best. Set *after* making the grid item to preserve previous.
                Data.SetOrUpdateMax(bodyName, observedValue, timestamp);
                return results;
            }

            var observedValueRounded = Math.Round(observedValue, Constants.EDASTRO_PRECISION);
            double thresholdFactor = 1.0 - Settings.MaxNearRecordThreshold / 100.0;
            var outcome = observedValueRounded > MaxValue ? Outcome.PotentialNew :
                observedValueRounded == MaxValue && MaxCount < Settings.HighCardinalityTieSuppression ? Outcome.Tie :
                    observedValueRounded >= Math.Round(MaxValue * thresholdFactor, Constants.EDASTRO_PRECISION) && observedValueRounded < MaxValue ? Outcome.NearRecord : Outcome.None;

            if (outcome != Outcome.None)
            {
                var gridItem = MakeGridItem(outcome, MaxFunction, observedValue, timestamp, bodyName, bodyId, isUndiscovered);
                if (gridItem != null) results.Add(gridItem);
            }
            return results;
        }

        protected List<Result> CheckMin(double observedValue, DateTime timestamp, string bodyName, int bodyId, bool isUndiscovered)
        {
            List<Result> results = [];

            if (RecordKind == RecordKind.Personal)
            {
                if (!IncludeBodyForPersonalRecord(bodyName, isUndiscovered)) return results;
                if (!Data.HasMin || observedValue < Data.MinValue)
                {
                    var gridItem = MakeGridItem(Outcome.PersonalNew, MinFunction, observedValue, timestamp, bodyName, bodyId, isUndiscovered);

                    if (gridItem != null) results.Add(gridItem);
                }
                // Setting a new personal best. Set *after* making the grid item to preserve previous.
                Data.SetOrUpdateMin(bodyName, observedValue, timestamp);
                return results; // Done with personal records.
            }

            var observedValueRounded = Math.Round(observedValue, Constants.EDASTRO_PRECISION);
            var thresholdFactor = 1.0 + Settings.MinNearRecordThreshold / 100.0;

            var outcome = observedValueRounded < MinValue ? Outcome.PotentialNew :
                observedValueRounded == MinValue && MinCount < Settings.HighCardinalityTieSuppression ? Outcome.Tie :
                    observedValueRounded <= Math.Round(MinValue * thresholdFactor, Constants.EDASTRO_PRECISION) && observedValueRounded > MinValue ? Outcome.NearRecord : Outcome.None;

            if (outcome != Outcome.None)
            {
                var gridItem = MakeGridItem(outcome, MinFunction, observedValue, timestamp, bodyName, bodyId, isUndiscovered);
                if (gridItem != null) results.Add(gridItem);
            }
            return results;
        }

        protected Result MakeGridItem(
            Outcome outcome, Function function, double observedValue, DateTime timestamp, string bodyName, int bodyId, bool isUndiscovered)
        {
            string recordValueStr;
            double recordTieCount;
            string recordTieCountStr;
            string recordHolder;
            int threshold;

            switch (function)
            {
                case Function.Minimum:
                    recordValueStr = HasMin ? string.Format(ValueFormat, MinValue) : "-";
                    recordTieCount = MinCount;
                    recordTieCountStr = HasMin ? $"{MinCount}" : "";
                    recordHolder = HasMin ? MinHolder : "";
                    threshold = Settings.MinNearRecordThreshold;
                    break;
                case Function.Sum:
                case Function.Count:
                case Function.Maximum:
                    recordValueStr = HasMax ? string.Format(ValueFormat, MaxValue) : "-";
                    recordTieCount = MaxCount;
                    recordTieCountStr = HasMax ? $"{MaxCount}" : "";
                    recordHolder = HasMax ? MaxHolder : "";
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
                    if (function == Function.Count) notificationClass = NotificationClass.None; // Bodies are too common to notify. Summary use only.
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
            if (RecordKind == RecordKind.Galactic && procGenHandling == StatScannerSettings.ProcGenHandlingMode.ProcGenOnly && bodyName != recordHolder
                    || RecordKind == RecordKind.GalacticProcGen && procGenHandling == StatScannerSettings.ProcGenHandlingMode.ProcGenIgnore
                    || Settings.FirstDiscoveriesOnly && !isUndiscovered && bodyName != recordHolder)
                return null;

            StatScannerGrid gridRow = new()
            {
                Timestamp = timestamp.ToString(),
                BodyOrItem = bodyName,
                ObjectClass = EDAstroObjectName,
                Variable = DisplayName,
                Function = function.ToString(),
                ObservedValue = string.Format(ValueFormat, observedValue),
                RecordValue = recordValueStr,
                Units = Units,
                RecordHolder = recordTieCount > 1 ? $"{recordHolder} (and {recordTieCount - 1} more)" : recordHolder,
                Details = details,
                DiscoveryStatus = isUndiscovered ? Constants.UI_FIRST_DISCOVERY : Constants.UI_ALREADY_DISCOVERED,
                RecordKind = RecordKind.ToString(),
            };
            return new(notificationClass, gridRow, bodyId);
        }

        private bool IncludeBodyForPersonalRecord(string bodyName, bool isUndiscovered)
        {
            var procGenHandling = (StatScannerSettings.ProcGenHandlingMode)Settings.ProcGenHandlingOptions[Settings.ProcGenHandling];
            if (procGenHandling == StatScannerSettings.ProcGenHandlingMode.ProcGenOnly && !JournalConstants.ProcGenNameRegex().IsMatch(bodyName)
                    || Settings.FirstDiscoveriesOnly && !isUndiscovered)
                return false;
            return true;
        }

        static protected bool IsNonProcGenOrTerraformedELW(Scan scan)
        {
            if (!string.IsNullOrEmpty(scan.PlanetClass) && scan.PlanetClass == Constants.SCAN_EARTHLIKE)
            {
                return !JournalConstants.ProcGenNameRegex().IsMatch(scan.BodyName) || !string.IsNullOrEmpty(scan.TerraformState);
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