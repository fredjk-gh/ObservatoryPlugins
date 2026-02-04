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
            components = new System.ComponentModel.Container();
            tlblLimpetStatus = new com.github.fredjk_gh.PluginCommon.UI.ThemeableImageLabel();
            ttManager = new ToolTip(components);
            dgvProspectorHistory = new DataGridView();
            colTimestamp = new DataGridViewTextBoxColumn();
            colMaterials = new DataGridViewTextBoxColumn();
            colCore = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvProspectorHistory).BeginInit();
            SuspendLayout();
            // 
            // tlblLimpetStatus
            // 
            tlblLimpetStatus.AutoSize = true;
            tlblLimpetStatus.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlblLimpetStatus.ImageColor = Color.Transparent;
            tlblLimpetStatus.LabelPosition = LabelPositionType.Left;
            tlblLimpetStatus.Location = new Point(8, 8);
            tlblLimpetStatus.Name = "tlblLimpetStatus";
            tlblLimpetStatus.Size = new Size(101, 15);
            tlblLimpetStatus.TabIndex = 1;
            tlblLimpetStatus.Text = "Limpets aboard: 0";
            // 
            // dgvProspectorHistory
            // 
            dgvProspectorHistory.AllowUserToAddRows = false;
            dgvProspectorHistory.AllowUserToDeleteRows = false;
            dgvProspectorHistory.AllowUserToOrderColumns = true;
            dgvProspectorHistory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvProspectorHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvProspectorHistory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvProspectorHistory.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvProspectorHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProspectorHistory.Columns.AddRange(new DataGridViewColumn[] { colTimestamp, colMaterials, colCore });
            dgvProspectorHistory.EnableHeadersVisualStyles = false;
            dgvProspectorHistory.Location = new Point(8, 32);
            dgvProspectorHistory.Name = "dgvProspectorHistory";
            dgvProspectorHistory.ReadOnly = true;
            dgvProspectorHistory.RowHeadersVisible = false;
            dgvProspectorHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProspectorHistory.Size = new Size(464, 168);
            dgvProspectorHistory.TabIndex = 2;
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
            colMaterials.HeaderText = "Materials";
            colMaterials.Name = "colMaterials";
            colMaterials.ReadOnly = true;
            // 
            // colCore
            // 
            colCore.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colCore.FillWeight = 50F;
            colCore.HeaderText = "Core";
            colCore.Name = "colCore";
            colCore.ReadOnly = true;
            // 
            // ProspectorContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ContentTitle = "Prospecting";
            Controls.Add(dgvProspectorHistory);
            Controls.Add(tlblLimpetStatus);
            Name = "ProspectorContent";
            Size = new Size(482, 208);
            ((System.ComponentModel.ISupportInitialize)dgvProspectorHistory).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private com.github.fredjk_gh.PluginCommon.UI.ThemeableImageLabel tlblLimpetStatus;
        private ToolTip ttManager;
        private DataGridView dgvProspectorHistory;
        private DataGridViewTextBoxColumn colTimestamp;
        private DataGridViewTextBoxColumn colMaterials;
        private DataGridViewTextBoxColumn colCore;
    }
}
