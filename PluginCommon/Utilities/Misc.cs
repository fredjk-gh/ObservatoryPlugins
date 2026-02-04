using System.Diagnostics;
using System.Text.RegularExpressions;

namespace com.github.fredjk_gh.PluginCommon.Utilities
{
    public static partial class Misc
    {
        public static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        public static void SetTextToClipboard(string value)
        {
            try
            {
                Clipboard.SetText(value);
            }
            catch
            { }
        }

        public static string SplitCamelCase(this string str)
        {
            return LowerUpper().Replace(UpperUpperLower().Replace(str, "$1 $2"), "$1 $2");
        }

        // https://stackoverflow.com/questions/5796383/insert-spaces-between-words-on-a-camel-cased-token
        [GeneratedRegex(@"(\p{Ll})(\P{Ll})")]
        private static partial Regex LowerUpper();
        [GeneratedRegex(@"(\P{Ll})(\P{Ll}\p{Ll})")]
        private static partial Regex UpperUpperLower();
    }
}
