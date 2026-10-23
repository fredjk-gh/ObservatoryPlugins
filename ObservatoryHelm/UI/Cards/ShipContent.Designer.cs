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
            lblJumpRange = new ThemeableImageLabel();
            lblShipType = new Label();
            pbFuel = new ThemeableProgressBar();
            lblShipIdentifier = new Label();
            label7 = new Label();
            lblShipName = new Label();
            label4 = new Label();
            tlblFuel = new ThemeableImageLabel();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblJumpRange
            // 
            lblJumpRange.Anchor = AnchorStyles.Left;
            lblJumpRange.AutoSize = true;
            lblJumpRange.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            lblJumpRange.ImageColor = Color.Transparent;
            lblJumpRange.LabelPosition = LabelPositionType.Right;
            lblJumpRange.Location = new Point(313, 30);
            lblJumpRange.Name = "lblJumpRange";
            lblJumpRange.Size = new Size(60, 15);
            lblJumpRange.TabIndex = 72;
            lblJumpRange.Text = "X ly (max)";
            // 
            // lblShipType
            // 
            lblShipType.Anchor = AnchorStyles.Left;
            lblShipType.AutoSize = true;
            lblShipType.Location = new Point(158, 30);
            lblShipType.Name = "lblShipType";
            lblShipType.Size = new Size(73, 15);
            lblShipType.TabIndex = 71;
            lblShipType.Text = "<Ship Type>";
            // 
            // pbFuel
            // 
            pbFuel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.SetColumnSpan(pbFuel, 2);
            pbFuel.Location = new Point(158, 55);
            pbFuel.Maximum = 1;
            pbFuel.Name = "pbFuel";
            pbFuel.Size = new Size(311, 20);
            pbFuel.Step = 1;
            pbFuel.Style = ProgressBarStyle.Continuous;
            pbFuel.TabIndex = 68;
            // 
            // lblShipIdentifier
            // 
            lblShipIdentifier.Anchor = AnchorStyles.Left;
            lblShipIdentifier.AutoSize = true;
            lblShipIdentifier.Location = new Point(313, 5);
            lblShipIdentifier.Name = "lblShipIdentifier";
            lblShipIdentifier.Size = new Size(34, 15);
            lblShipIdentifier.TabIndex = 67;
            lblShipIdentifier.Text = "<ID>";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Location = new Point(3, 30);
            label7.Name = "label7";
            label7.Size = new Size(125, 15);
            label7.TabIndex = 66;
            label7.Text = "Ship Type / Max jump:";
            // 
            // lblShipName
            // 
            lblShipName.Anchor = AnchorStyles.Left;
            lblShipName.AutoSize = true;
            lblShipName.Location = new Point(158, 5);
            lblShipName.Name = "lblShipName";
            lblShipName.Size = new Size(81, 15);
            lblShipName.TabIndex = 65;
            lblShipName.Text = "<Ship Name>";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(3, 5);
            label4.Name = "label4";
            label4.Size = new Size(90, 15);
            label4.TabIndex = 64;
            label4.Text = "Ship Name / ID:";
            // 
            // tlblFuel
            // 
            tlblFuel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tlblFuel.AutoSize = true;
            tlblFuel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblFuel.ImageColor = Color.Transparent;
            tlblFuel.LabelPosition = LabelPositionType.Left;
            tlblFuel.Location = new Point(3, 57);
            tlblFuel.Name = "tlblFuel";
            tlblFuel.Size = new Size(149, 15);
            tlblFuel.TabIndex = 73;
            tlblFuel.Text = "Fuel:";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            tableLayoutPanel1.Controls.Add(label4, 0, 0);
            tableLayoutPanel1.Controls.Add(pbFuel, 1, 2);
            tableLayoutPanel1.Controls.Add(tlblFuel, 0, 2);
            tableLayoutPanel1.Controls.Add(lblShipName, 1, 0);
            tableLayoutPanel1.Controls.Add(lblJumpRange, 2, 1);
            tableLayoutPanel1.Controls.Add(lblShipIdentifier, 2, 0);
            tableLayoutPanel1.Controls.Add(lblShipType, 1, 1);
            tableLayoutPanel1.Controls.Add(label7, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(472, 82);
            tableLayoutPanel1.TabIndex = 74;
            // 
            // ShipContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "ShipContent";
            Size = new Size(472, 82);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ThemeableImageLabel lblJumpRange;
        private Label lblShipType;
        private ThemeableProgressBar pbFuel;
        private Label lblShipIdentifier;
        private Label label7;
        private Label lblShipName;
        private Label label4;
        private ThemeableImageLabel tlblFuel;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
