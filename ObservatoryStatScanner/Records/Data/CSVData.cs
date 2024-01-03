using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    public class CSVData : IRecordData
    {
        private static readonly CultureInfo EN_US = CultureInfo.GetCultureInfo("en-US");

        public CSVData(string[] csvFields)
        {
            Table = RecordTableFromString(csvFields[Constants.CSV_Table]);
            EDAstroObjectName = csvFields[Constants.CSV_Type];
            JournalObjectName = null;
            if (Constants.JournalTypeMap.ContainsKey(EDAstroObjectName))
            {
                JournalObjectName = Constants.JournalTypeMap[EDAstroObjectName];
            }

            if (IsValid)
            { // Don't spend time parsing if we don't need to.
                Variable = csvFields[Constants.CSV_Variable];
                MinHolder = csvFields[Constants.CSV_MinBody];
                MaxHolder = csvFields[Constants.CSV_MaxBody];
                string errorContext = "";
                try
                {
                    errorContext = $"Error parsing MinCount from {csvFields[Constants.CSV_MinCount]}";
                    MinCount = long.Parse(csvFields[Constants.CSV_MinCount], EN_US);
                    errorContext = $"Error parsing MinValue from {csvFields[Constants.CSV_MinValue]}";
                    MinValue = double.Parse(csvFields[Constants.CSV_MinValue], EN_US);
                    errorContext = $"Error parsing MaxCount from {csvFields[Constants.CSV_MaxCount]}";
                    MaxCount = long.Parse(csvFields[Constants.CSV_MaxCount], EN_US);
                    errorContext = $"Error parsing MaxValue from {csvFields[Constants.CSV_MaxValue]}";
                    MaxValue = double.Parse(csvFields[Constants.CSV_MaxValue], EN_US);
                }
                catch (Exception ex)
                {
                    throw new RecordsCSVParseException(errorContext, ex);
                }
            }
        }

        public override bool IsMutable => false;
    }
}
