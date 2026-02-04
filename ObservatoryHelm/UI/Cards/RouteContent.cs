using System.ComponentModel;
using System.ComponentModel.Design;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework.Files;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    internal partial class RouteContent : HelmContentBase
    {
        private readonly ThemeableImageButton routeCreateButton;
        private readonly ThemeableImageButton routeClearButton;
        private readonly ThemeableImageButton routeViewButton;
        private readonly ThemeableImageButton destinationCopyButton;

        private CommanderData _commanderData = null;

        internal RouteContent(HelmContext context) : base(context)
        {
            InitializeComponent();

            ContentTitle = "Route";

            SuspendLayout();

            tlblArrowRight.AddImage(new(Images.ArrowRightImage)
            {
                Size = new(16, 16),
                Tag = "arrow",
                Visible = true,
            });

            routeCreateButton = new() // Coming Soon^TM
            {
                Visible = false,
            };
            routeCreateButton.SetOriginalImage(Images.RouteAddImage);
            ttManager.SetToolTip(routeCreateButton, "Create a new route");

            routeClearButton = new()
            {
                OriginalImage = Images.RouteClearImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(routeClearButton, "Clear current route");

            routeViewButton = new()
            {
                OriginalImage = Images.RouteImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(routeViewButton, "View current route");

            destinationCopyButton = new()
            {
                OriginalImage = Images.CopyImage,
                ImageSize = new(32, 32),
                Visible = false,
            };
            ttManager.SetToolTip(destinationCopyButton, "Copy destination to system clipboard");

            // Add in reverse order as the control is RTL.
            AddToolButton(routeViewButton);
            AddToolButton(routeClearButton);
            AddToolButton(routeCreateButton);
            AddToolButton(destinationCopyButton);

            _c.UIMgr.PropertyChanged += UIState_PropertyChanged;
            destinationCopyButton.Click += DestinationCopyButton_Click;

            InternalClear();
            ResumeLayout();

            _suppressEvents = false;
        }

        protected override void InternalDraw()
        {
            InternalDraw(false);
        }

        private void InternalDraw(bool forceExpand)
        {
            InternalClear();

            _commanderData = _c.Data.For(_c.UIMgr.ForMode().CommanderKey);

            // May need to cache the last nav route content.
            if (_commanderData?.DerivedRoute != null)
            {
                destinationCopyButton.Visible = true;
                lblOrigin.Text = _commanderData.DerivedRoute.OriginSystem.SystemName;
                lblDestination.Text = _commanderData.DerivedRoute.DestinationSystemName;
                pbRouteProgress.Maximum = _commanderData.DerivedRoute.Jumps;

                UpdateRouteProgress();
                if (forceExpand) Expanded = true;
            }
            else
            {
                Expanded = false; // Auto-collapse.
            }

            if (_commanderData?.LastStatistics != null)
            {
                lblStatsDistance.Text = $"{_commanderData.LastStatistics.Exploration.TotalHyperspaceDistance:#,##0} Ly";
            }
            SetTitle();
        }

        public void UpdateRouteProgress()
        {
            _isDirty = true;
            pbRouteProgress.Value = Math.Clamp(
                pbRouteProgress.Maximum - _commanderData.JumpsRemainingInRoute,
                pbRouteProgress.Minimum,
                pbRouteProgress.Maximum);
            lblSessionDistance.Text = $"{_commanderData.DistanceTravelled:#,##0} Ly";
            ttManager.SetToolTip(pbRouteProgress, $"Jumps Remaining: {_commanderData.JumpsRemainingInRoute}");
        }

        protected override void InternalClear()
        {
            destinationCopyButton.Visible = false;
            lblOrigin.Text = string.Empty;
            lblDestination.Text = string.Empty;
            pbRouteProgress.Value = 0;
            pbRouteProgress.Maximum = 1;
            ttManager.SetToolTip(pbRouteProgress, "");
            lblStatsDistance.Text = string.Empty;
            lblSessionDistance.Text = string.Empty;

            _commanderData = null;
        }

        internal void SetTitle()
        {
            if (_isExpanded)
                ContentTitle = "Route";
            else
            {
                if (_c.Data.HasCurrentCommander
                    && !string.IsNullOrEmpty(_commanderData.Destination)
                    && _commanderData.JumpsRemainingInRoute > 0)
                    ContentTitle = $"Route - {_commanderData.JumpsRemainingInRoute} jumps";
                else
                    ContentTitle = "No Route";
            }
        }

        private void DestinationCopyButton_Click(object sender, EventArgs e)
        {
            Misc.SetTextToClipboard(lblDestination.Text);
        }

        private void UIState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case UIStateManager.PROP_COMMANDERKEY:
                case UIStateManager.PROP_SYSTEMID64:
                case UIStateManager.PROP_JUMPSREMAINING:
                    InternalDraw();
                    break;
            }
        }

        protected override void OnBoxStateChanged(object sender, CollapsibleGroupBox.BoxStateEventArgs e)
        {
            _isExpanded = e.Expanded;
            SetTitle();
        }
    }
}
