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
            tabJournals = new TabPage();
            tlpJournals = new TableLayoutPanel();
            flpJournalTools = new FlowLayoutPanel();
            txtJournalFilter = new TextBox();
            lblFilter = new Label();
            lblSpanshAugmentedDataLabel = new Label();
            lbAugmented = new ListBox();
            ctxResultsMenu = new ContextMenuStrip(components);
            copyToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            resendAllEventsToolStripMenuItem = new ToolStripMenuItem();
            viaMessageBusToolStripMenuItem = new ToolStripMenuItem();
            viaCoreJournalEventsToolStripMenuItem = new ToolStripMenuItem();
            lblCollectedEventsLabel = new Label();
            lbJournals = new ListBox();
            tabNotifications = new TabPage();
            tlpNotifs = new TableLayoutPanel();
            txtNotifFilter = new TextBox();
            lblNotifFilterLabel = new Label();
            flpNotifSearchTools = new FlowLayoutPanel();
            dgvNotifSearchResults = new DataGridView();
            colTitle = new DataGridViewTextBoxColumn();
            colDetails = new DataGridViewTextBoxColumn();
            colExtDetails = new DataGridViewTextBoxColumn();
            colSender = new DataGridViewTextBoxColumn();
            tabAddrCache = new TabPage();
            tlpAddressCache = new TableLayoutPanel();
            lblOnlineLookupResult = new Label();
            lblCacheStatus = new Label();
            flpAddrCacheTools = new FlowLayoutPanel();
            pAddressCache = new Panel();
            lblPosCacheId64Value = new Label();
            lblId64 = new Label();
            lblPosCacheSystemNameValue = new Label();
            label1 = new Label();
            pLookupResult = new Panel();
            lblLookupId64 = new Label();
            label3 = new Label();
            lblLookupSystemName = new Label();
            label5 = new Label();
            lblSystemName = new Label();
            txtSystemName = new TextBox();
            lblMessages = new Label();
            txtMessages = new TextBox();
            lblRecordCommanderTitle = new Label();
            lblCurrentCommanderValue = new Label();
            cboCommanderFilter = new ComboBox();
            lblCommanderFilterTitle = new Label();
            lblFindSystem = new Label();
            lbRecentSystems = new ListBox();
            cboFindSystem = new ComboBox();
            ttipArchivistUI = new ToolTip(components);
            autoCompleteFetchTimer = new System.Windows.Forms.Timer(components);
            tlpArchivist = new TableLayoutPanel();
            gbStatus = new GroupBox();
            tlpStatus = new TableLayoutPanel();
            lvSystemDbSummary = new ListView();
            colDataType = new ColumnHeader();
            colStatus = new ColumnHeader();
            gbSearch = new GroupBox();
            tlpSearch = new TableLayoutPanel();
            flpSearch = new FlowLayoutPanel();
            tabPanels.SuspendLayout();
            tabJournals.SuspendLayout();
            tlpJournals.SuspendLayout();
            ctxResultsMenu.SuspendLayout();
            tabNotifications.SuspendLayout();
            tlpNotifs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvNotifSearchResults).BeginInit();
            tabAddrCache.SuspendLayout();
            tlpAddressCache.SuspendLayout();
            pAddressCache.SuspendLayout();
            pLookupResult.SuspendLayout();
            tlpArchivist.SuspendLayout();
            gbStatus.SuspendLayout();
            tlpStatus.SuspendLayout();
            gbSearch.SuspendLayout();
            tlpSearch.SuspendLayout();
            SuspendLayout();
            // 
            // tabPanels
            // 
            tlpSearch.SetColumnSpan(tabPanels, 3);
            tabPanels.Controls.Add(tabJournals);
            tabPanels.Controls.Add(tabNotifications);
            tabPanels.Controls.Add(tabAddrCache);
            tabPanels.Dock = DockStyle.Fill;
            tabPanels.Location = new Point(3, 108);
            tabPanels.Name = "tabPanels";
            tabPanels.SelectedIndex = 0;
            tabPanels.Size = new Size(626, 459);
            tabPanels.TabIndex = 2;
            // 
            // tabJournals
            // 
            tabJournals.Controls.Add(tlpJournals);
            tabJournals.Location = new Point(4, 24);
            tabJournals.Name = "tabJournals";
            tabJournals.Padding = new Padding(3);
            tabJournals.Size = new Size(618, 431);
            tabJournals.TabIndex = 0;
            tabJournals.Text = "Journals";
            tabJournals.UseVisualStyleBackColor = true;
            // 
            // tlpJournals
            // 
            tlpJournals.ColumnCount = 6;
            tlpJournals.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
            tlpJournals.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tlpJournals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpJournals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpJournals.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tlpJournals.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
            tlpJournals.Controls.Add(flpJournalTools, 5, 0);
            tlpJournals.Controls.Add(txtJournalFilter, 2, 0);
            tlpJournals.Controls.Add(lblFilter, 0, 0);
            tlpJournals.Controls.Add(lblSpanshAugmentedDataLabel, 3, 1);
            tlpJournals.Controls.Add(lbAugmented, 3, 2);
            tlpJournals.Controls.Add(lblCollectedEventsLabel, 1, 1);
            tlpJournals.Controls.Add(lbJournals, 1, 2);
            tlpJournals.Dock = DockStyle.Fill;
            tlpJournals.Location = new Point(3, 3);
            tlpJournals.Name = "tlpJournals";
            tlpJournals.RowCount = 3;
            tlpJournals.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tlpJournals.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tlpJournals.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpJournals.Size = new Size(612, 425);
            tlpJournals.TabIndex = 0;
            // 
            // flpJournalTools
            // 
            flpJournalTools.Dock = DockStyle.Fill;
            flpJournalTools.FlowDirection = FlowDirection.TopDown;
            flpJournalTools.Location = new Point(577, 0);
            flpJournalTools.Margin = new Padding(0);
            flpJournalTools.Name = "flpJournalTools";
            tlpJournals.SetRowSpan(flpJournalTools, 3);
            flpJournalTools.Size = new Size(35, 425);
            flpJournalTools.TabIndex = 2;
            // 
            // txtJournalFilter
            // 
            txtJournalFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlpJournals.SetColumnSpan(txtJournalFilter, 2);
            txtJournalFilter.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtJournalFilter.Location = new Point(88, 3);
            txtJournalFilter.Name = "txtJournalFilter";
            txtJournalFilter.PlaceholderText = "Type to filter";
            txtJournalFilter.Size = new Size(436, 27);
            txtJournalFilter.TabIndex = 6;
            txtJournalFilter.TextChanged += TxtJournalFilter_TextChanged;
            // 
            // lblFilter
            // 
            lblFilter.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblFilter.AutoSize = true;
            tlpJournals.SetColumnSpan(lblFilter, 2);
            lblFilter.Location = new Point(22, 0);
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(60, 25);
            lblFilter.TabIndex = 5;
            lblFilter.Text = "Text Filter:";
            lblFilter.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblSpanshAugmentedDataLabel
            // 
            lblSpanshAugmentedDataLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblSpanshAugmentedDataLabel.AutoSize = true;
            tlpJournals.SetColumnSpan(lblSpanshAugmentedDataLabel, 2);
            lblSpanshAugmentedDataLabel.Location = new Point(309, 35);
            lblSpanshAugmentedDataLabel.Name = "lblSpanshAugmentedDataLabel";
            lblSpanshAugmentedDataLabel.Size = new Size(141, 15);
            lblSpanshAugmentedDataLabel.TabIndex = 11;
            lblSpanshAugmentedDataLabel.Text = "Spansh Augmented Data:";
            // 
            // lbAugmented
            // 
            tlpJournals.SetColumnSpan(lbAugmented, 2);
            lbAugmented.ContextMenuStrip = ctxResultsMenu;
            lbAugmented.Dock = DockStyle.Fill;
            lbAugmented.FormattingEnabled = true;
            lbAugmented.HorizontalScrollbar = true;
            lbAugmented.ItemHeight = 15;
            lbAugmented.Items.AddRange(new object[] { "{\"timestamp\":\"2024-04-11T19:39:33\" \"event\":\"FSDJump\" ... }" });
            lbAugmented.Location = new Point(309, 53);
            lbAugmented.Name = "lbAugmented";
            lbAugmented.SelectionMode = SelectionMode.MultiExtended;
            lbAugmented.Size = new Size(265, 369);
            lbAugmented.TabIndex = 12;
            lbAugmented.DoubleClick += ListView_DoubleClick;
            // 
            // ctxResultsMenu
            // 
            ctxResultsMenu.Items.AddRange(new ToolStripItem[] { copyToolStripMenuItem, viewToolStripMenuItem, resendAllEventsToolStripMenuItem });
            ctxResultsMenu.Name = "ctxResultsMenu";
            ctxResultsMenu.Size = new Size(165, 70);
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new Size(164, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += CopyToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(164, 22);
            viewToolStripMenuItem.Text = "View...";
            viewToolStripMenuItem.Click += ViewToolStripMenuItem_Click;
            // 
            // resendAllEventsToolStripMenuItem
            // 
            resendAllEventsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { viaMessageBusToolStripMenuItem, viaCoreJournalEventsToolStripMenuItem });
            resendAllEventsToolStripMenuItem.Name = "resendAllEventsToolStripMenuItem";
            resendAllEventsToolStripMenuItem.Size = new Size(164, 22);
            resendAllEventsToolStripMenuItem.Text = "Resend all events";
            // 
            // viaMessageBusToolStripMenuItem
            // 
            viaMessageBusToolStripMenuItem.Name = "viaMessageBusToolStripMenuItem";
            viaMessageBusToolStripMenuItem.Size = new Size(207, 22);
            viaMessageBusToolStripMenuItem.Text = "... via Plugin message";
            viaMessageBusToolStripMenuItem.Click += ViaMessageBusToolStripMenuItem_Click;
            // 
            // viaCoreJournalEventsToolStripMenuItem
            // 
            viaCoreJournalEventsToolStripMenuItem.Name = "viaCoreJournalEventsToolStripMenuItem";
            viaCoreJournalEventsToolStripMenuItem.Size = new Size(207, 22);
            viaCoreJournalEventsToolStripMenuItem.Text = "... via Core Journal events";
            viaCoreJournalEventsToolStripMenuItem.ToolTipText = "Warning! This may cause notification spam or other undesired effects.";
            viaCoreJournalEventsToolStripMenuItem.Click += ViaCoreJournalEventsToolStripMenuItem_Click;
            // 
            // lblCollectedEventsLabel
            // 
            lblCollectedEventsLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblCollectedEventsLabel.AutoSize = true;
            tlpJournals.SetColumnSpan(lblCollectedEventsLabel, 2);
            lblCollectedEventsLabel.Location = new Point(38, 35);
            lblCollectedEventsLabel.Name = "lblCollectedEventsLabel";
            lblCollectedEventsLabel.Size = new Size(97, 15);
            lblCollectedEventsLabel.TabIndex = 10;
            lblCollectedEventsLabel.Text = "Collected Events:";
            // 
            // lbJournals
            // 
            tlpJournals.SetColumnSpan(lbJournals, 2);
            lbJournals.ContextMenuStrip = ctxResultsMenu;
            lbJournals.Dock = DockStyle.Fill;
            lbJournals.FormattingEnabled = true;
            lbJournals.HorizontalScrollbar = true;
            lbJournals.ItemHeight = 15;
            lbJournals.Items.AddRange(new object[] { "{\"timestamp\":\"2024-04-11T19:39:33\" \"event\":\"FSDJump\" ... }" });
            lbJournals.Location = new Point(38, 53);
            lbJournals.Name = "lbJournals";
            lbJournals.SelectionMode = SelectionMode.MultiExtended;
            lbJournals.Size = new Size(265, 369);
            lbJournals.TabIndex = 9;
            lbJournals.DoubleClick += ListView_DoubleClick;
            lbJournals.KeyDown += ListBox_KeyDown;
            lbJournals.MouseDown += ListBox_MouseDown;
            lbJournals.MouseUp += ListBox_MouseUp;
            // 
            // tabNotifications
            // 
            tabNotifications.Controls.Add(tlpNotifs);
            tabNotifications.Location = new Point(4, 24);
            tabNotifications.Name = "tabNotifications";
            tabNotifications.Padding = new Padding(3);
            tabNotifications.Size = new Size(618, 431);
            tabNotifications.TabIndex = 1;
            tabNotifications.Text = "Notifications";
            tabNotifications.UseVisualStyleBackColor = true;
            // 
            // tlpNotifs
            // 
            tlpNotifs.ColumnCount = 3;
            tlpNotifs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tlpNotifs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpNotifs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
            tlpNotifs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tlpNotifs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tlpNotifs.Controls.Add(txtNotifFilter, 1, 0);
            tlpNotifs.Controls.Add(lblNotifFilterLabel, 0, 0);
            tlpNotifs.Controls.Add(flpNotifSearchTools, 2, 0);
            tlpNotifs.Controls.Add(dgvNotifSearchResults, 0, 1);
            tlpNotifs.Dock = DockStyle.Fill;
            tlpNotifs.Location = new Point(3, 3);
            tlpNotifs.Name = "tlpNotifs";
            tlpNotifs.RowCount = 2;
            tlpNotifs.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tlpNotifs.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpNotifs.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlpNotifs.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlpNotifs.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlpNotifs.Size = new Size(612, 425);
            tlpNotifs.TabIndex = 0;
            // 
            // txtNotifFilter
            // 
            txtNotifFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtNotifFilter.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtNotifFilter.Location = new Point(153, 3);
            txtNotifFilter.Name = "txtNotifFilter";
            txtNotifFilter.PlaceholderText = "Type to filter";
            txtNotifFilter.Size = new Size(421, 27);
            txtNotifFilter.TabIndex = 8;
            txtNotifFilter.TextChanged += TxtNotifFilter_TextChanged;
            // 
            // lblNotifFilterLabel
            // 
            lblNotifFilterLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblNotifFilterLabel.AutoSize = true;
            lblNotifFilterLabel.Location = new Point(87, 0);
            lblNotifFilterLabel.Name = "lblNotifFilterLabel";
            lblNotifFilterLabel.Size = new Size(60, 32);
            lblNotifFilterLabel.TabIndex = 7;
            lblNotifFilterLabel.Text = "Text Filter:";
            lblNotifFilterLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // flpNotifSearchTools
            // 
            flpNotifSearchTools.Dock = DockStyle.Fill;
            flpNotifSearchTools.FlowDirection = FlowDirection.TopDown;
            flpNotifSearchTools.Location = new Point(580, 3);
            flpNotifSearchTools.Name = "flpNotifSearchTools";
            tlpNotifs.SetRowSpan(flpNotifSearchTools, 2);
            flpNotifSearchTools.Size = new Size(29, 419);
            flpNotifSearchTools.TabIndex = 9;
            // 
            // dgvNotifSearchResults
            // 
            dgvNotifSearchResults.AllowUserToAddRows = false;
            dgvNotifSearchResults.AllowUserToDeleteRows = false;
            dgvNotifSearchResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvNotifSearchResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvNotifSearchResults.Columns.AddRange(new DataGridViewColumn[] { colTitle, colDetails, colExtDetails, colSender });
            tlpNotifs.SetColumnSpan(dgvNotifSearchResults, 2);
            dgvNotifSearchResults.Dock = DockStyle.Fill;
            dgvNotifSearchResults.EnableHeadersVisualStyles = false;
            dgvNotifSearchResults.Location = new Point(3, 35);
            dgvNotifSearchResults.Name = "dgvNotifSearchResults";
            dgvNotifSearchResults.ReadOnly = true;
            dgvNotifSearchResults.RowHeadersWidth = 10;
            dgvNotifSearchResults.Size = new Size(571, 387);
            dgvNotifSearchResults.TabIndex = 10;
            // 
            // colTitle
            // 
            colTitle.FillWeight = 25F;
            colTitle.HeaderText = "Title";
            colTitle.Name = "colTitle";
            colTitle.ReadOnly = true;
            colTitle.Width = 54;
            // 
            // colDetails
            // 
            colDetails.FillWeight = 30F;
            colDetails.HeaderText = "Details";
            colDetails.Name = "colDetails";
            colDetails.ReadOnly = true;
            colDetails.Width = 67;
            // 
            // colExtDetails
            // 
            colExtDetails.FillWeight = 30F;
            colExtDetails.HeaderText = "Extended Details";
            colExtDetails.Name = "colExtDetails";
            colExtDetails.ReadOnly = true;
            colExtDetails.Width = 109;
            // 
            // colSender
            // 
            colSender.FillWeight = 15F;
            colSender.HeaderText = "Sender";
            colSender.Name = "colSender";
            colSender.ReadOnly = true;
            colSender.Width = 68;
            // 
            // tabAddrCache
            // 
            tabAddrCache.Controls.Add(tlpAddressCache);
            tabAddrCache.Location = new Point(4, 24);
            tabAddrCache.Name = "tabAddrCache";
            tabAddrCache.Size = new Size(618, 431);
            tabAddrCache.TabIndex = 3;
            tabAddrCache.Text = "Address Cache";
            tabAddrCache.UseVisualStyleBackColor = true;
            // 
            // tlpAddressCache
            // 
            tlpAddressCache.ColumnCount = 3;
            tlpAddressCache.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpAddressCache.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpAddressCache.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
            tlpAddressCache.Controls.Add(lblOnlineLookupResult, 1, 0);
            tlpAddressCache.Controls.Add(lblCacheStatus, 0, 0);
            tlpAddressCache.Controls.Add(flpAddrCacheTools, 2, 0);
            tlpAddressCache.Controls.Add(pAddressCache, 0, 1);
            tlpAddressCache.Controls.Add(pLookupResult, 1, 1);
            tlpAddressCache.Dock = DockStyle.Fill;
            tlpAddressCache.Location = new Point(0, 0);
            tlpAddressCache.Name = "tlpAddressCache";
            tlpAddressCache.RowCount = 3;
            tlpAddressCache.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tlpAddressCache.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpAddressCache.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpAddressCache.Size = new Size(618, 431);
            tlpAddressCache.TabIndex = 14;
            // 
            // lblOnlineLookupResult
            // 
            lblOnlineLookupResult.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblOnlineLookupResult.AutoSize = true;
            lblOnlineLookupResult.Location = new Point(294, 20);
            lblOnlineLookupResult.Name = "lblOnlineLookupResult";
            lblOnlineLookupResult.Size = new Size(117, 15);
            lblOnlineLookupResult.TabIndex = 20;
            lblOnlineLookupResult.Text = "Online lookup result:";
            // 
            // lblCacheStatus
            // 
            lblCacheStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblCacheStatus.AutoSize = true;
            lblCacheStatus.Location = new Point(3, 20);
            lblCacheStatus.Name = "lblCacheStatus";
            lblCacheStatus.Size = new Size(132, 15);
            lblCacheStatus.TabIndex = 17;
            lblCacheStatus.Text = "Search result|Not found";
            // 
            // flpAddrCacheTools
            // 
            flpAddrCacheTools.Dock = DockStyle.Fill;
            flpAddrCacheTools.FlowDirection = FlowDirection.TopDown;
            flpAddrCacheTools.Location = new Point(585, 3);
            flpAddrCacheTools.Name = "flpAddrCacheTools";
            tlpAddressCache.SetRowSpan(flpAddrCacheTools, 3);
            flpAddrCacheTools.Size = new Size(30, 425);
            flpAddrCacheTools.TabIndex = 15;
            // 
            // pAddressCache
            // 
            pAddressCache.Controls.Add(lblPosCacheId64Value);
            pAddressCache.Controls.Add(lblId64);
            pAddressCache.Controls.Add(lblPosCacheSystemNameValue);
            pAddressCache.Controls.Add(label1);
            pAddressCache.Dock = DockStyle.Fill;
            pAddressCache.Location = new Point(3, 38);
            pAddressCache.Name = "pAddressCache";
            pAddressCache.Size = new Size(285, 192);
            pAddressCache.TabIndex = 18;
            // 
            // lblPosCacheId64Value
            // 
            lblPosCacheId64Value.AutoSize = true;
            lblPosCacheId64Value.Location = new Point(112, 32);
            lblPosCacheId64Value.Name = "lblPosCacheId64Value";
            lblPosCacheId64Value.Size = new Size(45, 15);
            lblPosCacheId64Value.TabIndex = 3;
            lblPosCacheId64Value.Text = "<id64>";
            // 
            // lblId64
            // 
            lblId64.AutoSize = true;
            lblId64.Location = new Point(8, 32);
            lblId64.Name = "lblId64";
            lblId64.Size = new Size(32, 15);
            lblId64.TabIndex = 2;
            lblId64.Text = "Id64:";
            // 
            // lblPosCacheSystemNameValue
            // 
            lblPosCacheSystemNameValue.AutoSize = true;
            lblPosCacheSystemNameValue.Location = new Point(112, 8);
            lblPosCacheSystemNameValue.Name = "lblPosCacheSystemNameValue";
            lblPosCacheSystemNameValue.Size = new Size(93, 15);
            lblPosCacheSystemNameValue.TabIndex = 1;
            lblPosCacheSystemNameValue.Text = "<system name>";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 8);
            label1.Name = "label1";
            label1.Size = new Size(83, 15);
            label1.TabIndex = 0;
            label1.Text = "System Name:";
            // 
            // pLookupResult
            // 
            pLookupResult.Controls.Add(lblLookupId64);
            pLookupResult.Controls.Add(label3);
            pLookupResult.Controls.Add(lblLookupSystemName);
            pLookupResult.Controls.Add(label5);
            pLookupResult.Dock = DockStyle.Fill;
            pLookupResult.Location = new Point(294, 38);
            pLookupResult.Name = "pLookupResult";
            pLookupResult.Size = new Size(285, 192);
            pLookupResult.TabIndex = 19;
            // 
            // lblLookupId64
            // 
            lblLookupId64.AutoSize = true;
            lblLookupId64.Location = new Point(112, 32);
            lblLookupId64.Name = "lblLookupId64";
            lblLookupId64.Size = new Size(45, 15);
            lblLookupId64.TabIndex = 3;
            lblLookupId64.Text = "<id64>";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 32);
            label3.Name = "label3";
            label3.Size = new Size(32, 15);
            label3.TabIndex = 2;
            label3.Text = "Id64:";
            // 
            // lblLookupSystemName
            // 
            lblLookupSystemName.AutoSize = true;
            lblLookupSystemName.Location = new Point(112, 8);
            lblLookupSystemName.Name = "lblLookupSystemName";
            lblLookupSystemName.Size = new Size(93, 15);
            lblLookupSystemName.TabIndex = 1;
            lblLookupSystemName.Text = "<system name>";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(8, 8);
            label5.Name = "label5";
            label5.Size = new Size(83, 15);
            label5.TabIndex = 0;
            label5.Text = "System Name:";
            // 
            // lblSystemName
            // 
            lblSystemName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblSystemName.AutoSize = true;
            lblSystemName.Location = new Point(3, 0);
            lblSystemName.Name = "lblSystemName";
            lblSystemName.Size = new Size(91, 25);
            lblSystemName.TabIndex = 0;
            lblSystemName.Text = "Current System:";
            lblSystemName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtSystemName
            // 
            txtSystemName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSystemName.Location = new Point(115, 3);
            txtSystemName.Name = "txtSystemName";
            txtSystemName.PlaceholderText = "(unknown)";
            txtSystemName.ReadOnly = true;
            txtSystemName.Size = new Size(202, 23);
            txtSystemName.TabIndex = 1;
            // 
            // lblMessages
            // 
            lblMessages.AutoSize = true;
            lblMessages.Location = new Point(3, 300);
            lblMessages.Margin = new Padding(3);
            lblMessages.Name = "lblMessages";
            lblMessages.Size = new Size(61, 15);
            lblMessages.TabIndex = 3;
            lblMessages.Text = "Messages:";
            // 
            // txtMessages
            // 
            tlpStatus.SetColumnSpan(txtMessages, 3);
            txtMessages.Dock = DockStyle.Fill;
            txtMessages.Location = new Point(3, 325);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ReadOnly = true;
            txtMessages.ScrollBars = ScrollBars.Vertical;
            txtMessages.Size = new Size(340, 242);
            txtMessages.TabIndex = 3;
            ttipArchivistUI.SetToolTip(txtMessages, "Messages from Archivist; newest first.");
            // 
            // lblRecordCommanderTitle
            // 
            lblRecordCommanderTitle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblRecordCommanderTitle.AutoSize = true;
            lblRecordCommanderTitle.Location = new Point(3, 25);
            lblRecordCommanderTitle.Name = "lblRecordCommanderTitle";
            lblRecordCommanderTitle.Size = new Size(83, 25);
            lblRecordCommanderTitle.TabIndex = 7;
            lblRecordCommanderTitle.Text = "Current Cmdr:";
            lblRecordCommanderTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblCurrentCommanderValue
            // 
            lblCurrentCommanderValue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblCurrentCommanderValue.AutoSize = true;
            lblCurrentCommanderValue.Location = new Point(115, 25);
            lblCurrentCommanderValue.Name = "lblCurrentCommanderValue";
            lblCurrentCommanderValue.Size = new Size(0, 25);
            lblCurrentCommanderValue.TabIndex = 8;
            lblCurrentCommanderValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboCommanderFilter
            // 
            cboCommanderFilter.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cboCommanderFilter.FlatStyle = FlatStyle.Flat;
            cboCommanderFilter.FormattingEnabled = true;
            cboCommanderFilter.Location = new Point(129, 41);
            cboCommanderFilter.MaximumSize = new Size(500, 0);
            cboCommanderFilter.Name = "cboCommanderFilter";
            cboCommanderFilter.Size = new Size(247, 23);
            cboCommanderFilter.TabIndex = 8;
            cboCommanderFilter.SelectedIndexChanged += CboCommanderFilter_SelectedIndexChanged;
            // 
            // lblCommanderFilterTitle
            // 
            lblCommanderFilterTitle.Anchor = AnchorStyles.Right;
            lblCommanderFilterTitle.AutoSize = true;
            lblCommanderFilterTitle.Font = new Font("Segoe UI", 9F);
            lblCommanderFilterTitle.Location = new Point(17, 45);
            lblCommanderFilterTitle.Name = "lblCommanderFilterTitle";
            lblCommanderFilterTitle.Size = new Size(106, 15);
            lblCommanderFilterTitle.TabIndex = 7;
            lblCommanderFilterTitle.Text = "Commander Filter:";
            lblCommanderFilterTitle.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblFindSystem
            // 
            lblFindSystem.Anchor = AnchorStyles.Right;
            lblFindSystem.AutoSize = true;
            lblFindSystem.Location = new Point(49, 10);
            lblFindSystem.Name = "lblFindSystem";
            lblFindSystem.Size = new Size(74, 15);
            lblFindSystem.TabIndex = 0;
            lblFindSystem.Text = "Find System:";
            lblFindSystem.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbRecentSystems
            // 
            lbRecentSystems.Dock = DockStyle.Fill;
            lbRecentSystems.FormattingEnabled = true;
            lbRecentSystems.ItemHeight = 15;
            lbRecentSystems.Location = new Point(382, 3);
            lbRecentSystems.Name = "lbRecentSystems";
            tlpSearch.SetRowSpan(lbRecentSystems, 3);
            lbRecentSystems.Size = new Size(247, 99);
            lbRecentSystems.TabIndex = 13;
            ttipArchivistUI.SetToolTip(lbRecentSystems, "Recently visited systems. Double-click an item to initiate a search.");
            lbRecentSystems.DoubleClick += LbRecentSystems_DoubleClick;
            // 
            // cboFindSystem
            // 
            cboFindSystem.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cboFindSystem.FlatStyle = FlatStyle.Flat;
            cboFindSystem.Location = new Point(129, 6);
            cboFindSystem.Name = "cboFindSystem";
            cboFindSystem.Size = new Size(247, 23);
            cboFindSystem.TabIndex = 1;
            ttipArchivistUI.SetToolTip(cboFindSystem, "Enter search text (id64, system name or substring) and hit enter to search.");
            cboFindSystem.SelectedIndexChanged += CboFindSystem_SelectedIndexChanged;
            cboFindSystem.TextUpdate += CboFindSystem_TextUpdate;
            cboFindSystem.KeyDown += CboFindSystem_KeyDown;
            // 
            // autoCompleteFetchTimer
            // 
            autoCompleteFetchTimer.Interval = 500;
            autoCompleteFetchTimer.Tick += AutoCompleteFetchTimer_Tick;
            // 
            // tlpArchivist
            // 
            tlpArchivist.ColumnCount = 2;
            tlpArchivist.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tlpArchivist.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            tlpArchivist.Controls.Add(gbStatus, 0, 0);
            tlpArchivist.Controls.Add(gbSearch, 1, 0);
            tlpArchivist.Dock = DockStyle.Fill;
            tlpArchivist.Location = new Point(0, 0);
            tlpArchivist.Name = "tlpArchivist";
            tlpArchivist.RowCount = 1;
            tlpArchivist.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpArchivist.Size = new Size(1024, 598);
            tlpArchivist.TabIndex = 3;
            // 
            // gbStatus
            // 
            gbStatus.Controls.Add(tlpStatus);
            gbStatus.Dock = DockStyle.Fill;
            gbStatus.Location = new Point(3, 3);
            gbStatus.Name = "gbStatus";
            gbStatus.Size = new Size(352, 592);
            gbStatus.TabIndex = 0;
            gbStatus.TabStop = false;
            gbStatus.Text = "Status";
            // 
            // tlpStatus
            // 
            tlpStatus.ColumnCount = 3;
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 26F));
            tlpStatus.Controls.Add(lblCurrentCommanderValue, 1, 1);
            tlpStatus.Controls.Add(txtMessages, 0, 4);
            tlpStatus.Controls.Add(txtSystemName, 1, 0);
            tlpStatus.Controls.Add(lblSystemName, 0, 0);
            tlpStatus.Controls.Add(lblMessages, 0, 3);
            tlpStatus.Controls.Add(lblRecordCommanderTitle, 0, 1);
            tlpStatus.Controls.Add(lvSystemDbSummary, 0, 2);
            tlpStatus.Dock = DockStyle.Fill;
            tlpStatus.Location = new Point(3, 19);
            tlpStatus.Name = "tlpStatus";
            tlpStatus.RowCount = 5;
            tlpStatus.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tlpStatus.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tlpStatus.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpStatus.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tlpStatus.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpStatus.Size = new Size(346, 570);
            tlpStatus.TabIndex = 0;
            // 
            // lvSystemDbSummary
            // 
            lvSystemDbSummary.Columns.AddRange(new ColumnHeader[] { colDataType, colStatus });
            tlpStatus.SetColumnSpan(lvSystemDbSummary, 3);
            lvSystemDbSummary.Dock = DockStyle.Fill;
            lvSystemDbSummary.FullRowSelect = true;
            lvSystemDbSummary.Location = new Point(3, 53);
            lvSystemDbSummary.Name = "lvSystemDbSummary";
            lvSystemDbSummary.Size = new Size(340, 241);
            lvSystemDbSummary.TabIndex = 15;
            lvSystemDbSummary.UseCompatibleStateImageBehavior = false;
            lvSystemDbSummary.View = View.Details;
            // 
            // colDataType
            // 
            colDataType.Text = "Data";
            colDataType.Width = 200;
            // 
            // colStatus
            // 
            colStatus.Text = "Status";
            colStatus.Width = 200;
            // 
            // gbSearch
            // 
            gbSearch.Controls.Add(tlpSearch);
            gbSearch.Dock = DockStyle.Fill;
            gbSearch.Location = new Point(361, 3);
            gbSearch.Name = "gbSearch";
            gbSearch.Size = new Size(660, 592);
            gbSearch.TabIndex = 1;
            gbSearch.TabStop = false;
            gbSearch.Text = "Search";
            // 
            // tlpSearch
            // 
            tlpSearch.ColumnCount = 4;
            tlpSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tlpSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tlpSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tlpSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tlpSearch.Controls.Add(tabPanels, 0, 3);
            tlpSearch.Controls.Add(lblFindSystem, 0, 0);
            tlpSearch.Controls.Add(cboCommanderFilter, 1, 1);
            tlpSearch.Controls.Add(cboFindSystem, 1, 0);
            tlpSearch.Controls.Add(lblCommanderFilterTitle, 0, 1);
            tlpSearch.Controls.Add(lbRecentSystems, 2, 0);
            tlpSearch.Controls.Add(flpSearch, 1, 2);
            tlpSearch.Dock = DockStyle.Fill;
            tlpSearch.Location = new Point(3, 19);
            tlpSearch.Name = "tlpSearch";
            tlpSearch.RowCount = 4;
            tlpSearch.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tlpSearch.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tlpSearch.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tlpSearch.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpSearch.Size = new Size(654, 570);
            tlpSearch.TabIndex = 0;
            // 
            // flpSearch
            // 
            flpSearch.Dock = DockStyle.Fill;
            flpSearch.Location = new Point(129, 73);
            flpSearch.Name = "flpSearch";
            flpSearch.Size = new Size(247, 29);
            flpSearch.TabIndex = 15;
            // 
            // ArchivistUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tlpArchivist);
            DoubleBuffered = true;
            Name = "ArchivistUI";
            Size = new Size(1024, 598);
            tabPanels.ResumeLayout(false);
            tabJournals.ResumeLayout(false);
            tlpJournals.ResumeLayout(false);
            tlpJournals.PerformLayout();
            ctxResultsMenu.ResumeLayout(false);
            tabNotifications.ResumeLayout(false);
            tlpNotifs.ResumeLayout(false);
            tlpNotifs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvNotifSearchResults).EndInit();
            tabAddrCache.ResumeLayout(false);
            tlpAddressCache.ResumeLayout(false);
            tlpAddressCache.PerformLayout();
            pAddressCache.ResumeLayout(false);
            pAddressCache.PerformLayout();
            pLookupResult.ResumeLayout(false);
            pLookupResult.PerformLayout();
            tlpArchivist.ResumeLayout(false);
            gbStatus.ResumeLayout(false);
            tlpStatus.ResumeLayout(false);
            tlpStatus.PerformLayout();
            gbSearch.ResumeLayout(false);
            tlpSearch.ResumeLayout(false);
            tlpSearch.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Label lblCommanderFilterTitle;
        private ComboBox cboCommanderFilter;
        private TabControl tabPanels;
        private TabPage tabJournals;
        private TabPage tabNotifications;
        private TableLayoutPanel tlpJournals;
        private Label lblSystemName;
        private TextBox txtSystemName;
        private Label lblMessages;
        private TextBox txtMessages;
        private ContextMenuStrip ctxResultsMenu;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private TableLayoutPanel tlpNotifs;
        private Label lblFindSystem;
        private Label lblFilter;
        private TextBox txtJournalFilter;
        private ListBox lbJournals;
        private Label lblRecordCommanderTitle;
        private Label lblCurrentCommanderValue;
        private ToolTip ttipArchivistUI;
        private ListBox lbRecentSystems;
        private FlowLayoutPanel flpJournalTools;
        private ComboBox cboFindSystem;
        private System.Windows.Forms.Timer autoCompleteFetchTimer;
        private TableLayoutPanel tlpArchivist;
        private GroupBox gbStatus;
        private GroupBox gbSearch;
        private TableLayoutPanel tlpStatus;
        private TableLayoutPanel tlpSearch;
        private TabPage tabAddrCache;
        private ListView lvSystemDbSummary;
        private TextBox txtNotifFilter;
        private Label lblNotifFilterLabel;
        private FlowLayoutPanel flpNotifSearchTools;
        private TableLayoutPanel tlpAddressCache;
        private Label lblCacheStatus;
        private FlowLayoutPanel flpAddrCacheTools;
        private DataGridView dgvNotifSearchResults;
        private Label lblCollectedEventsLabel;
        private Label lblSpanshAugmentedDataLabel;
        private ListBox lbAugmented;
        private ToolStripMenuItem resendAllEventsToolStripMenuItem;
        private ToolStripMenuItem viaMessageBusToolStripMenuItem;
        private ToolStripMenuItem viaCoreJournalEventsToolStripMenuItem;
        private ColumnHeader colDataType;
        private ColumnHeader colStatus;
        private DataGridViewTextBoxColumn colTitle;
        private DataGridViewTextBoxColumn colDetails;
        private DataGridViewTextBoxColumn colExtDetails;
        private DataGridViewTextBoxColumn colSender;
        private FlowLayoutPanel flpSearch;
        private Panel pAddressCache;
        private Label lblPosCacheId64Value;
        private Label lblId64;
        private Label lblPosCacheSystemNameValue;
        private Label label1;
        private Label lblOnlineLookupResult;
        private Panel pLookupResult;
        private Label lblLookupId64;
        private Label label3;
        private Label lblLookupSystemName;
        private Label label5;
    }
}
