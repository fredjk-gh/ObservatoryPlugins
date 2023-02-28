using ObservatoryStatScanner.DB;
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

        public virtual RecordTable Table { get; protected set; }
        public virtual string Variable { get; protected set; }
        public virtual string EDAstroObjectName { get; protected set; }
        public virtual string JournalObjectName { get; protected set; }
        public bool IsValid { get => (Table != RecordTable.Unknown && JournalObjectName != null); }

        public virtual bool IsMutable { get => false; }

        public bool HasMax { get => MaxValue > 0.0 && MaxHolder?.Length > 0; }
        public virtual string MaxHolder { get; protected set; }
        public virtual long MaxCount { get; protected set; }
        public virtual double MaxValue { get; protected set; }

        public bool HasMin { get => MinValue != 0.0 && MinHolder?.Length > 0; }
        public virtual string MinHolder { get; protected set; }
        public virtual long MinCount { get; protected set; }
        public virtual double MinValue { get; protected set; }

        // An arbitrary string value.
        public virtual string ExtraData { get; set; }

        public virtual void Init(PersonalBestManager manager) { }

        public virtual void ResetMutable()
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

        internal virtual void Save() { }

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

            Save();
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

            Save();
        }

        public static RecordTable RecordTableFromString(string str)
        {
            RecordTable table;
            if (Enum.TryParse(str, true, out table)) return table;

            Debug.WriteLine("Unknown table value found in galactic records csv file: " + str);
            return RecordTable.Unknown;
        }
    }
}
