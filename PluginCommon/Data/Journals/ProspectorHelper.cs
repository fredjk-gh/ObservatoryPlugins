using Observatory.Framework.Files.Journal;
using System.Security.Cryptography;
using System.Text;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals
{
    public class ProspectorHelper
    {
        public static string GetEventFingerprint(ProspectedAsteroid prospectedAsteroid)
        {
            byte[] raw = SHA256.HashData(Encoding.UTF8.GetBytes(prospectedAsteroid.Json));

            var sb = new StringBuilder();
            for (int i = 0; i < raw.Length; i++)
            {
                sb.Append(raw[i].ToString("x2")); // convert to hexadecimal
            }

            return sb.ToString();
        }
    }
}
