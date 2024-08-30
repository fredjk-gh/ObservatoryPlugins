using com.github.fredjk_gh.ObservatoryAggregator.UI;
using Microsoft.VisualBasic;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class BodySummary
    {
        public FSSBodySignals BodySignals { get; set; }
        public Scan Scan { get; set; }
        public SAAScanComplete ScanComplete { get; set; }

        // Ring scans???
        public string SystemName
        {
            get
            {
                if (Scan != null) return Scan.StarSystem;
                return "";
            }
        }

        public int BodyID
        {
            get
            {
                if (Scan != null) return Scan.BodyID;
                if (BodySignals != null) return BodySignals.BodyID;
                return -1;
            }
        }

        public string BodyName
        {
            get
            {
                if (Scan != null) return Scan.BodyName;
                if (BodySignals != null) return BodySignals.BodyName;
                return "";
            }
        }

        public string BodyShortName
        {
            get
            {
                {
                    string shortName = BodyName;
                    if (Scan != null) shortName = BodyName.Replace(SystemName, "").Trim();

                    return (string.IsNullOrEmpty(shortName) ? Constants.PRIMARY_STAR : shortName);
                }
            }
        }


        public bool IsMapped
        {
            get => ScanComplete != null;
        }

        public bool IsValuable
        {
            get => Scan != null && (Constants.HighValueNonTerraformablePlanetClasses.Contains(Scan.PlanetClass) || Scan.TerraformState?.Length > 0);
        }

        public bool IsScoopableStar
        {
            get => Scan != null && (Scan.StarType != null && Constants.Scoopables.Contains(Scan.StarType));
        }

        public string GetBodyNameDisplayString()
        {
            // TODO: Suppart Barycenters
            if (Scan?.PlanetClass != null)
                return $"Body {BodyShortName}";
            else if (Scan?.StarType != null)
                return $"{(BodyShortName == Constants.PRIMARY_STAR ? "" : "Body ")}{BodyShortName}";
            return BodyShortName;
        }

        public string GetDetailsString()
        {
            if (Scan?.PlanetClass != null)
            {
                string detailsStr = $"{Scan.DistanceFromArrivalLS:n0} Ls"
                    + $"{Constants.DETAIL_SEP}{(Scan.SurfaceGravity / 9.81):n2}g"
                    + $"{Constants.DETAIL_SEP}{Scan.SurfaceTemperature:n0} K";

                if (!string.IsNullOrEmpty(Scan.Atmosphere))
                {
                    detailsStr += $"{Constants.DETAIL_SEP}{(Scan.SurfacePressure / 101325.0):n2} atm";
                }
                if (!string.IsNullOrEmpty(Scan.AtmosphereType))
                {
                    detailsStr += $"{Constants.DETAIL_SEP}{Scan.AtmosphereType}";
                }
                return detailsStr;
            }
            else if (Scan?.StarType != null)
            {
                return $"{Scan.DistanceFromArrivalLS:n0} Ls";
            }

            return (Scan == null ? "" : $"{Scan.DistanceFromArrivalLS:n0} Ls");
        }

        public string GetBodyType()
        {
            if (!string.IsNullOrEmpty(Scan?.StarType))
            {
                return $"☀ {Constants.JournalTypeMap[Scan.StarType]}";
            }
            else if (!string.IsNullOrEmpty(Scan?.PlanetClass))
            {

                if (Scan.PlanetClass.ToLower().Contains("giant"))
                {
                    return $"🪐 {Constants.JournalTypeMap[Scan.PlanetClass]}";
                }
                else
                {
                    return $"{(Scan.PlanetClass.Contains("Earth") ? "🌏" : "🌑")} {(!string.IsNullOrEmpty(Scan.TerraformState) ? "T-" : "")}{Constants.JournalTypeMap[Scan.PlanetClass]}";
                }
            }
            return "";
        }

        public List<EmojiSpec> GetFlagEmoji()
        {
            List<EmojiSpec> parts = new();
            if (Scan?.StarType != null)
            {
                if (IsScoopableStar) parts.Add(new("⛽"));
            }
            else if (Scan?.PlanetClass != null)
            {
                if (IsValuable) parts.Add(new("💰"));
                if (IsMapped) parts.Add(new("🌐"));
                if (Scan?.Landable ?? false) parts.Add(new("🛬"));

                if (BodySignals != null)
                {
                    foreach (var signal in BodySignals.Signals)
                    {
                        switch (signal.Type)
                        {
                            case "$SAA_SignalType_Biological;":
                                if (signal.Count > 0) parts.Add(new($"🧬 {signal.Count}"));
                                break;
                            case "$SAA_SignalType_Geological;":
                                if (signal.Count > 0) parts.Add(new($"🌋 {signal.Count}"));
                                break;
                        }
                    }
                }
            }
            return parts;
        }
    }
}
