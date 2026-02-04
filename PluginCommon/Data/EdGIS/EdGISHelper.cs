using Observatory.Framework.Files.ParameterTypes;
using System.Text.Json;

namespace com.github.fredjk_gh.PluginCommon.Data.EdGIS
{
    public static class EdGISHelper
    {
        // Caller is responsible for handling exceptions.
        // This should probably be run on a non-UI thread.
        // TODO: improve debuggability / logging by passing in something from the caller.
        public static CoordsResponse LookupCoords(HttpClient httpClient, CancellationToken ctoken, string systemName = "", UInt64? id64 = null)
        {
            string url;
            if (!string.IsNullOrEmpty(systemName))
            {
                url = $"https://edgis.elitedangereuse.fr/coords?q={Uri.EscapeDataString(systemName)}";
            }
            else if (id64.HasValue)
            {
                url = $"https://edgis.elitedangereuse.fr/coords?q={id64.Value}";
            }
            else
            {
                throw new ArgumentException($"systemName and id64 cannot both be blank or unset.");
            }

            if (!string.IsNullOrEmpty(url))
            {
                var fetchTask = httpClient.GetStringAsync(url, ctoken);
                var json = fetchTask.Result;
                return JsonSerializer.Deserialize<CoordsResponse>(json);
            }
            return null;
        }

        public static StarPosition CoordsToStarPosition(Coords c)
        {
            return new()
            {
                x = c.X,
                y = c.Y,
                z = c.Z,
            };
        }
    }
}
