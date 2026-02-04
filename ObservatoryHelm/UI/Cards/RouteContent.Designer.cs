using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    partial class RouteContent
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            label38 = new Label();
            label37 = new Label();
            lblSessionDistance = new Label();
            lblStatsDistance = new Label();
            label34 = new Label();
            pbRouteProgress = new ThemeableProgressBar();
            lblDestination = new Label();
            lblOrigin = new Label();
            ttManager = new ToolTip(components);
            tlblArrowRight = new ThemeableImageLabel();
            SuspendLayout();
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.Location = new Point(304, 56);
            label38.Name = "label38";
            label38.Size = new Size(70, 15);
            label38.TabIndex = 66;
            label38.Text = "This Session";
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Location = new Point(136, 56);
            label37.Name = "label37";
            label37.Size = new Size(87, 15);
            label37.TabIndex = 65;
            label37.Text = "From Statistics:";
            // 
            // lblSessionDistance
            // 
            lblSessionDistance.AutoSize = true;
            lblSessionDistance.Location = new Point(304, 73);
            lblSessionDistance.Name = "lblSessionDistance";
            lblSessionDistance.Size = new Size(83, 15);
            lblSessionDistance.TabIndex = 64;
            lblSessionDistance.Text = "<session dist>";
            ttManager.SetToolTip(lblSessionDistance, "This session (estimated)");
            // 
            // lblStatsDistance
            // 
            lblStatsDistance.AutoSize = true;
            lblStatsDistance.Location = new Point(136, 73);
            lblStatsDistance.Name = "lblStatsDistance";
            lblStatsDistance.Size = new Size(69, 15);
            lblStatsDistance.TabIndex = 63;
            lblStatsDistance.Text = "<stats dist>";
            ttManager.SetToolTip(lblStatsDistance, "From game Statistics");
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Location = new Point(8, 56);
            label34.Name = "label34";
            label34.Size = new Size(104, 15);
            label34.TabIndex = 62;
            label34.Text = "Distance Travelled:";
            // 
            // pbRouteProgress
            // 
            pbRouteProgress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbRouteProgress.Location = new Point(8, 32);
            pbRouteProgress.Maximum = 2;
            pbRouteProgress.Name = "pbRouteProgress";
            pbRouteProgress.Size = new Size(428, 20);
            pbRouteProgress.Step = 1;
            pbRouteProgress.TabIndex = 61;
            ttManager.SetToolTip(pbRouteProgress, "Jumps remaining: 1 / 2");
            pbRouteProgress.Value = 1;
            // 
            // lblDestination
            // 
            lblDestination.AutoSize = true;
            lblDestination.Dock = DockStyle.Right;
            lblDestination.Location = new Point(353, 0);
            lblDestination.Name = "lblDestination";
            lblDestination.Padding = new Padding(0, 8, 8, 0);
            lblDestination.Size = new Size(90, 23);
            lblDestination.TabIndex = 60;
            lblDestination.Text = "<destination>";
            lblDestination.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblOrigin
            // 
            lblOrigin.AutoSize = true;
            lblOrigin.Dock = DockStyle.Left;
            lblOrigin.Location = new Point(0, 0);
            lblOrigin.Name = "lblOrigin";
            lblOrigin.Padding = new Padding(8, 8, 0, 0);
            lblOrigin.Size = new Size(62, 23);
            lblOrigin.TabIndex = 59;
            lblOrigin.Text = "<origin>";
            lblOrigin.TextAlign = ContentAlignment.MiddleLeft;
            ttManager.SetToolTip(lblOrigin, "1234 jumps");
            // 
            // tlblArrowRight
            // 
            tlblArrowRight.Anchor = AnchorStyles.Top;
            tlblArrowRight.AutoSize = true;
            tlblArrowRight.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblArrowRight.ImageColor = Color.Transparent;
            tlblArrowRight.LabelPosition = LabelPositionType.Right;
            tlblArrowRight.Location = new Point(216, 8);
            tlblArrowRight.Name = "tlblArrowRight";
            tlblArrowRight.Size = new Size(0, 0);
            tlblArrowRight.TabIndex = 68;
            // 
            // RouteContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tlblArrowRight);
            Controls.Add(label38);
            Controls.Add(label37);
            Controls.Add(lblSessionDistance);
            Controls.Add(lblStatsDistance);
            Controls.Add(label34);
            Controls.Add(pbRouteProgress);
            Controls.Add(lblDestination);
            Controls.Add(lblOrigin);
            Name = "RouteContent";
            Size = new Size(443, 100);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label38;
        private Label label37;
        private Label lblSessionDistance;
        private Label lblStatsDistance;
        private Label label34;
        private ThemeableProgressBar pbRouteProgress;
        private Label lblDestination;
        private Label lblOrigin;
        private ToolTip ttManager;
        private ThemeableImageLabel tlblArrowRight;
    }
}
