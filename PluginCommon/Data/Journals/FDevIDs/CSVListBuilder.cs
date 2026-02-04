using System.Diagnostics;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    internal class CSVListBuilder
    {
        #region Dictionary Builder
        public class DictBuilderOptions<K, V>(string csvData, string headers, Func<string[], K> keyFactory, Func<string[], V> itemFactory)
        {
            public string CsvData { get; init; } = csvData;
            public string ExpectedHeaders { get; init; } = headers;
            public Func<string[], K> KeyFactory { get; init; } = keyFactory;
            public Func<string[], V> ItemFactory { get; init; } = itemFactory;
        }

        internal static Dictionary<K, V> DictFromCSV<K, V>(DictBuilderOptions<K, V> opts)
        {
            Dictionary<K, V> result = [];

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

        public class ListBuilderOptions<V>(string rawCsvData, string headers, Func<string, V> itemFactory)
        {
            public string CsvData { get; init; } = rawCsvData;
            public string ExpectedHeaders { get; init; } = headers;
            public Func<string, V> ItemFactory { get; init; } = itemFactory;
        }

        internal static List<V> ListFromCSV<V>(ListBuilderOptions<V> opts)
        {
            List<V> result = [];

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
