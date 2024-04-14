namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    partial class ExpanderTile
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
            tlpLayout = new TableLayoutPanel();
            lblTileTitle = new Label();
            btnToggle = new Button();
            tlpLayout.SuspendLayout();
            SuspendLayout();
            // 
            // tlpLayout
            // 
            tlpLayout.AutoSize = true;
            tlpLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlpLayout.ColumnCount = 2;
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tlpLayout.ColumnStyles.Add(new ColumnStyle());
            tlpLayout.Controls.Add(lblTileTitle, 1, 0);
            tlpLayout.Controls.Add(btnToggle, 0, 0);
            tlpLayout.Location = new Point(0, 0);
            tlpLayout.Name = "tlpLayout";
            tlpLayout.RowCount = 2;
            tlpLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.Size = new Size(125, 40);
            tlpLayout.TabIndex = 0;
            // 
            // lblTileTitle
            // 
            lblTileTitle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblTileTitle.AutoSize = true;
            lblTileTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            lblTileTitle.Location = new Point(43, 0);
            lblTileTitle.Name = "lblTileTitle";
            lblTileTitle.Size = new Size(79, 40);
            lblTileTitle.TabIndex = 1;
            lblTileTitle.Text = "<title>";
            lblTileTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTileTitle.DoubleClick += lblTileTitle_DoubleClick;
            // 
            // btnToggle
            // 
            btnToggle.Dock = DockStyle.Fill;
            btnToggle.FlatStyle = FlatStyle.Flat;
            btnToggle.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnToggle.Location = new Point(5, 5);
            btnToggle.Margin = new Padding(5);
            btnToggle.Name = "btnToggle";
            btnToggle.Size = new Size(30, 30);
            btnToggle.TabIndex = 2;
            btnToggle.Text = "➕";
            btnToggle.UseVisualStyleBackColor = true;
            btnToggle.Click += btnToggle_Click;
            // 
            // ExpanderTile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(tlpLayout);
            Name = "ExpanderTile";
            Size = new Size(128, 43);
            tlpLayout.ResumeLayout(false);
            tlpLayout.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tlpLayout;
        private Label lblTileTitle;
        private Button btnToggle;
    }
}
