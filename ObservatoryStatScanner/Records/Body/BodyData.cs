using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class BodyData
    {
        #region Reference Bodies
        public static readonly BodyData EARTH = new()
        {
            Name = "Earth",
            GravityG = 9.797759f / Constants.CONV_MperS2_TO_G_DIVISOR,
            SurfaceTempK = 288.0f,
            PressureAtm = 101231.656250f / Constants.CONV_PA_TO_ATM_DIVISOR,
            AtmosphereComposition = new()
            {
                { "Nitrogen", 77.886406f },
                { "Oxygen", 20.892998f },
                { "Argon", 0.931637f },
            },
            OrbitalPeriodDays = 31558150.649071f / Constants.CONV_S_TO_DAYS_DIVISOR,
            RotationalPeriodDays = 86164.106590f / Constants.CONV_S_TO_DAYS_DIVISOR,
            AxialTiltDegrees = 0.401426f * 180.0f / (float)Math.PI,
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
            GravityG = 3.697488f / Constants.CONV_MperS2_TO_G_DIVISOR,
            SurfaceTempK = 260.811890f,
            PressureAtm = 233391.062500f / Constants.CONV_PA_TO_ATM_DIVISOR,
            AtmosphereComposition = new()
            {
                { "Nitrogen", 91.169930f },
                { "Oxygen", 8.682851f },
                { "Water", 0.095125f },
            },
            OrbitalPeriodDays = 59354294.538498f / Constants.CONV_S_TO_DAYS_DIVISOR,
            RotationalPeriodDays = 88642.690263f / Constants.CONV_S_TO_DAYS_DIVISOR,
            AxialTiltDegrees = 0.439648f * 180.0f / (float)Math.PI,
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
            GravityG = elwScan.SurfaceGravity / Constants.CONV_MperS2_TO_G_DIVISOR;
            SurfaceTempK = elwScan.SurfaceTemperature;
            PressureAtm = elwScan.SurfacePressure / Constants.CONV_PA_TO_ATM_DIVISOR;
            AtmosphereComposition = elwScan.AtmosphereComposition.ToDictionary(s => s.Name, s => s.Percent);
            OrbitalPeriodDays = elwScan.OrbitalPeriod / Constants.CONV_S_TO_DAYS_DIVISOR;
            RotationalPeriodDays = elwScan.RotationPeriod / Constants.CONV_S_TO_DAYS_DIVISOR;
            AxialTiltDegrees = elwScan.AxialTilt * 180.0f / (float)Math.PI;
            Eccentricity = elwScan.Eccentricity;
            TidalLock = elwScan.TidalLock;
            IsTerraformed = "terraformed".Equals(elwScan.TerraformState, StringComparison.OrdinalIgnoreCase);
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
                if (AtmosphereComposition.ContainsKey(material.Key) && AtmosphereComposition[material.Key] > 0)
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
            score += WeightedScore(other.MoonCount + 1, MoonCount + 1, 2, 4); // no moon => no tide; More moons, not so bad? No moon mitigated if binary?
            //score += WeightedScore(other.GGCount + 1, GGCount + 1, 5, 6); // Less GGs => slightly higher chance for asteroids?
            return score;
        }
    }
}
