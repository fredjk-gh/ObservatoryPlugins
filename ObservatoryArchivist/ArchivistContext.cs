using com.github.fredjk_gh.ObservatoryArchivist.UI;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class ArchivistContext
    {
        private const string DATA_CACHE_FILENAME = "archivistStateCache.json";

        public ArchivistContext()
        {
            Data = new();
        }

        public IObservatoryCore Core { get; init; }
        
        public Archivist PluginWorker { get; init; }

        public ArchiveManager Manager { get; init; }

        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        public Action<Exception, string> ErrorLogger { get; init; }

        public ArchivistSettings Settings { get; init; }

        public ArchivistData Data { get; set; }

        public ArchivistUI UI { get; set; }

        public bool IsReadAll { get => (Core.CurrentLogMonitorState & LogMonitorState.Batch) != 0; }

        public bool IsResending { get; private set; }

        public void FlushIfDirty(bool forceFlush = false)
        {
            if (IsReadAll && !forceFlush) return;

            foreach (var c in Data.KnownCommanders.Values)
            {
                if (c.CurrentSystem != null && c.CurrentSystem.IsDirty)
                {
                    Manager.UpsertSystemData(c.CurrentSystem);
                }
            }
        }

        public void SerializeState(bool forceWrite = false)
        {
            if (IsReadAll && !forceWrite) return;

            string dataCacheFile = $"{Core.PluginStorageFolder}{DATA_CACHE_FILENAME}";
            string jsonString = JsonSerializer.Serialize(Data,
                new JsonSerializerOptions() { AllowTrailingCommas = true, WriteIndented = true });
            File.WriteAllText(dataCacheFile, jsonString);
        }

        public void DeserializeState()
        {
            string dataCacheFile = $"{Core.PluginStorageFolder}{DATA_CACHE_FILENAME}";
            if (!File.Exists(dataCacheFile)) return;

            try
            {
                string jsonString = File.ReadAllText(dataCacheFile);
                ArchivistData dataCache = JsonSerializer.Deserialize<ArchivistData>(jsonString);

                Data = dataCache;

                foreach (var cmdrData in Data.KnownCommanders)
                {
                    // Fetch current system data from DB (not serialized to the cache for brevity/simplicity).
                    if (!string.IsNullOrWhiteSpace(cmdrData.Value.CurrentSystemName) && cmdrData.Value.CurrentSystem == null)
                    {
                        var currentSystem = Manager.Get(cmdrData.Value.CurrentSystemName, cmdrData.Key);
                        if (currentSystem != null) // This may happen after an aborted Read-all.
                            cmdrData.Value.CurrentSystem = new(currentSystem);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger(ex, "Deserializing Archivist state cache");
            }
        }

        public void DisplaySummary(string prefix = "")
        {
            var summaryData = Manager.GetSummary();
            StringBuilder sb = new StringBuilder(prefix);

            if (!string.IsNullOrWhiteSpace(prefix)) sb.AppendLine();

            foreach (var r in summaryData)
            {
                sb.AppendLine($"{r["SystemCount"]} known systems for Cmdr {r["Cmdr"]}.");
            }

            UI.SetMessage(sb.ToString());
        }

        internal void SetResendAll(bool isResending)
        {
            IsResending = isResending;
        }
    }
}
