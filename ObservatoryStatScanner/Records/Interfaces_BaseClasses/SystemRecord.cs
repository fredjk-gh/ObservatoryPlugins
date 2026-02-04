using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses
{
    internal class SystemRecord(
        StatScannerSettings settings,
        RecordKind recordKind,
        IRecordData data,
        string displayName) : IRecord
    {
        protected readonly StatScannerSettings Settings = settings;
        protected readonly IRecordData Data = data;
        protected readonly Dictionary<string, bool> IsUndiscoveredSystem = [];
        protected List<LogMonitorState> _disallowedStates = [];

        public virtual bool Enabled => false;

        public RecordTable Table => Data.Table;
        public RecordKind RecordKind { get; } = recordKind;
        public List<LogMonitorState> DisallowedLogMonitorStates => _disallowedStates;

        public string VariableName => Data.Variable;

        public string DisplayName { get; } = displayName;

        public string EDAstroObjectName => Data.EDAstroObjectName;
        public string JournalObjectName => Data.JournalObjectName;
        public virtual string ValueFormat { get => "{0}"; }
        public virtual string Units { get => ""; }

        public bool HasMax => Data.HasMax;
        public string MaxHolder => Data.MaxHolder;
        public long MaxCount => Data.MaxCount;
        public double MaxValue => Data.MaxValue;
        public DateTime MaxRecordDateTime => Data.MaxRecordDateTime;
        public virtual Function MaxFunction { get => Function.Maximum; }

        public bool HasMin => Data.HasMin;
        public string MinHolder => Data.MinHolder;
        public long MinCount => Data.MinCount;
        public double MinValue => Data.MinValue;
        public DateTime MinRecordDateTime => Data.MinRecordDateTime;
        public virtual Function MinFunction { get => Function.Minimum; }

        public virtual List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, Dictionary<int, Scan> scans)
        {
            return [];
        }

        public virtual List<Result> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            return [];
        }

        public virtual List<Result> CheckScan(Scan scan, string currentSystem)
        {
            return [];
        }

        public virtual List<Result> CheckCodexEntry(CodexEntry codexEntry)
        {
            return [];
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
            return results;
        }

        public virtual void Reset()
        {
            Data.ResetMutable();
        }

        public void MaybeInitForPersonalBest(DB.PersonalBestManager manager)
        {
            Data.Init(manager);
        }

        protected void TrackIsSystemUndiscovered(Scan scan, string currentSystem)
        {
            // Check if arrival star is undiscovered.
            if (scan.DistanceFromArrivalLS == 0 && scan.PlanetClass != Constants.SCAN_BARYCENTRE)
            {
                IsUndiscoveredSystem[currentSystem] = scan.ScanType != Constants.SCAN_TYPE_NAV_BEACON && !scan.WasDiscovered;
            }
        }

        protected List<Result> CheckMax(NotificationClass notificationClass, double observedValue, DateTime timestamp, string systemName)
        {
            List<Result> results = [];

            var isUndiscovered = IsUndiscoveredSystem.GetValueOrDefault(systemName);
            if (Settings.FirstDiscoveriesOnly && !isUndiscovered || observedValue == 0) return [];

            var procGenHandlingMode = (StatScannerSettings.ProcGenHandlingMode)Settings.ProcGenHandlingOptions[Settings.ProcGenHandling];
            if ((!HasMax || observedValue >= MaxValue)
                && (procGenHandlingMode != StatScannerSettings.ProcGenHandlingMode.ProcGenOnly || JournalConstants.ProcGenNameRegex().IsMatch(systemName)))
            {
                // Consider holding tied values in a list in ExtraData so we can suppress re-triggers when revisiting a record system
                // This chould be done in any record.
                StatScannerGrid gridRow = new()
                {
                    Timestamp = timestamp.ToString(),
                    BodyOrItem = systemName,
                    ObjectClass = EDAstroObjectName,
                    Variable = DisplayName,
                    Function = MaxFunction.ToString(),
                    ObservedValue = string.Format(ValueFormat, observedValue),
                    RecordValue = HasMax ? string.Format(ValueFormat, MaxValue) : "-",
                    Units = Units,
                    RecordHolder = HasMax ? MaxCount > 1 ? $"{MaxHolder} (and {MaxCount - 1} more)" : MaxHolder : "",
                    Details = Constants.UI_NEW_PERSONAL_BEST,
                    DiscoveryStatus = isUndiscovered ? Constants.UI_FIRST_DISCOVERY : Constants.UI_ALREADY_DISCOVERED,
                    RecordKind = RecordKind.ToString(),
                };
                results.Add(new Result(notificationClass, gridRow, CoalescingIDs.SYSTEM_COALESCING_ID));

                // Update the record *AFTER* generating the GridRow to ensure we have access to the previous value.
                // When there's a tie, this increments the count only.
                Data.SetOrUpdateMax(systemName, observedValue, timestamp);
            }
            return results;
        }
    }
}