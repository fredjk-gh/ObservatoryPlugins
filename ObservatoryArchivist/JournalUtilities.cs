using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class JournalUtilities
    {

        private static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static List<string> SerializeJournals(List<JournalBase> augmentedJournals)
        {
            List<string> exportedJson = new();
            foreach (var journal in augmentedJournals)
            {
                string json = SerializeJournal(journal);
                exportedJson.Add(json);
#if DEBUG
                Debug.WriteLine(json);
#endif
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

        public static VisitedSystem AugmentJournals(VisitedSystem original, List<JournalBase> converted)
        {
            return AugmentJournals(original, SerializeJournals(converted));
        }

        public static VisitedSystem AugmentJournals(VisitedSystem original, List<string> exportedJson)
        {
            VisitedSystem augmented = new VisitedSystem();
            augmented.Commander = original.Commander;
            augmented.SystemName = original.SystemName;
            augmented.SystemId64 = original.SystemId64;
            augmented.FirstVisitDateTime = original.FirstVisitDateTime;
            augmented.VisitCount = original.VisitCount;
            augmented.LastVisitDateTime = original.LastVisitDateTime;
            augmented.PreambleJournalEntries = original.PreambleJournalEntries;
            augmented.SystemJournalEntries = exportedJson;

            return augmented;
        }
    }
}
