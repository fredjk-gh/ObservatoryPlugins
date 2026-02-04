using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class ArchivistCommanderData
    {
        private CurrentSystemInfo _currentSystemInfo = null;
        private string _currentSystemName = "";

        public ArchivistCommanderData()
        {
            FileHeaderInfo = new();
        }

        public string CommanderName
        {
            get => FileHeaderInfo.Commander;
            set => FileHeaderInfo.Commander = value;
        }

        public string CurrentSystemName
        {
            get => CurrentSystem == null ? _currentSystemName : _currentSystemName = CurrentSystem.SystemName;
            set => _currentSystemName = value;
        }

        public FileHeaderInfo FileHeaderInfo { get; set; }

        [JsonIgnore]
        public CurrentSystemInfo CurrentSystem
        {
            get => _currentSystemInfo;
            set => _currentSystemInfo = value;
        }
    }
}
