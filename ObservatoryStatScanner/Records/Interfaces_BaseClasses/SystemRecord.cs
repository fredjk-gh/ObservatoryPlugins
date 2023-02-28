using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using ObservatoryStatScanner.DB;
using static ObservatoryStatScanner.StatScannerSettings;

namespace ObservatoryStatScanner.Records
{
    internal class SystemRecord : IRecord
    {
        protected readonly StatScannerSettings Settings;
        protected readonly IRecordData Data;
        protected readonly Dictionary<string, bool> IsUndiscoveredSystem = new();

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
        public virtual Function MaxFunction { get => Function.MaxCount; }

        public bool HasMin => Data.HasMin;
        public string MinHolder => Data.MinHolder;
        public long MinCount => Data.MinCount;
        public double MinValue => Data.MinValue;
        public virtual Function MinFunction { get => Function.Min; }

        public virtual List<StatScannerGrid> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans)
        {
            return new();
        }

        public virtual List<StatScannerGrid> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            return new();
        }

        public virtual List<StatScannerGrid> CheckScan(Scan scan)
        {
            return new();
        }

        public virtual List<StatScannerGrid> CheckCodexEntry(CodexEntry codexEntry)
        {
            return new();
        }

        public List<StatScannerGrid> Summary()
        {
            var results = new List<StatScannerGrid>();

            if (HasMax)
            {
                results.Add(new()
                {
                    ObjectClass = EDAstroObjectName,
                    Variable = DisplayName,
                    Function = MaxFunction.ToString(),
                    RecordValue = String.Format(ValueFormat, MaxValue),
                    Units = Units,
                    RecordHolder = (MaxCount > 1 ? $"{MaxHolder} (and {MaxCount} more)" : MaxHolder),
                    Details = Constants.UI_CURRENT_PERSONAL_BEST,
                    RecordKind = RecordKind.ToString(),
                });
            }
            return results;
        }

        public virtual void Reset()
        {
            Data.ResetMutable();
        }

        public void MaybeInitForPersonalBest(PersonalBestManager manager)
        {
            Data.Init(manager);
        }

        protected void TrackIsSystemUndiscovered(Scan scan)
        {
            // Check if arrival star is undiscovered.
            if (scan.DistanceFromArrivalLS == 0 && scan.PlanetClass != Constants.SCAN_BARYCENTRE)
            {
                IsUndiscoveredSystem[scan.StarSystem] = (scan.ScanType != Constants.SCAN_TYPE_NAV_BEACON && !scan.WasDiscovered);
            }
        }

        protected List<StatScannerGrid> CheckMax(double observedValue, string timestamp, string systemName)
        {
            List<StatScannerGrid> results = new();

            var isUndiscovered = IsUndiscoveredSystem.GetValueOrDefault(systemName);
            if ((Settings.FirstDiscoveriesOnly && !isUndiscovered) || observedValue == 0) return new();

            var procGenHandlingMode = (ProcGenHandlingMode)Settings.ProcGenHandlingOptions[Settings.ProcGenHandling];
            if ((!HasMax || observedValue >= MaxValue)
                && (procGenHandlingMode != ProcGenHandlingMode.ProcGenOnly || Constants.RE.IsMatch(systemName)))
            {
                if (HasMax && observedValue > MaxValue)
                {
                    StatScannerGrid gridRow = new()
                    {
                        Timestamp = timestamp,
                        Body = systemName,
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
                    results.Add(gridRow);
                }

                // Update the record *AFTER* generating the GridRow to ensure we have access to the previous value.
                // When there's a tie, this increments the count only.
                Data.SetOrUpdateMax(systemName, observedValue);
            }
            return results;
        }
    }
}