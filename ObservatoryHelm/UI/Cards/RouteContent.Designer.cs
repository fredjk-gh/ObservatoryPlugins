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
            label38 = new Label();
            label37 = new Label();
            lblSessionDistance = new Label();
            lblStatsDistance = new Label();
            label34 = new Label();
            pbRouteProgress = new ThemeableProgressBar();
            lblDestination = new Label();
            lblOrigin = new Label();
            tlblArrowRight = new ThemeableImageLabel();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // label38
            // 
            label38.Anchor = AnchorStyles.Left;
            label38.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(label38, 2);
            label38.Location = new Point(238, 55);
            label38.Name = "label38";
            label38.Size = new Size(73, 15);
            label38.TabIndex = 66;
            label38.Text = "This Session:";
            // 
            // label37
            // 
            label37.Anchor = AnchorStyles.Left;
            label37.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(label37, 2);
            label37.Location = new Point(126, 55);
            label37.Name = "label37";
            label37.Size = new Size(87, 15);
            label37.TabIndex = 65;
            label37.Text = "From Statistics:";
            // 
            // lblSessionDistance
            // 
            lblSessionDistance.Anchor = AnchorStyles.Left;
            lblSessionDistance.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lblSessionDistance, 2);
            lblSessionDistance.Location = new Point(238, 80);
            lblSessionDistance.Name = "lblSessionDistance";
            lblSessionDistance.Size = new Size(83, 15);
            lblSessionDistance.TabIndex = 64;
            lblSessionDistance.Text = "<session dist>";
            // 
            // lblStatsDistance
            // 
            lblStatsDistance.Anchor = AnchorStyles.Left;
            lblStatsDistance.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lblStatsDistance, 2);
            lblStatsDistance.Location = new Point(126, 80);
            lblStatsDistance.Name = "lblStatsDistance";
            lblStatsDistance.Size = new Size(69, 15);
            lblStatsDistance.TabIndex = 63;
            lblStatsDistance.Text = "<stats dist>";
            // 
            // label34
            // 
            label34.Anchor = AnchorStyles.Left;
            label34.AutoSize = true;
            label34.Location = new Point(3, 55);
            label34.Name = "label34";
            label34.Size = new Size(104, 15);
            label34.TabIndex = 62;
            label34.Text = "Distance Travelled:";
            // 
            // pbRouteProgress
            // 
            pbRouteProgress.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.SetColumnSpan(pbRouteProgress, 5);
            pbRouteProgress.Location = new Point(3, 28);
            pbRouteProgress.Maximum = 2;
            pbRouteProgress.Name = "pbRouteProgress";
            pbRouteProgress.Size = new Size(435, 19);
            pbRouteProgress.Step = 1;
            pbRouteProgress.TabIndex = 61;
            pbRouteProgress.Value = 1;
            // 
            // lblDestination
            // 
            lblDestination.Anchor = AnchorStyles.Right;
            lblDestination.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lblDestination, 2);
            lblDestination.Location = new Point(356, 5);
            lblDestination.Name = "lblDestination";
            lblDestination.Size = new Size(82, 15);
            lblDestination.TabIndex = 60;
            lblDestination.Text = "<destination>";
            lblDestination.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblOrigin
            // 
            lblOrigin.Anchor = AnchorStyles.Left;
            lblOrigin.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lblOrigin, 2);
            lblOrigin.Location = new Point(3, 5);
            lblOrigin.Name = "lblOrigin";
            lblOrigin.Size = new Size(54, 15);
            lblOrigin.TabIndex = 59;
            lblOrigin.Text = "<origin>";
            lblOrigin.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tlblArrowRight
            // 
            tlblArrowRight.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tlblArrowRight.AutoSize = true;
            tlblArrowRight.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblArrowRight.ImageColor = Color.Transparent;
            tlblArrowRight.LabelPosition = LabelPositionType.Right;
            tlblArrowRight.Location = new Point(208, 3);
            tlblArrowRight.Name = "tlblArrowRight";
            tlblArrowRight.Size = new Size(24, 19);
            tlblArrowRight.TabIndex = 68;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.Controls.Add(lblOrigin, 0, 0);
            tableLayoutPanel1.Controls.Add(label38, 3, 2);
            tableLayoutPanel1.Controls.Add(tlblArrowRight, 2, 0);
            tableLayoutPanel1.Controls.Add(label37, 1, 2);
            tableLayoutPanel1.Controls.Add(lblStatsDistance, 1, 3);
            tableLayoutPanel1.Controls.Add(pbRouteProgress, 0, 1);
            tableLayoutPanel1.Controls.Add(label34, 0, 2);
            tableLayoutPanel1.Controls.Add(lblDestination, 3, 0);
            tableLayoutPanel1.Controls.Add(lblSessionDistance, 3, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(441, 106);
            tableLayoutPanel1.TabIndex = 69;
            // 
            // RouteContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "RouteContent";
            Size = new Size(441, 106);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
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
        private ThemeableImageLabel tlblArrowRight;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
