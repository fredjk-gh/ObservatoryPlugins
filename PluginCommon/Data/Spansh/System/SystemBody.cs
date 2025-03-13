using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class SystemBody : UpdateTimeJsonBase
    {
        [JsonPropertyName("axialTilt")]
        public double AxialTilt { get; init; }

        [JsonPropertyName("bodyId")]
        public int BodyId { get; init; }

        [JsonPropertyName("distanceToArrival")]
        public double DistanceToArrival { get; init; }

        [JsonPropertyName("id64")]
        public ulong Id64 { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("parents")]
        [JsonConverter(typeof(ParentsConverter))]
        public List<ParentBody> Parents { get; init; }

        [JsonPropertyName("reserveLevel")]
        public string ReserveLevel { get; init; }

        private List<Asteroids> _rings;
        [JsonPropertyName("rings")]
        public List<Asteroids> Rings
        {
            get => _rings == null || _rings.Count == 0 ? null : _rings;
            init => _rings = value;
        }

        [JsonPropertyName("rotationalPeriod")]
        public double RotationalPeriod { get; init; } // In days

        [JsonPropertyName("rotationalPeriodTidallyLocked")]
        public bool RotationalPeriodTidallyLocked { get; init; }

        [JsonPropertyName("semiMajorAxis")]
        public double SemiMajorAxis { get; init; }  // In AU

        [JsonPropertyName("stations")]
        public List<SystemStation> Stations { get; init; }

        [JsonPropertyName("subType")]
        public string SubType { get; init; }

        [JsonPropertyName("surfaceTemperature")]
        public double SurfaceTemperature { get; init; }

        [JsonPropertyName("timestamps")]
        [JsonConverter(typeof(TimestampsConverter))]
        public List<NamedTimestamp> Timestamps { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; }

        // Star
        [JsonPropertyName("absoluteMagnitude")]
        public double AbsoluteMagnitude { get; init; } // Star

        [JsonPropertyName("age")]
        public int Age { get; init; } // Star

        [JsonPropertyName("belts")]
        public List<Asteroids> Belts { get; init; } // Star

        [JsonPropertyName("luminosity")]
        public string Luminosity { get; init; }

        [JsonPropertyName("mainStar")]
        public bool MainStar { get; init; }

        [JsonPropertyName("solarMasses")]
        public double SolarMasses { get; init; }

        [JsonPropertyName("solarRadius")]
        public double SolarRadius { get; init; }

        [JsonPropertyName("spectralClass")]
        public string SpectralClass { get; init; }

        // Planet
        [JsonPropertyName("argOfPeriapsis")]
        public double ArgOfPeriapsis { get; init; }

        [JsonPropertyName("ascendingNode")]
        public double AscendingNode { get; init; }

        private List<CompositionItem> _atmosphereComp;
        [JsonPropertyName("atmosphereComposition")]
        [JsonConverter(typeof(CompositionItemConverter))]
        public List<CompositionItem> AtmosphereComposition
        {
            get => _atmosphereComp == null || _atmosphereComp.Count == 0 ? null : _atmosphereComp;
            init => _atmosphereComp = value;
        }

        private string _atmosphereType;
        [JsonPropertyName("atmosphereType")]
        public string AtmosphereType
        {
            get => _atmosphereType ?? "None";
            init => _atmosphereType = value;
        }

        [JsonPropertyName("earthMasses")]
        public double EarthMasses { get; init; }

        [JsonPropertyName("gravity")]
        public double Gravity { get; init; }

        [JsonPropertyName("isLandable")]
        public bool IsLandable { get; init; }

        [JsonPropertyName("materials")]
        [JsonConverter(typeof(CompositionItemConverter))]
        public List<CompositionItem> Materials { get; init; }

        [JsonPropertyName("meanAnomaly")]
        public double MeanAnomaly { get; init; }

        [JsonPropertyName("orbitalEccentricity")]
        public double OrbitalEccentricity { get; init; }

        [JsonPropertyName("orbitalInclination")]
        public double OrbitalInclination { get; init; }

        [JsonPropertyName("orbitalPeriod")]
        public double OrbitalPeriod { get; init; } // In Days!

        [JsonPropertyName("radius")]
        public double Radius { get; init; } // in km

        [JsonPropertyName("signals")]
        public BodySignals Signals { get; init; }

        [JsonPropertyName("solidComposition")]
        [JsonConverter(typeof(CompositionItemConverter))]
        public List<CompositionItem> SolidComposition { get; init; }

        [JsonPropertyName("surfacePressure")]
        public double SurfacePressure { get; init; }

        private string _terraformingState;
        [JsonPropertyName("terraformingState")]
        public string TerraformingState
        {
            get
            {
                // TODO: Filter by body type?
                if (_terraformingState?.ToLower() == "not terraformable") return string.Empty;
                return _terraformingState;
            }
            init
            {
                _terraformingState = value;
            }
        }

        private string _volcanismType;
        [JsonPropertyName("volcanismType")]
        public string VolcanismType
        {
            get => _volcanismType ?? "";
            init => _volcanismType = value;
        }
    }
}
