using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner
{
    class StatScannerSettings
    {
        //[SettingDisplayName("Enable tracking personal bests (read-all after enabling)")]
        //public bool EnablePersonalBests { get; set; }

        [SettingDisplayName("Near Maximum record threshold % (Caution: high values can generate lots of hits; 0 disables [default])")]
        [SettingNumericBounds(0.0, 10.0)]
        public int MaxNearRecordThreshold { get; set; }

        [SettingDisplayName("Near Minimum record threshold % (Caution: high values can generate lots of hits; 0 disables [default])")]
        [SettingNumericBounds(0.0, 20.0)]
        public int MinNearRecordThreshold { get; set; }

        // For some combinations (ie. MIN(icy body surface pressure) == 0), the
        // extreme value is VERY common meaning we generate a lot of ties and this
        // slows down the UI rendering incredibly (not to mention, it's not terribly
        // interesting/rare). So if there's a high cardinality of bodies sharing
        // a record, we don't care.
        [SettingDisplayName("Control frequency of ties with very common records (lower means fewer hits)")]
        [SettingNumericBounds(1000, 100000, 1000)]
        public int HighCardinalityTieSuppression { get; set; }

        [SettingDisplayName("Show Records for first discovered objects only")]
        public bool FirstDiscoveriesOnly { get; set; }

        // Individual record tracking controls
        //
        // Per Body records
        // Planets
        [SettingDisplayName("Enable checking Planet Mass records")]
        public bool EnableEarthMassesRecord { get; set; }

        [SettingDisplayName("Enable checking Planet Radius records")]
        public bool EnablePlanetaryRadiusRecord { get; set; }

        [SettingDisplayName("Enable checking Surface Gravity records")]
        public bool EnableSurfaceGravityRecord { get; set; }

        [SettingDisplayName("Enable checking Surface Pressure records")]
        public bool EnableSurfacePressureRecord { get; set; }

        // Planets + stars common
        [SettingDisplayName("Enable checking Surface Temperature records")]
        public bool EnableSurfaceTemperatureRecord { get; set; }

        [SettingDisplayName("Enable checking Orbital Eccentricity records")]
        public bool EnableOrbitalEccentricityRecord { get; set; }

        [SettingDisplayName("Enable checking Orbital Period records")]
        public bool EnableOrbitalPeriodRecord { get; set; }

        // Stars
        [SettingDisplayName("Enable checking Star Mass records")]
        public bool EnableSolarMassesRecord { get; set; }

        [SettingDisplayName("Enable checking Star Radius records")]
        public bool EnableSolarRadiusRecord { get; set; }

        [SettingDisplayName("Force Update Galactic Records")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action ForceUpdateGalacticRecords { get; internal set; }

        [SettingIgnore]
        public bool DevMode { get => false; }
    }
}
