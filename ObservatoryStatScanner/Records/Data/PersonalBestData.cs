using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    public class PersonalBestData : IRecordData
    {
        private readonly DB.PersonalBest dbRow = new();
        private DB.PersonalBestManager manager;

        // For creating non EDAstro-based records.
        public PersonalBestData(RecordTable table, string objectName, string variable, string journalObjectName = null)
        {
            dbRow.Table = table;
            dbRow.EDAstroObjectName = objectName;
            dbRow.JournalObjectName = journalObjectName ?? objectName;
            dbRow.Variable = variable;
        }

        // Assumed to be copying from galactic record CSV field data... We copy in metadata, but omit the actual values.
        public PersonalBestData(string[] copyFrom)
        {
            dbRow.Table = RecordTableFromString(copyFrom[Constants.CSV_Table]);
            dbRow.EDAstroObjectName = copyFrom[Constants.CSV_Type];
            dbRow.JournalObjectName = null;
            if (Constants.JournalTypeMap.ContainsKey(dbRow.EDAstroObjectName))
            {
                dbRow.JournalObjectName = Constants.JournalTypeMap[dbRow.EDAstroObjectName];
            }
            dbRow.Variable = copyFrom[Constants.CSV_Variable];
        }

        public override bool IsMutable => true;

        public override RecordTable Table { get => dbRow.Table; protected set => dbRow.Table = value; }
        public override string Variable { get => dbRow.Variable; protected set => dbRow.Variable = value; }
        public override string EDAstroObjectName { get => dbRow.EDAstroObjectName; protected set => dbRow.EDAstroObjectName = value; }
        public override string JournalObjectName { get => dbRow.JournalObjectName; protected set => dbRow.JournalObjectName = value; }

        public override string MaxHolder { get => dbRow.MaxHolder; protected set => dbRow.MaxHolder = value; }
        public override long MaxCount { get => dbRow.MaxCount; protected set => dbRow.MaxCount = value; }
        public override double MaxValue { get => dbRow.MaxValue; protected set => dbRow.MaxValue = value; }
        public override DateTime MaxRecordDateTime { get => dbRow.MaxRecordDateTime; protected set => dbRow.MaxRecordDateTime = value; }

        public override string MinHolder { get => dbRow.MinHolder; protected set => dbRow.MinHolder = value; }
        public override long MinCount { get => dbRow.MinCount; protected set => dbRow.MinCount = value; }
        public override double MinValue { get => dbRow.MinValue; protected set => dbRow.MinValue = value; }
        public override DateTime MinRecordDateTime { get => dbRow.MinRecordDateTime; protected set => dbRow.MinRecordDateTime = value; }

        public override string ExtraData { get => dbRow.ExtraData; set => dbRow.ExtraData = value; }

        public override void Init(DB.PersonalBestManager manager)
        {
            this.manager = manager;

            manager.Load(dbRow);
        }

        internal override void Save()
        {
            if (!IsMutable || manager == null) return;

            manager.Upsert(dbRow);
        }

        public override void ResetMutable()
        {
            base.ResetMutable();
            dbRow._id = default;
        }

        public PersonalBestData Clone()
        {
            PersonalBestData clone = new(dbRow.Table, dbRow.EDAstroObjectName, dbRow.Variable);
            clone.dbRow.JournalObjectName = dbRow.JournalObjectName;
            return clone;
        }
    }
}
