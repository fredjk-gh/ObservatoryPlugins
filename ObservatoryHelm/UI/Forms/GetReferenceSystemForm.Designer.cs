namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    partial class GetReferenceSystemForm
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
            label2 = new Label();
            txtSystemName = new TextBox();
            txtId64 = new TextBox();
            btnLookup = new Button();
            label3 = new Label();
            txtX = new TextBox();
            txtY = new TextBox();
            txtZ = new TextBox();
            btnSet = new Button();
            lblLine = new Label();
            lblArchivistRequired = new Label();
            SuspendLayout();
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
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 40);
            label2.Name = "label2";
            label2.Size = new Size(87, 15);
            label2.TabIndex = 1;
            label2.Text = "Id64 (optional):";
            // 
            // txtSystemName
            // 
            txtSystemName.Location = new Point(128, 8);
            txtSystemName.Name = "txtSystemName";
            txtSystemName.Size = new Size(232, 23);
            txtSystemName.TabIndex = 2;
            txtSystemName.TextChanged += TextBox_TextChanged;
            // 
            // txtId64
            // 
            txtId64.Location = new Point(128, 40);
            txtId64.Name = "txtId64";
            txtId64.Size = new Size(232, 23);
            txtId64.TabIndex = 3;
            txtId64.TextChanged += TextBox_TextChanged;
            // 
            // btnLookup
            // 
            btnLookup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLookup.AutoSize = true;
            btnLookup.Location = new Point(277, 72);
            btnLookup.Name = "btnLookup";
            btnLookup.Size = new Size(80, 32);
            btnLookup.TabIndex = 4;
            btnLookup.Text = "Lookup";
            btnLookup.UseVisualStyleBackColor = true;
            btnLookup.Click += BtnLookup_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 128);
            label3.Name = "label3";
            label3.Size = new Size(114, 15);
            label3.TabIndex = 5;
            label3.Text = "Coordinates (x, y, z):";
            // 
            // txtX
            // 
            txtX.Location = new Point(128, 128);
            txtX.Name = "txtX";
            txtX.PlaceholderText = "x";
            txtX.Size = new Size(72, 23);
            txtX.TabIndex = 6;
            txtX.TextChanged += TextBox_TextChanged;
            // 
            // txtY
            // 
            txtY.Location = new Point(208, 128);
            txtY.Name = "txtY";
            txtY.PlaceholderText = "y";
            txtY.Size = new Size(72, 23);
            txtY.TabIndex = 7;
            txtY.TextChanged += TextBox_TextChanged;
            // 
            // txtZ
            // 
            txtZ.Location = new Point(288, 128);
            txtZ.Name = "txtZ";
            txtZ.PlaceholderText = "z";
            txtZ.Size = new Size(72, 23);
            txtZ.TabIndex = 8;
            txtZ.TextChanged += TextBox_TextChanged;
            // 
            // btnSet
            // 
            btnSet.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSet.AutoSize = true;
            btnSet.Location = new Point(277, 160);
            btnSet.Name = "btnSet";
            btnSet.Size = new Size(80, 32);
            btnSet.TabIndex = 9;
            btnSet.Text = "Set";
            btnSet.UseVisualStyleBackColor = true;
            btnSet.Click += BtnSet_Click;
            // 
            // lblLine
            // 
            lblLine.BorderStyle = BorderStyle.Fixed3D;
            lblLine.Location = new Point(8, 112);
            lblLine.Name = "lblLine";
            lblLine.Size = new Size(352, 2);
            lblLine.TabIndex = 10;
            // 
            // lblArchivistRequired
            // 
            lblArchivistRequired.AutoSize = true;
            lblArchivistRequired.Location = new Point(8, 80);
            lblArchivistRequired.Name = "lblArchivistRequired";
            lblArchivistRequired.Size = new Size(201, 15);
            lblArchivistRequired.TabIndex = 11;
            lblArchivistRequired.Text = "Lookup requires the Archivist plugin.";
            // 
            // GetReferenceSystemForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 200);
            Controls.Add(lblArchivistRequired);
            Controls.Add(lblLine);
            Controls.Add(btnSet);
            Controls.Add(txtZ);
            Controls.Add(txtY);
            Controls.Add(txtX);
            Controls.Add(label3);
            Controls.Add(btnLookup);
            Controls.Add(txtId64);
            Controls.Add(txtSystemName);
            Controls.Add(label2);
            Controls.Add(label1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "GetReferenceSystemForm";
            ShowIcon = false;
            Text = "Set Reference System";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox txtSystemName;
        private TextBox txtId64;
        private Button btnLookup;
        private Label label3;
        private TextBox txtX;
        private TextBox txtY;
        private TextBox txtZ;
        private Button btnSet;
        private Label lblLine;
        private Label lblArchivistRequired;
    }
}