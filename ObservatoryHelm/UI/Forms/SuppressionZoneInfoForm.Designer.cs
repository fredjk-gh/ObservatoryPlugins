namespace com.github.fredjk_gh.ObservatoryHelm.UI.Forms
{
    partial class SuppressionZoneInfoForm
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
            label1 = new Label();
            dgvZoneInfo = new DataGridView();
            colName = new DataGridViewTextBoxColumn();
            colRules = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvZoneInfo).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 8);
            label1.Name = "label1";
            label1.Size = new Size(440, 15);
            label1.TabIndex = 0;
            label1.Text = "Please find information about how various zones are calculated in the table below.";
            // 
            // dgvZoneInfo
            // 
            dgvZoneInfo.AllowUserToAddRows = false;
            dgvZoneInfo.AllowUserToDeleteRows = false;
            dgvZoneInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvZoneInfo.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvZoneInfo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvZoneInfo.Columns.AddRange(new DataGridViewColumn[] { colName, colRules });
            dgvZoneInfo.EnableHeadersVisualStyles = false;
            dgvZoneInfo.Location = new Point(8, 32);
            dgvZoneInfo.Name = "dgvZoneInfo";
            dgvZoneInfo.ReadOnly = true;
            dgvZoneInfo.RowHeadersVisible = false;
            dgvZoneInfo.Size = new Size(784, 408);
            dgvZoneInfo.TabIndex = 1;
            // 
            // colName
            // 
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colName.FillWeight = 200F;
            colName.HeaderText = "Object";
            colName.Name = "colName";
            colName.ReadOnly = true;
            // 
            // colRules
            // 
            colRules.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colRules.FillWeight = 400F;
            colRules.HeaderText = "Rules";
            colRules.Name = "colRules";
            colRules.ReadOnly = true;
            // 
            // SuppressionZoneInfoForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgvZoneInfo);
            Controls.Add(label1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "SuppressionZoneInfoForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Suppression Zone Info";
            ((System.ComponentModel.ISupportInitialize)dgvZoneInfo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView dgvZoneInfo;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colRules;
    }
}