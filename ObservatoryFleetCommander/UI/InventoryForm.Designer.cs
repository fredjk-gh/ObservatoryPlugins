namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    partial class InventoryForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            tableLayoutPanel1 = new TableLayoutPanel();
            lblLastUpdatedValue = new Label();
            lblKnownCargoUsageValue = new Label();
            label5 = new Label();
            tlblInventory = new com.github.fredjk_gh.PluginCommon.UI.ThemeableImageLabel();
            lblDockingAccessValue = new Label();
            lblCapacityFreeValue = new Label();
            label1 = new Label();
            tlblCargo = new com.github.fredjk_gh.PluginCommon.UI.ThemeableImageLabel();
            label6 = new Label();
            invSplitter = new SplitContainer();
            dgvInventory = new DataGridView();
            colQty = new DataGridViewTextBoxColumn();
            colItem = new DataGridViewComboBoxColumn();
            dgvTradeOrders = new DataGridView();
            colTradeType = new DataGridViewTextBoxColumn();
            colTradeQty = new DataGridViewTextBoxColumn();
            colTradeItemName = new DataGridViewTextBoxColumn();
            colBlkMarket = new DataGridViewCheckBoxColumn();
            label4 = new Label();
            label2 = new Label();
            txtMessages = new TextBox();
            label3 = new Label();
            tTips = new ToolTip(components);
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)invSplitter).BeginInit();
            invSplitter.Panel1.SuspendLayout();
            invSplitter.Panel2.SuspendLayout();
            invSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvInventory).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvTradeOrders).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(lblLastUpdatedValue, 3, 0);
            tableLayoutPanel1.Controls.Add(lblKnownCargoUsageValue, 1, 2);
            tableLayoutPanel1.Controls.Add(label5, 2, 0);
            tableLayoutPanel1.Controls.Add(tlblInventory, 0, 2);
            tableLayoutPanel1.Controls.Add(lblDockingAccessValue, 1, 0);
            tableLayoutPanel1.Controls.Add(lblCapacityFreeValue, 3, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(tlblCargo, 1, 1);
            tableLayoutPanel1.Controls.Add(label6, 2, 1);
            tableLayoutPanel1.Controls.Add(invSplitter, 0, 3);
            tableLayoutPanel1.Controls.Add(label4, 3, 2);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(txtMessages, 1, 4);
            tableLayoutPanel1.Controls.Add(label3, 0, 4);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F));
            tableLayoutPanel1.Size = new Size(800, 446);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // lblLastUpdatedValue
            // 
            lblLastUpdatedValue.AutoSize = true;
            lblLastUpdatedValue.Location = new Point(603, 3);
            lblLastUpdatedValue.Margin = new Padding(3, 3, 3, 0);
            lblLastUpdatedValue.Name = "lblLastUpdatedValue";
            lblLastUpdatedValue.Size = new Size(132, 15);
            lblLastUpdatedValue.TabIndex = 5;
            lblLastUpdatedValue.Text = "yyyy-mm-dd hh:mm:ss";
            // 
            // lblKnownCargoUsageValue
            // 
            lblKnownCargoUsageValue.AutoSize = true;
            lblKnownCargoUsageValue.Location = new Point(203, 51);
            lblKnownCargoUsageValue.Margin = new Padding(3);
            lblKnownCargoUsageValue.Name = "lblKnownCargoUsageValue";
            lblKnownCargoUsageValue.Size = new Size(135, 15);
            lblKnownCargoUsageValue.TabIndex = 10;
            lblKnownCargoUsageValue.Text = "<known cargo usage> T";
            tTips.SetToolTip(lblKnownCargoUsageValue, "Value is the sum of quantities in the grid below.");
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(403, 3);
            label5.Margin = new Padding(3, 3, 3, 0);
            label5.Name = "label5";
            label5.Size = new Size(71, 15);
            label5.TabIndex = 4;
            label5.Text = "Last update:";
            // 
            // tlblInventory
            // 
            tlblInventory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlblInventory.AutoSize = true;
            tlblInventory.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblInventory.ImageColor = Color.Transparent;
            tlblInventory.LabelPosition = PluginCommon.UI.Shared.LabelPositionType.Left;
            tlblInventory.Location = new Point(3, 51);
            tlblInventory.Name = "tlblInventory";
            tlblInventory.Size = new Size(194, 15);
            tlblInventory.TabIndex = 11;
            tlblInventory.Text = "Inventory (editable):";
            // 
            // lblDockingAccessValue
            // 
            lblDockingAccessValue.AutoSize = true;
            lblDockingAccessValue.Location = new Point(203, 3);
            lblDockingAccessValue.Margin = new Padding(3, 3, 3, 0);
            lblDockingAccessValue.Name = "lblDockingAccessValue";
            lblDockingAccessValue.Size = new Size(102, 15);
            lblDockingAccessValue.TabIndex = 1;
            lblDockingAccessValue.Text = "<dockingAccess>";
            // 
            // lblCapacityFreeValue
            // 
            lblCapacityFreeValue.AutoSize = true;
            lblCapacityFreeValue.Location = new Point(603, 27);
            lblCapacityFreeValue.Margin = new Padding(3, 3, 3, 0);
            lblCapacityFreeValue.Name = "lblCapacityFreeValue";
            lblCapacityFreeValue.Size = new Size(107, 15);
            lblCapacityFreeValue.TabIndex = 8;
            lblCapacityFreeValue.Text = "<capacity / free> T";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 3);
            label1.Margin = new Padding(3, 3, 3, 0);
            label1.Name = "label1";
            label1.Size = new Size(93, 15);
            label1.TabIndex = 0;
            label1.Text = "Docking Access:";
            // 
            // tlblCargo
            // 
            tlblCargo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlblCargo.AutoSize = true;
            tlblCargo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblCargo.ImageColor = Color.Transparent;
            tlblCargo.LabelPosition = PluginCommon.UI.Shared.LabelPositionType.Right;
            tlblCargo.Location = new Point(203, 27);
            tlblCargo.Name = "tlblCargo";
            tlblCargo.Size = new Size(194, 15);
            tlblCargo.TabIndex = 9;
            tlblCargo.Text = "<cargo> T";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(403, 27);
            label6.Margin = new Padding(3, 3, 3, 0);
            label6.Name = "label6";
            label6.Size = new Size(89, 15);
            label6.TabIndex = 7;
            label6.Text = "Capacity / Free:";
            // 
            // invSplitter
            // 
            tableLayoutPanel1.SetColumnSpan(invSplitter, 4);
            invSplitter.Dock = DockStyle.Fill;
            invSplitter.Location = new Point(3, 75);
            invSplitter.Name = "invSplitter";
            // 
            // invSplitter.Panel1
            // 
            invSplitter.Panel1.Controls.Add(dgvInventory);
            // 
            // invSplitter.Panel2
            // 
            invSplitter.Panel2.Controls.Add(dgvTradeOrders);
            invSplitter.Size = new Size(794, 283);
            invSplitter.SplitterDistance = 378;
            invSplitter.TabIndex = 0;
            // 
            // dgvInventory
            // 
            dgvInventory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInventory.Columns.AddRange(new DataGridViewColumn[] { colQty, colItem });
            dgvInventory.Dock = DockStyle.Fill;
            dgvInventory.EnableHeadersVisualStyles = false;
            dgvInventory.Location = new Point(0, 0);
            dgvInventory.Name = "dgvInventory";
            dgvInventory.RowHeadersWidth = 35;
            dgvInventory.Size = new Size(378, 283);
            dgvInventory.TabIndex = 0;
            // 
            // colQty
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            colQty.DefaultCellStyle = dataGridViewCellStyle1;
            colQty.HeaderText = "Qty";
            colQty.Name = "colQty";
            colQty.Width = 75;
            // 
            // colItem
            // 
            colItem.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colItem.HeaderText = "Commodity";
            colItem.Name = "colItem";
            colItem.Resizable = DataGridViewTriState.True;
            colItem.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // dgvTradeOrders
            // 
            dgvTradeOrders.AllowUserToAddRows = false;
            dgvTradeOrders.AllowUserToDeleteRows = false;
            dgvTradeOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTradeOrders.Columns.AddRange(new DataGridViewColumn[] { colTradeType, colTradeQty, colTradeItemName, colBlkMarket });
            dgvTradeOrders.Dock = DockStyle.Fill;
            dgvTradeOrders.EnableHeadersVisualStyles = false;
            dgvTradeOrders.Location = new Point(0, 0);
            dgvTradeOrders.Name = "dgvTradeOrders";
            dgvTradeOrders.ReadOnly = true;
            dgvTradeOrders.RowHeadersWidth = 10;
            dgvTradeOrders.Size = new Size(412, 283);
            dgvTradeOrders.TabIndex = 0;
            // 
            // colTradeType
            // 
            colTradeType.HeaderText = "Type";
            colTradeType.Name = "colTradeType";
            colTradeType.ReadOnly = true;
            colTradeType.Resizable = DataGridViewTriState.True;
            colTradeType.SortMode = DataGridViewColumnSortMode.NotSortable;
            colTradeType.Width = 85;
            // 
            // colTradeQty
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = null;
            colTradeQty.DefaultCellStyle = dataGridViewCellStyle2;
            colTradeQty.HeaderText = "Qty";
            colTradeQty.Name = "colTradeQty";
            colTradeQty.ReadOnly = true;
            colTradeQty.Resizable = DataGridViewTriState.True;
            colTradeQty.SortMode = DataGridViewColumnSortMode.NotSortable;
            colTradeQty.Width = 75;
            // 
            // colTradeItemName
            // 
            colTradeItemName.HeaderText = "Commodity";
            colTradeItemName.Name = "colTradeItemName";
            colTradeItemName.ReadOnly = true;
            colTradeItemName.Width = 300;
            // 
            // colBlkMarket
            // 
            colBlkMarket.HeaderText = "Blk Mkt?";
            colBlkMarket.Name = "colBlkMarket";
            colBlkMarket.ReadOnly = true;
            colBlkMarket.ToolTipText = "Is Black Market?";
            colBlkMarket.Width = 90;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label4.Location = new Point(717, 51);
            label4.Margin = new Padding(3, 3, 3, 0);
            label4.Name = "label4";
            label4.Size = new Size(80, 16);
            label4.TabIndex = 3;
            label4.Text = "Trade Orders";
            tTips.SetToolTip(label4, "To remove a Sell order when sold out, convert to a buy order, then cancel the buy order.");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 27);
            label2.Margin = new Padding(3, 3, 3, 0);
            label2.Name = "label2";
            label2.Size = new Size(129, 15);
            label2.TabIndex = 6;
            label2.Text = "Cargo Used / Reserved:";
            tTips.SetToolTip(label2, "Reserved refers to capacity blocked for buy orders.");
            // 
            // txtMessages
            // 
            tableLayoutPanel1.SetColumnSpan(txtMessages, 3);
            txtMessages.Dock = DockStyle.Fill;
            txtMessages.Location = new Point(203, 364);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ReadOnly = true;
            txtMessages.ScrollBars = ScrollBars.Vertical;
            txtMessages.Size = new Size(594, 79);
            txtMessages.TabIndex = 12;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(125, 364);
            label3.Margin = new Padding(3, 3, 3, 0);
            label3.Name = "label3";
            label3.Size = new Size(72, 15);
            label3.TabIndex = 13;
            label3.Text = "Instructions:";
            // 
            // InventoryForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 446);
            Controls.Add(tableLayoutPanel1);
            DoubleBuffered = true;
            Name = "InventoryForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "<carrier>: Inventory";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            invSplitter.Panel1.ResumeLayout(false);
            invSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)invSplitter).EndInit();
            invSplitter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvInventory).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvTradeOrders).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private SplitContainer invSplitter;
        private DataGridView dgvInventory;
        private DataGridView dgvTradeOrders;
        private Label lblDockingAccessValue;
        private Label label1;
        private Label label4;
        private Label lblLastUpdatedValue;
        private Label label5;
        private Label label6;
        private Label label2;
        private Label lblCapacityFreeValue;
        private PluginCommon.UI.ThemeableImageLabel tlblCargo;
        private ToolTip tTips;
        private Label lblKnownCargoUsageValue;
        private DataGridViewTextBoxColumn colQty;
        private DataGridViewComboBoxColumn colItem;
        private DataGridViewTextBoxColumn colTradeType;
        private DataGridViewTextBoxColumn colTradeQty;
        private DataGridViewTextBoxColumn colTradeItemName;
        private DataGridViewCheckBoxColumn colBlkMarket;
        private PluginCommon.UI.ThemeableImageLabel tlblInventory;
        private TextBox txtMessages;
        private Label label3;
    }
}