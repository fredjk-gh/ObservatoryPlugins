using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    internal class CSVListBuilder
    {
        private const string CSV_RESOURCE_PREFIX = "com.github.fredjk_gh.PluginCommon.Resources.";

        #region Dictionary Builder
        public class DictBuilderOptions<K, V>
        {
            public DictBuilderOptions(string csvData, string headers, Func<string[], K> keyFactory, Func<string[], V> itemFactory)
            {
                CsvData = csvData;
                ExpectedHeaders = headers;
                KeyFactory = keyFactory;
                ItemFactory = itemFactory;
            }
            public string CsvData { get; init; }
            public string ExpectedHeaders { get; init; }
            public Func<string[], K> KeyFactory { get; init; }
            public Func<string[], V> ItemFactory { get; init; }
        }

        internal static Dictionary<K, V> DictFromCSV<K, V>(DictBuilderOptions<K, V> opts)
        {
            Dictionary<K, V> result = new();
            
            bool headerSeen = false;
            int headerColumnsCount = -1;
            foreach (var l in opts.CsvData.Split('\n'))
            {
                var line = l.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line == opts.ExpectedHeaders)
                {
                    headerSeen = true;
                    headerColumnsCount = line.Split(',').Length;
                    continue;
                }
                if (!headerSeen || headerColumnsCount <= 0) throw new InvalidDataException($"{opts.CsvData} format has changed or has invalid header! Unable to proceed.");

                var parts = line.Split(',');
                if (parts.Length != headerColumnsCount)
                {
                    Debug.WriteLine($"File {opts.CsvData} contains invalid data; expected {headerColumnsCount} columns: {line}");
                    continue;
                }

                result.Add(opts.KeyFactory(parts), opts.ItemFactory(parts));
            }
            return result;
        }
        #endregion

        #region List Builder

        public class ListBuilderOptions<V>
        {
            public ListBuilderOptions(string rawCsvData, string headers, Func<string, V> itemFactory)
            {
                CsvData = rawCsvData;
                ExpectedHeaders = headers;
                ItemFactory = itemFactory;
            }
            public string CsvData { get; init; }
            public string ExpectedHeaders { get; init; }
            public Func<string, V> ItemFactory { get; init; }
        }

        internal static List<V> ListFromCSV<V>(ListBuilderOptions<V> opts)
        {
            List<V> result = new();

            bool headerSeen = false;
            foreach (var l in opts.CsvData.Split('\n'))
            {
                var line = l.Trim();
                if (line == opts.ExpectedHeaders)
                {
                    headerSeen = true;
                    continue;
                }
                if (!headerSeen) throw new InvalidDataException($"{opts.CsvData} format has changed or has invalid header! Unable to proceed.");

                result.Add(opts.ItemFactory(line));
            }
            return result;
        }
        #endregion
    }
}
