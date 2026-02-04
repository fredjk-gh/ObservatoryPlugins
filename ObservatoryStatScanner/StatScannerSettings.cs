using Observatory.Framework;

namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    class StatScannerSettings
    {
        public const string DEFAULT_PROCGEN_HANDLING = "Ignore ProcGen records";

        private static readonly Dictionary<string, object> PG_HANDLING_VALUES = new()
            {
                {DEFAULT_PROCGEN_HANDLING, ProcGenHandlingMode.ProcGenIgnore},
                {"Use both ProcGen + Galactic records", ProcGenHandlingMode.ProcGenPlusGalactic},
                {"Only show ProcGen records (+ visited Galactic records)", ProcGenHandlingMode.ProcGenOnly}
            };

        public StatScannerSettings()
        {
            // Defaults go here.
            MaxNearRecordThreshold = 0;
            MinNearRecordThreshold = 0;
            MaxPersonalBestsPerBody = 3;
            HighCardinalityTieSuppression = 10000;
            ProcGenHandling = StatScannerSettings.DEFAULT_PROCGEN_HANDLING;
            FirstDiscoveriesOnly = false;
            EnablePersonalBests = true;
            EnableGridOutputAfterReadAll = false;
            NotifyPossibleNewGalacticRecords = true;
            NotifyMatchedGalacticRecords = true;
            NotifyVisitedGalacticRecords = true;
            NotifyNearGalacticRecords = false;
            NotifyNewPersonalBests = true;
            NotifyNewCodexEntries = false;
            NotifyTallies = false;
            NotifySilentFallback = true;
            EnableEarthMassesRecord = true;
            EnablePlanetaryRadiusRecord = true;
            EnableSurfaceGravityRecord = true;
            EnableSurfacePressureRecord = true;
            EnableSurfaceTemperatureRecord = true;
            EnableOrbitalEccentricityRecord = true;
            EnableOrbitalPeriodRecord = true;
            EnableRotationalPeriodRecord = true;
            EnableSolarMassesRecord = true;
            EnableSolarRadiusRecord = true;
            EnableRingOuterRadiusRecord = true;
            EnableRingWidthRecord = true;
            EnableRingMassRecord = true;
            EnableRingDensityRecord = true;
            EnableOdysseySurfaceBioRecord = true;
            EnableElwSimilarityRecords = true;
            EnableSystemBodyCountRecords = true;
            EnableRegionCodexCountRecords = false;
            EnableVisitedRegionRecords = true;
            EnableUndiscoveredSystemCountRecord = true;
            EnableAutoUpdates = true;
        }

        public enum ProcGenHandlingMode
        {
            ProcGenIgnore,
            ProcGenPlusGalactic,
            ProcGenOnly,
        }

        // General settings
        //
        [SettingNewGroup("General")]
        [SettingDisplayName("Records file usage")]
        [SettingBackingValue("ProcGenHandling")]
        public Dictionary<string, object> ProcGenHandlingOptions
        {
            get => PG_HANDLING_VALUES;
        }

        [SettingIgnore]
        public string ProcGenHandling { get; set; }

        [SettingDisplayName("Only consider 1st discovered objects")]
        public bool FirstDiscoveriesOnly { get; set; }
        [SettingDisplayName("Track Personal Bests, Tallies and new Codex Entries")]
        public bool EnablePersonalBests { get; set; }
        [SettingDisplayName("Show ALL results after a read-all")]
        public bool EnableGridOutputAfterReadAll { get; set; }

        // Notification settings
        //
        [SettingNewGroup("Notifications")]
        [SettingDisplayName("Notify possible new Galactic records")]
        public bool NotifyPossibleNewGalacticRecords { get; set; }

        [SettingDisplayName("Notify matched Galactic records")]
        public bool NotifyMatchedGalacticRecords { get; set; }

        [SettingDisplayName("Notify Visited Galactic records")]
        public bool NotifyVisitedGalacticRecords { get; set; }

        [SettingDisplayName("Notify near Galactic records")]
        public bool NotifyNearGalacticRecords { get; set; }

        [SettingDisplayName("Notify new Personal bests")]
        public bool NotifyNewPersonalBests { get; set; }

        [SettingDisplayName("Notify new Region Codex entries")]
        public bool NotifyNewCodexEntries { get; set; }

        [SettingDisplayName("Notify Tallies")]
        public bool NotifyTallies { get; set; }

        [SettingDisplayName("Silently notify anything disabled above")]
        public bool NotifySilentFallback { get; set; }


        // Individual record tracking controls
        //
        // Planets + stars common
        [SettingNewGroup("Common body records")]
        [SettingDisplayName("Check Surface Temperature records")]
        public bool EnableSurfaceTemperatureRecord { get; set; }

        [SettingDisplayName("Check Orbital Eccentricity records")]
        public bool EnableOrbitalEccentricityRecord { get; set; }

        [SettingDisplayName("Check Orbital Period records")]
        public bool EnableOrbitalPeriodRecord { get; set; }

        [SettingDisplayName("Check Rotational Period records")]
        public bool EnableRotationalPeriodRecord { get; set; }

        // Per Body records
        // Planets
        [SettingNewGroup("Planet-specific records")]
        [SettingDisplayName("Check Planet Mass records")]
        public bool EnableEarthMassesRecord { get; set; }

        [SettingDisplayName("Check Planet Radius records")]
        public bool EnablePlanetaryRadiusRecord { get; set; }

        [SettingDisplayName("Check Surface Gravity records")]
        public bool EnableSurfaceGravityRecord { get; set; }

        [SettingDisplayName("Check Surface Pressure records")]
        public bool EnableSurfacePressureRecord { get; set; }

        [SettingDisplayName("Check Odyssey surface bio records")]
        public bool EnableOdysseySurfaceBioRecord { get; set; }

        [SettingDisplayName("Check Earth/Mars similarity records")]
        public bool EnableElwSimilarityRecords { get; set; }

        // Stars
        [SettingNewGroup("Star-specific records")]
        [SettingDisplayName("Check Star Mass records")]
        public bool EnableSolarMassesRecord { get; set; }

        [SettingDisplayName("Check Star Radius records")]
        public bool EnableSolarRadiusRecord { get; set; }

        // Rings
        [SettingNewGroup("Ring Records")]
        [SettingDisplayName("Check Ring Outer Radius records")]
        public bool EnableRingOuterRadiusRecord { get; set; }

        [SettingDisplayName("Check Ring Width records")]
        public bool EnableRingWidthRecord { get; set; }

        [SettingDisplayName("Check Ring Mass records")]
        public bool EnableRingMassRecord { get; set; }

        [SettingDisplayName("Check Ring Density records")]
        public bool EnableRingDensityRecord { get; set; }

        // System
        [SettingNewGroup("Misc. Personal bests/tallies")]
        [SettingDisplayName("Check System Body Count")]
        public bool EnableSystemBodyCountRecords { get; set; }

        [SettingDisplayName("Tally Undiscovered Systems")]
        public bool EnableUndiscoveredSystemCountRecord { get; set; }

        // Regional
        [SettingNewGroup()]
        [SettingDisplayName("Tally Visited Regions")]
        public bool EnableVisitedRegionRecords { get; set; }

        [SettingDisplayName("Track new Region Codex entries")]
        public bool EnableRegionCodexCountRecords { get; set; }

        // Output tuning knobs and options.
        [SettingNewGroup("Tuning")]
        [SettingDisplayName("Near Maximum record threshold % (0 disables [default])")]
        [SettingNumericBounds(0.0, 10.0)]
        public int MaxNearRecordThreshold { get; set; }

        [SettingDisplayName("Near Minimum record threshold % (0 disables [default])")]
        [SettingNumericBounds(0.0, 20.0)]
        public int MinNearRecordThreshold { get; set; }

        [SettingDisplayName("Maximum personal best notifications per body")]
        [SettingNumericBounds(0.0, 20.0)]
        public int MaxPersonalBestsPerBody { get; set; }

        // For some combinations (ie. MIN(icy body surface pressure) == 0), the
        // extreme value is VERY common meaning we generate a lot of ties and this
        // slows down the UI rendering incredibly (not to mention, it's not terribly
        // interesting/rare). So if there's a high cardinality of bodies sharing
        // a record, we don't care.
        [SettingDisplayName("Common record tie limit")]
        [SettingNumericBounds(1000, 100000, 1000)]
        public int HighCardinalityTieSuppression { get; set; }


        // Actions
        [SettingDisplayName("Force Update Galactic Records")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action ForceUpdateGalacticRecords { get; internal set; }

        [SettingDisplayName("Open Wiki (settings help and more)")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action OpenStatScannerWiki { get; internal set; }

        
        [SettingNewGroup("Updates")]
        [SettingDisplayName("Enable automatic updates")]
        public bool EnableAutoUpdates { get; set; }

        [SettingDisplayName("Enable Beta versions (warning: things may break)")]
        public bool EnableBetaUpdates { get; set; }

        // Internal
        [SettingIgnore]
        public bool DevMode { get => false; }

        [SettingIgnore]
        public double DevModeMaxScaleFactor = 1.0; //0.8;

        [SettingIgnore]
        public double DevModeMinScaleFactor = 1.0; //1.2;
    }
}
