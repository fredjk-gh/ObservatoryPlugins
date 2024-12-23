using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    // DEPRECATED
    public class CoordConverter : JsonConverter<StarPosition>
    {
        public override StarPosition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            double[] values = (double[])JsonSerializer.Deserialize(ref reader, typeof(double[]));

            return new StarPosition() { x = values[0], y = values[1], z = values[2] };
        }

        public override void Write(Utf8JsonWriter writer, StarPosition value, JsonSerializerOptions options)
        {
            double[] serializeMe = new double[3] { value.x, value.y, value.z };

            JsonSerializer.Serialize(writer, serializeMe, options);
        }
    }
}
