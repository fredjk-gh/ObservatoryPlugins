using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class HelmDataCache
    {
        internal const int CURRENT_FILE_VERSON = 1;

        public static HelmDataCache FromTrackedData(TrackedData data)
        {
            return new()
            {
                IsOdyssey = data.IsOdyssey,
                CurrentCommander = data.CurrentCommander,
                CommanderData = [.. data.Commanders.Values],
            };
        }

        public long FileVersion
        {
            get => CURRENT_FILE_VERSON;
            set
            {
                Debug.Assert(value == CURRENT_FILE_VERSON);
            }
        }

        public bool IsOdyssey { get; set; }

        public CommanderKey CurrentCommander { get; set; }

        public List<CommanderData> CommanderData { get; set; }

        public void ToTrackedData(TrackedData data)
        {
            foreach (var d in CommanderData)
            {
                if (d.CurrentSystemData is not null)
                {
                    // Rewire objects with back references.
                    foreach (var s in d.CurrentSystemData?.Stars)
                    {
                        s.Value.Init(d.CurrentSystemData);
                    }
                    foreach (var p in d.CurrentSystemData?.Planets)
                    {
                        p.Value.Init(d.CurrentSystemData);
                    }
                    d.SystemInit();
                }
                data.AddCommanderData(d);
            }
            data.CurrentCommander = CurrentCommander;
            data.IsOdyssey = IsOdyssey;
        }
    }
}
