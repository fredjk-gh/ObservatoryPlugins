using com.github.fredjk_gh.PluginCommon.Data.Spansh.System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters
{
    public class ParentsConverter : JsonConverter<List<ParentBody>>
    {
        public override List<ParentBody> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<ParentBody> parsed = new();

            var node = JsonNode.Parse(ref reader);
            foreach (var item in node.AsArray())
            {
                foreach (var parent in item.AsObject())
                {
                    parsed.Add(new ParentBody()
                    {
                        ParentType = parent.Key,
                        ParentBodyId = ((int)parent.Value.AsValue()),
                    });
                }
            }
            return parsed;
        }

        public override void Write(Utf8JsonWriter writer, List<ParentBody> value, JsonSerializerOptions options)
        {
            JsonArray array = new JsonArray();
            foreach (var item in value)
            {
                JsonObject jsonObject = new JsonObject();
                jsonObject[item.ParentType] = item.ParentBodyId;
                array.Add(jsonObject);
            }
            array.WriteTo(writer);
        }
    }
}