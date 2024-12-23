using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using com.github.fredjk_gh.PluginCommon.Data.Spansh;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals
{
    public class JournalGenerator
    {

        public static List<JournalBase> FromSpansh(SpanshSystem spansh, DateTime? journalVisitDate = null)
        {
            // Fabricate the following sequence of Journal events from spansh data:
            // - Location (to trigger system changes). Ideally this will be suppressed by a real FSDJump event from the game.
            //   For simplicity, Location is going to be the main star in the system.
            // - FSSDiscoveryScan event
            // - Various Scan events (along with any associated FSSBodySignals for bios and geos OR ring signals)
            // - FSSAllBodiesFound
            // - SAA scans and FSSBodySignals
            //
            List<JournalBase> converted = new();
            List<JournalBase> convertedPostFSS = new();
            JournalDateGenerator dateGenerator = new(journalVisitDate);

            Body mainStar = spansh.Bodies.Where(b => b.MainStar).First();
            Location loc = new Location()
            {
                Timestamp = dateGenerator.NextFormatted(),
                Event = "Location",
                DistFromStarLS = mainStar.DistanceToArrival,
                Docked = false,
                Taxi = false,
                Multicrew = false,
                StarSystem = spansh.Name,
                SystemAddress = spansh.Id64,
                StarPos = SpanshToJournal.ConvertSystemCoords(spansh.Coords),
                SystemAllegiance = SpanshToJournal.ConvertAllegiance(spansh.Allegiance),
                SystemEconomy = SpanshToJournal.ConvertEconomy(spansh.PrimaryEconomy),
                SystemEconomy_Localised = spansh.PrimaryEconomy,
                SystemSecondEconomy = SpanshToJournal.ConvertEconomy(spansh.SecondaryEconomy),
                SystemSecondEconomy_Localised = spansh.SecondaryEconomy,
                SystemGovernment = SpanshToJournal.ConvertGovernment(spansh.Government),
                SystemGovernment_Localised = spansh.Government,
                SystemSecurity = SpanshToJournal.ConvertSecurity(spansh.Security),
                SystemSecurity_Localised = spansh.Security,
                Population = spansh.Population,
                Body = mainStar.Name,
                BodyID = mainStar.BodyId,
                BodyType = mainStar.Type,
            };
            converted.Add(loc);

            FSSDiscoveryScan honk = new FSSDiscoveryScan()
            {
                Timestamp = dateGenerator.NextFormatted(),
                Event = "FSSDiscoveryScan",
                Progress = 100.0f,
                BodyCount = spansh.BodyCount,
                NonBodyCount = spansh.Bodies.Count - spansh.BodyCount, // Bodies includes Barycentres, BodyCount does not.
                SystemName = spansh.Name,
                SystemAddress = spansh.Id64,
            };
            converted.Add(honk);

            // NOTE: No Asteroid clusters will be generated.
            // Prepare Barycenters  as they are emitted before the first body or barycentre below it.
            Dictionary<int, ScanBaryCentre> bcByBodyId = new();
            foreach (var body in spansh.Bodies.Where(b => b.Type == "Barycentre"))
            {
                ScanBaryCentre bc = new()
                {
                    Timestamp = dateGenerator.NextFormatted(),
                    Event = "ScanBaryCentre",
                    StarSystem = spansh.Name,
                    SystemAddress = spansh.Id64,
                    BodyID = body.BodyId,
                    SemiMajorAxis = Conversions.AuToMeters((float)body.SemiMajorAxis),
                    Eccentricity = (float)body.OrbitalEccentricity,
                    OrbitalInclination = (float)body.OrbitalInclination,
                    Periapsis = (float)body.ArgOfPeriapsis,
                    OrbitalPeriod = Conversions.DaysToSeconds((float)body.OrbitalPeriod),
                    AscendingNode = (float)body.AscendingNode,
                    MeanAnomaly = (float)body.MeanAnomaly,
                };
                bcByBodyId.Add(bc.BodyID, bc);
            }

            // Next stars, order by distance from Arrival. These would all be discovered by honk.
            foreach (var body in spansh.Bodies.Where(b => b.Type == "Star").OrderBy(b => b.BodyId))
            {
                MaybeAddBarycentre(converted, bcByBodyId, body);

                Scan starScan = new Scan()
                {
                    Timestamp = dateGenerator.NextFormatted(),
                    Event = "Scan",
                    ScanType = "AutoScan",
                    BodyName = body.Name,
                    BodyID = body.BodyId,
                    Parents = SpanshToJournal.ConvertParents(body.Parents),
                    StarSystem = spansh.Name,
                    SystemAddress = spansh.Id64,
                    DistanceFromArrivalLS = body.DistanceToArrival,
                    StarType = SpanshToJournal.ConvertBodyType(body.SubType),
                    Subclass = SpanshToJournal.ConvertSpectralClass(body.SpectralClass),
                    StellarMass = (float)body.SolarMasses,
                    Radius = Conversions.SrToMeters((float)body.SolarRadius),
                    AbsoluteMagnitude = (float)body.AbsoluteMagnitude,
                    Age_MY = body.Age,
                    SurfaceTemperature = (float)body.SurfaceTemperature,
                    Luminosity = body.Luminosity,
                    SemiMajorAxis = Conversions.AuToMeters((float)body.SemiMajorAxis),
                    Eccentricity = (float)body.OrbitalEccentricity,
                    OrbitalInclination = (float)body.OrbitalInclination,
                    Periapsis = (float)body.ArgOfPeriapsis,
                    OrbitalPeriod = Conversions.DaysToSeconds((float)body.OrbitalPeriod),
                    AscendingNode = (float)body.AscendingNode,
                    MeanAnomaly = (float)body.MeanAnomaly,
                    RotationPeriod = Conversions.DaysToSeconds((float)body.RotationalPeriod),
                    AxialTilt = (float)body.AxialTilt,
                    Rings = SpanshToJournal.ConvertRings(body.Rings),
                    WasDiscovered = true, // If I'm getting it from Spansh, yes.
                    WasMapped = false, // Can't map stars.
                };
                converted.Add(starScan);
            }

            foreach (var body in spansh.Bodies.Where(b => b.Type == "Planet").OrderBy(b => b.BodyId)) // Barycenters are already dealt with.
            {
                var bioGeoSignals = body.Signals?.Signals.Where(s => FDevIDs.FDevIDs.SaaSignalsById.ContainsKey(s.Name)).ToList();
                if (bioGeoSignals != null && bioGeoSignals.Count > 0)
                {
                    FSSBodySignals fssBS = new()
                    {
                        Timestamp = dateGenerator.NextFormatted(),
                        Event = "FSSBodySignals",
                        BodyName = body.Name,
                        BodyID = body.BodyId,
                        SystemAddress = spansh.Id64,
                        Signals = SpanshToJournal.ConvertBodySAASignals(bioGeoSignals),
                    };
                    converted.Add(fssBS);

                    if ((body.Signals?.Genuses?.Count ?? 0 ) > 0)
                    {
                        SAAScanComplete saaScan = new()
                        {
                            Timestamp = dateGenerator.NextFormatted(),
                            Event = "SAAScanComplete",
                            BodyName = body.Name,
                            BodyID = body.BodyId,
                            SystemAddress = spansh.Id64,
                            // No probes, efficiency target.
                        };
                        SAASignalsFound saaSignals = new()
                        {
                            Timestamp = dateGenerator.NextFormatted(),
                            Event = "SAASignalsFound",
                            BodyName = body.Name,
                            BodyID = body.BodyId,
                            SystemAddress = spansh.Id64,
                            Signals = SpanshToJournal.ConvertBodySAASignals(bioGeoSignals),
                            Genuses = SpanshToJournal.ConvertGenuses(body.Signals.Genuses),
                        };
                        // Add these to Post FSS list.
                        convertedPostFSS.Add(saaScan);
                        convertedPostFSS.Add(saaSignals);
                    }
                }

                if (body.Rings != null && body.Rings.Count > 0)
                {
                    foreach (var r in body.Rings)
                    {
                        if ((r.Signals?.Signals.Count ?? 0) > 0)
                        {
                            int ringBodyId = (int)(r.Id64 >> (64 - 9));
                            SAAScanComplete saaRingScan = new()
                            {
                                Timestamp = dateGenerator.NextFormatted(),
                                Event = "SAAScanComplete",
                                BodyName = r.Name,
                                BodyID = ringBodyId,
                                SystemAddress = spansh.Id64,
                                // No probes, efficiency target.
                            };
                            SAASignalsFound saaRingSignals = new()
                            {
                                Timestamp = dateGenerator.NextFormatted(),
                                Event = "SAASignalsFound",
                                BodyName = r.Name,
                                BodyID = ringBodyId,
                                SystemAddress = spansh.Id64,
                                Signals = SpanshToJournal.ConvertRingCommoditySignals(r.Signals.Signals),
                            };
                            convertedPostFSS.Add(saaRingScan);
                            convertedPostFSS.Add(saaRingSignals);
                        }
                    }
                }

                MaybeAddBarycentre(converted, bcByBodyId, body);
                Scan bodyScan = new Scan()
                {
                    Timestamp = dateGenerator.NextFormatted(),
                    Event = "Scan",
                    ScanType = "Detailed",
                    BodyName = body.Name,
                    BodyID = body.BodyId,
                    Parents = SpanshToJournal.ConvertParents(body.Parents),
                    StarSystem = spansh.Name,
                    SystemAddress = spansh.Id64,
                    DistanceFromArrivalLS = body.DistanceToArrival,
                    TidalLock = body.RotationalPeriodTidallyLocked,
                    TerraformState = body.TerraformingState,
                    PlanetClass = SpanshToJournal.ConvertBodyType(body.SubType),
                    Atmosphere = string.IsNullOrWhiteSpace(body.AtmosphereType) ? "" : $"{body.AtmosphereType.ToLower()} atmosphere", // May need work / conversion
                    AtmosphereType = body.AtmosphereType, // May need work / conversion
                    AtmosphereComposition = SpanshToJournal.ConvertMaterialComposition(body.AtmosphereComposition),
                    Volcanism = string.IsNullOrWhiteSpace(body.VolcanismType) ? "" : $"{body.VolcanismType?.ToLower()} volcanism", // May need work / conversion
                    MassEM = (float)body.EarthMasses,
                    Radius = Conversions.KmToMeters((float)body.Radius),
                    SurfaceGravity = Conversions.GToMperS2((float)body.Gravity),
                    SurfaceTemperature = (float)body.SurfaceTemperature,
                    SurfacePressure = Conversions.AtmsToPa((float)body.SurfacePressure),
                    Landable = body.IsLandable,
                    Materials = SpanshToJournal.ConvertMaterialComposition(body.Materials, true),
                    Composition = SpanshToJournal.ConvertComposition(body.SolidComposition),
                    SemiMajorAxis = Conversions.AuToMeters((float)body.SemiMajorAxis),
                    Eccentricity = (float)body.OrbitalEccentricity,
                    OrbitalInclination = (float)body.OrbitalInclination,
                    Periapsis = (float)body.ArgOfPeriapsis,
                    OrbitalPeriod = Conversions.DaysToSeconds((float)body.OrbitalPeriod),
                    AscendingNode = (float)body.AscendingNode,
                    MeanAnomaly = (float)body.MeanAnomaly,
                    RotationPeriod = Conversions.DaysToSeconds((float)body.RotationalPeriod),
                    AxialTilt = (float)body.AxialTilt,
                    Rings = SpanshToJournal.ConvertRings(body.Rings),
                    WasDiscovered = true, // If I'm getting it from Spansh, yes.
                    WasMapped = true, // Harder to know, but assuming yes, similar argument as above.
                };
                converted.Add(bodyScan);
            }

            FSSAllBodiesFound fssAllBodies = new()
            {
                Timestamp = dateGenerator.NextFormatted(),
                Event = "FSSAllBodiesFound",
                SystemName = spansh.Name,
                SystemAddress = spansh.Id64,
                Count = spansh.BodyCount,
            };

            converted.Add(fssAllBodies);
            converted.AddRange(convertedPostFSS);
            return converted;
        }
        
        private static void MaybeAddBarycentre(List<JournalBase> converted, Dictionary<int, ScanBaryCentre> bcByBodyId, Body body)
        {
            if (body.Parents == null) return;
            foreach (var p in body.Parents)
            {
                if ("Null".Equals(p.ParentType) && bcByBodyId.ContainsKey(p.ParentBodyId))
                {
                    // Add the barycentre to converted output, remove it from list of outstanding barycentres.
                    converted.Add(bcByBodyId[p.ParentBodyId]);
                    bcByBodyId.Remove(p.ParentBodyId);
                    // Don't break, there may be multiple barycentres to emit.
                }
            }
        }
    }
}
