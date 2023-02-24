using ObservatoryStatScanner.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    public class PersonalBestData : IRecordData
    {
        // For creating non EDAstro-based records.
        public PersonalBestData(RecordTable table, string objectName, string variable)
        {
            Table = table;
            EDAstroObjectName = objectName;
            JournalObjectName = objectName;
            Variable = variable;
        }

        // Assumed to be copying from galactic record CSV field data... We copy in metadata, but omit the actual values.
        public PersonalBestData(string[] copyFrom)
        {
            Table = RecordTableFromString(copyFrom[Constants.CSV_Table]);
            EDAstroObjectName = copyFrom[Constants.CSV_Type];
            JournalObjectName = null;
            if (Constants.JournalTypeMap.ContainsKey(EDAstroObjectName))
            {
                JournalObjectName = Constants.JournalTypeMap[EDAstroObjectName];
            }
            Variable = copyFrom[Constants.CSV_Variable];
        }

        public override bool IsMutable => true;
    }
}
