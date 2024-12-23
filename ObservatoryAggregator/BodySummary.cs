using com.github.fredjk_gh.ObservatoryAggregator.UI;
using Microsoft.VisualBasic;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class BodySummary
    {
        private TrackedData _allData;
        private AggregatorGrid _gridItem = null;
        private int? _explicitBodyID = null;
        private Scan _scan = null;
        private SAAScanComplete _saaScan = null;
        private ScanBaryCentre _scanBarycentre = null;
        private ObservableCollection<ScanBaryCentre> _barycentreChildren = new();

        internal BodySummary(TrackedData allData, int bodyID)
        {
            _allData = allData;
            _barycentreChildren.CollectionChanged += ChildrenChanged;
            _explicitBodyID = bodyID;
        }

        public FSSBodySignals BodySignals { get; set; }
        public Scan Scan
        {
            get => _scan;
            set
            {
                _scan = value;
                // Regenerate the grid item in case it was generated before the Scan (and thus doesn't have a short-name).
                RegenerateGridItem();
            }
        }

        public SAAScanComplete ScanComplete
        { 
            get => _saaScan;
            set
            {
                _saaScan = value;
                // Regenerate the grid item in case it was generated before the Scan (and thus doesn't have a short-name).
                // This is mainly for rings.
                RegenerateGridItem();
            }
        }

        public ScanBaryCentre ScanBarycentre
        {
            get => _scanBarycentre;
            set
            {
                _scanBarycentre = value;
                // Regenerate the grid item in case it was generated before the Scan (and thus doesn't have a short-name).
                // This is mainly for barycentres.
                RegenerateGridItem();
            }
        }

        public ObservableCollection<ScanBaryCentre> BarycentreChildren
        {
            get => _barycentreChildren;
        }

        // Ring scans??? Unfortunately, no SystemName; Fallback to global system name.
        public string SystemName
        {
            get => Scan?.StarSystem
                    ?? ScanBarycentre?.StarSystem 
                    ?? _allData.CurrentSystem?.Name 
                    ?? "";
        }

        public int BodyID
        {
            get => _explicitBodyID
                    ?? Scan?.BodyID
                    ?? ScanBarycentre?.BodyID
                    ?? BodySignals?.BodyID
                    ?? ScanComplete?.BodyID
                    ?? 0; // System implicit barycentre? Or there's no barycentre scan (hence the _explicitBodyID fallback)
            set => _explicitBodyID = value;
        }

        public string BodyName
        {
            get =>  Scan?.BodyName
                ?? BodySignals?.BodyName
                ?? ScanComplete?.BodyName
                ?? $"X{{{BodyID}}}";
        }

        public string BodyShortName
        {
            get
            {
                {
                    string shortName = BodyName;
                    if (IsBarycentre && BarycentreChildren.Count >= 1)
                    {
                        var childIds = BarycentreChildren.Select(bc => bc.BodyID).ToHashSet();
                        shortName = $"({string.Join("-", _allData.BodyData.Where(e => childIds.Contains(e.Key)).Select(e => e.Value.BodyShortName))})";
                    }
                    else if (!IsBarycentre)
                    {
                        var sysName = SystemName;
                        if (!string.IsNullOrWhiteSpace(sysName)) shortName = BodyName.Replace(sysName, "").Trim();
                    }
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

        public bool IsBarycentre
        {
            get => ScanBarycentre != null
                || BarycentreChildren.Count > 0;
        }

        public string GetBodyNameDisplayString()
        {
            if (Scan?.PlanetClass != null)
                return $"Body {BodyShortName}";
            else if (Scan?.StarType != null)
                return $"{(BodyShortName == Constants.PRIMARY_STAR ? "" : "Body ")}{BodyShortName}";
            else if (IsBarycentre)
            {
                if (BodyID == 0)
                    return "System barycentre";
                else
                    return $"Barycentre {BodyShortName}";
            }
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

        public AggregatorGrid ToGrid()
        {
            if (_gridItem == null)
            {
                _gridItem = new(_allData, this);
            }
            return _gridItem;
        }

        private void RegenerateGridItem()
        {
            if (_gridItem != null)
            {
                _gridItem = null;
                ToGrid();
            }
        }

        private void ChildrenChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_barycentreChildren.Count >= 2)
            {
                RegenerateGridItem();
            }
        }
    }
}
