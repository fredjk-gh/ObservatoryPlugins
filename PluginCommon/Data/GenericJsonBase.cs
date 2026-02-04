using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data
{
    public class GenericJsonBase
    {
        internal static DateTime? ParseDateTime(string value)
        {
            if (!string.IsNullOrWhiteSpace(value)
                && DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateTimeValue))
            {
                return dateTimeValue;
            }
            else
            {
                return null;
            }
        }

        [JsonExtensionData]
        Dictionary<string, object> AdditionalProperties { get; init; }
    }
}
