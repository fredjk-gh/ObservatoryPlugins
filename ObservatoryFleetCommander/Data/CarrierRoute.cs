using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    public class CarrierRoute
    {
        private List<JumpInfo> _jumps = [];

        public string JobID { get; set; }

        public string StartSystem { get; set; }

        public string DestinationSystem { get; set; }

        public List<JumpInfo> Jumps { get => _jumps; set => _jumps = value; }

        public static CarrierRoute FromSpanshResultJson(JsonElement resultElement, string jobId = "")
        {
            CarrierRoute route = new()
            {
                JobID = jobId,
                StartSystem = resultElement.GetProperty("source").GetString(),
                DestinationSystem = resultElement.GetProperty("destinations")[0].GetString()
            };

            var jumpsArray = resultElement.GetProperty("jumps");
            for (int j = 0; j < jumpsArray.GetArrayLength(); j++)
            {
                var jump = JumpInfo.FromSpanshResultJson(jumpsArray[j]);
                if (j > 0 && jump.SystemName == route.Jumps[^1].SystemName)
                {
                    // This is a waypoint destination which appears twice in the results; don't include it.
                    continue;
                }
                route.Jumps.Add(jump);
            }

            return route;
        }

        public JumpInfo? Find(string systemName)
        {
            int index = IndexOf(systemName);
            if (index == -1) return null;

            return Jumps[index];
        }

        public JumpInfo? GetNextJump(string systemName)
        {
            int index = IndexOf(systemName);
            var nextJumpIndex = index + 1;

            if (index == -1 || nextJumpIndex >= Jumps.Count) return null;

            return Jumps[nextJumpIndex];
        }

        public bool IsDestination(string systemName)
        {
            return systemName == Jumps[^1].SystemName;
        }

        public int IndexOf(string systemName)
        {
            int index = -1;
            if (string.IsNullOrWhiteSpace(systemName)) return index;

            for (int j = 0; j < Jumps.Count; j++)
            {
                JumpInfo jump = Jumps[j];

                if (jump.SystemName == systemName)
                {
                    index = j;
                    break;
                }
            }

            return index;
        }
    }
}
