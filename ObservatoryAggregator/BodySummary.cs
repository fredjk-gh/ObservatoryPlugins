using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using com.github.fredjk_gh.PluginCommon.UI;
using Observatory.Framework.Files.Journal;
using System.Collections.ObjectModel;
using System.Text;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class BodySummary
    {
        public enum GeneralizedBodyType
        {
            Unknown,
            Barycentre,
            Star,
            Giant,
            Earthlike,
            Water,
            Ring,
            Other,
        }

        private readonly AggregatorContext _allData;
        private AggregatorGrid _gridItem = null;
        private int? _explicitBodyID = null;
        private Scan _scan = null;
        private FSSBodySignals _bodySignals = null;
        private SAAScanComplete _saaScan = null;
        private SAASignalsFound _saaSignals = null;
        private ScanBaryCentre _scanBarycentre = null;
        private readonly ObservableCollection<ScanBaryCentre> _barycentreChildren = [];
        private RingData _ringData = null;

        internal BodySummary(AggregatorContext allData, int bodyID)
        {
            _allData = allData;
            _barycentreChildren.CollectionChanged += ChildrenChanged;
            _explicitBodyID = bodyID;
        }

        public FSSBodySignals BodySignals
        { 
            get => _bodySignals; 
            set => _bodySignals = value;
        }

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
                if (IsRing && _ringData is null)
                {
                    var shortName = BodyShortName;
                    if (_allData.RingData.TryGetValue(shortName, out RingData ringData))
                        _ringData = ringData;
                }
                // Regenerate the grid item in case it was generated before the Scan (and thus doesn't have a short-name).
                // This is mainly for rings.
                RegenerateGridItem();
            }
        }

        public SAASignalsFound ScanSignals
        {
            get => _saaSignals;
            set
            {
                _saaSignals = value;
                if (IsRing && _ringData is null)
                {
                    var shortName = BodyShortName;
                    if (_allData.RingData.TryGetValue(shortName, out RingData ringData))
                        _ringData = ringData;
                }
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
                    ?? ScanSignals?.BodyID
                    ?? 0; // System implicit barycentre? Or there's no barycentre scan (hence the _explicitBodyID fallback)
            set => _explicitBodyID = value;
        }

        public string BodyName
        {
            get => Scan?.BodyName
                ?? BodySignals?.BodyName
                ?? ScanComplete?.BodyName
                ?? ScanSignals?.BodyName
                ?? $"X{{{BodyID}}}";
        }

        public string BodyShortName
        {
            get
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
                    if (!string.IsNullOrWhiteSpace(sysName))
                        shortName = BodyName.Replace(sysName, "").Trim();
                }
                return (string.IsNullOrEmpty(shortName) ? UIConstants.PRIMARY_STAR : shortName);
            }
        }

        public bool IsMapped
        {
            get => ScanComplete != null;
        }

        public bool IsValuable
        {
            get => Scan != null && (JournalConstants.HighValueNonTerraformablePlanetClasses.Contains(Scan.PlanetClass) || Scan.TerraformState?.Length > 0);
        }

        public bool IsScoopableStar
        {
            get => Scan != null && (Scan.StarType != null && JournalConstants.Scoopables.Contains(Scan.StarType));
        }

        public bool IsBarycentre
        {
            get => ScanBarycentre != null
                || BarycentreChildren.Count > 0;
        }

        public bool IsRing
        {
            get => _ringData is not null || JournalConstants.IsRing(BodyName);
        }

        public string GetBodyNameDisplayString()
        {
            if (Scan?.PlanetClass != null)
                return UIFormatter.BodyLabelDisplay(BodyShortName);
            else if (Scan?.StarType != null)
                return UIFormatter.BodyLabelDisplay(BodyShortName);
            else if (IsBarycentre)
            {
                if (BodyID == 0)
                    return UIConstants.SYSTEM_BARYCENTRE;
                else
                    return UIFormatter.BodyLabelDisplay(BodyShortName, UIConstants.BARYCENTRE);
            }
            // This works for Rings.
            return BodyShortName;
        }

        public string GetDetailsString()
        {
            if (Scan?.PlanetClass is not null)
            {
                StringBuilder sb = new();
                sb.Append(UIFormatter.DistanceLs(Scan.DistanceFromArrivalLS))
                    .Append(Constants.DETAIL_SEP)
                    .Append(UIFormatter.GravityG(Scan.SurfaceGravity))
                    .Append(Constants.DETAIL_SEP)
                    .Append(UIFormatter.TemperatureK(Scan.SurfaceTemperature));

                if (!string.IsNullOrEmpty(Scan.Atmosphere))
                {
                    sb.Append(Constants.DETAIL_SEP)
                        .Append(UIFormatter.PressureAtm(Scan.SurfacePressure));
                }
                if (!string.IsNullOrEmpty(Scan.AtmosphereType))
                {
                    sb.Append(Constants.DETAIL_SEP)
                        .Append(Scan.AtmosphereType);
                }
                return sb.ToString();
            }
            else if (Scan?.StarType is not null)
            {
                return UIFormatter.DistanceLs(Scan.DistanceFromArrivalLS);
            }
            else if (ScanBarycentre is not null)
                return string.Empty;
            else if (IsRing)
            {
                StringBuilder sb = new();
                sb.Append(_ringData.ReserveLevel);

                int bodySignals = 0;
                if (ScanSignals is not null && ScanSignals.Signals.Count > 0)
                {
                    bodySignals = ScanSignals.Signals.Select(s => s.Count).Sum();
                    sb.Append(Constants.DETAIL_SEP)
                        .Append("{bodySignals} Hotspots");
                }
                return sb.ToString();
            }
            return (Scan == null ? "" : UIFormatter.DistanceLs(Scan.DistanceFromArrivalLS));
        }

        public string RingHotspotDetails()
        {
            if (ScanSignals is null) return string.Empty;
            return string.Join(Environment.NewLine, ScanSignals.Signals
                .OrderByDescending(s => s.Count)
                .Select(s =>
                {
                    var type = s.Type_Localised ?? s.Type;
                    var key = s.Type.ToLower();
                    if (FDevIDs.CommodityBySymbol.TryGetValue(key, out CommodityResource commodity))
                        type = commodity.Name;

                    return $"- {type}: {s.Count}";
                }));
        }

        public string GetBodyTypeLabel()
        {
            if (IsBarycentre)
                return UIConstants.BARYCENTRE;
            else if (!string.IsNullOrWhiteSpace(Scan?.StarType))
                return UIFormatter.StarLabelAbbreviated(Scan.StarType);
            else if (!string.IsNullOrWhiteSpace(Scan?.PlanetClass))
                return UIFormatter.PlanetLabelAbbreviated(Scan.PlanetClass);
            else if (IsRing && _ringData is not null)
                // Ring Details: Show ring type.
                return UIFormatter.RingTypeLabel(_ringData.Ring.RingClass);

            return string.Empty;
        }

        public string GetBodyTypeDetail()
        {
            if (!string.IsNullOrWhiteSpace(Scan?.StarType))
                return Scan.StarType;
            else if (!string.IsNullOrWhiteSpace(Scan?.PlanetClass))
            {
                return UIFormatter.PlanetLabelWithTerraformState(Scan.StarType, Scan.TerraformState);
            }

            return string.Empty;
        }

        public GeneralizedBodyType GetBodyType()
        {
            GeneralizedBodyType result = GeneralizedBodyType.Unknown;

            if (!string.IsNullOrEmpty(Scan?.StarType))
            {
                result = GeneralizedBodyType.Star;
            }
            else if (!string.IsNullOrEmpty(Scan?.PlanetClass))
            {
                if (Scan.PlanetClass.ToLower().Contains("giant", StringComparison.OrdinalIgnoreCase))
                    result = GeneralizedBodyType.Giant;
                else if (Scan.PlanetClass.Contains("water", StringComparison.OrdinalIgnoreCase))
                {
                    result = GeneralizedBodyType.Water;
                }
                else if (Scan.PlanetClass.Contains("Earth", StringComparison.OrdinalIgnoreCase))
                {
                    result = GeneralizedBodyType.Earthlike;
                }
                else
                {
                    result = GeneralizedBodyType.Other;
                }
            }
            else if (IsBarycentre)
            {
                result = GeneralizedBodyType.Barycentre;
            }
            else if (IsRing)
            {
                result = GeneralizedBodyType.Ring;
            }
            return result;
        }

        public AggregatorGrid ToGrid()
        {
            _gridItem ??= new(_allData, this);
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
