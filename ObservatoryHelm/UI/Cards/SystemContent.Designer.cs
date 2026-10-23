using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    partial class SystemContent
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
            btnSystemCoordsCopy = new ThemeableImageButton();
            btnSystemSetReference = new ThemeableImageButton();
            lblRefSystem = new Label();
            lblRefDistance = new Label();
            label23 = new Label();
            cbShowOnlyScoopable = new CheckBox();
            lvStars = new ListView();
            colFuelAndType = new ColumnHeader();
            colName = new ColumnHeader();
            colDistance = new ColumnHeader();
            colStarAge = new ColumnHeader();
            imgListForStarsListView = new ImageList(components);
            label11 = new Label();
            btnSystemZoneHelp = new ThemeableImageButton();
            label26 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            cboSystem = new ComboBox();
            tlblCoordinates = new ThemeableImageLabel();
            tlblZones = new ThemeableImageLabel();
            tlblSystemAttributes = new ThemeableImageLabel();
            label1 = new Label();
            lblSystemBodyCount = new Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnSystemCoordsCopy
            // 
            btnSystemCoordsCopy.Anchor = AnchorStyles.Right;
            btnSystemCoordsCopy.AutoSize = true;
            btnSystemCoordsCopy.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSystemCoordsCopy.FlatAppearance.BorderSize = 0;
            btnSystemCoordsCopy.FlatStyle = FlatStyle.Flat;
            btnSystemCoordsCopy.ImageSize = null;
            btnSystemCoordsCopy.Location = new Point(426, 64);
            btnSystemCoordsCopy.Margin = new Padding(0);
            btnSystemCoordsCopy.Name = "btnSystemCoordsCopy";
            btnSystemCoordsCopy.OriginalImage = null;
            btnSystemCoordsCopy.Size = new Size(6, 6);
            btnSystemCoordsCopy.TabIndex = 80;
            btnSystemCoordsCopy.UseVisualStyleBackColor = true;
            // 
            // btnSystemSetReference
            // 
            btnSystemSetReference.Anchor = AnchorStyles.Right;
            btnSystemSetReference.AutoSize = true;
            btnSystemSetReference.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSystemSetReference.FlatAppearance.BorderSize = 0;
            btnSystemSetReference.FlatStyle = FlatStyle.Flat;
            btnSystemSetReference.ImageSize = null;
            btnSystemSetReference.Location = new Point(423, 89);
            btnSystemSetReference.Name = "btnSystemSetReference";
            btnSystemSetReference.OriginalImage = null;
            btnSystemSetReference.Size = new Size(6, 6);
            btnSystemSetReference.TabIndex = 78;
            btnSystemSetReference.UseVisualStyleBackColor = true;
            // 
            // lblRefSystem
            // 
            lblRefSystem.Anchor = AnchorStyles.Left;
            lblRefSystem.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lblRefSystem, 2);
            lblRefSystem.Location = new Point(203, 85);
            lblRefSystem.Name = "lblRefSystem";
            lblRefSystem.Size = new Size(72, 15);
            lblRefSystem.TabIndex = 77;
            lblRefSystem.Text = "<sys name>";
            // 
            // lblRefDistance
            // 
            lblRefDistance.Anchor = AnchorStyles.Left;
            lblRefDistance.AutoSize = true;
            lblRefDistance.Location = new Point(123, 85);
            lblRefDistance.Name = "lblRefDistance";
            lblRefDistance.Size = new Size(64, 15);
            lblRefDistance.TabIndex = 76;
            lblRefDistance.Text = "<xx.xx Ly>";
            // 
            // label23
            // 
            label23.Anchor = AnchorStyles.Left;
            label23.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(label23, 2);
            label23.Location = new Point(3, 85);
            label23.Name = "label23";
            label23.Size = new Size(91, 15);
            label23.TabIndex = 75;
            label23.Text = "Ref System Dist:";
            // 
            // cbShowOnlyScoopable
            // 
            cbShowOnlyScoopable.Anchor = AnchorStyles.Left;
            cbShowOnlyScoopable.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(cbShowOnlyScoopable, 3);
            cbShowOnlyScoopable.Location = new Point(203, 108);
            cbShowOnlyScoopable.Name = "cbShowOnlyScoopable";
            cbShowOnlyScoopable.Size = new Size(131, 19);
            cbShowOnlyScoopable.TabIndex = 72;
            cbShowOnlyScoopable.Text = "Show only fuel stars";
            cbShowOnlyScoopable.UseVisualStyleBackColor = true;
            cbShowOnlyScoopable.CheckedChanged += cbShowOnlyScoopable_CheckedChanged;
            // 
            // lvStars
            // 
            lvStars.Columns.AddRange(new ColumnHeader[] { colFuelAndType, colName, colDistance, colStarAge });
            tableLayoutPanel1.SetColumnSpan(lvStars, 6);
            lvStars.Dock = DockStyle.Fill;
            lvStars.FullRowSelect = true;
            lvStars.GridLines = true;
            lvStars.Location = new Point(3, 133);
            lvStars.Name = "lvStars";
            lvStars.Size = new Size(426, 108);
            lvStars.SmallImageList = imgListForStarsListView;
            lvStars.TabIndex = 71;
            lvStars.UseCompatibleStateImageBehavior = false;
            lvStars.View = View.Details;
            // 
            // colFuelAndType
            // 
            colFuelAndType.Text = "Type";
            colFuelAndType.Width = 250;
            // 
            // colName
            // 
            colName.Text = "Name";
            colName.Width = 150;
            // 
            // colDistance
            // 
            colDistance.Text = "Dist (Ls)";
            colDistance.Width = 150;
            // 
            // colStarAge
            // 
            colStarAge.Text = "Age";
            colStarAge.Width = 100;
            // 
            // imgListForStarsListView
            // 
            imgListForStarsListView.ColorDepth = ColorDepth.Depth32Bit;
            imgListForStarsListView.ImageSize = new Size(24, 24);
            imgListForStarsListView.TransparentColor = Color.Transparent;
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Left;
            label11.AutoSize = true;
            label11.Location = new Point(3, 110);
            label11.Name = "label11";
            label11.Size = new Size(35, 15);
            label11.TabIndex = 70;
            label11.Text = "Stars:";
            // 
            // btnSystemZoneHelp
            // 
            btnSystemZoneHelp.Anchor = AnchorStyles.Left;
            btnSystemZoneHelp.AutoSize = true;
            btnSystemZoneHelp.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSystemZoneHelp.FlatAppearance.BorderSize = 0;
            btnSystemZoneHelp.FlatStyle = FlatStyle.Flat;
            btnSystemZoneHelp.ImageSize = null;
            btnSystemZoneHelp.Location = new Point(75, 64);
            btnSystemZoneHelp.Name = "btnSystemZoneHelp";
            btnSystemZoneHelp.OriginalImage = null;
            btnSystemZoneHelp.Size = new Size(6, 6);
            btnSystemZoneHelp.TabIndex = 69;
            btnSystemZoneHelp.UseVisualStyleBackColor = true;
            // 
            // label26
            // 
            label26.Anchor = AnchorStyles.Left;
            label26.AutoSize = true;
            label26.Location = new Point(3, 60);
            label26.Name = "label26";
            label26.Size = new Size(42, 15);
            label26.TabIndex = 66;
            label26.Text = "Zones:";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 6;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.Controls.Add(cboSystem, 0, 0);
            tableLayoutPanel1.Controls.Add(lvStars, 0, 5);
            tableLayoutPanel1.Controls.Add(cbShowOnlyScoopable, 3, 4);
            tableLayoutPanel1.Controls.Add(btnSystemSetReference, 5, 3);
            tableLayoutPanel1.Controls.Add(btnSystemCoordsCopy, 5, 2);
            tableLayoutPanel1.Controls.Add(label11, 0, 4);
            tableLayoutPanel1.Controls.Add(tlblCoordinates, 3, 2);
            tableLayoutPanel1.Controls.Add(lblRefSystem, 3, 3);
            tableLayoutPanel1.Controls.Add(lblRefDistance, 2, 3);
            tableLayoutPanel1.Controls.Add(label23, 0, 3);
            tableLayoutPanel1.Controls.Add(tlblZones, 2, 2);
            tableLayoutPanel1.Controls.Add(btnSystemZoneHelp, 1, 2);
            tableLayoutPanel1.Controls.Add(tlblSystemAttributes, 4, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 1);
            tableLayoutPanel1.Controls.Add(label26, 0, 2);
            tableLayoutPanel1.Controls.Add(lblSystemBodyCount, 2, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(432, 244);
            tableLayoutPanel1.TabIndex = 87;
            // 
            // cboSystem
            // 
            cboSystem.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.SetColumnSpan(cboSystem, 4);
            cboSystem.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSystem.FlatStyle = FlatStyle.Flat;
            cboSystem.FormattingEnabled = true;
            cboSystem.Location = new Point(2, 3);
            cboSystem.Margin = new Padding(2);
            cboSystem.Name = "cboSystem";
            cboSystem.Size = new Size(264, 23);
            cboSystem.TabIndex = 84;
            cboSystem.SelectedIndexChanged += cboSystem_SelectedIndexChanged;
            // 
            // tlblCoordinates
            // 
            tlblCoordinates.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tlblCoordinates.AutoSize = true;
            tlblCoordinates.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.SetColumnSpan(tlblCoordinates, 2);
            tlblCoordinates.ImageColor = Color.Transparent;
            tlblCoordinates.LabelPosition = LabelPositionType.Right;
            tlblCoordinates.Location = new Point(203, 60);
            tlblCoordinates.Margin = new Padding(3, 0, 3, 0);
            tlblCoordinates.Name = "tlblCoordinates";
            tlblCoordinates.Size = new Size(194, 15);
            tlblCoordinates.TabIndex = 83;
            tlblCoordinates.Text = "<x, y, z>";
            // 
            // tlblZones
            // 
            tlblZones.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tlblZones.AutoSize = true;
            tlblZones.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblZones.ImageColor = Color.Transparent;
            tlblZones.LabelPosition = LabelPositionType.Right;
            tlblZones.Location = new Point(123, 67);
            tlblZones.Margin = new Padding(3, 0, 3, 0);
            tlblZones.Name = "tlblZones";
            tlblZones.Size = new Size(74, 0);
            tlblZones.TabIndex = 82;
            // 
            // tlblSystemAttributes
            // 
            tlblSystemAttributes.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tlblSystemAttributes.AutoSize = true;
            tlblSystemAttributes.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.SetColumnSpan(tlblSystemAttributes, 2);
            tlblSystemAttributes.ImageColor = Color.Transparent;
            tlblSystemAttributes.LabelPosition = LabelPositionType.Right;
            tlblSystemAttributes.Location = new Point(271, 7);
            tlblSystemAttributes.Name = "tlblSystemAttributes";
            tlblSystemAttributes.Size = new Size(158, 15);
            tlblSystemAttributes.TabIndex = 81;
            tlblSystemAttributes.Text = "<attrs>";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(3, 35);
            label1.Name = "label1";
            label1.Size = new Size(45, 15);
            label1.TabIndex = 85;
            label1.Text = "Bodies:";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblSystemBodyCount
            // 
            lblSystemBodyCount.Anchor = AnchorStyles.Left;
            lblSystemBodyCount.AutoSize = true;
            lblSystemBodyCount.Location = new Point(123, 35);
            lblSystemBodyCount.Name = "lblSystemBodyCount";
            lblSystemBodyCount.Size = new Size(58, 15);
            lblSystemBodyCount.TabIndex = 86;
            lblSystemBodyCount.Text = "<bodies>";
            lblSystemBodyCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // SystemContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "SystemContent";
            Size = new Size(432, 244);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ThemeableImageButton btnSystemCoordsCopy;
        private ThemeableImageButton btnSystemSetReference;
        private Label lblRefSystem;
        private Label lblRefDistance;
        private Label label23;
        private CheckBox cbShowOnlyScoopable;
        private ListView lvStars;
        private ColumnHeader colDistance;
        private ColumnHeader colFuelAndType;
        private ColumnHeader colName;
        private Label label11;
        private ThemeableImageButton btnSystemZoneHelp;
        private Label label26;
        private ThemeableImageLabel tlblSystemAttributes;
        private ThemeableImageLabel tlblZones;
        private ThemeableImageLabel tlblCoordinates;
        private ComboBox cboSystem;
        private Label label1;
        private Label lblSystemBodyCount;
        private ImageList imgListForStarsListView;
        private ColumnHeader colStarAge;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
