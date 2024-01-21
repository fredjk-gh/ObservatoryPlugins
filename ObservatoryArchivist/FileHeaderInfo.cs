using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class FileHeaderInfo
    {
        public string Commander { get; set; }
        public FileHeader FileHeader { get; set; }
        public LoadGame LoadGame { get; set; }
        public Statistics Statistics { get; set; }

        public List<string> PreambleJournalEntries {
            get 
            {
                List<string> entries = new();
                if (FileHeader != null) entries.Add(FileHeader.Json);
                if (LoadGame != null) entries.Add(LoadGame.Json);
                if (Statistics != null) entries.Add(Statistics.Json);
                return entries;
            }
        }
    }
}
