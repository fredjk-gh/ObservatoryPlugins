using System.ComponentModel;
using System.ComponentModel.Design;
using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using com.github.fredjk_gh.PluginCommon.Utilities;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    internal partial class StationContent : HelmContentBase
    {
        private readonly ThemeableImageButton btnStationNameCopy;
        private readonly ScalableImageButton btnViewOnSpansh;
        private readonly ScalableImageButton btnViewOnInara;
        private readonly ThemeableImageButton btnViewStoredShips;
        private readonly ThemeableImageButton btnOpenCommodityMarket;
        private readonly ThemeableImageButton btnOpenShipyard;
        private readonly ThemeableImageButton btnOpenOutfitting;

        private readonly ImageSpec ICON_POSITION = new(PluginCommon.Images.PositionImage)
        {
            Size = new(16, 16),
            Tag = "position",
            ToolTip = "Coordinates",
            Visible = true,
        };
        private readonly ImageSpec ICON_WARNING_SIGN = new(PluginCommon.Images.WarningSignImage)
        {
            Color = Color.Orange,
            Size = new(16, 16),
            Tag = "warning",
            ToolTip = "Warning: No available pads will fit your ship!",
        };
        private readonly ImageSpec ICON_PAD_S = new(PluginCommon.Images.PadSmallImage)
        {
            Size = new(16, 16),
            Tag = "pad_small",
        };
        private readonly ImageSpec ICON_PAD_M = new(PluginCommon.Images.PadMedImage)
        {
            Size = new(16, 16),
            Tag = "pad_med",
        };
        private readonly ImageSpec ICON_PAD_L = new(PluginCommon.Images.PadLargeImage)
        {
            Size = new(16, 16),
            Tag = "pad_large",
        };

        private readonly List<string> StationServicesFilter =
        [
            //"Dock",
            //"Autodock",
            "Black Market",
            "Market",
            //"Contacts",
            "Universal Cartographics",
            "Missions",
            "Outfitting",
            "Crew Lounge",
            "Restock",
            "Refuel",
            "Repair",
            "Shipyard",
            //"Tuning",
            "Workshop",
            //"Missions Generated",
            //"Station Operations",
            "Powerplay",
            "Search and Rescue",
            "Material Trader",
            "Technology Broker",
            //"Station Menu",
            //"Shop",
            "Livery",
            //"Social Space",
            "Bartender",
            "Vista Genomics",
            "Pioneer Supplies",
            "Apex Interstellar",
            "Frontline Solutions",
        ];

        private const string SYSTEM_NAME = "Diaguandri";

        internal StationContent(HelmContext context) : base(context)
        {
            InitializeComponent();

            ContentTitle = "Station";

            // Initialize:
            // Landing pad sizes and fit (TODO: have "too small" versions.)
            ImageSpec padSFits = new(ICON_PAD_S)
            {
                Color = Color.Green,
                Tag = "pad_small_fits",
                ToolTip = "Small pads are available",
                Visible = false,
            };
            ImageSpec padSTooSmall = new(ICON_PAD_S)
            {
                Color = Color.Red,
                Tag = "pad_small_too_small",
                ToolTip = "Small pads are available but are too small for your ship",
                Visible = false,
            };
            ImageSpec padMFits = new(ICON_PAD_M)
            {
                Color = Color.Green,
                Tag = "pad_med_fits",
                ToolTip = "Medium pads are available",
                Visible = false,
            };
            ImageSpec padMTooSmall = new(ICON_PAD_M)
            {
                Color = Color.Red,
                Tag = "pad_med_too_small",
                ToolTip = "Medium pads are available but are too small for your ship",
                Visible = false,
            };
            ImageSpec padLFits = new(ICON_PAD_L)
            {
                Color = Color.Green,
                Tag = "pad_large_fits",
                ToolTip = "Large pads are available",
                Visible = false,
            };

            SuspendLayout();

            listBox1.Items.AddRange([.. StationServicesFilter]);

            tlblCoordinates.ToolTipManager = ttManager;
            tlblLandingPads.ToolTipManager = ttManager;

            tlblCoordinates.AddImage(ICON_POSITION);

            tlblLandingPads.AddImage(padSFits);
            tlblLandingPads.AddImage(padSTooSmall);
            tlblLandingPads.AddImage(padMFits);
            tlblLandingPads.AddImage(padMTooSmall);
            tlblLandingPads.AddImage(padLFits);
            tlblLandingPads.AddImage(ICON_WARNING_SIGN);

            // Toolbar buttons:
            // Copy
            // View on Spansh/Inara
            // Open commodity market
            // Local fleet ?
            // Open available ships for purchase
            // Open available outfitting modules

            btnOpenCommodityMarket = new()
            {
                OriginalImage = Images.CommoditiesImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(btnOpenCommodityMarket, "View commodity market information");
            AddToolButton(btnOpenCommodityMarket);

            btnViewStoredShips = new()
            {
                OriginalImage = Images.ShipsStoredHereImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(btnViewStoredShips, "View ships stored here");
            AddToolButton(btnViewStoredShips);

            btnOpenShipyard = new()
            {
                OriginalImage = Images.ShipsToBuyImage,
                ImageSize = new Size(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(btnOpenShipyard, "View ships available to purchase");
            AddToolButton(btnOpenShipyard);

            btnOpenOutfitting = new()
            {
                OriginalImage = Images.OutfittingImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(btnOpenOutfitting, "View available outfitting modules");
            AddToolButton(btnOpenOutfitting);

            btnViewOnSpansh = new()
            {
                OriginalImage = Images.SpanshImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            btnViewOnSpansh.Click += BtnViewOnSpansh_Click;
            ttManager.SetToolTip(btnViewOnSpansh, "View station on Spansh");
            AddToolButton(btnViewOnSpansh);

            btnViewOnInara = new()
            {
                OriginalImage = Images.InaraImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            btnViewOnInara.Click += BtnViewOnInara_Click;
            ttManager.SetToolTip(btnViewOnInara, "View station on Inara");
            AddToolButton(btnViewOnInara);

            btnStationNameCopy = new()
            {
                OriginalImage = Images.CopyImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(btnStationNameCopy, "Copy name of station into system clipboard");
            AddToolButton(btnStationNameCopy);

            cboStation.SelectedIndex = 18;
            ResumeLayout(false);
            PerformLayout();

            _suppressEvents = false;
        }

        protected override void InternalDraw()
        {

        }

        protected override void InternalClear()
        {

        }

        private void BtnViewOnSpansh_Click(object? sender, EventArgs e)
        {
            string url = $"https://spansh.co.uk/station/{lblMarketId.Text}";

            Misc.OpenUrl(url);
        }

        private void BtnViewOnInara_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboStation.Text)) return;

            string searchQuery = $"{cboStation.Text} [{SYSTEM_NAME}]";

            string url = $"https://inara.cz/elite/station/?search={Uri.EscapeDataString(searchQuery)}";

            Misc.OpenUrl(url);
        }
    }
}
