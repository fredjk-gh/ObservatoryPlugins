using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    partial class ShipContent
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
            lblJumpRange = new ThemeableImageLabel();
            lblShipType = new Label();
            pbFuel = new ThemeableProgressBar();
            lblShipIdentifier = new Label();
            label7 = new Label();
            lblShipName = new Label();
            label4 = new Label();
            ttManager = new ToolTip(components);
            tlblFuel = new ThemeableImageLabel();
            SuspendLayout();
            // 
            // lblJumpRange
            // 
            lblJumpRange.AutoSize = true;
            lblJumpRange.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            lblJumpRange.ImageColor = Color.Transparent;
            lblJumpRange.LabelPosition = LabelPositionType.Right;
            lblJumpRange.Location = new Point(344, 31);
            lblJumpRange.Name = "lblJumpRange";
            lblJumpRange.Size = new Size(60, 15);
            lblJumpRange.TabIndex = 72;
            lblJumpRange.Text = "X ly (max)";
            // 
            // lblShipType
            // 
            lblShipType.AutoSize = true;
            lblShipType.Location = new Point(168, 32);
            lblShipType.Name = "lblShipType";
            lblShipType.Size = new Size(73, 15);
            lblShipType.TabIndex = 71;
            lblShipType.Text = "<Ship Type>";
            // 
            // pbFuel
            // 
            pbFuel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbFuel.Location = new Point(168, 55);
            pbFuel.Maximum = 1;
            pbFuel.Name = "pbFuel";
            pbFuel.Size = new Size(296, 20);
            pbFuel.Step = 1;
            pbFuel.Style = ProgressBarStyle.Continuous;
            pbFuel.TabIndex = 68;
            // 
            // lblShipIdentifier
            // 
            lblShipIdentifier.AutoSize = true;
            lblShipIdentifier.Location = new Point(344, 8);
            lblShipIdentifier.Name = "lblShipIdentifier";
            lblShipIdentifier.Size = new Size(34, 15);
            lblShipIdentifier.TabIndex = 67;
            lblShipIdentifier.Text = "<ID>";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(8, 32);
            label7.Name = "label7";
            label7.Size = new Size(125, 15);
            label7.TabIndex = 66;
            label7.Text = "Ship Type / Max jump:";
            // 
            // lblShipName
            // 
            lblShipName.AutoSize = true;
            lblShipName.Location = new Point(168, 8);
            lblShipName.Name = "lblShipName";
            lblShipName.Size = new Size(81, 15);
            lblShipName.TabIndex = 65;
            lblShipName.Text = "<Ship Name>";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 8);
            label4.Name = "label4";
            label4.Size = new Size(90, 15);
            label4.TabIndex = 64;
            label4.Text = "Ship Name / ID:";
            // 
            // tlblFuel
            // 
            tlblFuel.AutoSize = true;
            tlblFuel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblFuel.ImageColor = Color.Transparent;
            tlblFuel.LabelPosition = LabelPositionType.Left;
            tlblFuel.Location = new Point(8, 56);
            tlblFuel.Name = "tlblFuel";
            tlblFuel.Size = new Size(32, 15);
            tlblFuel.TabIndex = 73;
            tlblFuel.Text = "Fuel:";
            // 
            // ShipContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tlblFuel);
            Controls.Add(lblJumpRange);
            Controls.Add(lblShipType);
            Controls.Add(pbFuel);
            Controls.Add(lblShipIdentifier);
            Controls.Add(label7);
            Controls.Add(lblShipName);
            Controls.Add(label4);
            Name = "ShipContent";
            Size = new Size(472, 87);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ThemeableImageLabel lblJumpRange;
        private Label lblShipType;
        private ThemeableProgressBar pbFuel;
        private Label lblShipIdentifier;
        private Label label7;
        private Label lblShipName;
        private Label label4;
        private ToolTip ttManager;
        private ThemeableImageLabel tlblFuel;
    }
}
