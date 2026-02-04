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
            components = new System.ComponentModel.Container();
            btnBodyCoordsCopy = new ThemeableImageButton();
            lblBodyPressure = new Label();
            lblBodyType = new ScalableLabel();
            label13 = new Label();
            label29 = new Label();
            lblBodyAltitude = new Label();
            label18 = new Label();
            ttManager = new ToolTip(components);
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
            SuspendLayout();
            // 
            // btnBodyCoordsCopy
            // 
            btnBodyCoordsCopy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBodyCoordsCopy.AutoSize = true;
            btnBodyCoordsCopy.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnBodyCoordsCopy.FlatAppearance.BorderSize = 0;
            btnBodyCoordsCopy.FlatStyle = FlatStyle.Flat;
            btnBodyCoordsCopy.Font = new Font("Material Symbols Rounded", 10F);
            btnBodyCoordsCopy.ImageSize = null;
            btnBodyCoordsCopy.Location = new Point(479, 60);
            btnBodyCoordsCopy.Margin = new Padding(0);
            btnBodyCoordsCopy.Name = "btnBodyCoordsCopy";
            btnBodyCoordsCopy.OriginalImage = null;
            btnBodyCoordsCopy.Size = new Size(6, 6);
            btnBodyCoordsCopy.TabIndex = 94;
            ttManager.SetToolTip(btnBodyCoordsCopy, "Copy coordinates to the system clipboard");
            btnBodyCoordsCopy.UseVisualStyleBackColor = true;
            // 
            // lblBodyPressure
            // 
            lblBodyPressure.AutoSize = true;
            lblBodyPressure.Location = new Point(112, 112);
            lblBodyPressure.Name = "lblBodyPressure";
            lblBodyPressure.Size = new Size(91, 15);
            lblBodyPressure.TabIndex = 86;
            lblBodyPressure.Text = "<pressure> atm";
            // 
            // lblBodyType
            // 
            lblBodyType.AutoSize = true;
            lblBodyType.Font = new Font("Segoe UI", 12F);
            lblBodyType.FontSizeAdjustment = 3F;
            lblBodyType.Location = new Point(216, 8);
            lblBodyType.Name = "lblBodyType";
            lblBodyType.Size = new Size(101, 21);
            lblBodyType.TabIndex = 85;
            lblBodyType.Text = "<body type>";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(8, 88);
            label13.Name = "label13";
            label13.Size = new Size(76, 15);
            label13.TabIndex = 84;
            label13.Text = "Temperature:";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(8, 64);
            label29.Name = "label29";
            label29.Size = new Size(47, 15);
            label29.TabIndex = 81;
            label29.Text = "Gravity:";
            // 
            // lblBodyAltitude
            // 
            lblBodyAltitude.AutoSize = true;
            lblBodyAltitude.Location = new Point(304, 40);
            lblBodyAltitude.Name = "lblBodyAltitude";
            lblBodyAltitude.Size = new Size(83, 15);
            lblBodyAltitude.TabIndex = 76;
            lblBodyAltitude.Text = "<altitude> km";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(216, 40);
            label18.Name = "label18";
            label18.Size = new Size(52, 15);
            label18.TabIndex = 75;
            label18.Text = "Altitude:";
            // 
            // tlblBodyAttributes
            // 
            tlblBodyAttributes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tlblBodyAttributes.AutoSize = true;
            tlblBodyAttributes.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblBodyAttributes.ImageColor = Color.Transparent;
            tlblBodyAttributes.LabelPosition = LabelPositionType.Right;
            tlblBodyAttributes.Location = new Point(432, 8);
            tlblBodyAttributes.Name = "tlblBodyAttributes";
            tlblBodyAttributes.Size = new Size(46, 15);
            tlblBodyAttributes.TabIndex = 3;
            tlblBodyAttributes.Text = "<attrs>";
            // 
            // tlblGravityValue
            // 
            tlblGravityValue.AutoSize = true;
            tlblGravityValue.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblGravityValue.ImageColor = Color.Transparent;
            tlblGravityValue.LabelPosition = LabelPositionType.Left;
            tlblGravityValue.Location = new Point(112, 64);
            tlblGravityValue.Name = "tlblGravityValue";
            tlblGravityValue.Size = new Size(69, 15);
            tlblGravityValue.TabIndex = 96;
            tlblGravityValue.Text = "<gravity> g";
            // 
            // tlblTempValue
            // 
            tlblTempValue.AutoSize = true;
            tlblTempValue.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblTempValue.ImageColor = Color.Transparent;
            tlblTempValue.LabelPosition = LabelPositionType.Left;
            tlblTempValue.Location = new Point(112, 88);
            tlblTempValue.Name = "tlblTempValue";
            tlblTempValue.Size = new Size(61, 15);
            tlblTempValue.TabIndex = 97;
            tlblTempValue.Text = "<temp> K";
            // 
            // tlblGeoSignals
            // 
            tlblGeoSignals.AutoSize = true;
            tlblGeoSignals.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblGeoSignals.ImageColor = Color.Transparent;
            tlblGeoSignals.LabelPosition = LabelPositionType.Right;
            tlblGeoSignals.Location = new Point(216, 88);
            tlblGeoSignals.Name = "tlblGeoSignals";
            tlblGeoSignals.Size = new Size(48, 15);
            tlblGeoSignals.TabIndex = 98;
            tlblGeoSignals.Text = "<geos>";
            tlblGeoSignals.Click += BodySignalsLabelClick;
            // 
            // tlblBioSignals
            // 
            tlblBioSignals.AutoSize = true;
            tlblBioSignals.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblBioSignals.ImageColor = Color.Transparent;
            tlblBioSignals.LabelPosition = LabelPositionType.Right;
            tlblBioSignals.Location = new Point(304, 88);
            tlblBioSignals.Name = "tlblBioSignals";
            tlblBioSignals.Size = new Size(45, 15);
            tlblBioSignals.TabIndex = 99;
            tlblBioSignals.Text = "<bios>";
            tlblBioSignals.Click += BodySignalsLabelClick;
            // 
            // tlblBodyRings
            // 
            tlblBodyRings.AutoSize = true;
            tlblBodyRings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblBodyRings.ImageColor = Color.Transparent;
            tlblBodyRings.LabelPosition = LabelPositionType.Right;
            tlblBodyRings.Location = new Point(400, 88);
            tlblBodyRings.Name = "tlblBodyRings";
            tlblBodyRings.Size = new Size(49, 15);
            tlblBodyRings.TabIndex = 100;
            tlblBodyRings.Text = "<rings>";
            tlblBodyRings.Click += BodySignalsLabelClick;
            // 
            // lblBodyDistance
            // 
            lblBodyDistance.AutoSize = true;
            lblBodyDistance.Location = new Point(216, 136);
            lblBodyDistance.Name = "lblBodyDistance";
            lblBodyDistance.Size = new Size(56, 15);
            lblBodyDistance.TabIndex = 101;
            lblBodyDistance.Text = "<dist> Ls";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 136);
            label2.Name = "label2";
            label2.Size = new Size(119, 15);
            label2.TabIndex = 102;
            label2.Text = "Distance from arrival:";
            // 
            // tlblCoordinates
            // 
            tlblCoordinates.AutoSize = true;
            tlblCoordinates.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblCoordinates.ImageColor = Color.Transparent;
            tlblCoordinates.LabelPosition = LabelPositionType.Right;
            tlblCoordinates.Location = new Point(216, 64);
            tlblCoordinates.Name = "tlblCoordinates";
            tlblCoordinates.Size = new Size(41, 15);
            tlblCoordinates.TabIndex = 103;
            tlblCoordinates.Text = "<x, y>";
            // 
            // cboBody
            // 
            cboBody.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBody.FlatStyle = FlatStyle.Flat;
            cboBody.FormattingEnabled = true;
            cboBody.Location = new Point(8, 8);
            cboBody.Name = "cboBody";
            cboBody.Size = new Size(184, 23);
            cboBody.TabIndex = 104;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 112);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 106;
            label1.Text = "Atmosphere:";
            // 
            // lblAtmosphereTypeValue
            // 
            lblAtmosphereTypeValue.AutoSize = true;
            lblAtmosphereTypeValue.Location = new Point(216, 112);
            lblAtmosphereTypeValue.Name = "lblAtmosphereTypeValue";
            lblAtmosphereTypeValue.Size = new Size(112, 15);
            lblAtmosphereTypeValue.TabIndex = 105;
            lblAtmosphereTypeValue.Text = "<atmosphere type>";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 40);
            label3.Name = "label3";
            label3.Size = new Size(45, 15);
            label3.TabIndex = 107;
            label3.Text = "Radius:";
            // 
            // lblRadiusValue
            // 
            lblRadiusValue.AutoSize = true;
            lblRadiusValue.Location = new Point(112, 40);
            lblRadiusValue.Name = "lblRadiusValue";
            lblRadiusValue.Size = new Size(75, 15);
            lblRadiusValue.TabIndex = 108;
            lblRadiusValue.Text = "<radius> km";
            // 
            // PlanetContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblRadiusValue);
            Controls.Add(label3);
            Controls.Add(tlblBodyAttributes);
            Controls.Add(lblAtmosphereTypeValue);
            Controls.Add(cboBody);
            Controls.Add(tlblCoordinates);
            Controls.Add(label2);
            Controls.Add(lblBodyDistance);
            Controls.Add(tlblBodyRings);
            Controls.Add(tlblBioSignals);
            Controls.Add(tlblGeoSignals);
            Controls.Add(tlblTempValue);
            Controls.Add(tlblGravityValue);
            Controls.Add(btnBodyCoordsCopy);
            Controls.Add(label1);
            Controls.Add(lblBodyPressure);
            Controls.Add(lblBodyType);
            Controls.Add(label13);
            Controls.Add(label29);
            Controls.Add(lblBodyAltitude);
            Controls.Add(label18);
            Name = "PlanetContent";
            Size = new Size(490, 165);
            Click += BodySignalsLabelClick;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ThemeableImageButton btnBodyCoordsCopy;
        private Label lblBodyPressure;
        private ScalableLabel lblBodyType;
        private Label label13;
        private Label label29;
        private Label lblBodyAltitude;
        private Label label18;
        private ToolTip ttManager;
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
    }
}
