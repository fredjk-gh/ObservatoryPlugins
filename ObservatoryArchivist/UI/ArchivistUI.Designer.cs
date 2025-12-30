using com.github.fredjk_gh.PluginCommon.UI;

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
            lblMessages = new Label();
            txtMessages = new TextBox();
            lblLastRecord = new Label();
            txtLastEntry = new TextBox();
            lblRecordCommanderTitle = new Label();
            lblRecordCommanderValue = new Label();
            btnOpenInViewer = new ThemeableIconButton();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnOpenInSearch = new ThemeableIconButton();
            lblId64Details = new Label();
            txtId64Details = new TextBox();
            btnMessagesClear = new ThemeableImageButton();
            tabSearch = new TabPage();
            tableLayoutPanel3 = new TableLayoutPanel();
            btnFindMessagesClear = new ThemeableImageButton();
            cboCommanderFilter = new ComboBox();
            lblCommanderFilterTitle = new Label();
            lblFindSystem = new Label();
            txtFindMessages = new TextBox();
            lblFilter = new Label();
            txtFilter = new TextBox();
            lbJournals = new ListBox();
            ctxResultsMenu = new ContextMenuStrip(components);
            copyToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            lblRecentSystems = new Label();
            lbRecentSystems = new ListBox();
            flowLayoutPanel2 = new FlowLayoutPanel();
            btnSearchDB = new ThemeableIconButton();
            btnLoadFromSpansh = new ThemeableIconButton();
            cboFindSystem = new ComboBox();
            flowLayoutPanel3 = new FlowLayoutPanel();
            btnResendAll = new ThemeableIconButton();
            btnSendViaMsg = new ThemeableImageButton();
            btnCopy = new ThemeableIconButton();
            btnView = new ThemeableIconButton();
            ttipArchivistUI = new ToolTip(components);
            autoCompleteFetchTimer = new System.Windows.Forms.Timer(components);
            tabPanels.SuspendLayout();
            tabCurrentSystem.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            tabSearch.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ctxResultsMenu.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            flowLayoutPanel3.SuspendLayout();
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
            tabPanels.Size = new Size(965, 545);
            tabPanels.TabIndex = 2;
            // 
            // tabCurrentSystem
            // 
            tabCurrentSystem.Controls.Add(tableLayoutPanel2);
            tabCurrentSystem.Location = new Point(4, 24);
            tabCurrentSystem.Name = "tabCurrentSystem";
            tabCurrentSystem.Padding = new Padding(3);
            tabCurrentSystem.Size = new Size(957, 517);
            tabCurrentSystem.TabIndex = 0;
            tabCurrentSystem.Text = "Current System";
            tabCurrentSystem.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel2.Controls.Add(lblSystemName, 0, 0);
            tableLayoutPanel2.Controls.Add(txtSystemName, 1, 0);
            tableLayoutPanel2.Controls.Add(lblMessages, 0, 2);
            tableLayoutPanel2.Controls.Add(txtMessages, 1, 2);
            tableLayoutPanel2.Controls.Add(lblLastRecord, 0, 3);
            tableLayoutPanel2.Controls.Add(txtLastEntry, 0, 4);
            tableLayoutPanel2.Controls.Add(lblRecordCommanderTitle, 0, 1);
            tableLayoutPanel2.Controls.Add(lblRecordCommanderValue, 1, 1);
            tableLayoutPanel2.Controls.Add(btnOpenInViewer, 1, 3);
            tableLayoutPanel2.Controls.Add(flowLayoutPanel1, 3, 0);
            tableLayoutPanel2.Controls.Add(lblId64Details, 2, 3);
            tableLayoutPanel2.Controls.Add(txtId64Details, 2, 4);
            tableLayoutPanel2.Controls.Add(btnMessagesClear, 3, 2);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 5;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66666F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 83.3333359F));
            tableLayoutPanel2.Size = new Size(951, 511);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // lblSystemName
            // 
            lblSystemName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblSystemName.AutoSize = true;
            lblSystemName.Location = new Point(3, 0);
            lblSystemName.Name = "lblSystemName";
            lblSystemName.Size = new Size(83, 32);
            lblSystemName.TabIndex = 0;
            lblSystemName.Text = "System Name:";
            lblSystemName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtSystemName
            // 
            txtSystemName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.SetColumnSpan(txtSystemName, 2);
            txtSystemName.Location = new Point(153, 6);
            txtSystemName.Margin = new Padding(3, 6, 3, 3);
            txtSystemName.Name = "txtSystemName";
            txtSystemName.PlaceholderText = "(unknown)";
            txtSystemName.ReadOnly = true;
            txtSystemName.Size = new Size(645, 23);
            txtSystemName.TabIndex = 1;
            // 
            // lblMessages
            // 
            lblMessages.AutoSize = true;
            lblMessages.Location = new Point(3, 67);
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
            txtMessages.Location = new Point(153, 67);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ReadOnly = true;
            txtMessages.ScrollBars = ScrollBars.Vertical;
            txtMessages.Size = new Size(645, 63);
            txtMessages.TabIndex = 3;
            // 
            // lblLastRecord
            // 
            lblLastRecord.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblLastRecord.AutoSize = true;
            lblLastRecord.Location = new Point(3, 147);
            lblLastRecord.Margin = new Padding(3);
            lblLastRecord.Name = "lblLastRecord";
            lblLastRecord.Size = new Size(115, 15);
            lblLastRecord.TabIndex = 5;
            lblLastRecord.Text = "Latest Saved Record:";
            // 
            // txtLastEntry
            // 
            tableLayoutPanel2.SetColumnSpan(txtLastEntry, 2);
            txtLastEntry.Dock = DockStyle.Fill;
            txtLastEntry.Location = new Point(3, 168);
            txtLastEntry.Multiline = true;
            txtLastEntry.Name = "txtLastEntry";
            txtLastEntry.ReadOnly = true;
            txtLastEntry.ScrollBars = ScrollBars.Vertical;
            txtLastEntry.Size = new Size(595, 340);
            txtLastEntry.TabIndex = 5;
            // 
            // lblRecordCommanderTitle
            // 
            lblRecordCommanderTitle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblRecordCommanderTitle.AutoSize = true;
            lblRecordCommanderTitle.Location = new Point(3, 32);
            lblRecordCommanderTitle.Name = "lblRecordCommanderTitle";
            lblRecordCommanderTitle.Size = new Size(77, 32);
            lblRecordCommanderTitle.TabIndex = 7;
            lblRecordCommanderTitle.Text = "Commander:";
            lblRecordCommanderTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblRecordCommanderValue
            // 
            lblRecordCommanderValue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblRecordCommanderValue.AutoSize = true;
            lblRecordCommanderValue.Location = new Point(153, 32);
            lblRecordCommanderValue.Name = "lblRecordCommanderValue";
            lblRecordCommanderValue.Size = new Size(0, 32);
            lblRecordCommanderValue.TabIndex = 8;
            lblRecordCommanderValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnOpenInViewer
            // 
            btnOpenInViewer.FlatAppearance.BorderSize = 0;
            btnOpenInViewer.FlatStyle = FlatStyle.Flat;
            btnOpenInViewer.Location = new Point(153, 136);
            btnOpenInViewer.Name = "btnOpenInViewer";
            btnOpenInViewer.Padding = new Padding(1);
            btnOpenInViewer.Size = new Size(26, 26);
            btnOpenInViewer.TabIndex = 9;
            ttipArchivistUI.SetToolTip(btnOpenInViewer, "Open latest record in JSON Viewer");
            btnOpenInViewer.UseVisualStyleBackColor = true;
            btnOpenInViewer.Click += btnOpenInViewer_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnOpenInSearch);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(801, 0);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(150, 32);
            flowLayoutPanel1.TabIndex = 11;
            // 
            // btnOpenInSearch
            // 
            btnOpenInSearch.BackColor = Color.Transparent;
            btnOpenInSearch.FlatAppearance.BorderSize = 0;
            btnOpenInSearch.FlatAppearance.MouseOverBackColor = Color.FromArgb(215, 215, 215);
            btnOpenInSearch.FlatStyle = FlatStyle.Flat;
            btnOpenInSearch.Location = new Point(3, 3);
            btnOpenInSearch.Name = "btnOpenInSearch";
            btnOpenInSearch.Padding = new Padding(1);
            btnOpenInSearch.Size = new Size(26, 26);
            btnOpenInSearch.TabIndex = 10;
            ttipArchivistUI.SetToolTip(btnOpenInSearch, "Open in Search tab");
            btnOpenInSearch.UseVisualStyleBackColor = false;
            btnOpenInSearch.Click += btnOpenInSearch_Click;
            // 
            // lblId64Details
            // 
            lblId64Details.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblId64Details.AutoSize = true;
            lblId64Details.Location = new Point(604, 147);
            lblId64Details.Margin = new Padding(3);
            lblId64Details.Name = "lblId64Details";
            lblId64Details.Size = new Size(112, 15);
            lblId64Details.TabIndex = 12;
            lblId64Details.Text = "System ID64 Details:";
            // 
            // txtId64Details
            // 
            tableLayoutPanel2.SetColumnSpan(txtId64Details, 2);
            txtId64Details.Dock = DockStyle.Fill;
            txtId64Details.Location = new Point(604, 168);
            txtId64Details.Multiline = true;
            txtId64Details.Name = "txtId64Details";
            txtId64Details.ReadOnly = true;
            txtId64Details.ScrollBars = ScrollBars.Vertical;
            txtId64Details.Size = new Size(344, 340);
            txtId64Details.TabIndex = 13;
            // 
            // btnMessagesClear
            // 
            btnMessagesClear.BackColor = Color.Transparent;
            btnMessagesClear.FlatAppearance.BorderSize = 0;
            btnMessagesClear.FlatAppearance.MouseOverBackColor = Color.FromArgb(215, 215, 215);
            btnMessagesClear.FlatStyle = FlatStyle.Flat;
            btnMessagesClear.ImageSize = null;
            btnMessagesClear.Location = new Point(804, 67);
            btnMessagesClear.Name = "btnMessagesClear";
            btnMessagesClear.OriginalImage = null;
            btnMessagesClear.Padding = new Padding(1);
            btnMessagesClear.Size = new Size(26, 26);
            btnMessagesClear.TabIndex = 14;
            ttipArchivistUI.SetToolTip(btnMessagesClear, "Open in Search tab");
            btnMessagesClear.UseVisualStyleBackColor = false;
            btnMessagesClear.Click += btnMessagesClear_Click;
            // 
            // tabSearch
            // 
            tabSearch.Controls.Add(tableLayoutPanel3);
            tabSearch.Location = new Point(4, 24);
            tabSearch.Name = "tabSearch";
            tabSearch.Padding = new Padding(3);
            tabSearch.Size = new Size(957, 517);
            tabSearch.TabIndex = 1;
            tabSearch.Text = "Search";
            tabSearch.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 5;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel3.Controls.Add(btnFindMessagesClear, 4, 0);
            tableLayoutPanel3.Controls.Add(cboCommanderFilter, 1, 2);
            tableLayoutPanel3.Controls.Add(lblCommanderFilterTitle, 0, 2);
            tableLayoutPanel3.Controls.Add(lblFindSystem, 0, 0);
            tableLayoutPanel3.Controls.Add(txtFindMessages, 3, 0);
            tableLayoutPanel3.Controls.Add(lblFilter, 0, 1);
            tableLayoutPanel3.Controls.Add(txtFilter, 1, 1);
            tableLayoutPanel3.Controls.Add(lbJournals, 0, 3);
            tableLayoutPanel3.Controls.Add(lblRecentSystems, 3, 2);
            tableLayoutPanel3.Controls.Add(lbRecentSystems, 3, 3);
            tableLayoutPanel3.Controls.Add(flowLayoutPanel2, 2, 0);
            tableLayoutPanel3.Controls.Add(cboFindSystem, 1, 0);
            tableLayoutPanel3.Controls.Add(flowLayoutPanel3, 0, 4);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 5;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel3.Size = new Size(951, 511);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // btnFindMessagesClear
            // 
            btnFindMessagesClear.BackColor = Color.Transparent;
            btnFindMessagesClear.FlatAppearance.BorderSize = 0;
            btnFindMessagesClear.FlatAppearance.MouseOverBackColor = Color.FromArgb(215, 215, 215);
            btnFindMessagesClear.FlatStyle = FlatStyle.Flat;
            btnFindMessagesClear.ImageSize = null;
            btnFindMessagesClear.Location = new Point(904, 3);
            btnFindMessagesClear.Name = "btnFindMessagesClear";
            btnFindMessagesClear.OriginalImage = null;
            btnFindMessagesClear.Padding = new Padding(1);
            btnFindMessagesClear.Size = new Size(26, 26);
            btnFindMessagesClear.TabIndex = 25;
            ttipArchivistUI.SetToolTip(btnFindMessagesClear, "Open in Search tab");
            btnFindMessagesClear.UseVisualStyleBackColor = false;
            btnFindMessagesClear.Click += btnFindMessagesClear_Click;
            // 
            // cboCommanderFilter
            // 
            cboCommanderFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboCommanderFilter.Font = new Font("Segoe UI", 11F);
            cboCommanderFilter.FormattingEnabled = true;
            cboCommanderFilter.Location = new Point(153, 69);
            cboCommanderFilter.MaximumSize = new Size(500, 0);
            cboCommanderFilter.Name = "cboCommanderFilter";
            cboCommanderFilter.Size = new Size(295, 28);
            cboCommanderFilter.TabIndex = 8;
            cboCommanderFilter.SelectedIndexChanged += cboCommanderFilter_SelectedIndexChanged;
            // 
            // lblCommanderFilterTitle
            // 
            lblCommanderFilterTitle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblCommanderFilterTitle.AutoSize = true;
            lblCommanderFilterTitle.Font = new Font("Segoe UI", 9F);
            lblCommanderFilterTitle.Location = new Point(41, 66);
            lblCommanderFilterTitle.Name = "lblCommanderFilterTitle";
            lblCommanderFilterTitle.Size = new Size(106, 33);
            lblCommanderFilterTitle.TabIndex = 7;
            lblCommanderFilterTitle.Text = "Commander Filter:";
            lblCommanderFilterTitle.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblFindSystem
            // 
            lblFindSystem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblFindSystem.AutoSize = true;
            lblFindSystem.Location = new Point(73, 0);
            lblFindSystem.Name = "lblFindSystem";
            lblFindSystem.Size = new Size(74, 33);
            lblFindSystem.TabIndex = 0;
            lblFindSystem.Text = "Find System:";
            lblFindSystem.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtFindMessages
            // 
            txtFindMessages.Dock = DockStyle.Fill;
            txtFindMessages.Location = new Point(554, 3);
            txtFindMessages.Multiline = true;
            txtFindMessages.Name = "txtFindMessages";
            txtFindMessages.ReadOnly = true;
            tableLayoutPanel3.SetRowSpan(txtFindMessages, 2);
            txtFindMessages.ScrollBars = ScrollBars.Vertical;
            txtFindMessages.Size = new Size(344, 60);
            txtFindMessages.TabIndex = 22;
            txtFindMessages.Text = "Nothing found";
            ttipArchivistUI.SetToolTip(txtFindMessages, "Activity messages.");
            // 
            // lblFilter
            // 
            lblFilter.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblFilter.AutoSize = true;
            lblFilter.Location = new Point(87, 33);
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(60, 33);
            lblFilter.TabIndex = 5;
            lblFilter.Text = "Text Filter:";
            lblFilter.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtFilter
            // 
            txtFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilter.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtFilter.Location = new Point(153, 39);
            txtFilter.Margin = new Padding(3, 6, 3, 3);
            txtFilter.Name = "txtFilter";
            txtFilter.PlaceholderText = "Type to filter";
            txtFilter.Size = new Size(295, 27);
            txtFilter.TabIndex = 6;
            txtFilter.TextChanged += txtFilter_TextChanged;
            // 
            // lbJournals
            // 
            tableLayoutPanel3.SetColumnSpan(lbJournals, 3);
            lbJournals.ContextMenuStrip = ctxResultsMenu;
            lbJournals.Dock = DockStyle.Fill;
            lbJournals.FormattingEnabled = true;
            lbJournals.HorizontalScrollbar = true;
            lbJournals.ItemHeight = 15;
            lbJournals.Items.AddRange(new object[] { "{\"timestamp\":\"2024-04-11T19:39:33\" \"event\":\"FSDJump\" ... }" });
            lbJournals.Location = new Point(3, 102);
            lbJournals.Name = "lbJournals";
            lbJournals.SelectionMode = SelectionMode.MultiExtended;
            lbJournals.Size = new Size(545, 373);
            lbJournals.TabIndex = 9;
            lbJournals.KeyDown += lbJournals_KeyDown;
            lbJournals.MouseDown += lbJournals_MouseDown;
            lbJournals.MouseUp += lbJournals_MouseUp;
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
            // lblRecentSystems
            // 
            lblRecentSystems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblRecentSystems.AutoSize = true;
            lblRecentSystems.Location = new Point(554, 66);
            lblRecentSystems.Name = "lblRecentSystems";
            lblRecentSystems.Size = new Size(137, 33);
            lblRecentSystems.TabIndex = 23;
            lblRecentSystems.Text = "Recently visited systems:";
            lblRecentSystems.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbRecentSystems
            // 
            tableLayoutPanel3.SetColumnSpan(lbRecentSystems, 2);
            lbRecentSystems.Dock = DockStyle.Fill;
            lbRecentSystems.FormattingEnabled = true;
            lbRecentSystems.ItemHeight = 15;
            lbRecentSystems.Location = new Point(554, 102);
            lbRecentSystems.Name = "lbRecentSystems";
            lbRecentSystems.Size = new Size(394, 373);
            lbRecentSystems.TabIndex = 13;
            ttipArchivistUI.SetToolTip(lbRecentSystems, "Double-click an item to search for the system.");
            lbRecentSystems.DoubleClick += lbRecentSystems_DoubleClick;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(btnSearchDB);
            flowLayoutPanel2.Controls.Add(btnLoadFromSpansh);
            flowLayoutPanel2.Location = new Point(451, 0);
            flowLayoutPanel2.Margin = new Padding(0);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(100, 33);
            flowLayoutPanel2.TabIndex = 2;
            // 
            // btnSearchDB
            // 
            btnSearchDB.BackColor = Color.Transparent;
            btnSearchDB.FlatAppearance.BorderSize = 0;
            btnSearchDB.FlatAppearance.MouseOverBackColor = Color.FromArgb(215, 215, 215);
            btnSearchDB.FlatStyle = FlatStyle.Flat;
            btnSearchDB.Location = new Point(3, 3);
            btnSearchDB.Name = "btnSearchDB";
            btnSearchDB.Padding = new Padding(1);
            btnSearchDB.Size = new Size(26, 26);
            btnSearchDB.TabIndex = 3;
            ttipArchivistUI.SetToolTip(btnSearchDB, "Search collected journals");
            btnSearchDB.UseVisualStyleBackColor = false;
            btnSearchDB.Click += btnSearchDB_Click;
            // 
            // btnLoadFromSpansh
            // 
            btnLoadFromSpansh.BackColor = Color.Transparent;
            btnLoadFromSpansh.FlatAppearance.BorderSize = 0;
            btnLoadFromSpansh.FlatAppearance.MouseOverBackColor = Color.FromArgb(215, 215, 215);
            btnLoadFromSpansh.FlatStyle = FlatStyle.Flat;
            btnLoadFromSpansh.Location = new Point(35, 3);
            btnLoadFromSpansh.Name = "btnLoadFromSpansh";
            btnLoadFromSpansh.Padding = new Padding(1);
            btnLoadFromSpansh.Size = new Size(26, 26);
            btnLoadFromSpansh.TabIndex = 4;
            ttipArchivistUI.SetToolTip(btnLoadFromSpansh, "Generate journals from Spansh data");
            btnLoadFromSpansh.UseVisualStyleBackColor = false;
            btnLoadFromSpansh.Click += btnLoadFromSpansh_Click;
            // 
            // cboFindSystem
            // 
            cboFindSystem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboFindSystem.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cboFindSystem.Location = new Point(153, 3);
            cboFindSystem.Name = "cboFindSystem";
            cboFindSystem.Size = new Size(295, 28);
            cboFindSystem.TabIndex = 1;
            ttipArchivistUI.SetToolTip(cboFindSystem, "Enter search text (id64, system name or substring)");
            cboFindSystem.SelectedIndexChanged += cboFindSystem_SelectedIndexChanged;
            cboFindSystem.TextUpdate += cboFindSystem_TextUpdate;
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.Controls.Add(btnResendAll);
            flowLayoutPanel3.Controls.Add(btnSendViaMsg);
            flowLayoutPanel3.Controls.Add(btnCopy);
            flowLayoutPanel3.Controls.Add(btnView);
            flowLayoutPanel3.Dock = DockStyle.Fill;
            flowLayoutPanel3.Location = new Point(0, 478);
            flowLayoutPanel3.Margin = new Padding(0);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(150, 33);
            flowLayoutPanel3.TabIndex = 24;
            // 
            // btnResendAll
            // 
            btnResendAll.FlatAppearance.BorderSize = 0;
            btnResendAll.FlatStyle = FlatStyle.Flat;
            btnResendAll.Location = new Point(3, 3);
            btnResendAll.Name = "btnResendAll";
            btnResendAll.Size = new Size(27, 27);
            btnResendAll.TabIndex = 10;
            ttipArchivistUI.SetToolTip(btnResendAll, "Replay ALL system journals via Core (WARNING: may cause notification spam).");
            btnResendAll.UseVisualStyleBackColor = true;
            btnResendAll.Click += btnResendAll_Click;
            // 
            // btnSendViaMsg
            // 
            btnSendViaMsg.FlatAppearance.BorderSize = 0;
            btnSendViaMsg.FlatStyle = FlatStyle.Flat;
            btnSendViaMsg.ImageSize = null;
            btnSendViaMsg.Location = new Point(36, 3);
            btnSendViaMsg.Name = "btnSendViaMsg";
            btnSendViaMsg.OriginalImage = null;
            btnSendViaMsg.Size = new Size(27, 27);
            btnSendViaMsg.TabIndex = 11;
            ttipArchivistUI.SetToolTip(btnSendViaMsg, "Send ALL system journals via plugin message.");
            btnSendViaMsg.UseVisualStyleBackColor = true;
            btnSendViaMsg.Click += btnSendViaMsg_Click;
            // 
            // btnCopy
            // 
            btnCopy.FlatAppearance.BorderSize = 0;
            btnCopy.FlatStyle = FlatStyle.Flat;
            btnCopy.Location = new Point(69, 3);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(27, 27);
            btnCopy.TabIndex = 12;
            ttipArchivistUI.SetToolTip(btnCopy, "Copy selection (or all, if no selection)");
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += btnCopy_Click;
            // 
            // btnView
            // 
            btnView.FlatAppearance.BorderSize = 0;
            btnView.FlatStyle = FlatStyle.Flat;
            btnView.Location = new Point(102, 3);
            btnView.Name = "btnView";
            btnView.Size = new Size(27, 27);
            btnView.TabIndex = 13;
            ttipArchivistUI.SetToolTip(btnView, "View selected journal");
            btnView.UseVisualStyleBackColor = true;
            btnView.Click += btnView_Click;
            // 
            // autoCompleteFetchTimer
            // 
            autoCompleteFetchTimer.Interval = 500;
            autoCompleteFetchTimer.Tick += autoCompleteFetchTimer_Tick;
            // 
            // ArchivistUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabPanels);
            DoubleBuffered = true;
            Name = "ArchivistUI";
            Size = new Size(965, 545);
            tabPanels.ResumeLayout(false);
            tabCurrentSystem.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            tabSearch.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ctxResultsMenu.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel3.ResumeLayout(false);
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
        private Label lblMessages;
        private TextBox txtMessages;
        private Label lblLastRecord;
        private TextBox txtLastEntry;
        private ContextMenuStrip ctxResultsMenu;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel3;
        private Label lblFindSystem;
        private TextBox txtFindMessages;
        private Label lblFilter;
        private TextBox txtFilter;
        private ListBox lbJournals;
        private Label lblRecordCommanderTitle;
        private Label lblRecordCommanderValue;
        private ThemeableIconButton btnOpenInViewer;
        private ThemeableIconButton btnOpenInSearch;
        private ToolTip ttipArchivistUI;
        private ListBox lbRecentSystems;
        private Label lblRecentSystems;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private ThemeableIconButton btnLoadFromSpansh;
        private ThemeableIconButton btnSearchDB;
        private ThemeableIconButton btnResendAll;
        private ComboBox cboFindSystem;
        private System.Windows.Forms.Timer autoCompleteFetchTimer;
        private FlowLayoutPanel flowLayoutPanel3;
        private ThemeableIconButton btnCopy;
        private ThemeableIconButton btnView;
        private Label lblId64Details;
        private TextBox txtId64Details;
        private ThemeableImageButton btnSendViaMsg;
        private ThemeableImageButton btnMessagesClear;
        private ThemeableImageButton btnFindMessagesClear;
    }
}
