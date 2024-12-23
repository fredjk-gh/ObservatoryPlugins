using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class SystemRecord : IRecord
    {
        protected readonly StatScannerSettings Settings;
        protected readonly IRecordData Data;
        protected readonly Dictionary<string, bool> IsUndiscoveredSystem = new();
        protected List<LogMonitorState> _disallowedStates = new();

        public SystemRecord(
            StatScannerSettings settings,
            RecordKind recordKind,
            IRecordData data,
            string displayName)
        {
            Settings = settings;
            RecordKind = recordKind;
            Data = data;
            DisplayName = displayName;
        }

        public virtual bool Enabled => false;

        public RecordTable Table => Data.Table;
        public RecordKind RecordKind { get; }
        public List<LogMonitorState> DisallowedLogMonitorStates => _disallowedStates;

        public string VariableName => Data.Variable;

        public string DisplayName { get; }

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
            return new();
        }

        public virtual List<Result> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            return new();
        }

        public virtual List<Result> CheckScan(Scan scan, string currentSystem)
        {
            return new();
        }

        public virtual List<Result> CheckCodexEntry(CodexEntry codexEntry)
        {
            return new();
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
                            RecordHolder = (MaxCount > 1 ? $"{MaxHolder} (and {MaxCount} more)" : MaxHolder),
                            Details = Constants.UI_CURRENT_PERSONAL_BEST,
                            DiscoveryStatus = Settings.FirstDiscoveriesOnly ? Constants.UI_FIRST_DISCOVERY : Constants.UI_DISCOVERY_STATE_ANY,
                            RecordKind = RecordKind.ToString(),
                        },
                        Constants.SUMMARY_COALESCING_ID));
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
                IsUndiscoveredSystem[currentSystem] = (scan.ScanType != Constants.SCAN_TYPE_NAV_BEACON && !scan.WasDiscovered);
            }
        }

        protected List<Result> CheckMax(NotificationClass notificationClass, double observedValue, DateTime timestamp, string systemName)
        {
            List<Result> results = new();

            var isUndiscovered = IsUndiscoveredSystem.GetValueOrDefault(systemName);
            if ((Settings.FirstDiscoveriesOnly && !isUndiscovered) || observedValue == 0) return new();

            var procGenHandlingMode = (StatScannerSettings.ProcGenHandlingMode)Settings.ProcGenHandlingOptions[Settings.ProcGenHandling];
            if ((!HasMax || observedValue >= MaxValue)
                && (procGenHandlingMode != StatScannerSettings.ProcGenHandlingMode.ProcGenOnly || Constants.PROCGEN_NAME_RE.IsMatch(systemName)))
            {
                StatScannerGrid gridRow = new()
                {
                    Timestamp = timestamp.ToString(),
                    BodyOrItem = systemName,
                    ObjectClass = EDAstroObjectName,
                    Variable = DisplayName,
                    Function = MaxFunction.ToString(),
                    ObservedValue = String.Format(ValueFormat, observedValue),
                    RecordValue = (HasMax ? String.Format(ValueFormat, MaxValue) : "-"),
                    Units = Units,
                    RecordHolder = (HasMax ? (MaxCount > 1 ? $"{MaxHolder} (and {MaxCount - 1} more)" : MaxHolder) : ""),
                    Details = Constants.UI_NEW_PERSONAL_BEST,
                    DiscoveryStatus = (isUndiscovered ? Constants.UI_FIRST_DISCOVERY : Constants.UI_ALREADY_DISCOVERED),
                    RecordKind = RecordKind.ToString(),
                };
                results.Add(new Result(notificationClass, gridRow, Constants.SYSTEM_COALESCING_ID));

                // Update the record *AFTER* generating the GridRow to ensure we have access to the previous value.
                // When there's a tie, this increments the count only.
                Data.SetOrUpdateMax(systemName, observedValue, timestamp);
            }
            return results;
        }
    }
}