using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class BodyData
    {
        #region Reference Bodies
        public static readonly BodyData EARTH = new()
        {
            Name = "Earth",
            GravityG = Conversions.Mpers2ToG(9.797759f),
            SurfaceTempK = 288.0f,
            PressureAtm = Conversions.PaToAtm(101231.656250f),
            AtmosphereComposition = new()
            {
                { "Nitrogen", 77.886406f },
                { "Oxygen", 20.892998f },
                { "Argon", 0.931637f },
            },
            OrbitalPeriodDays = Conversions.SecondsToDays(31558150.649071f),
            RotationalPeriodDays = Conversions.SecondsToDays(86164.106590f),
            AxialTiltDegrees = Conversions.RadToDegrees(0.401426f),
            Eccentricity = 0.016700f,
            TidalLock = false,
            IsTerraformed = false,
            IsMoon = false,
            IsBinary = false,
            StarCount = 1,
            MoonCount = 1,
            GGCount = 4,
        };

        public static readonly BodyData MARS = new()
        {
            Name = "Mars",
            GravityG = Conversions.Mpers2ToG(3.697488f),
            SurfaceTempK = 260.811890f,
            PressureAtm = Conversions.PaToAtm(233391.062500f),
            AtmosphereComposition = new()
            {
                { "Nitrogen", 91.169930f },
                { "Oxygen", 8.682851f },
                { "Water", 0.095125f },
            },
            OrbitalPeriodDays = Conversions.SecondsToDays(59354294.538498f),
            RotationalPeriodDays = Conversions.SecondsToDays(88642.690263f),
            AxialTiltDegrees = Conversions.RadToDegrees(0.439648f),
            Eccentricity = 0.093400f,
            TidalLock = false,
            IsTerraformed = true,
            IsMoon = false,
            IsBinary = false,
            StarCount = 1,
            MoonCount = 0,
            GGCount = 4,
        };
        #endregion

        private BodyData() { } // Used for building reference bodies, above.

        public BodyData(Scan elwScan, Dictionary<int, Scan> allScans)
        {
            Name = elwScan.BodyName;
            GravityG = Conversions.Mpers2ToG(elwScan.SurfaceGravity);
            SurfaceTempK = elwScan.SurfaceTemperature;
            PressureAtm = Conversions.PaToAtm(elwScan.SurfacePressure);
            AtmosphereComposition = elwScan.AtmosphereComposition.ToDictionary(s => s.Name, s => s.Percent);
            OrbitalPeriodDays = Conversions.SecondsToDays(elwScan.OrbitalPeriod);
            RotationalPeriodDays = Conversions.SecondsToDays(elwScan.RotationPeriod);
            AxialTiltDegrees = Conversions.RadToDegrees(elwScan.AxialTilt);
            Eccentricity = elwScan.Eccentricity;
            TidalLock = elwScan.TidalLock;
            IsTerraformed = JournalConstants.IsTerraformed(elwScan.TerraformState);
            IsMoon = elwScan.Parent.Any(p => p.ParentType == ParentType.Planet);
            IsBinary = elwScan.Parent[0].ParentType == ParentType.Null;
            StarCount = allScans.Values.Count(s => !string.IsNullOrWhiteSpace(s.StarType));
            MoonCount = allScans.Values.Count(s => s.Parent?.Any(p => p.Body == elwScan.BodyID) ?? false);
            GGCount = allScans.Values.Count(s => !string.IsNullOrWhiteSpace(s.PlanetClass) && s.PlanetClass.Contains("gas giant", StringComparison.OrdinalIgnoreCase));
        }

        #region Static methods
        private static double WeightedScore(double targetVal, double referenceVal, double posWeight, double negWeight)
        {
            double factor = (targetVal - referenceVal) / referenceVal;
            if (factor >= 0)
                return factor * posWeight;
            else
                return factor * negWeight * -1;
        }
        #endregion

        internal string Name { get; init; }
        internal float GravityG { get; init; }
        internal float SurfaceTempK { get; init; }
        internal float PressureAtm { get; init; }
        internal Dictionary<string, float> AtmosphereComposition { get; init; }
        internal float OrbitalPeriodDays { get; init; }
        internal float RotationalPeriodDays { get; init; }
        internal float AxialTiltDegrees { get; init; }
        internal float Eccentricity { get; init; }
        internal bool TidalLock { get; init; }
        internal bool IsTerraformed { get; init; }
        internal bool IsMoon { get; init; }
        internal bool IsBinary { get; init; }
        internal int StarCount { get; init; }
        internal int MoonCount { get; init; }
        internal int GGCount { get; init; }

        /// <summary>
        /// Score the given body and with reference to this body (this body being the reference in the function).
        /// </summary>
        /// <param name="other">Another body to compare to this.</param>
        /// <returns>A similarity score</returns>
        internal double GetSimilarityScore(BodyData other)
        {
            double score = 0.0;
            score += WeightedScore(other.GravityG, GravityG, 25, 20);
            score += WeightedScore(other.SurfaceTempK, SurfaceTempK, 2, 1);
            score += WeightedScore(other.PressureAtm, PressureAtm, 1, 2);
            foreach (var material in other.AtmosphereComposition)
            {
                if (AtmosphereComposition.TryGetValue(material.Key, out float materialPct) && materialPct > 0)
                {
                    if (material.Key == "Oxygen") // Different weights for Oxygen.
                        score += WeightedScore(other.PressureAtm * material.Value, PressureAtm * AtmosphereComposition[material.Key], 3, 5);
                    else
                        score += WeightedScore(other.PressureAtm * material.Value, PressureAtm * AtmosphereComposition[material.Key], 1, 1);
                }
                else
                    score += 1; // Non-existent material in the reference body's atmosphere.
            }
            score += WeightedScore(other.Eccentricity, Eccentricity, 20, 0);

            // The next three can have negative values to begin with.We really only want to look at absolute differences.
            score += WeightedScore(Math.Abs(other.OrbitalPeriodDays), Math.Abs(OrbitalPeriodDays), 1, 1);
            score += WeightedScore(Math.Abs(other.RotationalPeriodDays), Math.Abs(RotationalPeriodDays), 10, 15);
            score += WeightedScore(Math.Abs(other.AxialTiltDegrees), Math.Abs(AxialTiltDegrees), 1, 1);

            // Booleans: Convert to numeric and compute score.
            score += WeightedScore((other.TidalLock ? 2 : 1), (TidalLock ? 2 : 1), 1, 1);
            score += WeightedScore((other.IsTerraformed ? 2 : 1), (IsTerraformed ? 2 : 1), 0.1, 0.1);
            //score += WeightedScore((other.IsMoon ? 2 : 1), (IsMoon ? 2 : 1), 1, 1);
            //score += WeightedScore((other.IsBinary ? 2 : 1), (IsBinary ? 2 : 1), 1, 1);

            // We may have 0 stars, moons or gas giants; offset by 1 to avoid dividing by 0.
            // TODO: Consider star type, # of *parent* stars, apparent brightness.
            score += WeightedScore(other.StarCount + 1, StarCount + 1, 2, 2); // Equal weight for different star count in either direction (for now).
            // TODO: not counting moon because the CC currently doesn't
            //score += WeightedScore(other.MoonCount + 1, MoonCount + 1, 2, 4); // no moon => no tide; More moons, not so bad? No moon mitigated if binary?
            //score += WeightedScore(other.GGCount + 1, GGCount + 1, 5, 6); // Less GGs => slightly higher chance for asteroids?
            return score;
        }
    }
}
