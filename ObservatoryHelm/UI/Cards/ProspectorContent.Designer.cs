using com.github.fredjk_gh.PluginCommon.UI.Shared;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    partial class ProspectorContent
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
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            tlblLimpetStatus = new com.github.fredjk_gh.PluginCommon.UI.ThemeableImageLabel();
            dgvProspectorHistory = new DataGridView();
            tableLayoutPanel1 = new TableLayoutPanel();
            colTimestamp = new DataGridViewTextBoxColumn();
            colMaterials = new com.github.fredjk_gh.PluginCommon.UI.DGVImageLabelColumn();
            colMatContent = new DataGridViewTextBoxColumn();
            colCore = new com.github.fredjk_gh.PluginCommon.UI.DGVImageLabelColumn();
            ((System.ComponentModel.ISupportInitialize)dgvProspectorHistory).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tlblLimpetStatus
            // 
            tlblLimpetStatus.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tlblLimpetStatus.AutoSize = true;
            tlblLimpetStatus.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblLimpetStatus.ImageColor = Color.Transparent;
            tlblLimpetStatus.LabelPosition = LabelPositionType.Left;
            tlblLimpetStatus.Location = new Point(3, 7);
            tlblLimpetStatus.Name = "tlblLimpetStatus";
            tlblLimpetStatus.Size = new Size(426, 15);
            tlblLimpetStatus.TabIndex = 1;
            tlblLimpetStatus.Text = "Limpets aboard: 0";
            // 
            // dgvProspectorHistory
            // 
            dgvProspectorHistory.AllowUserToAddRows = false;
            dgvProspectorHistory.AllowUserToDeleteRows = false;
            dgvProspectorHistory.AllowUserToOrderColumns = true;
            dgvProspectorHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvProspectorHistory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvProspectorHistory.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvProspectorHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProspectorHistory.Columns.AddRange(new DataGridViewColumn[] { colTimestamp, colMaterials, colMatContent, colCore });
            dgvProspectorHistory.Dock = DockStyle.Fill;
            dgvProspectorHistory.EnableHeadersVisualStyles = false;
            dgvProspectorHistory.Location = new Point(3, 33);
            dgvProspectorHistory.Name = "dgvProspectorHistory";
            dgvProspectorHistory.ReadOnly = true;
            dgvProspectorHistory.RowHeadersVisible = false;
            dgvProspectorHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProspectorHistory.Size = new Size(426, 100);
            dgvProspectorHistory.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(tlblLimpetStatus, 0, 0);
            tableLayoutPanel1.Controls.Add(dgvProspectorHistory, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(432, 136);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // colTimestamp
            // 
            colTimestamp.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTimestamp.FillWeight = 50F;
            colTimestamp.HeaderText = "Time";
            colTimestamp.Name = "colTimestamp";
            colTimestamp.ReadOnly = true;
            // 
            // colMaterials
            // 
            colMaterials.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.NullValue = null;
            colMaterials.DefaultCellStyle = dataGridViewCellStyle1;
            colMaterials.HeaderText = "Commodities";
            colMaterials.Name = "colMaterials";
            colMaterials.ReadOnly = true;
            colMaterials.Resizable = DataGridViewTriState.True;
            colMaterials.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // colMatContent
            // 
            colMatContent.HeaderText = "Mat Content";
            colMatContent.Name = "colMatContent";
            colMatContent.ReadOnly = true;
            colMatContent.Width = 99;
            // 
            // colCore
            // 
            colCore.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.NullValue = null;
            colCore.DefaultCellStyle = dataGridViewCellStyle2;
            colCore.FillWeight = 50F;
            colCore.HeaderText = "Core";
            colCore.Name = "colCore";
            colCore.ReadOnly = true;
            colCore.Resizable = DataGridViewTriState.True;
            colCore.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // ProspectorContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ContentTitle = "Prospecting";
            Controls.Add(tableLayoutPanel1);
            Name = "ProspectorContent";
            Size = new Size(432, 136);
            ((System.ComponentModel.ISupportInitialize)dgvProspectorHistory).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private com.github.fredjk_gh.PluginCommon.UI.ThemeableImageLabel tlblLimpetStatus;
        private DataGridView dgvProspectorHistory;
        private TableLayoutPanel tableLayoutPanel1;
        private DataGridViewTextBoxColumn colTimestamp;
        private PluginCommon.UI.DGVImageLabelColumn colMaterials;
        private DataGridViewTextBoxColumn colMatContent;
        private PluginCommon.UI.DGVImageLabelColumn colCore;
    }
}
