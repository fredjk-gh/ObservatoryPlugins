using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    public class CommanderKey
    {
        public static CommanderKey FromLoadGame(LoadGame loadGame)
        {
            return new(loadGame.FID, loadGame.Commander);
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
                    key = new(parts[0], parts[1]);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }


        public CommanderKey(string fid, string name)
        {
            if (string.IsNullOrWhiteSpace(fid) || string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("FID or Name is null/empty!");
            }
            FID = fid;
            Name = name;
        }

        public string FID { get; init; }
        public string Name { get; init; }

        public override string ToString()
        {
            return $"{FID}|{Name}";
        }

        public override bool Equals(object obj)
        {
            if (obj is not CommanderKey key) return false;

            return FID == key.FID && Name == key.Name;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
