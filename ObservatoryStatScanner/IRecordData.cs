using ObservatoryStatScanner.Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner
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

        public bool HasMax { get => MaxValue > 0.0 && MaxBody?.Length > 0; }
        public string MaxBody { get; protected set; }
        public long MaxCount { get; protected set; }
        public double MaxValue { get; protected set; }

        public bool HasMin { get => MinValue != 0.0 && MinBody?.Length > 0; }
        public string MinBody { get; protected set; }
        public long MinCount { get; protected set; }
        public double MinValue { get; protected set; }

        public void ResetMutable()
        {
            if (!IsMutable) return;

            MaxValue = 0.0;
            MaxCount = 0;
            MaxBody = "";

            MinValue = 0.0;
            MinCount = 0;
            MinBody = "";
        }

        public void SetOrUpdateMax(string maxBody, double maxValue, int maxCount = 1)
        {
            if (!IsMutable || (HasMax && maxValue < MaxValue)) return;
            if (maxValue == MaxValue)  // It's a tie, increment the counter.
            {
                MaxCount += maxCount;
                return;
            }
            MaxValue = maxValue;
            MaxBody = maxBody;
            MaxCount = maxCount;

            // TODO: Tell the Manager?
        }

        public void SetOrUpdateMin(string minBody, double minValue, int minCount = 1)
        {
            if (!IsMutable || (HasMin && minValue > MinValue)) return;
            if (minValue == MinValue)  // It's a tie, increment the counter.
            {
                MinCount += minCount;
                return;
            }
            MinBody = minBody;
            MinValue = minValue;
            MinCount = minCount;

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
