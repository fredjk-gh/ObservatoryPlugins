using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.System;
using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters
{
    public partial class SpanshToJournal
    {
        public static StarPosition ConvertSystemCoords(SystemCoords source)
        {
            return new StarPosition()
            {
                x = source.x,
                y = source.y,
                z = source.z,
            };
        }

        public static ImmutableList<Parent> ConvertParents(List<ParentBody> parents)
        {
            if (parents == null) return null;

            List<Parent> jParents = [];
            foreach (var parent in parents)
            {
                switch (parent.ParentType)
                {
                    case "Null":
                        jParents.Add(new()
                        {
                            Null = parent.ParentBodyId
                        });
                        break;
                    case "Planet":
                        jParents.Add(new()
                        {
                            Planet = parent.ParentBodyId
                        });
                        break;
                    case "Star":
                        jParents.Add(new()
                        {
                            Star = parent.ParentBodyId
                        });
                        break;

                }
            }
            return [.. jParents];
        }

        public static ImmutableList<MaterialComposition> ConvertMaterialComposition(List<CompositionItem> composition, bool convertCase = false)
        {
            if (composition == null) return null;
            return [.. composition.OrderByDescending(c => c.Percent).Select(c => new MaterialComposition()
            {
                Name = (convertCase ? c.Material.ToLower() : c.Material),
                Percent = (float)c.Percent,
            })];
        }

        public static Composition ConvertComposition(List<CompositionItem> composition)
        {
            if (composition == null) return null;
            return new Composition()
            {
                Ice = composition.Where(i => i.Material == "Ice").Select(i => (float)i.Percent / 100.0f).First(),
                Metal = composition.Where(i => i.Material == "Metal").Select(i => (float)i.Percent / 100.0f).First(),
                Rock = composition.Where(i => i.Material == "Rock").Select(i => (float)i.Percent / 100.0f).First(),
            };
        }

        public static ImmutableList<Ring> ConvertRings(List<Asteroids> rings)
        {
            if (rings == null) return [];

            return [.. rings.Select(r => new Ring()
            {
                Name = r.Name,
                InnerRad = r.InnerRadius,
                OuterRad = r.OuterRadius,
                MassMT = r.Mass,
                RingClass = ConvertRingType(r.Type),
            })];
        }

        public static ImmutableList<Signal> ConvertBodySAASignals(List<NameIntItem> signals)
        {
            if (signals == null) return null;
            return [.. signals.Select(s => new Signal()
            {
                Type = s.Name,
                Type_Localised = ConvertBodySAASignal(s.Name),
                Count = s.Value,
            })];
        }

        public static ImmutableList<Signal> ConvertRingCommoditySignals(List<NameIntItem> signals)
        {
            if (signals == null) return null;
            return [.. signals.Select(s => new Signal()
            {
                Type = s.Name,
                Type_Localised = FDevIDs.CommodityBySymbol.GetValueOrDefault(s.Name, null)?.Name ?? "",
                Count = s.Value,
            })];
        }

        public static ImmutableList<GenusType> ConvertGenuses(List<string> genuses)
        {
            if (genuses == null) return null;
            return [.. genuses.Select(s => new GenusType()
            {
                Genus = ConvertGenus(s),
                Genus_Localised = s,
            })];
        }

        public static string ConvertEconomy(string spanshValue)
        {
            return IdFromName(FDevIDs.EconomyById, spanshValue);
        }

        public static string ConvertGovernment(string spanshValue)
        {
            return IdFromName(FDevIDs.GovernmentById, spanshValue);
        }

        public static string ConvertAllegiance(string spanshValue)
        {
            return IdFromName(FDevIDs.SystemAllegianceById, spanshValue);
        }

        public static string ConvertSecurity(string spanshValue)
        {
            return IdFromName(FDevIDs.SecurityById, spanshValue);
        }

        public static string ConvertGenus(string spanshValue)
        {
            var value = FDevIDs.GenusById.Where(kv => kv.Value.Name == spanshValue).Select(kv => kv.Key).FirstOrDefault("");
            Debug.WriteLineIf(string.IsNullOrWhiteSpace(value), $"Coudn't map input Genus '{spanshValue}' to an ID.");
            return value;
        }

        public static string ConvertBodySAASignal(string spanshValue)
        {
            return IdFromName(FDevIDs.SaaSignalsById, spanshValue);
        }

        public static string ConvertRingType(string spanshValue)
        {
            return IdFromName(FDevIDs.RingsById, spanshValue);
        }

        public static string ConvertCommodity(string spanshValue)
        {
            var value = FDevIDs.CommodityBySymbol.Where(kv => kv.Value.Name == spanshValue).Select(kv => kv.Key).FirstOrDefault("");
            Debug.WriteLineIf(string.IsNullOrWhiteSpace(value), $"Coudn't map input Commodity '{spanshValue}' to a symbol.");
            return value;
        }

        public static string ConvertBodyType(string spanshValue)
        {
            return IdFromName(FDevIDs.BodiesById, spanshValue);
        }

        private static readonly Regex _spectralClassRE = SpectralClassRegex();
        public static int ConvertSpectralClass(string spanshValue)
        {
            if (string.IsNullOrWhiteSpace(spanshValue)) return -1;

            Match m = _spectralClassRE.Match(spanshValue);
            if (m.Success && m.Groups.Count > 1)
            {
                return Convert.ToInt32(m.Groups[1].Value);
            }
            throw new ArgumentException($"Unparseable Spectral Class: {spanshValue}");
        }

        #region Private methods
        private static string IdFromName(Dictionary<string, IdName> map, string name)
        {
            string value = map.Where(kv => kv.Value.Name == name).Select(kv => kv.Key).FirstOrDefault("");
            if (string.IsNullOrWhiteSpace(value))
                Debug.WriteLine($"Coudn't map input '{name}' to an ID.");
            return value;
        }

        [GeneratedRegex(@".*(\d)$")]
        private static partial Regex SpectralClassRegex();
        #endregion
    }
}
