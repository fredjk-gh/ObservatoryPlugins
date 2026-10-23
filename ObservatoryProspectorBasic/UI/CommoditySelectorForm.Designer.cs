namespace com.github.fredjk_gh.ObservatoryProspectorBasic.UI
{
    partial class CommoditySelectorForm
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
            tlpLayout = new TableLayoutPanel();
            btnClose = new Button();
            txtNameFilter = new TextBox();
            cboMiningMethod = new ComboBox();
            lvCommodityDetails = new ListView();
            colName = new ColumnHeader();
            colType = new ColumnHeader();
            colMiningMethods = new ColumnHeader();
            colRingTypes = new ColumnHeader();
            colBodyTypes = new ColumnHeader();
            lblItemsSelected = new Label();
            btnClearFilters = new Button();
            txtRingBodyTypeFilter = new TextBox();
            btnClearSelection = new Button();
            ttManager = new ToolTip(components);
            tlpLayout.SuspendLayout();
            SuspendLayout();
            // 
            // tlpLayout
            // 
            tlpLayout.ColumnCount = 4;
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpLayout.Controls.Add(btnClose, 3, 2);
            tlpLayout.Controls.Add(txtNameFilter, 0, 0);
            tlpLayout.Controls.Add(cboMiningMethod, 1, 0);
            tlpLayout.Controls.Add(lvCommodityDetails, 0, 1);
            tlpLayout.Controls.Add(lblItemsSelected, 0, 2);
            tlpLayout.Controls.Add(btnClearFilters, 3, 0);
            tlpLayout.Controls.Add(txtRingBodyTypeFilter, 2, 0);
            tlpLayout.Controls.Add(btnClearSelection, 1, 2);
            tlpLayout.Dock = DockStyle.Fill;
            tlpLayout.Location = new Point(0, 0);
            tlpLayout.Name = "tlpLayout";
            tlpLayout.RowCount = 3;
            tlpLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpLayout.Size = new Size(1026, 450);
            tlpLayout.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(948, 424);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 23);
            btnClose.TabIndex = 0;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // txtNameFilter
            // 
            txtNameFilter.Dock = DockStyle.Bottom;
            txtNameFilter.Location = new Point(3, 14);
            txtNameFilter.Name = "txtNameFilter";
            txtNameFilter.PlaceholderText = "Type to filter by name";
            txtNameFilter.Size = new Size(250, 23);
            txtNameFilter.TabIndex = 2;
            ttManager.SetToolTip(txtNameFilter, "Filter commodity name");
            txtNameFilter.TextChanged += txtFilter_TextChanged;
            // 
            // cboMiningMethod
            // 
            cboMiningMethod.Dock = DockStyle.Bottom;
            cboMiningMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMiningMethod.FlatStyle = FlatStyle.Flat;
            cboMiningMethod.FormattingEnabled = true;
            cboMiningMethod.Location = new Point(259, 14);
            cboMiningMethod.Name = "cboMiningMethod";
            cboMiningMethod.Size = new Size(250, 23);
            cboMiningMethod.TabIndex = 3;
            ttManager.SetToolTip(cboMiningMethod, "Filter primary mining method");
            cboMiningMethod.SelectedIndexChanged += cboMiningMethod_SelectedIndexChanged;
            // 
            // lvCommodityDetails
            // 
            lvCommodityDetails.CheckBoxes = true;
            lvCommodityDetails.Columns.AddRange(new ColumnHeader[] { colName, colType, colMiningMethods, colRingTypes, colBodyTypes });
            tlpLayout.SetColumnSpan(lvCommodityDetails, 4);
            lvCommodityDetails.Dock = DockStyle.Fill;
            lvCommodityDetails.FullRowSelect = true;
            lvCommodityDetails.Location = new Point(3, 43);
            lvCommodityDetails.Name = "lvCommodityDetails";
            lvCommodityDetails.Size = new Size(1020, 364);
            lvCommodityDetails.TabIndex = 4;
            lvCommodityDetails.UseCompatibleStateImageBehavior = false;
            lvCommodityDetails.View = View.Details;
            lvCommodityDetails.ItemChecked += lvCommodityDetails_ItemChecked;
            // 
            // colName
            // 
            colName.Text = "Name";
            colName.Width = 200;
            // 
            // colType
            // 
            colType.Text = "Type";
            colType.Width = 100;
            // 
            // colMiningMethods
            // 
            colMiningMethods.Text = "Mining Method(s)";
            colMiningMethods.Width = 200;
            // 
            // colRingTypes
            // 
            colRingTypes.Text = "Ring Type(s)";
            colRingTypes.Width = 300;
            // 
            // colBodyTypes
            // 
            colBodyTypes.Text = "Body Type(s)";
            colBodyTypes.Width = 300;
            // 
            // lblItemsSelected
            // 
            lblItemsSelected.Anchor = AnchorStyles.Right;
            lblItemsSelected.AutoSize = true;
            lblItemsSelected.Location = new Point(113, 422);
            lblItemsSelected.Name = "lblItemsSelected";
            lblItemsSelected.Size = new Size(140, 15);
            lblItemsSelected.TabIndex = 5;
            lblItemsSelected.Text = "{0} commodities selected";
            // 
            // btnClearFilters
            // 
            btnClearFilters.Anchor = AnchorStyles.Bottom;
            btnClearFilters.FlatAppearance.BorderSize = 0;
            btnClearFilters.FlatStyle = FlatStyle.Flat;
            btnClearFilters.Location = new Point(859, 14);
            btnClearFilters.Name = "btnClearFilters";
            btnClearFilters.Size = new Size(75, 23);
            btnClearFilters.TabIndex = 6;
            btnClearFilters.Text = "Clear filters";
            btnClearFilters.UseVisualStyleBackColor = true;
            btnClearFilters.Click += btnClearFilters_Click;
            // 
            // txtRingBodyTypeFilter
            // 
            txtRingBodyTypeFilter.Dock = DockStyle.Bottom;
            txtRingBodyTypeFilter.Location = new Point(515, 14);
            txtRingBodyTypeFilter.Name = "txtRingBodyTypeFilter";
            txtRingBodyTypeFilter.PlaceholderText = "Type to filter by ring/body type";
            txtRingBodyTypeFilter.Size = new Size(250, 23);
            txtRingBodyTypeFilter.TabIndex = 7;
            ttManager.SetToolTip(txtRingBodyTypeFilter, "Filter ring or body type");
            txtRingBodyTypeFilter.TextChanged += txtFilter_TextChanged;
            // 
            // btnClearSelection
            // 
            btnClearSelection.Anchor = AnchorStyles.Left;
            btnClearSelection.FlatAppearance.BorderSize = 0;
            btnClearSelection.FlatStyle = FlatStyle.Flat;
            btnClearSelection.Location = new Point(259, 418);
            btnClearSelection.Name = "btnClearSelection";
            btnClearSelection.Size = new Size(109, 23);
            btnClearSelection.TabIndex = 8;
            btnClearSelection.Text = "Clear Selection";
            btnClearSelection.UseVisualStyleBackColor = true;
            btnClearSelection.Click += btnClearSelection_Click;
            // 
            // CommoditySelectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1026, 450);
            Controls.Add(tlpLayout);
            DoubleBuffered = true;
            Name = "CommoditySelectorForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Commodity Watch Selector";
            FormClosing += CommoditySelectorForm_FormClosing;
            tlpLayout.ResumeLayout(false);
            tlpLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpLayout;
        private Button btnClose;
        private TextBox txtNameFilter;
        private ComboBox cboMiningMethod;
        private ListView lvCommodityDetails;
        private ToolTip ttManager;
        private ColumnHeader colName;
        private ColumnHeader colType;
        private ColumnHeader colMiningMethods;
        private ColumnHeader colRingTypes;
        private ColumnHeader colBodyTypes;
        private Label lblItemsSelected;
        private Button btnClearFilters;
        private TextBox txtRingBodyTypeFilter;
        private Button btnClearSelection;
    }
}