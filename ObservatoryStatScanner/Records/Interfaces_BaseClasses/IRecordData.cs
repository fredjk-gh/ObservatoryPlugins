using ObservatoryStatScanner.Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    public class IRecordData
    {
        // public PersonalBestManager Manager { get; protected set; }

        public RecordTable Table { get; protected set; }
        public string Variable { get; protected set; }
        public string EDAstroObjectName { get; protected set; }
        public string JournalObjectName { get; protected set; }
        public bool IsValid { get => (Table != RecordTable.Unknown && JournalObjectName != null); }

        public virtual bool IsMutable { get => false; }

        public bool HasMax { get => MaxValue > 0.0 && MaxHolder?.Length > 0; }
        public string MaxHolder { get; protected set; }
        public long MaxCount { get; protected set; }
        public double MaxValue { get; protected set; }

        public bool HasMin { get => MinValue != 0.0 && MinHolder?.Length > 0; }
        public string MinHolder { get; protected set; }
        public long MinCount { get; protected set; }
        public double MinValue { get; protected set; }

        // An arbitrary string value.
        public string ExtraData { get; set; }

        public void ResetMutable()
        {
            if (!IsMutable) return;

            MaxValue = 0.0;
            MaxCount = 0;
            MaxHolder = "";

            MinValue = 0.0;
            MinCount = 0;
            MinHolder = "";

            ExtraData = "";
        }

        public void SetOrUpdateMax(string maxHolder, double maxValue, int maxCount = 1, string extraData = "")
        {
            if (!IsMutable || (HasMax && maxValue < MaxValue)) return;
            if (maxValue == MaxValue)  // It's a tie, increment the counter.
            {
                MaxCount += maxCount;
                return;
            }
            MaxValue = maxValue;
            MaxHolder = maxHolder;
            MaxCount = maxCount;
            if (extraData.Length > 0) ExtraData = extraData;

            // TODO: Tell the Manager?
        }

        public void SetOrUpdateMin(string minHolder, double minValue, int minCount = 1, string extraData = "")
        {
            if (!IsMutable || (HasMin && minValue > MinValue)) return;
            if (minValue == MinValue)  // It's a tie, increment the counter.
            {
                MinCount += minCount;
                return;
            }
            MinHolder = minHolder;
            MinValue = minValue;
            MinCount = minCount;
            if (extraData.Length > 0) ExtraData = extraData;

            // TODO: Tell the Manager?
        }

        public static RecordTable RecordTableFromString(string str)
        {
            switch (str)
            {
                case "planets":
                    return RecordTable.Planets;
                case "rings":
                    return RecordTable.Rings;
                case "stars":
                    return RecordTable.Stars;
                case "systems":
                    return RecordTable.Systems;
                default:
                    Debug.WriteLine("Unknown table value found in galactic records csv file: " + str);
                    break;
            }
            return RecordTable.Unknown;
        }
    }
}
