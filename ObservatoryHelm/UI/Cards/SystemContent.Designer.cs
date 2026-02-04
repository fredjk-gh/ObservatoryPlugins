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
            imgListForListView24 = new ImageList(components);
            label11 = new Label();
            btnSystemZoneHelp = new ThemeableImageButton();
            label26 = new Label();
            ttManager = new ToolTip(components);
            tlblSystemAttributes = new ThemeableImageLabel();
            tlblZones = new ThemeableImageLabel();
            tlblCoordinates = new ThemeableImageLabel();
            cboSystem = new ComboBox();
            label1 = new Label();
            lblSystemBodyCount = new Label();
            SuspendLayout();
            // 
            // btnSystemCoordsCopy
            // 
            btnSystemCoordsCopy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSystemCoordsCopy.AutoSize = true;
            btnSystemCoordsCopy.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSystemCoordsCopy.FlatAppearance.BorderSize = 0;
            btnSystemCoordsCopy.FlatStyle = FlatStyle.Flat;
            btnSystemCoordsCopy.ImageSize = null;
            btnSystemCoordsCopy.Location = new Point(416, 64);
            btnSystemCoordsCopy.Margin = new Padding(0);
            btnSystemCoordsCopy.Name = "btnSystemCoordsCopy";
            btnSystemCoordsCopy.OriginalImage = null;
            btnSystemCoordsCopy.Size = new Size(6, 6);
            btnSystemCoordsCopy.TabIndex = 80;
            ttManager.SetToolTip(btnSystemCoordsCopy, "Copy coordinates to the system clipboard");
            btnSystemCoordsCopy.UseVisualStyleBackColor = true;
            // 
            // btnSystemSetReference
            // 
            btnSystemSetReference.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSystemSetReference.AutoSize = true;
            btnSystemSetReference.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSystemSetReference.FlatAppearance.BorderSize = 0;
            btnSystemSetReference.FlatStyle = FlatStyle.Flat;
            btnSystemSetReference.ImageSize = null;
            btnSystemSetReference.Location = new Point(416, 88);
            btnSystemSetReference.Name = "btnSystemSetReference";
            btnSystemSetReference.OriginalImage = null;
            btnSystemSetReference.Size = new Size(6, 6);
            btnSystemSetReference.TabIndex = 78;
            ttManager.SetToolTip(btnSystemSetReference, "Select a reference system");
            btnSystemSetReference.UseVisualStyleBackColor = true;
            // 
            // lblRefSystem
            // 
            lblRefSystem.AutoSize = true;
            lblRefSystem.Location = new Point(216, 92);
            lblRefSystem.Name = "lblRefSystem";
            lblRefSystem.Size = new Size(72, 15);
            lblRefSystem.TabIndex = 77;
            lblRefSystem.Text = "<sys name>";
            // 
            // lblRefDistance
            // 
            lblRefDistance.AutoSize = true;
            lblRefDistance.Location = new Point(120, 92);
            lblRefDistance.Name = "lblRefDistance";
            lblRefDistance.Size = new Size(64, 15);
            lblRefDistance.TabIndex = 76;
            lblRefDistance.Text = "<xx.xx Ly>";
            ttManager.SetToolTip(lblRefDistance, "Distance to the reference system show to the right.");
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(8, 92);
            label23.Name = "label23";
            label23.Size = new Size(91, 15);
            label23.TabIndex = 75;
            label23.Text = "Ref System Dist:";
            // 
            // cbShowOnlyScoopable
            // 
            cbShowOnlyScoopable.AutoSize = true;
            cbShowOnlyScoopable.Location = new Point(216, 116);
            cbShowOnlyScoopable.Name = "cbShowOnlyScoopable";
            cbShowOnlyScoopable.Size = new Size(131, 19);
            cbShowOnlyScoopable.TabIndex = 72;
            cbShowOnlyScoopable.Text = "Show only fuel stars";
            cbShowOnlyScoopable.UseVisualStyleBackColor = true;
            cbShowOnlyScoopable.CheckedChanged += cbShowOnlyScoopable_CheckedChanged;
            // 
            // lvStars
            // 
            lvStars.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lvStars.Columns.AddRange(new ColumnHeader[] { colFuelAndType, colName, colDistance });
            lvStars.FullRowSelect = true;
            lvStars.GridLines = true;
            lvStars.Location = new Point(8, 140);
            lvStars.Name = "lvStars";
            lvStars.Size = new Size(418, 96);
            lvStars.SmallImageList = imgListForListView24;
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
            // imgListForListView24
            // 
            imgListForListView24.ColorDepth = ColorDepth.Depth32Bit;
            imgListForListView24.ImageSize = new Size(24, 24);
            imgListForListView24.TransparentColor = Color.Transparent;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(8, 116);
            label11.Name = "label11";
            label11.Size = new Size(35, 15);
            label11.TabIndex = 70;
            label11.Text = "Stars:";
            // 
            // btnSystemZoneHelp
            // 
            btnSystemZoneHelp.AutoSize = true;
            btnSystemZoneHelp.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSystemZoneHelp.FlatAppearance.BorderSize = 0;
            btnSystemZoneHelp.FlatStyle = FlatStyle.Flat;
            btnSystemZoneHelp.ImageSize = null;
            btnSystemZoneHelp.Location = new Point(88, 64);
            btnSystemZoneHelp.Name = "btnSystemZoneHelp";
            btnSystemZoneHelp.OriginalImage = null;
            btnSystemZoneHelp.Size = new Size(6, 6);
            btnSystemZoneHelp.TabIndex = 69;
            ttManager.SetToolTip(btnSystemZoneHelp, "More information about Notable Zones");
            btnSystemZoneHelp.UseVisualStyleBackColor = true;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new Point(8, 68);
            label26.Name = "label26";
            label26.Size = new Size(42, 15);
            label26.TabIndex = 66;
            label26.Text = "Zones:";
            // 
            // tlblSystemAttributes
            // 
            tlblSystemAttributes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tlblSystemAttributes.AutoSize = true;
            tlblSystemAttributes.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblSystemAttributes.ImageColor = Color.Transparent;
            tlblSystemAttributes.LabelPosition = LabelPositionType.Right;
            tlblSystemAttributes.Location = new Point(378, 8);
            tlblSystemAttributes.Name = "tlblSystemAttributes";
            tlblSystemAttributes.Size = new Size(46, 15);
            tlblSystemAttributes.TabIndex = 81;
            tlblSystemAttributes.Text = "<attrs>";
            // 
            // tlblZones
            // 
            tlblZones.AutoSize = true;
            tlblZones.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblZones.ImageColor = Color.Transparent;
            tlblZones.LabelPosition = LabelPositionType.Right;
            tlblZones.Location = new Point(120, 64);
            tlblZones.Name = "tlblZones";
            tlblZones.Size = new Size(0, 0);
            tlblZones.TabIndex = 82;
            // 
            // tlblCoordinates
            // 
            tlblCoordinates.AutoSize = true;
            tlblCoordinates.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblCoordinates.ImageColor = Color.Transparent;
            tlblCoordinates.LabelPosition = LabelPositionType.Right;
            tlblCoordinates.Location = new Point(216, 68);
            tlblCoordinates.Name = "tlblCoordinates";
            tlblCoordinates.Size = new Size(52, 15);
            tlblCoordinates.TabIndex = 83;
            tlblCoordinates.Text = "<x, y, z>";
            // 
            // cboSystem
            // 
            cboSystem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboSystem.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSystem.FlatStyle = FlatStyle.Flat;
            cboSystem.FormattingEnabled = true;
            cboSystem.Location = new Point(8, 8);
            cboSystem.Name = "cboSystem";
            cboSystem.Size = new Size(280, 23);
            cboSystem.TabIndex = 84;
            cboSystem.SelectedIndexChanged += cboSystem_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 44);
            label1.Name = "label1";
            label1.Size = new Size(45, 15);
            label1.TabIndex = 85;
            label1.Text = "Bodies:";
            // 
            // lblSystemBodyCount
            // 
            lblSystemBodyCount.AutoSize = true;
            lblSystemBodyCount.Location = new Point(120, 44);
            lblSystemBodyCount.Name = "lblSystemBodyCount";
            lblSystemBodyCount.Size = new Size(58, 15);
            lblSystemBodyCount.TabIndex = 86;
            lblSystemBodyCount.Text = "<bodies>";
            // 
            // SystemContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblSystemBodyCount);
            Controls.Add(label1);
            Controls.Add(cboSystem);
            Controls.Add(tlblSystemAttributes);
            Controls.Add(btnSystemZoneHelp);
            Controls.Add(tlblZones);
            Controls.Add(tlblCoordinates);
            Controls.Add(btnSystemCoordsCopy);
            Controls.Add(label23);
            Controls.Add(lblRefDistance);
            Controls.Add(lblRefSystem);
            Controls.Add(btnSystemSetReference);
            Controls.Add(cbShowOnlyScoopable);
            Controls.Add(lvStars);
            Controls.Add(label11);
            Controls.Add(label26);
            Name = "SystemContent";
            Size = new Size(432, 243);
            ResumeLayout(false);
            PerformLayout();
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
        private ToolTip ttManager;
        private ThemeableImageLabel tlblSystemAttributes;
        private ThemeableImageLabel tlblZones;
        private ThemeableImageLabel tlblCoordinates;
        private ComboBox cboSystem;
        private Label label1;
        private Label lblSystemBodyCount;
        private ImageList imgListForListView24;
    }
}
