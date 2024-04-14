using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    public class CoordConverter : JsonConverter<(double x, double y, double z)>
    {
        public override (double x, double y, double z) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            double[] values = (double[])JsonSerializer.Deserialize(ref reader, typeof(double[]));

            return (x: values[0], y: values[1], z: values[2]);
        }

        public override void Write(Utf8JsonWriter writer, (double x, double y, double z) value, JsonSerializerOptions options)
        {
            double[] serializeMe = new double[3] { value.x, value.y, value.z };

            JsonSerializer.Serialize(writer, serializeMe, options);
        }
    }
}
