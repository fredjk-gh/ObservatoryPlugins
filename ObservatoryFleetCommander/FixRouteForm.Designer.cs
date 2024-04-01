namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    partial class FixRouteForm
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
            tableLayoutPanel1 = new TableLayoutPanel();
            lblCurrentSystem = new Label();
            lblNextJump = new Label();
            cbCurrentSystem = new ComboBox();
            txtNextJumpSystem = new TextBox();
            btnApply = new Button();
            lblCommander = new Label();
            cbCommander = new ComboBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(lblCurrentSystem, 0, 1);
            tableLayoutPanel1.Controls.Add(lblNextJump, 0, 2);
            tableLayoutPanel1.Controls.Add(cbCurrentSystem, 1, 1);
            tableLayoutPanel1.Controls.Add(txtNextJumpSystem, 1, 2);
            tableLayoutPanel1.Controls.Add(btnApply, 0, 3);
            tableLayoutPanel1.Controls.Add(lblCommander, 0, 0);
            tableLayoutPanel1.Controls.Add(cbCommander, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(524, 244);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // lblCurrentSystem
            // 
            lblCurrentSystem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblCurrentSystem.AutoSize = true;
            lblCurrentSystem.Location = new Point(49, 61);
            lblCurrentSystem.Name = "lblCurrentSystem";
            lblCurrentSystem.Size = new Size(210, 61);
            lblCurrentSystem.TabIndex = 0;
            lblCurrentSystem.Text = "Where is the carrier now?";
            lblCurrentSystem.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblNextJump
            // 
            lblNextJump.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblNextJump.AutoSize = true;
            lblNextJump.Location = new Point(47, 122);
            lblNextJump.Name = "lblNextJump";
            lblNextJump.Size = new Size(212, 61);
            lblNextJump.TabIndex = 1;
            lblNextJump.Text = "Next Jump (auto-copied):";
            lblNextJump.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbCurrentSystem
            // 
            cbCurrentSystem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbCurrentSystem.FormattingEnabled = true;
            cbCurrentSystem.Location = new Point(265, 76);
            cbCurrentSystem.Margin = new Padding(3, 15, 5, 3);
            cbCurrentSystem.Name = "cbCurrentSystem";
            cbCurrentSystem.Size = new Size(254, 33);
            cbCurrentSystem.TabIndex = 2;
            cbCurrentSystem.SelectedIndexChanged += cbCurrentSystem_SelectedIndexChanged;
            // 
            // txtNextJumpSystem
            // 
            txtNextJumpSystem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtNextJumpSystem.Location = new Point(265, 137);
            txtNextJumpSystem.Margin = new Padding(3, 15, 5, 3);
            txtNextJumpSystem.Name = "txtNextJumpSystem";
            txtNextJumpSystem.ReadOnly = true;
            txtNextJumpSystem.Size = new Size(254, 31);
            txtNextJumpSystem.TabIndex = 3;
            txtNextJumpSystem.MouseDoubleClick += txtNextJumpSystem_MouseDoubleClick;
            // 
            // btnApply
            // 
            btnApply.Anchor = AnchorStyles.Top;
            tableLayoutPanel1.SetColumnSpan(btnApply, 2);
            btnApply.DialogResult = DialogResult.OK;
            btnApply.Location = new Point(212, 198);
            btnApply.Margin = new Padding(3, 15, 3, 3);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(100, 34);
            btnApply.TabIndex = 4;
            btnApply.Text = "Apply";
            btnApply.UseVisualStyleBackColor = true;
            btnApply.Click += btnApply_Click;
            // 
            // lblCommander
            // 
            lblCommander.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblCommander.AutoSize = true;
            lblCommander.Location = new Point(93, 0);
            lblCommander.Name = "lblCommander";
            lblCommander.Size = new Size(166, 61);
            lblCommander.TabIndex = 0;
            lblCommander.Text = "Select Commander:";
            lblCommander.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbCommander
            // 
            cbCommander.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbCommander.FormattingEnabled = true;
            cbCommander.Location = new Point(265, 15);
            cbCommander.Margin = new Padding(3, 15, 5, 3);
            cbCommander.Name = "cbCommander";
            cbCommander.Size = new Size(254, 33);
            cbCommander.TabIndex = 6;
            cbCommander.SelectedIndexChanged += cbCommander_SelectedIndexChanged;
            // 
            // FixRouteForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 244);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "FixRouteForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Fix Route";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label lblCurrentSystem;
        private Label lblNextJump;
        private ComboBox cbCurrentSystem;
        private TextBox txtNextJumpSystem;
        private Button btnApply;
        private Label lblCommander;
        private ComboBox cbCommander;
    }
}