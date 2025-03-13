using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters
{
    internal class TimestampsConverter : JsonConverter<List<NamedTimestamp>>
    {
        public override List<NamedTimestamp> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<NamedTimestamp> parsed = new();

            var node = JsonNode.Parse(ref reader);
            foreach (var property in node.AsObject())
            {
                parsed.Add(new NamedTimestamp()
                {
                    Name = property.Key,
                    Value = property.Value.ToString(),
                });
            }
            return parsed;
        }

        public override void Write(Utf8JsonWriter writer, List<NamedTimestamp> value, JsonSerializerOptions options)
        {
            JsonObject jsonObject = new JsonObject();
            foreach (var namedTimestamp in value)
            {
                jsonObject[namedTimestamp.Name] = namedTimestamp.Value;
            }

            jsonObject.WriteTo(writer);
        }
    }
}
