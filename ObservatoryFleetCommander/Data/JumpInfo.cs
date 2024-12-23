using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    public class JumpInfo
    {
        public string SystemName { get; set; }
        public ulong SystemAddress { get; set; }

        [JsonConverter(typeof(StarPosConverter))]
        public StarPosition Position { get; set; }
        public double Distance { get; set; }
        public double DistanceFromDestination { get; set; }
        public int FuelRequired { get; set; }

        public static JumpInfo FromSpanshResultJson(JsonElement jumpElement)
        {
            JumpInfo jump = new JumpInfo();

            jump.SystemName = jumpElement.GetProperty("name").GetString();
            jump.SystemAddress = jumpElement.GetProperty("id64").GetUInt64();
            jump.Distance = jumpElement.GetProperty("distance").GetDouble();
            jump.DistanceFromDestination = jumpElement.GetProperty("distance_to_destination").GetDouble();
            jump.FuelRequired = jumpElement.GetProperty("fuel_used").GetInt32();
            jump.Position = new() {
                x = jumpElement.GetProperty("x").GetDouble(),
                y = jumpElement.GetProperty("y").GetDouble(),
                z = jumpElement.GetProperty("z").GetDouble()
            };
            return jump;
        }
    }
}
