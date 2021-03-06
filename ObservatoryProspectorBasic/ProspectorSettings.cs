using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryProspectorBasic
{
    class ProspectorSettings
    {
        public enum Commodities
        {
            // Minerals:
            Alexandrite,
            Bauxite,
            Benitoite,
            Bertrandite,
            Bromellite,
            Coltan,
            Gallite,
            Grandidierite,
            Indite,
            Lepidolite,
            LithiumHydroxide,
            LowTemperatureDiamond,
            MethaneClathrate,
            MethanolMonohydrateCrystals,
            Monazite,
            Musgravite,
            Opal,
            Painite,
            Rhodplumsite,
            Rutile,
            Serendibite,
            Uraninite,

            // Metals:
            Cobalt,
            Gold,
            Osmium,
            Palladium,
            Platinum,
            Praseodymium,
            Samarium,
            Silver,
            Thorium,

            // Chemicals:
            HydrogenPeroxide,
            LiquidOxygen,
            Tritium,
            Water,
        }

        private Dictionary<Commodities, Boolean> ProspectingFor = new();
        internal bool getFor(Commodities commodity)
        {
            return ProspectingFor.GetValueOrDefault(commodity, false);
        }
        internal void setFor(Commodities c, bool value)
        {
            ProspectingFor[c] = value;
        }

        [SettingDisplayName("Minimum Percent")]
        [SettingNumericBounds(0.0, 66.0)]
        public int MinimumPercent { get; set; }


        [SettingDisplayName("Prospect high material content")]
        public bool ProspectHighMaterialContent { get; set; }


        // Minerals:
        [SettingDisplayName("Prospect Alexandrite")]
        public bool ProspectAlexandrite
        {
            get { return getFor(Commodities.Alexandrite); }
            set { setFor(Commodities.Alexandrite, value); }
        }
        [SettingDisplayName("Prospect Bauxite")]
        public bool ProspectBauxite
        {
            get { return getFor(Commodities.Bauxite); }
            set { setFor(Commodities.Bauxite, value); }
        }
        [SettingDisplayName("Prospect Benitoite")]
        public bool ProspectBenitoite
        {
            get { return getFor(Commodities.Benitoite); }
            set { setFor(Commodities.Benitoite, value); }
        }
        [SettingDisplayName("Prospect Bertrandite")]
        public bool ProspectBertrandite
        {
            get { return getFor(Commodities.Bertrandite); }
            set { setFor(Commodities.Bertrandite, value); }
        }
        [SettingDisplayName("Prospect Bromellite")]
        public bool ProspectBromellite
        {
            get { return getFor(Commodities.Bromellite); }
            set { setFor(Commodities.Bromellite, value); }
        }
        [SettingDisplayName("Prospect Coltan")]
        public bool ProspectColtan
        {
            get { return getFor(Commodities.Coltan); }
            set { setFor(Commodities.Coltan, value); }
        }
        [SettingDisplayName("Prospect Gallite")]
        public bool ProspectGallite
        {
            get { return getFor(Commodities.Gallite); }
            set { setFor(Commodities.Gallite, value); }
        }
        [SettingDisplayName("Prospect Grandidierite")]
        public bool ProspectGrandidierite
        {
            get { return getFor(Commodities.Grandidierite); }
            set { setFor(Commodities.Grandidierite, value); }
        }
        [SettingDisplayName("Prospect Indite")]
        public bool ProspectIndite
        {
            get { return getFor(Commodities.Indite); }
            set { setFor(Commodities.Indite, value); }
        }
        [SettingDisplayName("Prospect Lepidolite")]
        public bool ProspectLepidolite
        {
            get { return getFor(Commodities.Lepidolite); }
            set { setFor(Commodities.Lepidolite, value); }
        }
        [SettingDisplayName("Prospect Lithium ydroxide")]
        public bool ProspectLithiumHydroxide
        {
            get { return getFor(Commodities.LithiumHydroxide); }
            set { setFor(Commodities.LithiumHydroxide, value); }
        }
        [SettingDisplayName("Prospect Low Temperature Diamonds")]
        public bool ProspectLowTemperatureDiamond
        {
            get { return getFor(Commodities.LowTemperatureDiamond); }
            set { setFor(Commodities.LowTemperatureDiamond, value); }
        }
        [SettingDisplayName("Prospect Methane Clathrate")]
        public bool ProspectMethaneClathrate
        {
            get { return getFor(Commodities.MethaneClathrate); }
            set { setFor(Commodities.MethaneClathrate, value); }
        }
        [SettingDisplayName("Prospect Methanol Monohydrate Crystals")]
        public bool ProspectMethanolMonohydrateCrystals
        {
            get { return getFor(Commodities.MethanolMonohydrateCrystals); }
            set { setFor(Commodities.MethanolMonohydrateCrystals, value); }
        }
        [SettingDisplayName("Prospect Monazite")]
        public bool ProspectMonazite
        {
            get { return getFor(Commodities.Monazite); }
            set { setFor(Commodities.Monazite, value); }
        }
        [SettingDisplayName("Prospect Musgravite")]
        public bool ProspectMusgravite
        {
            get { return getFor(Commodities.Musgravite); }
            set { setFor(Commodities.Musgravite, value); }
        }
        [SettingDisplayName("Prospect Void Opal")]
        public bool ProspectOpal
        {
            get { return getFor(Commodities.Opal); }
            set { setFor(Commodities.Opal, value); }
        }
        [SettingDisplayName("Prospect Painite")]
        public bool ProspectPainite
        {
            get { return getFor(Commodities.Painite); }
            set { setFor(Commodities.Painite, value); }
        }
        [SettingDisplayName("Prospect Rhodplumsite")]
        public bool ProspectRhodplumsite
        {
            get { return getFor(Commodities.Rhodplumsite); }
            set { setFor(Commodities.Rhodplumsite, value); }
        }
        [SettingDisplayName("Prospect Rutile")]
        public bool ProspectRutile
        {
            get { return getFor(Commodities.Rutile); }
            set { setFor(Commodities.Rutile, value); }
        }
        [SettingDisplayName("Prospect Serendibite")]
        public bool ProspectSerendibite
        {
            get { return getFor(Commodities.Serendibite); }
            set { setFor(Commodities.Serendibite, value); }
        }
        [SettingDisplayName("Prospect Uraninite")]
        public bool ProspectUraninite
        {
            get { return getFor(Commodities.Uraninite); }
            set { setFor(Commodities.Uraninite, value); }
        }
 
        // Metals:
        [SettingDisplayName("Prospect Cobalt")]
        public bool ProspectCobalt
        {
            get { return getFor(Commodities.Cobalt); }
            set { setFor(Commodities.Cobalt, value); }
        }
        [SettingDisplayName("Prospect Gold")]
        public bool ProspectGold
        {
            get { return getFor(Commodities.Gold); }
            set { setFor(Commodities.Gold, value); }
        }
        [SettingDisplayName("Prospect Osmium")]
        public bool ProspectOsmium
        {
            get { return getFor(Commodities.Osmium); }
            set { setFor(Commodities.Osmium, value); }
        }
        [SettingDisplayName("Prospect Palladium")]
        public bool ProspectPalladium
        {
            get { return getFor(Commodities.Palladium); }
            set { setFor(Commodities.Palladium, value); }
        }
        [SettingDisplayName("Prospect Platinum")]
        public bool ProspectPlatinum
        {
            get { return getFor(Commodities.Platinum); }
            set { setFor(Commodities.Platinum, value); }
        }
        [SettingDisplayName("Prospect Praseodymium")]
        public bool ProspectPraseodymium
        {
            get { return getFor(Commodities.Praseodymium); }
            set { setFor(Commodities.Praseodymium, value); }
        }
        [SettingDisplayName("Prospect Samarium")]
        public bool ProspectSamarium
        {
            get { return getFor(Commodities.Samarium); }
            set { setFor(Commodities.Samarium, value); }
        }
        [SettingDisplayName("Prospect Silver")]
        public bool ProspectSilver
        {
            get { return getFor(Commodities.Silver); }
            set { setFor(Commodities.Silver, value); }
        }
        [SettingDisplayName("Prospect Thorium")]
 
        // Chemicals:
        public bool ProspectThorium
        {
            get { return getFor(Commodities.Thorium); }
            set { setFor(Commodities.Thorium, value); }
        }
        [SettingDisplayName("Prospect Hydrogen Peroxide")]
        public bool ProspectHydrogenPeroxide
        {
            get { return getFor(Commodities.HydrogenPeroxide); }
            set { setFor(Commodities.HydrogenPeroxide, value); }
        }
        [SettingDisplayName("Prospect Liquid Oxygen")]
        public bool ProspectLiquidOxygen
        {
            get { return getFor(Commodities.LiquidOxygen); }
            set { setFor(Commodities.LiquidOxygen, value); }
        }
        [SettingDisplayName("Prospect Tritium")]
        public bool ProspectTritium
        {
            get { return getFor(Commodities.Tritium); }
            set { setFor(Commodities.Tritium, value); }
        }
        [SettingDisplayName("Prospect Water")]
        public bool ProspectWater
        {
            get { return getFor(Commodities.Water); }
            set { setFor(Commodities.Water, value); }
        }


    }
}
