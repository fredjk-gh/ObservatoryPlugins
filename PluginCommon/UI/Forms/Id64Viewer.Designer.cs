namespace com.github.fredjk_gh.PluginCommon.UI.Forms
{
    partial class Id64Viewer
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
            dgvId64 = new DataGridView();
            colName = new DataGridViewTextBoxColumn();
            colValue = new DataGridViewTextBoxColumn();
            lblTitle = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvId64).BeginInit();
            SuspendLayout();
            // 
            // dgvId64
            // 
            dgvId64.AllowUserToAddRows = false;
            dgvId64.AllowUserToDeleteRows = false;
            dgvId64.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvId64.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvId64.Columns.AddRange(new DataGridViewColumn[] { colName, colValue });
            dgvId64.EnableHeadersVisualStyles = false;
            dgvId64.Location = new Point(8, 32);
            dgvId64.Name = "dgvId64";
            dgvId64.ReadOnly = true;
            dgvId64.RowHeadersWidth = 10;
            dgvId64.Size = new Size(504, 489);
            dgvId64.TabIndex = 0;
            // 
            // colName
            // 
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colName.HeaderText = "Name";
            colName.Name = "colName";
            colName.ReadOnly = true;
            // 
            // colValue
            // 
            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colValue.HeaderText = "Value";
            colValue.Name = "colValue";
            colValue.ReadOnly = true;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(8, 8);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(142, 15);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "ID65 Details for <system>";
            // 
            // Id64Viewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(522, 530);
            Controls.Add(lblTitle);
            Controls.Add(dgvId64);
            Name = "Id64Viewer";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Id64 Viewer";
            ((System.ComponentModel.ISupportInitialize)dgvId64).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvId64;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colValue;
        private Label lblTitle;
    }
}