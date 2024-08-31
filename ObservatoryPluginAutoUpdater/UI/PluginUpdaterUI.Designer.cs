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
            tblLayout = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnCheckForUpdates = new Button();
            llblGitHubWiki = new LinkLabel();
            lblMessages = new Label();
            dgvPlugins = new DataGridView();
            chkAllowBeta = new CheckBox();
            tblLayout.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPlugins).BeginInit();
            SuspendLayout();
            // 
            // txtMessages
            // 
            txtMessages.Dock = DockStyle.Fill;
            txtMessages.Location = new Point(305, 406);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ReadOnly = true;
            txtMessages.ScrollBars = ScrollBars.Vertical;
            txtMessages.Size = new Size(601, 94);
            txtMessages.TabIndex = 26;
            // 
            // tblLayout
            // 
            tblLayout.AutoSize = true;
            tblLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tblLayout.ColumnCount = 2;
            tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
            tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tblLayout.Controls.Add(txtMessages, 1, 2);
            tblLayout.Controls.Add(flowLayoutPanel1, 0, 2);
            tblLayout.Controls.Add(lblMessages, 1, 1);
            tblLayout.Controls.Add(dgvPlugins, 0, 0);
            tblLayout.Controls.Add(chkAllowBeta, 0, 1);
            tblLayout.Dock = DockStyle.Fill;
            tblLayout.Location = new Point(0, 0);
            tblLayout.Name = "tblLayout";
            tblLayout.RowCount = 3;
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tblLayout.Size = new Size(910, 504);
            tblLayout.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnCheckForUpdates);
            flowLayoutPanel1.Controls.Add(llblGitHubWiki);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(4, 406);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(294, 94);
            flowLayoutPanel1.TabIndex = 27;
            // 
            // btnCheckForUpdates
            // 
            btnCheckForUpdates.AutoSize = true;
            btnCheckForUpdates.FlatAppearance.BorderSize = 0;
            btnCheckForUpdates.FlatStyle = FlatStyle.Flat;
            btnCheckForUpdates.Location = new Point(3, 3);
            btnCheckForUpdates.Name = "btnCheckForUpdates";
            btnCheckForUpdates.Size = new Size(113, 25);
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
            lblMessages.AutoSize = true;
            lblMessages.Location = new Point(305, 380);
            lblMessages.Margin = new Padding(3);
            lblMessages.Name = "lblMessages";
            lblMessages.Size = new Size(61, 15);
            lblMessages.TabIndex = 28;
            lblMessages.Text = "Messages:";
            // 
            // dgvPlugins
            // 
            dgvPlugins.AllowUserToAddRows = false;
            dgvPlugins.AllowUserToDeleteRows = false;
            dgvPlugins.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tblLayout.SetColumnSpan(dgvPlugins, 2);
            dgvPlugins.Dock = DockStyle.Fill;
            dgvPlugins.Location = new Point(4, 4);
            dgvPlugins.Name = "dgvPlugins";
            dgvPlugins.ReadOnly = true;
            dgvPlugins.Size = new Size(902, 369);
            dgvPlugins.TabIndex = 29;
            // 
            // chkAllowBeta
            // 
            chkAllowBeta.AutoSize = true;
            chkAllowBeta.Location = new Point(4, 380);
            chkAllowBeta.Name = "chkAllowBeta";
            chkAllowBeta.Size = new Size(160, 19);
            chkAllowBeta.TabIndex = 30;
            chkAllowBeta.Text = "Allow using Beta versions";
            chkAllowBeta.UseVisualStyleBackColor = true;
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
            ((System.ComponentModel.ISupportInitialize)dgvPlugins).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMessages;
        private TableLayoutPanel tblLayout;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnCheckForUpdates;
        private LinkLabel llblGitHubWiki;
        private Label lblMessages;
        private DataGridView dgvPlugins;
        private CheckBox chkAllowBeta;
    }
}
