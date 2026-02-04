using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses
{
    internal class RegionRecord(
        StatScannerSettings settings,
        RecordKind recordKind,
        IRecordData data,
        string displayName) : IRecord
    {
        protected readonly StatScannerSettings Settings = settings;
        protected readonly IRecordData Data = data;
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
        public virtual Function MaxFunction { get => Function.Count; }

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
            List<Result> results = [];

            if (HasMax)
            {
                results.Add(new(
                    NotificationClass.None,
                    new()
                        {
                            Timestamp = Data.MaxRecordDateTime.ToString(),
                            ObjectClass = EDAstroObjectName,
                            Variable = DisplayName,
                            Function = MaxFunction.ToString(),
                            RecordValue = string.Format(ValueFormat, MaxValue),
                            Units = Units,
                            RecordHolder = MaxHolder,
                            Details = Constants.UI_CURRENT_PERSONAL_BEST,
                            DiscoveryStatus = "-",
                            RecordKind = RecordKind.ToString(),
                        },
                        CoalescingIDs.SUMMARY_COALESCING_ID));
            }
            return results;
        }

        public void MaybeInitForPersonalBest(DB.PersonalBestManager manager)
        {
            Data.Init(manager);
        }

        public virtual void Reset()
        {
            Data.ResetMutable();
        }

        protected List<Result> CheckMax(NotificationClass notificationClass, double observedValue, DateTime timestamp, string objectName, string discoveryStatus = "", string extraData = "")
        {
            List<Result> results = [];

            if (!HasMax || observedValue >= MaxValue)
            {
                StatScannerGrid gridRow = new()
                {
                    Timestamp = timestamp.ToString(),
                    BodyOrItem = objectName,
                    ObjectClass = EDAstroObjectName,
                    Variable = DisplayName,
                    Function = MaxFunction.ToString(),
                    ObservedValue = string.Format(ValueFormat, observedValue),
                    RecordValue = HasMax ? string.Format(ValueFormat, MaxValue) : "-",
                    Units = Units,
                    RecordHolder = HasMax ? MaxHolder : "",
                    Details = discoveryStatus,
                    DiscoveryStatus = "-",
                    RecordKind = RecordKind.ToString(),
                };
                results.Add(new Result(notificationClass, gridRow, CoalescingIDs.REGION_COALESCING_ID));

                // Update the record *AFTER* generating the GridRow to ensure we have access to the previous value.
                // When there's a tie, this increments the count only.
                Data.SetOrUpdateMax(objectName, observedValue, timestamp, 1, extraData);
            }
            return results;
        }
    }
}
