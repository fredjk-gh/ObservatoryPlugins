using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public static class FDevIDs
    {
        #region Career Ranks
        private static Dictionary<int, Rank> _CQCRankByNumber = null;
        public static Dictionary<int, Rank> CQCRankByNumber
        {
            get => (_CQCRankByNumber ??= CSVListBuilder.DictFromCSV(Rank.CQCRankByNumberOptions));
        }

        private static Dictionary<int, Rank> _ExplorationRankByNumber = null;
        public static Dictionary<int, Rank> ExplorationRankByNumber
        {
            get => (_ExplorationRankByNumber ??= CSVListBuilder.DictFromCSV(Rank.ExplorationRankByNumberOptions));
        }

        private static Dictionary<int, Rank> _CombatRankByNumber = null;
        public static Dictionary<int, Rank> CombatRankByNumber
        {
            get => (_CombatRankByNumber ??= CSVListBuilder.DictFromCSV(Rank.CombatRankByNumberOptions));
        }

        private static Dictionary<int, Rank> _TradeRankByNumber = null;
        public static Dictionary<int, Rank> TradeRankByNumber
        {
            get => (_TradeRankByNumber ??= CSVListBuilder.DictFromCSV(Rank.TradeRankByNumberOptions));
        }
        #endregion

        #region Superpower Ranks
        private static Dictionary<int, Rank> _EmpireRankByNumber = null;
        public static Dictionary<int, Rank> EmpireRankByNumber
        {
            get => (_EmpireRankByNumber ??= CSVListBuilder.DictFromCSV(Rank.EmpireRankByNumberOptions));
        }

        private static Dictionary<int, Rank> _FederationRankByNumber = null;
        public static Dictionary<int, Rank> FederationRankByNumber
        {
            get => (_FederationRankByNumber ??= CSVListBuilder.DictFromCSV(Rank.FederationRankByNumberOptions));
        }
        #endregion

        #region Gameplay IDs
        private static Dictionary<string, CommodityResource> _CommodityBySymbol = null;
        public static Dictionary<string, CommodityResource> CommodityBySymbol
        {
            get => (_CommodityBySymbol ??= CSVListBuilder.DictFromCSV(CommodityResource.CommoditiesBySymbolOptions));
        }

        private static Dictionary<string, CommodityResource> _MicroResourcesBySymbol = null;
        public static Dictionary<string, CommodityResource> MicroResourcesBySymbol
        {
            get => (_MicroResourcesBySymbol ??= CSVListBuilder.DictFromCSV(CommodityResource.MicroResourcesBySymbolOptions));
        }

        private static Dictionary<string, CommodityResource> _RareCommoditiesBySymbol = null;
        public static Dictionary<string, CommodityResource> RareCommoditiesBySymbol
        {
            get => (_RareCommoditiesBySymbol ??= CSVListBuilder.DictFromCSV(CommodityResource.RareCommoditiesBySymbolOptions));
        }

        private static Dictionary<string, Material> _MaterialsBySymbol = null;
        public static Dictionary<string, Material> MaterialsBySymbol
        {
            get => (_MaterialsBySymbol ??= CSVListBuilder.DictFromCSV(Material.BySymbolOptions));
        }

        private static Dictionary<string, Outfitting> _OutfittingBySymbol = null;
        public static Dictionary<string, Outfitting> OutfittingBySymbol
        {
            get => (_OutfittingBySymbol ??= CSVListBuilder.DictFromCSV(Outfitting.BySymbolOptions));
        }

        private static Dictionary<string, Outfitting> _ShipyardBySymbol = null;
        public static Dictionary<string, Outfitting> ShipyardBySymbol
        {
            get => (_ShipyardBySymbol ??= CSVListBuilder.DictFromCSV(Outfitting.BySymbolOptions));
        }

        #endregion

        #region Misc Ids
        private static Dictionary<string, IdName> _BodiesById = null;
        public static Dictionary<string, IdName> BodiesById
        {
            get => (_BodiesById ??= CSVListBuilder.DictFromCSV(IdName.BodiesByIdOptions));
        }

        private static Dictionary<string, IdName> _EconomyById = null;
        public static Dictionary<string, IdName> EconomyById
        {
            get => (_EconomyById ??= CSVListBuilder.DictFromCSV(IdName.EconomyByIdOptions));
        }

        private static Dictionary<string, IdName> _FactionsById = null;
        public static Dictionary<string, IdName> FactionsById
        {
            get => (_FactionsById ??= CSVListBuilder.DictFromCSV(IdName.FactionsByIdOptions));
        }
        
        private static Dictionary<string, IdName> _FactionStatesById = null;
        public static Dictionary<string, IdName> FactionStatesById
        {
            get => (_FactionStatesById ??= CSVListBuilder.DictFromCSV(IdName.FactionStatesByIdOptions));
        }

        private static Dictionary<string, IdName> _GovernmentById = null;
        public static Dictionary<string, IdName> GovernmentById
        {
            get => (_GovernmentById ??= CSVListBuilder.DictFromCSV(IdName.GovernmentByIdOptions));
        }

        private static Dictionary<string, IdName> _HappinessById = null;
        public static Dictionary<string, IdName> HappinessById
        {
            get => (_HappinessById ??= CSVListBuilder.DictFromCSV(IdName.HappinessByIdOptions));
        }

        private static Dictionary<string, IdName> _PassengersById = null;
        public static Dictionary<string, IdName> PassengersById
        {
            get => (_PassengersById ??= CSVListBuilder.DictFromCSV(IdName.PassengersByIdOptions));
        }

        private static Dictionary<string, IdName> _RingsById = null;
        public static Dictionary<string, IdName> RingsById
        {
            get => (_RingsById ??= CSVListBuilder.DictFromCSV(IdName.RingsByIdOptions));
        }

        private static Dictionary<string, IdName> _SecurityById = null;
        public static Dictionary<string, IdName> SecurityById
        {
            get => (_SecurityById ??= CSVListBuilder.DictFromCSV(IdName.SecurityByIdOptions));
        }

        private static Dictionary<string, IdName> _SaaSignalsById = null;
        public static Dictionary<string, IdName> SaaSignalsById
        {
            get => (_SaaSignalsById ??= CSVListBuilder.DictFromCSV(IdName.SAASignalsByIdOptions));
        }

        private static Dictionary<string, IdName> _SkuById = null;
        public static Dictionary<string, IdName> SkuById
        {
            get => (_SkuById ??= CSVListBuilder.DictFromCSV(IdName.SkuByIdOptions));
        }


        private static Dictionary<string, IdName> _SystemAllegianceById = null;
        public static Dictionary<string, IdName> SystemAllegianceById
        {
            get => (_SystemAllegianceById ??= CSVListBuilder.DictFromCSV(IdName.SystemAllegianceByIdOptions));
        }

        private static Dictionary<string, IdName> _TerraformingStateById = null;
        public static Dictionary<string, IdName> TerraformingStateById
        {
            get => (_TerraformingStateById ??= CSVListBuilder.DictFromCSV(IdName.TerraformingStateByIdOptions));
        }


        private static Dictionary<long, Bundles> _BundlesById = null;
        public static Dictionary<long, Bundles> BundlesById
        {
            get => (_BundlesById ??= CSVListBuilder.DictFromCSV(Bundles.ByIdOptions));
        }

        private static Dictionary<string, Genus> _GenusById = null;
        public static Dictionary<string, Genus> GenusById
        {
            get => (_GenusById ??= CSVListBuilder.DictFromCSV(Genus.ByIdOptions));
        }

        private static Dictionary<string, Region> _RegionById = null;
        public static Dictionary<string, Region> RegionById
        {
            get => (_RegionById ??= CSVListBuilder.DictFromCSV(Region.ByIdOptions));
        }


        #endregion

        #region Lists

        private static List<string> _Crimes = null;
        public static List<string> CrimesList
        {
            get => (_Crimes ??= CSVListBuilder.ListFromCSV(CSVIdLists.CrimeOptions));
        }

        private static List<string> _DockingDeniedReasons = null;
        public static List<string> DockingDeniedReasonsList
        {
            get => (_DockingDeniedReasons ??= CSVListBuilder.ListFromCSV(CSVIdLists.DockingDeniedReasonsOptions));
        }

        #endregion
    }
}
