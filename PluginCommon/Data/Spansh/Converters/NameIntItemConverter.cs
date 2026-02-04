using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters
{
    internal class NameIntItemConverter<T> : JsonConverter<List<T>> where T : NameIntItem, new()
    {
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<T> parsed = [];

            var node = JsonNode.Parse(ref reader);
            foreach (var property in node.AsObject())
            {
                parsed.Add(new T()
                {
                    Name = property.Key,
                    Value = Convert.ToInt32(property.Value.GetValue<Double>()),
                });
            }
            return parsed;
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            JsonObject jsonObject = [];
            foreach (var item in value)
            {
                jsonObject[item.Name] = item.Value;
            }
            jsonObject.WriteTo(writer);
        }
    }
}
