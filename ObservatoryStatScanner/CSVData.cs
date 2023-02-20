﻿using ObservatoryStatScanner.Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner
{
    internal class CSVData
    {
        // Field indexes
        public const int CSV_Type = 0;
        public const int CSV_Variable = 1;
        public const int CSV_MaxCount = 2;
        public const int CSV_MaxValue = 3;
        public const int CSV_MaxBody = 4;
        public const int CSV_MinCount = 5;
        public const int CSV_MinValue = 6;
        public const int CSV_MinBody = 7;
        public const int CSV_Average = 8;
        public const int CSV_StandardDeviation = 9;
        public const int CSV_Count = 10;
        public const int CSV_Table = 11;

        private static readonly CultureInfo EN_US = CultureInfo.GetCultureInfo("en-US");

        public CSVData(string[] csvFields)
        {
            Table = RecordTableFromString(csvFields[CSV_Table]);
            EDAstroObjectName = csvFields[CSV_Type];
            JournalObjectName = null;
            if (Constants.JournalTypeMap.ContainsKey(EDAstroObjectName))
            {
                JournalObjectName = Constants.JournalTypeMap[EDAstroObjectName];
            }
            IsValid = (Table != RecordTable.Unknown && JournalObjectName != null);

            if (IsValid) { // Don't spend time parsing if we don't need to.
                Variable = csvFields[CSV_Variable];
                MinBody = csvFields[CSV_MinBody];
                MaxBody = csvFields[CSV_MaxBody];
                string errorContext = "";
                try
                {
                    errorContext = $"Error parsing MinCount from {csvFields[CSV_MinCount]}";
                    MinCount = Int64.Parse(csvFields[CSV_MinCount], EN_US);
                    errorContext = $"Error parsing MinValue from {csvFields[CSV_MinValue]}";
                    MinValue = Double.Parse(csvFields[CSV_MinValue], EN_US);
                    errorContext = $"Error parsing MaxCount from {csvFields[CSV_MaxCount]}";
                    MaxCount = Int64.Parse(csvFields[CSV_MaxCount], EN_US);
                    errorContext = $"Error parsing MaxValue from {csvFields[CSV_MaxValue]}";
                    MaxValue = Double.Parse(csvFields[CSV_MaxValue], EN_US);
                }
                catch (Exception ex)
                {
                    throw new RecordsCSVParseException(errorContext, ex);
                }
            }
        }

        public RecordTable Table { get; }
        public string Variable { get; }
        public string EDAstroObjectName { get; }
        public string JournalObjectName { get; }
        public string MaxBody { get; }
        public long MaxCount { get; }
        public double MaxValue { get; }
        public string MinBody { get; }
        public long MinCount { get; }
        public double MinValue { get; }
        public bool IsValid { get; }

        static RecordTable RecordTableFromString(string str)
        {
            switch (str)
            {
                case "planets":
                    return RecordTable.Planets;
                case "rings":
                    return RecordTable.Rings;
                case "stars":
                    return RecordTable.Stars;
                default:
                    Debug.WriteLine("Unknown table value found in galactic records csv file: " + str);
                    break;
            }
            return RecordTable.Unknown;
        }
    }
}
