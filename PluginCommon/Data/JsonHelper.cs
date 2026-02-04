using com.github.fredjk_gh.PluginCommon.Data.Spansh.System;
using Observatory.Framework.Files.Journal;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerOptions PRETTY_PRINT_OPTIONS = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
        };

        public static readonly JsonSerializerOptions STANDARD_PARSE_OPTIONS = new()
        {
            AllowTrailingCommas = true,
        };

        public static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static readonly JsonSerializerOptions JSON_FIELD_SERIALIZER_OPTIONS = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IncludeFields = true,
        };

        public static string PrettyPrintJson(string rawJson)
        {
            return PrettyPrintJson(rawJson, PRETTY_PRINT_OPTIONS);
        }

        public static string PrettyPrintJson(string rawJson, JsonSerializerOptions options)
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(rawJson);

            return JsonSerializer.Serialize(jsonElement, options);
        }

        public static SpanshSystem ParseSpanshSystemDump(string spanshJson)
        {
            var deserialized = JsonSerializer.Deserialize<SpanshSystemDump>(spanshJson, STANDARD_PARSE_OPTIONS);

            return deserialized.System;
        }

        public static List<string> SerializeJournals(List<JournalBase> augmentedJournals)
        {
            List<string> exportedJson = [];
            foreach (var journal in augmentedJournals)
            {
                string json = SerializeJournal(journal);
                exportedJson.Add(json);
            }
            return exportedJson;
        }

        public static string SerializeJournal(JournalBase journal)
        {
            string json = string.Empty;
            switch (journal.Event)
            {
                case "Location":
                    json = JsonSerializer.Serialize((Location)journal, JSON_SERIALIZER_OPTIONS);
                    break;
                case "FSSDiscoveryScan":
                    json = JsonSerializer.Serialize((FSSDiscoveryScan)journal, JSON_SERIALIZER_OPTIONS);
                    break;
                case "FSSBodySignals":
                    json = JsonSerializer.Serialize((FSSBodySignals)journal, JSON_SERIALIZER_OPTIONS);
                    break;
                case "ScanBaryCentre":
                    json = JsonSerializer.Serialize((ScanBaryCentre)journal, JSON_SERIALIZER_OPTIONS);
                    break;
                case "Scan":
                    json = JsonSerializer.Serialize((Scan)journal, JSON_SERIALIZER_OPTIONS);
                    break;
                case "SAAScanComplete":
                    json = JsonSerializer.Serialize((SAAScanComplete)journal, JSON_SERIALIZER_OPTIONS);
                    break;
                case "SAASignalsFound":
                    json = JsonSerializer.Serialize((SAASignalsFound)journal, JSON_SERIALIZER_OPTIONS);
                    break;
                case "FSSAllBodiesFound":
                    json = JsonSerializer.Serialize((FSSAllBodiesFound)journal, JSON_SERIALIZER_OPTIONS);
                    break;
            }
            if (!string.IsNullOrEmpty(json)) journal.Json = json;
            return json;
        }

        public static string SerialzeFIeldObject(object o)
        {
            return JsonSerializer.Serialize(o, JSON_FIELD_SERIALIZER_OPTIONS);
        }

        public static T DeserializeFieldObject<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, JSON_FIELD_SERIALIZER_OPTIONS);
        }
    }
}
