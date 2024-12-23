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
    internal class NameIntItemConverter : JsonConverter<List<NameIntItem>>
    {
        public override List<NameIntItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<NameIntItem> parsed = new();

            var node = JsonNode.Parse(ref reader);
            foreach (var property in node.AsObject())
            {
                parsed.Add(new NameIntItem()
                {
                    Name = property.Key,
                    Value = Convert.ToInt32(property.Value.GetValue<Double>()),
                });
            }
            return parsed;
        }

        public override void Write(Utf8JsonWriter writer, List<NameIntItem> value, JsonSerializerOptions options)
        {
            JsonObject jsonObject = new JsonObject();
            foreach (var item in value)
            {
                jsonObject[item.Name] = item.Value;
            }
            jsonObject.WriteTo(writer);
        }
    }
}
