using Observatory.Framework.Files.ParameterTypes;
using System.Text.Json;

namespace com.github.fredjk_gh.PluginCommon.Data.EdAstro
{
    public static class EdAstroHelper
    {
        // Caller is responsible for handling exceptions.
        // This should probably be run on a non-UI thread.
        // TODO: improve debuggability / logging by passing in something from the caller.
        public static StarPosition LookupCoords(HttpClient httpClient, string systemName, CancellationToken ctoken)
        {
            string url = $"https://edastro.com/api/starsystem?q={Uri.EscapeDataString(systemName)}";
            JsonElement root;
            var jsonFetchTask = httpClient.GetStringAsync(url, ctoken);

            string jsonStr = jsonFetchTask.Result;
            if (string.IsNullOrWhiteSpace(jsonStr)) return null;

            using var jsonDoc = JsonDocument.Parse(jsonStr);
            root = jsonDoc.RootElement;
            // root[0].coordinates is an array of 3 doubles.
            if (root.GetArrayLength() > 0 && root[0].GetProperty("coordinates").GetArrayLength() == 3)
            {
                var coordsArray = root[0].GetProperty("coordinates");

                StarPosition position = new()
                {
                    x = coordsArray[0].GetDouble(),
                    y = coordsArray[1].GetDouble(),
                    z = coordsArray[2].GetDouble()
                };
                return position;
            }
            return null;
        }
    }
}
