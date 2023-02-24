using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class RegionRecord : IRecord
    {
        protected readonly StatScannerSettings Settings;
        protected readonly IRecordData Data;
        public RegionRecord(
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

        public bool HasMin => Data.HasMin;
        public string MinHolder => Data.MinHolder;
        public long MinCount => Data.MinCount;
        public double MinValue => Data.MinValue;

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

        public virtual void Reset()
        {
            Data.ResetMutable();
        }

        protected List<StatScannerGrid> CheckMax(double observedValue, string timestamp, string objectName, string extraData = "", Function function = Function.Count)
        {
            List<StatScannerGrid> results = new();

            if (!HasMax || observedValue >= MaxValue)
            {
                if (HasMax && observedValue > MaxValue)
                {
                    StatScannerGrid gridRow = new()
                    {
                        Timestamp = timestamp,
                        Body = objectName,
                        ObjectClass = EDAstroObjectName,
                        Variable = DisplayName,
                        Function = function.ToString(),
                        ObservedValue = String.Format(ValueFormat, observedValue),
                        RecordValue = (HasMax ? String.Format(ValueFormat, MaxValue) : "-"),
                        Units = Units,
                        RecordHolder = (HasMax ? MaxHolder : ""),
                        Details = Constants.UI_NEW_PERSONAL_BEST,
                        DiscoveryStatus = "-",
                        RecordKind = RecordKind.ToString(),
                    };
                    results.Add(gridRow);
                }

                // Update the record *AFTER* generating the GridRow to ensure we have access to the previous value.
                // When there's a tie, this increments the count only.
                Data.SetOrUpdateMax(objectName, observedValue, 1, extraData);
            }
            return results;
        }
    }
}
