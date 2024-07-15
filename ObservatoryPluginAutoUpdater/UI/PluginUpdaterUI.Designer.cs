namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI
{
    partial class PluginUpdaterUI
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
            txtMessages = new TextBox();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            label25 = new Label();
            tblLayout = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnCheckForUpdates = new Button();
            llblGitHubWiki = new LinkLabel();
            lblMessages = new Label();
            tblLayout.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // txtMessages
            // 
            tblLayout.SetColumnSpan(txtMessages, 3);
            txtMessages.Dock = DockStyle.Fill;
            txtMessages.Location = new Point(305, 328);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ReadOnly = true;
            txtMessages.ScrollBars = ScrollBars.Vertical;
            txtMessages.Size = new Size(581, 172);
            txtMessages.TabIndex = 26;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(506, 1);
            label3.Name = "label3";
            label3.Size = new Size(57, 21);
            label3.TabIndex = 2;
            label3.Text = "Status";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(305, 1);
            label2.Name = "label2";
            label2.Size = new Size(136, 21);
            label2.TabIndex = 1;
            label2.Text = "Installed version";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(4, 1);
            label1.Name = "label1";
            label1.Size = new Size(110, 21);
            label1.TabIndex = 0;
            label1.Text = "Plugin Name";
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label25.Location = new Point(707, 1);
            label25.Name = "label25";
            label25.Size = new Size(67, 21);
            label25.TabIndex = 24;
            label25.Text = "Actions";
            // 
            // tblLayout
            // 
            tblLayout.AutoSize = true;
            tblLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tblLayout.ColumnCount = 4;
            tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
            tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            tblLayout.Controls.Add(label25, 3, 0);
            tblLayout.Controls.Add(label1, 0, 0);
            tblLayout.Controls.Add(label2, 1, 0);
            tblLayout.Controls.Add(label3, 2, 0);
            tblLayout.Controls.Add(txtMessages, 1, 9);
            tblLayout.Controls.Add(flowLayoutPanel1, 0, 9);
            tblLayout.Controls.Add(lblMessages, 1, 8);
            tblLayout.Location = new Point(0, 0);
            tblLayout.Name = "tblLayout";
            tblLayout.RowCount = 10;
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tblLayout.Size = new Size(890, 504);
            tblLayout.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnCheckForUpdates);
            flowLayoutPanel1.Controls.Add(llblGitHubWiki);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(4, 328);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(294, 172);
            flowLayoutPanel1.TabIndex = 27;
            // 
            // btnCheckForUpdates
            // 
            btnCheckForUpdates.AutoSize = true;
            btnCheckForUpdates.FlatAppearance.BorderSize = 0;
            btnCheckForUpdates.FlatStyle = FlatStyle.Flat;
            btnCheckForUpdates.Location = new Point(3, 3);
            btnCheckForUpdates.Name = "btnCheckForUpdates";
            btnCheckForUpdates.Size = new Size(145, 25);
            btnCheckForUpdates.TabIndex = 1;
            btnCheckForUpdates.Text = "Check for updates";
            btnCheckForUpdates.UseVisualStyleBackColor = true;
            btnCheckForUpdates.Click += CheckForUpdates_Click;
            // 
            // llblGitHubWiki
            // 
            llblGitHubWiki.AutoSize = true;
            llblGitHubWiki.Location = new Point(3, 31);
            llblGitHubWiki.Name = "llblGitHubWiki";
            llblGitHubWiki.Size = new Size(103, 15);
            llblGitHubWiki.TabIndex = 0;
            llblGitHubWiki.TabStop = true;
            llblGitHubWiki.Text = "Open GitHub Wiki";
            llblGitHubWiki.LinkClicked += llblGitHubWiki_LinkClicked;
            // 
            // lblMessages
            // 
            lblMessages.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblMessages.AutoSize = true;
            lblMessages.Location = new Point(305, 309);
            lblMessages.Name = "lblMessages";
            lblMessages.Size = new Size(61, 15);
            lblMessages.TabIndex = 28;
            lblMessages.Text = "Messages:";
            // 
            // PluginUpdaterUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tblLayout);
            DoubleBuffered = true;
            Name = "PluginUpdaterUI";
            Size = new Size(910, 504);
            tblLayout.ResumeLayout(false);
            tblLayout.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMessages;
        private TableLayoutPanel tblLayout;
        private Label label25;
        private Label label1;
        private Label label2;
        private Label label3;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnCheckForUpdates;
        private LinkLabel llblGitHubWiki;
        private Label lblMessages;
    }
}
