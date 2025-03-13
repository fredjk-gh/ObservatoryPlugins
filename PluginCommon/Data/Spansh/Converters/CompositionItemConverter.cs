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
    public class CompositionItemConverter : JsonConverter<List<CompositionItem>>
    {
        public override List<CompositionItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<CompositionItem> parsed = new();

            var node = JsonNode.Parse(ref reader);
            foreach (var property in node.AsObject())
            {
                parsed.Add(new CompositionItem()
                {
                    Material = property.Key,
                    Percent = ((double)property.Value.AsValue()),
                });
            }
            return parsed;
        }

        public override void Write(Utf8JsonWriter writer, List<CompositionItem> value, JsonSerializerOptions options)
        {
            if (value == null || value.Count == 0) return;

            JsonObject jsonObject = new JsonObject();
            foreach (var item in value)
            {
                jsonObject[item.Material] = item.Percent;
            }

            jsonObject.WriteTo(writer);
        }
    }
}
