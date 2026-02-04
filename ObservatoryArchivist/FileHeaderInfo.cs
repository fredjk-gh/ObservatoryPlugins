using Observatory.Framework.Files.Journal;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class FileHeaderInfo
    {
        public string Commander { get; set; }

        [JsonIgnore]
        public FileHeader FileHeader { get; set; }
        [JsonIgnore]
        public LoadGame LoadGame { get; set; }
        [JsonIgnore]
        public Statistics Statistics { get; set; }

        [JsonIgnore]
        public List<string> PreambleJournalEntries
        {
            get
            {
                List<string> entries = [];
                if (FileHeader != null) entries.Add(FileHeader.Json);
                if (LoadGame != null) entries.Add(LoadGame.Json);
                if (Statistics != null) entries.Add(Statistics.Json);
                return entries;
            }
        }
    }
}
