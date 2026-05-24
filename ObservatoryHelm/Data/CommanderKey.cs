using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    // Do not rely on FID. It is not present in older journals.
    public class CommanderKey
    {
        public static CommanderKey FromLoadGame(LoadGame loadGame)
        {
            return new(loadGame.Commander, loadGame.FID);
        }

        public static bool TryParse(string serialized, out CommanderKey key)
        {
            key = null;
            if (string.IsNullOrWhiteSpace(serialized)) return false;

            try
            {
                string[] parts = serialized.Split('|');
                if (parts.Length > 1)
                {
                    key = new(/*Name*/ parts[1], /*FID*/ parts[0]);
                    return true;
                }
                // No delimiter, Name only.
                else if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0]))
                {
                    key = new(parts[0]);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public CommanderKey(string name, string fid = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Commander Name is null/empty!");
            }
            Name = name;
            FID = fid;
        }

        /// <summary>
        /// This may be empty or null in older jounals. Don't rely on it.
        /// </summary>
        public string FID { get; set; }

        public string Name { get; init; }

        public override string ToString()
        {
            return $"{Name}";
        }

        public override bool Equals(object obj)
        {
            if (obj is not CommanderKey key) return false;

            return Name == key.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
