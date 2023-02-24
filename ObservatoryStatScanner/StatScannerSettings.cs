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
        public const string DEFAULT_PROCGEN_HANDLING = "Ignore ProcGen records";
        public enum ProcGenHandlingMode
        {
            ProcGenIgnore,
            ProcGenPlusGalactic,
            ProcGenOnly,
        }

        // Output tuning knobs and options.
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

        [SettingDisplayName("How should the experimental ProcGen-only records be used?")]
        [SettingBackingValue("ProcGenHandling")]
        public Dictionary<string, object> ProcGenHandlingOptions
        {
            get => new()
            {
                {DEFAULT_PROCGEN_HANDLING, ProcGenHandlingMode.ProcGenIgnore},
                {"Use both ProcGen + Galactic records", ProcGenHandlingMode.ProcGenPlusGalactic},
                {"Only show ProcGen records (+ visited Galactic records)", ProcGenHandlingMode.ProcGenOnly}
            };
        }

        [SettingIgnore]
        public string ProcGenHandling { get; set; }

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

        [SettingDisplayName("Enable checking Odyssey surface bio records")]
        public bool EnableOdysseySurfaceBioRecord { get; set; }

        // Planets + stars common
        [SettingDisplayName("Enable checking Surface Temperature records")]
        public bool EnableSurfaceTemperatureRecord { get; set; }

        [SettingDisplayName("Enable checking Orbital Eccentricity records")]
        public bool EnableOrbitalEccentricityRecord { get; set; }

        [SettingDisplayName("Enable checking Orbital Period records")]
        public bool EnableOrbitalPeriodRecord { get; set; }

        [SettingDisplayName("Enable checking Rotational Period records")]
        public bool EnableRotationalPeriodRecord { get; set; }

        // Stars
        [SettingDisplayName("Enable checking Star Mass records")]
        public bool EnableSolarMassesRecord { get; set; }

        [SettingDisplayName("Enable checking Star Radius records")]
        public bool EnableSolarRadiusRecord { get; set; }

        // Rings
        [SettingDisplayName("Enable checking Ring Outer Radius records")]
        public bool EnableRingOuterRadiusRecord { get; set; }

        [SettingDisplayName("Enable checking Ring Width records")]
        public bool EnableRingWidthRecord { get; set; }

        [SettingDisplayName("Enable checking Ring Mass records")]
        public bool EnableRingMassRecord { get; set; }

        [SettingDisplayName("Enable checking Ring Density records")]
        public bool EnableRingDensityRecord { get; set; }

        // System stats
        [SettingDisplayName("Enable checking System Body Count records")]
        public bool EnableSystemBodyCountRecords { get; set; }

        // Regional stats
        [SettingDisplayName("Enable checking Visited Region Count records")]
        public bool EnableVisitedRegionRecords { get; set; }

        [SettingDisplayName("Enable checking Region Codex Count records")]
        public bool EnableRegionCodexCountRecords { get; set; }

        // Actions
        [SettingDisplayName("Force Update Galactic Records")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action ForceUpdateGalacticRecords { get; internal set; }

        // Internal
        [SettingIgnore]
        public bool DevMode { get => false; }

        [SettingIgnore]
        public double DevModeMaxScaleFactor = 0.8;

        [SettingIgnore]
        public double DevModeMinScaleFactor = 1.2;
    }
}
