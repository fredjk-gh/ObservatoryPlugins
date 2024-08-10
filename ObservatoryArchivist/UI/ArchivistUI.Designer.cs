namespace com.github.fredjk_gh.ObservatoryArchivist.UI
{
    partial class ArchivistUI
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
            tabPanels = new TabControl();
            tabCurrentSystem = new TabPage();
            tableLayoutPanel2 = new TableLayoutPanel();
            lblSystemName = new Label();
            txtSystemName = new TextBox();
            btnOpenInSearch = new Button();
            lblMessages = new Label();
            txtMessages = new TextBox();
            lblLastRecord = new Label();
            txtLastEntry = new TextBox();
            lblRecordCommanderTitle = new Label();
            lblRecordCommanderValue = new Label();
            btnOpenInViewer = new Button();
            tabSearch = new TabPage();
            tableLayoutPanel3 = new TableLayoutPanel();
            cboCommanderFilter = new ComboBox();
            lblCommanderFilterTitle = new Label();
            lblFindSystem = new Label();
            txtFindSystem = new TextBox();
            lblFindMessages = new Label();
            lblFilter = new Label();
            txtFilter = new TextBox();
            lbJournals = new ListBox();
            ctxResultsMenu = new ContextMenuStrip(components);
            copyToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            btnResendAll = new Button();
            tabPanels.SuspendLayout();
            tabCurrentSystem.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tabSearch.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ctxResultsMenu.SuspendLayout();
            SuspendLayout();
            // 
            // tabPanels
            // 
            tabPanels.Controls.Add(tabCurrentSystem);
            tabPanels.Controls.Add(tabSearch);
            tabPanels.Dock = DockStyle.Fill;
            tabPanels.Location = new Point(0, 0);
            tabPanels.Name = "tabPanels";
            tabPanels.SelectedIndex = 0;
            tabPanels.Size = new Size(612, 545);
            tabPanels.TabIndex = 2;
            // 
            // tabCurrentSystem
            // 
            tabCurrentSystem.Controls.Add(tableLayoutPanel2);
            tabCurrentSystem.Location = new Point(4, 24);
            tabCurrentSystem.Name = "tabCurrentSystem";
            tabCurrentSystem.Padding = new Padding(3);
            tabCurrentSystem.Size = new Size(604, 517);
            tabCurrentSystem.TabIndex = 0;
            tabCurrentSystem.Text = "Current System";
            tabCurrentSystem.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel2.Controls.Add(lblSystemName, 0, 0);
            tableLayoutPanel2.Controls.Add(txtSystemName, 1, 0);
            tableLayoutPanel2.Controls.Add(btnOpenInSearch, 2, 0);
            tableLayoutPanel2.Controls.Add(lblMessages, 0, 2);
            tableLayoutPanel2.Controls.Add(txtMessages, 1, 2);
            tableLayoutPanel2.Controls.Add(lblLastRecord, 0, 3);
            tableLayoutPanel2.Controls.Add(txtLastEntry, 0, 4);
            tableLayoutPanel2.Controls.Add(lblRecordCommanderTitle, 0, 1);
            tableLayoutPanel2.Controls.Add(lblRecordCommanderValue, 1, 1);
            tableLayoutPanel2.Controls.Add(btnOpenInViewer, 1, 3);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 5;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66666F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 83.3333359F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Size = new Size(598, 511);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // lblSystemName
            // 
            lblSystemName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblSystemName.AutoSize = true;
            lblSystemName.Location = new Point(3, 0);
            lblSystemName.Name = "lblSystemName";
            lblSystemName.Size = new Size(83, 33);
            lblSystemName.TabIndex = 0;
            lblSystemName.Text = "System Name:";
            lblSystemName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtSystemName
            // 
            txtSystemName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSystemName.Location = new Point(153, 6);
            txtSystemName.Margin = new Padding(3, 6, 3, 3);
            txtSystemName.Name = "txtSystemName";
            txtSystemName.PlaceholderText = "(unknown)";
            txtSystemName.ReadOnly = true;
            txtSystemName.Size = new Size(242, 23);
            txtSystemName.TabIndex = 1;
            // 
            // btnOpenInSearch
            // 
            btnOpenInSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenInSearch.AutoSize = true;
            btnOpenInSearch.FlatAppearance.BorderSize = 0;
            btnOpenInSearch.FlatStyle = FlatStyle.Flat;
            btnOpenInSearch.Location = new Point(496, 3);
            btnOpenInSearch.Name = "btnOpenInSearch";
            btnOpenInSearch.Size = new Size(99, 27);
            btnOpenInSearch.TabIndex = 2;
            btnOpenInSearch.Text = "Open in Search";
            btnOpenInSearch.UseVisualStyleBackColor = true;
            btnOpenInSearch.Click += btnOpenInSearch_Click;
            // 
            // lblMessages
            // 
            lblMessages.AutoSize = true;
            lblMessages.Location = new Point(3, 69);
            lblMessages.Margin = new Padding(3);
            lblMessages.Name = "lblMessages";
            lblMessages.Size = new Size(61, 15);
            lblMessages.TabIndex = 3;
            lblMessages.Text = "Messages:";
            // 
            // txtMessages
            // 
            tableLayoutPanel2.SetColumnSpan(txtMessages, 2);
            txtMessages.Dock = DockStyle.Fill;
            txtMessages.Location = new Point(153, 69);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ReadOnly = true;
            txtMessages.Size = new Size(442, 62);
            txtMessages.TabIndex = 3;
            // 
            // lblLastRecord
            // 
            lblLastRecord.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblLastRecord.AutoSize = true;
            lblLastRecord.Location = new Point(3, 149);
            lblLastRecord.Margin = new Padding(3);
            lblLastRecord.Name = "lblLastRecord";
            lblLastRecord.Size = new Size(115, 15);
            lblLastRecord.TabIndex = 5;
            lblLastRecord.Text = "Latest Saved Record:";
            // 
            // txtLastEntry
            // 
            tableLayoutPanel2.SetColumnSpan(txtLastEntry, 3);
            txtLastEntry.Dock = DockStyle.Fill;
            txtLastEntry.Location = new Point(3, 170);
            txtLastEntry.Multiline = true;
            txtLastEntry.Name = "txtLastEntry";
            txtLastEntry.ReadOnly = true;
            txtLastEntry.ScrollBars = ScrollBars.Vertical;
            txtLastEntry.Size = new Size(592, 338);
            txtLastEntry.TabIndex = 5;
            // 
            // lblRecordCommanderTitle
            // 
            lblRecordCommanderTitle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblRecordCommanderTitle.AutoSize = true;
            lblRecordCommanderTitle.Location = new Point(3, 33);
            lblRecordCommanderTitle.Name = "lblRecordCommanderTitle";
            lblRecordCommanderTitle.Size = new Size(77, 33);
            lblRecordCommanderTitle.TabIndex = 7;
            lblRecordCommanderTitle.Text = "Commander:";
            lblRecordCommanderTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblRecordCommanderValue
            // 
            lblRecordCommanderValue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblRecordCommanderValue.AutoSize = true;
            lblRecordCommanderValue.Location = new Point(153, 33);
            lblRecordCommanderValue.Name = "lblRecordCommanderValue";
            lblRecordCommanderValue.Size = new Size(0, 33);
            lblRecordCommanderValue.TabIndex = 8;
            lblRecordCommanderValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnOpenInViewer
            // 
            btnOpenInViewer.AutoSize = true;
            btnOpenInViewer.FlatAppearance.BorderSize = 0;
            btnOpenInViewer.FlatStyle = FlatStyle.Flat;
            btnOpenInViewer.Location = new Point(153, 137);
            btnOpenInViewer.Name = "btnOpenInViewer";
            btnOpenInViewer.Size = new Size(101, 25);
            btnOpenInViewer.TabIndex = 4;
            btnOpenInViewer.Text = "Open in Viewer";
            btnOpenInViewer.UseVisualStyleBackColor = true;
            btnOpenInViewer.Click += btnOpenInViewer_Click;
            // 
            // tabSearch
            // 
            tabSearch.Controls.Add(tableLayoutPanel3);
            tabSearch.Location = new Point(4, 24);
            tabSearch.Name = "tabSearch";
            tabSearch.Padding = new Padding(3);
            tabSearch.Size = new Size(604, 517);
            tabSearch.TabIndex = 1;
            tabSearch.Text = "Search";
            tabSearch.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.Controls.Add(cboCommanderFilter, 1, 2);
            tableLayoutPanel3.Controls.Add(lblCommanderFilterTitle, 0, 2);
            tableLayoutPanel3.Controls.Add(lblFindSystem, 0, 0);
            tableLayoutPanel3.Controls.Add(txtFindSystem, 1, 0);
            tableLayoutPanel3.Controls.Add(lblFindMessages, 2, 0);
            tableLayoutPanel3.Controls.Add(lblFilter, 0, 1);
            tableLayoutPanel3.Controls.Add(txtFilter, 1, 1);
            tableLayoutPanel3.Controls.Add(lbJournals, 0, 3);
            tableLayoutPanel3.Controls.Add(btnResendAll, 0, 4);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 5;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel3.Size = new Size(598, 511);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // cboCommanderFilter
            // 
            cboCommanderFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboCommanderFilter.Font = new Font("Segoe UI", 11F);
            cboCommanderFilter.FormattingEnabled = true;
            cboCommanderFilter.Location = new Point(152, 69);
            cboCommanderFilter.MaximumSize = new Size(500, 0);
            cboCommanderFilter.Name = "cboCommanderFilter";
            cboCommanderFilter.Size = new Size(293, 28);
            cboCommanderFilter.TabIndex = 3;
            cboCommanderFilter.SelectedIndexChanged += cboCommanderFilter_SelectedIndexChanged;
            // 
            // lblCommanderFilterTitle
            // 
            lblCommanderFilterTitle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblCommanderFilterTitle.AutoSize = true;
            lblCommanderFilterTitle.Font = new Font("Segoe UI", 9F);
            lblCommanderFilterTitle.Location = new Point(40, 66);
            lblCommanderFilterTitle.Name = "lblCommanderFilterTitle";
            lblCommanderFilterTitle.Size = new Size(106, 33);
            lblCommanderFilterTitle.TabIndex = 0;
            lblCommanderFilterTitle.Text = "Commander Filter:";
            lblCommanderFilterTitle.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblFindSystem
            // 
            lblFindSystem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblFindSystem.AutoSize = true;
            lblFindSystem.Location = new Point(72, 0);
            lblFindSystem.Name = "lblFindSystem";
            lblFindSystem.Size = new Size(74, 33);
            lblFindSystem.TabIndex = 0;
            lblFindSystem.Text = "Find System:";
            lblFindSystem.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtFindSystem
            // 
            txtFindSystem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFindSystem.Location = new Point(152, 6);
            txtFindSystem.Margin = new Padding(3, 6, 3, 3);
            txtFindSystem.Name = "txtFindSystem";
            txtFindSystem.PlaceholderText = "Hit Enter to search";
            txtFindSystem.Size = new Size(293, 23);
            txtFindSystem.TabIndex = 1;
            txtFindSystem.KeyPress += txtFindSystem_KeyPress;
            // 
            // lblFindMessages
            // 
            lblFindMessages.AutoSize = true;
            lblFindMessages.Dock = DockStyle.Fill;
            lblFindMessages.Location = new Point(451, 3);
            lblFindMessages.Margin = new Padding(3);
            lblFindMessages.Name = "lblFindMessages";
            tableLayoutPanel3.SetRowSpan(lblFindMessages, 3);
            lblFindMessages.Size = new Size(144, 93);
            lblFindMessages.TabIndex = 2;
            lblFindMessages.Text = "Nothing found";
            // 
            // lblFilter
            // 
            lblFilter.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblFilter.AutoSize = true;
            lblFilter.Location = new Point(86, 33);
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(60, 33);
            lblFilter.TabIndex = 3;
            lblFilter.Text = "Text Filter:";
            lblFilter.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtFilter
            // 
            txtFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilter.Location = new Point(152, 39);
            txtFilter.Margin = new Padding(3, 6, 3, 3);
            txtFilter.Name = "txtFilter";
            txtFilter.PlaceholderText = "Type to filter";
            txtFilter.Size = new Size(293, 23);
            txtFilter.TabIndex = 2;
            txtFilter.TextChanged += txtFilter_TextChanged;
            // 
            // lbJournals
            // 
            tableLayoutPanel3.SetColumnSpan(lbJournals, 3);
            lbJournals.ContextMenuStrip = ctxResultsMenu;
            lbJournals.Dock = DockStyle.Fill;
            lbJournals.FormattingEnabled = true;
            lbJournals.ItemHeight = 15;
            lbJournals.Items.AddRange(new object[] { "{\"timestamp\":\"2024-04-11T19:39:33\" \"event\":\"FSDJump\" ... }" });
            lbJournals.Location = new Point(3, 102);
            lbJournals.Name = "lbJournals";
            lbJournals.Size = new Size(592, 373);
            lbJournals.TabIndex = 5;
            lbJournals.MouseDown += lbJournals_MouseDown;
            // 
            // ctxResultsMenu
            // 
            ctxResultsMenu.Items.AddRange(new ToolStripItem[] { copyToolStripMenuItem, viewToolStripMenuItem });
            ctxResultsMenu.Name = "ctxResultsMenu";
            ctxResultsMenu.Size = new Size(109, 48);
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new Size(108, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(108, 22);
            viewToolStripMenuItem.Text = "View...";
            viewToolStripMenuItem.Click += viewToolStripMenuItem_Click;
            // 
            // btnResendAll
            // 
            btnResendAll.AutoSize = true;
            btnResendAll.FlatAppearance.BorderSize = 0;
            btnResendAll.FlatStyle = FlatStyle.Flat;
            btnResendAll.Location = new Point(3, 481);
            btnResendAll.Name = "btnResendAll";
            btnResendAll.Size = new Size(75, 27);
            btnResendAll.TabIndex = 6;
            btnResendAll.Text = "Resend All";
            btnResendAll.UseVisualStyleBackColor = true;
            btnResendAll.Click += btnResendAll_Click;
            // 
            // ArchivistUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabPanels);
            DoubleBuffered = true;
            Name = "ArchivistUI";
            Size = new Size(612, 545);
            tabPanels.ResumeLayout(false);
            tabCurrentSystem.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tabSearch.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ctxResultsMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Label lblCommanderFilterTitle;
        private ComboBox cboCommanderFilter;
        private TabControl tabPanels;
        private TabPage tabCurrentSystem;
        private TabPage tabSearch;
        private TableLayoutPanel tableLayoutPanel2;
        private Label lblSystemName;
        private TextBox txtSystemName;
        private Button btnOpenInSearch;
        private Label lblMessages;
        private TextBox txtMessages;
        private Label lblLastRecord;
        private TextBox txtLastEntry;
        private ContextMenuStrip ctxResultsMenu;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel3;
        private Label lblFindSystem;
        private TextBox txtFindSystem;
        private Label lblFindMessages;
        private Label lblFilter;
        private TextBox txtFilter;
        private ListBox lbJournals;
        private Button btnResendAll;
        private Label lblRecordCommanderTitle;
        private Label lblRecordCommanderValue;
        private Button btnOpenInViewer;
    }
}
