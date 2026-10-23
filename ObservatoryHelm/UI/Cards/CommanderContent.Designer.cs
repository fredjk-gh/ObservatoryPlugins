namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    partial class CommanderContent
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
            lblLastActive = new Label();
            label2 = new Label();
            cbCommander = new ComboBox();
            tbtnRealtimeLock = new com.github.fredjk_gh.PluginCommon.UI.ThemeableImageButton();
            lblUIMode = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblLastActive
            // 
            lblLastActive.Anchor = AnchorStyles.Left;
            lblLastActive.AutoSize = true;
            lblLastActive.Location = new Point(109, 37);
            lblLastActive.Name = "lblLastActive";
            lblLastActive.Size = new Size(110, 15);
            lblLastActive.TabIndex = 10;
            lblLastActive.Text = "2024-11-28 15:18:03";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(3, 37);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 9;
            label2.Text = "Last Active:";
            // 
            // cbCommander
            // 
            cbCommander.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.SetColumnSpan(cbCommander, 4);
            cbCommander.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCommander.FlatStyle = FlatStyle.Flat;
            cbCommander.FormattingEnabled = true;
            cbCommander.Location = new Point(3, 3);
            cbCommander.Name = "cbCommander";
            cbCommander.Size = new Size(448, 23);
            cbCommander.TabIndex = 8;
            // 
            // tbtnRealtimeLock
            // 
            tbtnRealtimeLock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tbtnRealtimeLock.FlatAppearance.BorderSize = 0;
            tbtnRealtimeLock.FlatStyle = FlatStyle.Flat;
            tbtnRealtimeLock.ImageSize = null;
            tbtnRealtimeLock.Location = new Point(426, 33);
            tbtnRealtimeLock.Name = "tbtnRealtimeLock";
            tbtnRealtimeLock.OriginalImage = null;
            tbtnRealtimeLock.Size = new Size(25, 24);
            tbtnRealtimeLock.TabIndex = 11;
            tbtnRealtimeLock.UseVisualStyleBackColor = true;
            // 
            // lblUIMode
            // 
            lblUIMode.Anchor = AnchorStyles.Right;
            lblUIMode.Location = new Point(304, 37);
            lblUIMode.Name = "lblUIMode";
            lblUIMode.Size = new Size(116, 16);
            lblUIMode.TabIndex = 12;
            lblUIMode.Text = "Mode: Realtime";
            lblUIMode.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.Controls.Add(cbCommander, 0, 0);
            tableLayoutPanel1.Controls.Add(tbtnRealtimeLock, 3, 1);
            tableLayoutPanel1.Controls.Add(lblUIMode, 2, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(lblLastActive, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(454, 60);
            tableLayoutPanel1.TabIndex = 13;
            // 
            // CommanderContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "CommanderContent";
            Size = new Size(454, 60);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblLastActive;
        private Label label2;
        private ComboBox cbCommander;
        private PluginCommon.UI.ThemeableImageButton tbtnRealtimeLock;
        private Label lblUIMode;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
