namespace com.github.fredjk_gh.PluginCommon.PluginInterop
{
    public class PluginVersionNumber : IComparable<PluginVersionNumber>
    {
        internal const string NO_VERSION = "0.0.0.0";
        internal static readonly int[] NO_VERSION_PARSED = [0, 0, 0, 0];

        private readonly string _version = NO_VERSION;
        private readonly int[] _versionParsed = NO_VERSION_PARSED;

        public PluginVersionNumber(string version)
        {
            _version = version;
            _versionParsed = ParseVersion(_version);
        }

        public int[] VersionParsed { get; }

        public override string ToString()
        {
            return _version;
        }

        public int CompareTo(PluginVersionNumber other)
        {
            return Compare(this, other);
        }

        internal static int Compare(PluginVersionNumber leftObj, PluginVersionNumber rightObj)
        {
            var left = leftObj._versionParsed;
            var right = rightObj._versionParsed;

            int compareResult = 0; // equal.
            for (int i = 0; i < left.Length && i < right.Length; i++)
            {
                if (left[i] > right[i])
                {
                    return 1;
                }
                else if (left[i] < right[i])
                {
                    return -1; // All previous were equal, thus we can't be newer.
                }
            }

            return compareResult;
        }

        internal static int[] ParseVersion(string version)
        {
            if (string.IsNullOrEmpty(version) || version == NO_VERSION) return NO_VERSION_PARSED;

            if (version.StartsWith('v'))
            {
                version = version.Replace("v", ""); // Just in case I'm dumb and leave a v prefix.
            }
            string[] parts = version.Split('.');
            int[] numericParts = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                numericParts[i] = int.Parse(parts[i]);
            }
            return numericParts;
        }

    }
}
