using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    partial class PlanetContent
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
            btnBodyCoordsCopy = new ThemeableImageButton();
            lblBodyPressure = new Label();
            lblBodyType = new ScalableLabel();
            label13 = new Label();
            label29 = new Label();
            lblBodyAltitude = new Label();
            label18 = new Label();
            tlblBodyAttributes = new ThemeableImageLabel();
            tlblGravityValue = new ThemeableImageLabel();
            tlblTempValue = new ThemeableImageLabel();
            tlblGeoSignals = new ThemeableImageLabel();
            tlblBioSignals = new ThemeableImageLabel();
            tlblBodyRings = new ThemeableImageLabel();
            lblBodyDistance = new Label();
            label2 = new Label();
            tlblCoordinates = new ThemeableImageLabel();
            cboBody = new ComboBox();
            label1 = new Label();
            lblAtmosphereTypeValue = new Label();
            label3 = new Label();
            lblRadiusValue = new Label();
            tlblBodyMiningLocations = new ThemeableImageLabel();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnBodyCoordsCopy
            // 
            btnBodyCoordsCopy.Anchor = AnchorStyles.Right;
            btnBodyCoordsCopy.AutoSize = true;
            btnBodyCoordsCopy.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnBodyCoordsCopy.FlatAppearance.BorderSize = 0;
            btnBodyCoordsCopy.FlatStyle = FlatStyle.Flat;
            btnBodyCoordsCopy.Font = new Font("Material Symbols Rounded", 10F);
            btnBodyCoordsCopy.ImageSize = null;
            btnBodyCoordsCopy.Location = new Point(482, 69);
            btnBodyCoordsCopy.Margin = new Padding(0, 0, 2, 0);
            btnBodyCoordsCopy.Name = "btnBodyCoordsCopy";
            btnBodyCoordsCopy.OriginalImage = null;
            btnBodyCoordsCopy.Size = new Size(6, 6);
            btnBodyCoordsCopy.TabIndex = 94;
            btnBodyCoordsCopy.UseVisualStyleBackColor = true;
            // 
            // lblBodyPressure
            // 
            lblBodyPressure.Anchor = AnchorStyles.Left;
            lblBodyPressure.AutoSize = true;
            lblBodyPressure.Location = new Point(101, 115);
            lblBodyPressure.Name = "lblBodyPressure";
            lblBodyPressure.Size = new Size(91, 15);
            lblBodyPressure.TabIndex = 86;
            lblBodyPressure.Text = "<pressure> atm";
            // 
            // lblBodyType
            // 
            lblBodyType.Anchor = AnchorStyles.Left;
            lblBodyType.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lblBodyType, 4);
            lblBodyType.Font = new Font("Segoe UI", 12F);
            lblBodyType.FontSizeAdjustment = 3F;
            lblBodyType.Location = new Point(101, 7);
            lblBodyType.Name = "lblBodyType";
            lblBodyType.Size = new Size(101, 21);
            lblBodyType.TabIndex = 85;
            lblBodyType.Text = "<body type>";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(3, 90);
            label13.Margin = new Padding(3, 5, 3, 3);
            label13.Name = "label13";
            label13.Size = new Size(76, 15);
            label13.TabIndex = 84;
            label13.Text = "Temperature:";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(3, 65);
            label29.Margin = new Padding(3, 5, 3, 3);
            label29.Name = "label29";
            label29.Size = new Size(47, 15);
            label29.TabIndex = 81;
            label29.Text = "Gravity:";
            // 
            // lblBodyAltitude
            // 
            lblBodyAltitude.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lblBodyAltitude, 2);
            lblBodyAltitude.Location = new Point(286, 40);
            lblBodyAltitude.Margin = new Padding(3, 5, 3, 3);
            lblBodyAltitude.Name = "lblBodyAltitude";
            lblBodyAltitude.Size = new Size(56, 15);
            lblBodyAltitude.TabIndex = 76;
            lblBodyAltitude.Text = "<alt> km";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(218, 40);
            label18.Margin = new Padding(3, 5, 3, 3);
            label18.Name = "label18";
            label18.Size = new Size(52, 15);
            label18.TabIndex = 75;
            label18.Text = "Altitude:";
            // 
            // tlblBodyAttributes
            // 
            tlblBodyAttributes.AutoSize = true;
            tlblBodyAttributes.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblBodyAttributes.Dock = DockStyle.Fill;
            tlblBodyAttributes.ImageColor = Color.Transparent;
            tlblBodyAttributes.LabelPosition = LabelPositionType.Right;
            tlblBodyAttributes.Location = new Point(422, 3);
            tlblBodyAttributes.MinimumSize = new Size(0, 21);
            tlblBodyAttributes.Name = "tlblBodyAttributes";
            tableLayoutPanel1.SetRowSpan(tlblBodyAttributes, 2);
            tlblBodyAttributes.Size = new Size(65, 54);
            tlblBodyAttributes.TabIndex = 3;
            tlblBodyAttributes.Text = "<attrs>";
            // 
            // tlblGravityValue
            // 
            tlblGravityValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlblGravityValue.AutoSize = true;
            tlblGravityValue.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblGravityValue.ImageColor = Color.Transparent;
            tlblGravityValue.LabelPosition = LabelPositionType.Left;
            tlblGravityValue.Location = new Point(101, 65);
            tlblGravityValue.Margin = new Padding(3, 5, 3, 3);
            tlblGravityValue.MinimumSize = new Size(0, 17);
            tlblGravityValue.Name = "tlblGravityValue";
            tlblGravityValue.Size = new Size(111, 17);
            tlblGravityValue.TabIndex = 96;
            tlblGravityValue.Text = "<gravity> g";
            // 
            // tlblTempValue
            // 
            tlblTempValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlblTempValue.AutoSize = true;
            tlblTempValue.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblTempValue.ImageColor = Color.Transparent;
            tlblTempValue.LabelPosition = LabelPositionType.Left;
            tlblTempValue.Location = new Point(101, 90);
            tlblTempValue.Margin = new Padding(3, 5, 3, 3);
            tlblTempValue.MinimumSize = new Size(0, 17);
            tlblTempValue.Name = "tlblTempValue";
            tlblTempValue.Size = new Size(111, 17);
            tlblTempValue.TabIndex = 97;
            tlblTempValue.Text = "<temp> K";
            // 
            // tlblGeoSignals
            // 
            tlblGeoSignals.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlblGeoSignals.AutoSize = true;
            tlblGeoSignals.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblGeoSignals.ImageColor = Color.Transparent;
            tlblGeoSignals.LabelPosition = LabelPositionType.Right;
            tlblGeoSignals.Location = new Point(286, 90);
            tlblGeoSignals.Margin = new Padding(3, 5, 3, 3);
            tlblGeoSignals.MinimumSize = new Size(0, 17);
            tlblGeoSignals.Name = "tlblGeoSignals";
            tlblGeoSignals.Size = new Size(62, 17);
            tlblGeoSignals.TabIndex = 98;
            tlblGeoSignals.Text = "<geos>";
            tlblGeoSignals.Click += BodySignalsLabelClick;
            // 
            // tlblBioSignals
            // 
            tlblBioSignals.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlblBioSignals.AutoSize = true;
            tlblBioSignals.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblBioSignals.ImageColor = Color.Transparent;
            tlblBioSignals.LabelPosition = LabelPositionType.Right;
            tlblBioSignals.Location = new Point(218, 90);
            tlblBioSignals.Margin = new Padding(3, 5, 3, 3);
            tlblBioSignals.MinimumSize = new Size(0, 17);
            tlblBioSignals.Name = "tlblBioSignals";
            tlblBioSignals.Size = new Size(62, 17);
            tlblBioSignals.TabIndex = 99;
            tlblBioSignals.Text = "<bios>";
            tlblBioSignals.Click += BodySignalsLabelClick;
            // 
            // tlblBodyRings
            // 
            tlblBodyRings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlblBodyRings.AutoSize = true;
            tlblBodyRings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblBodyRings.ImageColor = Color.Transparent;
            tlblBodyRings.LabelPosition = LabelPositionType.Right;
            tlblBodyRings.Location = new Point(422, 90);
            tlblBodyRings.Margin = new Padding(3, 5, 3, 3);
            tlblBodyRings.MinimumSize = new Size(0, 17);
            tlblBodyRings.Name = "tlblBodyRings";
            tlblBodyRings.Size = new Size(65, 17);
            tlblBodyRings.TabIndex = 100;
            tlblBodyRings.Text = "<rings>";
            tlblBodyRings.Click += BodySignalsLabelClick;
            // 
            // lblBodyDistance
            // 
            lblBodyDistance.AutoSize = true;
            lblBodyDistance.Location = new Point(218, 140);
            lblBodyDistance.Margin = new Padding(3, 5, 3, 3);
            lblBodyDistance.Name = "lblBodyDistance";
            lblBodyDistance.Size = new Size(56, 15);
            lblBodyDistance.TabIndex = 101;
            lblBodyDistance.Text = "<dist> Ls";
            // 
            // label2
            // 
            label2.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(label2, 2);
            label2.Location = new Point(3, 140);
            label2.Margin = new Padding(3, 5, 3, 3);
            label2.Name = "label2";
            label2.Size = new Size(119, 15);
            label2.TabIndex = 102;
            label2.Text = "Distance from arrival:";
            // 
            // tlblCoordinates
            // 
            tlblCoordinates.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlblCoordinates.AutoSize = true;
            tlblCoordinates.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.SetColumnSpan(tlblCoordinates, 3);
            tlblCoordinates.ImageColor = Color.Transparent;
            tlblCoordinates.LabelPosition = LabelPositionType.Right;
            tlblCoordinates.Location = new Point(218, 65);
            tlblCoordinates.Margin = new Padding(3, 5, 3, 3);
            tlblCoordinates.MinimumSize = new Size(0, 17);
            tlblCoordinates.Name = "tlblCoordinates";
            tlblCoordinates.Size = new Size(198, 17);
            tlblCoordinates.TabIndex = 103;
            tlblCoordinates.Text = "<x, y>";
            // 
            // cboBody
            // 
            cboBody.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cboBody.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBody.FlatStyle = FlatStyle.Flat;
            cboBody.FormattingEnabled = true;
            cboBody.Location = new Point(3, 6);
            cboBody.Name = "cboBody";
            cboBody.Size = new Size(92, 23);
            cboBody.TabIndex = 104;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 115);
            label1.Margin = new Padding(3, 5, 3, 3);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 106;
            label1.Text = "Atmosphere:";
            // 
            // lblAtmosphereTypeValue
            // 
            lblAtmosphereTypeValue.Anchor = AnchorStyles.Left;
            lblAtmosphereTypeValue.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lblAtmosphereTypeValue, 4);
            lblAtmosphereTypeValue.Location = new Point(218, 115);
            lblAtmosphereTypeValue.Name = "lblAtmosphereTypeValue";
            lblAtmosphereTypeValue.Size = new Size(112, 15);
            lblAtmosphereTypeValue.TabIndex = 105;
            lblAtmosphereTypeValue.Text = "<atmosphere type>";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 40);
            label3.Margin = new Padding(3, 5, 3, 3);
            label3.Name = "label3";
            label3.Size = new Size(45, 15);
            label3.TabIndex = 107;
            label3.Text = "Radius:";
            // 
            // lblRadiusValue
            // 
            lblRadiusValue.AutoSize = true;
            lblRadiusValue.Location = new Point(101, 40);
            lblRadiusValue.Margin = new Padding(3, 5, 3, 3);
            lblRadiusValue.Name = "lblRadiusValue";
            lblRadiusValue.Size = new Size(75, 15);
            lblRadiusValue.TabIndex = 108;
            lblRadiusValue.Text = "<radius> km";
            // 
            // tlblBodyMiningLocations
            // 
            tlblBodyMiningLocations.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlblBodyMiningLocations.AutoSize = true;
            tlblBodyMiningLocations.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblBodyMiningLocations.ImageColor = Color.Transparent;
            tlblBodyMiningLocations.LabelPosition = LabelPositionType.Right;
            tlblBodyMiningLocations.Location = new Point(354, 90);
            tlblBodyMiningLocations.Margin = new Padding(3, 5, 3, 3);
            tlblBodyMiningLocations.MinimumSize = new Size(0, 17);
            tlblBodyMiningLocations.Name = "tlblBodyMiningLocations";
            tlblBodyMiningLocations.Size = new Size(62, 17);
            tlblBodyMiningLocations.TabIndex = 109;
            tlblBodyMiningLocations.Text = "<mining>";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 6;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14F));
            tableLayoutPanel1.Controls.Add(cboBody, 0, 0);
            tableLayoutPanel1.Controls.Add(lblBodyDistance, 2, 5);
            tableLayoutPanel1.Controls.Add(label2, 0, 5);
            tableLayoutPanel1.Controls.Add(lblAtmosphereTypeValue, 2, 4);
            tableLayoutPanel1.Controls.Add(lblRadiusValue, 1, 1);
            tableLayoutPanel1.Controls.Add(tlblCoordinates, 2, 2);
            tableLayoutPanel1.Controls.Add(lblBodyPressure, 1, 4);
            tableLayoutPanel1.Controls.Add(label1, 0, 4);
            tableLayoutPanel1.Controls.Add(tlblBodyAttributes, 5, 0);
            tableLayoutPanel1.Controls.Add(label3, 0, 1);
            tableLayoutPanel1.Controls.Add(label18, 2, 1);
            tableLayoutPanel1.Controls.Add(tlblTempValue, 1, 3);
            tableLayoutPanel1.Controls.Add(tlblGravityValue, 1, 2);
            tableLayoutPanel1.Controls.Add(label29, 0, 2);
            tableLayoutPanel1.Controls.Add(label13, 0, 3);
            tableLayoutPanel1.Controls.Add(btnBodyCoordsCopy, 5, 2);
            tableLayoutPanel1.Controls.Add(lblBodyAltitude, 3, 1);
            tableLayoutPanel1.Controls.Add(tlblBodyMiningLocations, 4, 3);
            tableLayoutPanel1.Controls.Add(tlblBodyRings, 5, 3);
            tableLayoutPanel1.Controls.Add(tlblBioSignals, 2, 3);
            tableLayoutPanel1.Controls.Add(tlblGeoSignals, 3, 3);
            tableLayoutPanel1.Controls.Add(lblBodyType, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(490, 164);
            tableLayoutPanel1.TabIndex = 110;
            // 
            // PlanetContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "PlanetContent";
            Size = new Size(490, 164);
            Click += BodySignalsLabelClick;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ThemeableImageButton btnBodyCoordsCopy;
        private Label lblBodyPressure;
        private ScalableLabel lblBodyType;
        private Label label13;
        private Label label29;
        private Label lblBodyAltitude;
        private Label label18;
        private ThemeableImageLabel tlblBodyAttributes;
        private ThemeableImageLabel tlblGravityValue;
        private ThemeableImageLabel tlblTempValue;
        private ThemeableImageLabel tlblGeoSignals;
        private ThemeableImageLabel tlblBioSignals;
        private ThemeableImageLabel tlblBodyRings;
        private Label lblBodyDistance;
        private Label label2;
        private ThemeableImageLabel tlblCoordinates;
        private ComboBox cboBody;
        private Label label1;
        private Label lblAtmosphereTypeValue;
        private Label label3;
        private Label lblRadiusValue;
        private ThemeableImageLabel tlblBodyMiningLocations;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
