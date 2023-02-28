using ObservatoryStatScanner.Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.DB
{
    public class PersonalBest
    {
        public long _id { get; set; }
        public RecordTable Table { get; set; }
        public string Variable { get;  set; }
        public string EDAstroObjectName { get;  set; }
        public string JournalObjectName { get;  set; }

        public string MaxHolder { get;  set; }
        public long MaxCount { get;  set; }
        public double MaxValue { get;  set; }

        public string MinHolder { get;  set; }
        public long MinCount { get;  set; }
        public double MinValue { get;  set; }

        // An arbitrary string value.
        public string ExtraData { get; set; }

        public void MergeFrom(PersonalBest pb)
        {
            Debug.Assert(Table == pb.Table);
            Debug.Assert(EDAstroObjectName == pb.EDAstroObjectName);
            Debug.Assert(Variable == pb.Variable);

            _id = pb._id;
            MaxHolder = pb.MaxHolder;
            MaxCount = pb.MaxCount;
            MaxValue = pb.MaxValue;
            MinHolder = pb.MinHolder;
            MinCount = pb.MinCount;
            MinValue = pb.MinValue;
            ExtraData = pb.ExtraData;
        }
    }
}
