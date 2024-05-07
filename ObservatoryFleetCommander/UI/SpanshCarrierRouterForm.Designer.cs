namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    partial class SpanshCarrierRouterForm
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
            mainLayoutPanel = new TableLayoutPanel();
            lblCommander = new Label();
            cbCommanders = new ComboBox();
            lblCarrier = new Label();
            lblCarrierNameAndId = new Label();
            lblStartSystem = new Label();
            txtStartSystem = new TextBox();
            lblDestinationSystem = new Label();
            txtDestSystem = new TextBox();
            lblUsedCapacity = new Label();
            txtOutput = new TextBox();
            buttonLayoutPanel = new FlowLayoutPanel();
            btnSaveRoute = new Button();
            btnClearRoute = new Button();
            btnCancel = new Button();
            nudUsedCapacity = new NumericUpDown();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnGenerateRoute = new Button();
            mainLayoutPanel.SuspendLayout();
            buttonLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudUsedCapacity).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.ColumnCount = 2;
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.9597511F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 69.04025F));
            mainLayoutPanel.Controls.Add(lblCommander, 0, 0);
            mainLayoutPanel.Controls.Add(cbCommanders, 1, 0);
            mainLayoutPanel.Controls.Add(lblCarrier, 0, 1);
            mainLayoutPanel.Controls.Add(lblCarrierNameAndId, 1, 1);
            mainLayoutPanel.Controls.Add(lblStartSystem, 0, 2);
            mainLayoutPanel.Controls.Add(txtStartSystem, 1, 2);
            mainLayoutPanel.Controls.Add(lblDestinationSystem, 0, 3);
            mainLayoutPanel.Controls.Add(txtDestSystem, 1, 3);
            mainLayoutPanel.Controls.Add(lblUsedCapacity, 0, 4);
            mainLayoutPanel.Controls.Add(txtOutput, 1, 5);
            mainLayoutPanel.Controls.Add(buttonLayoutPanel, 1, 6);
            mainLayoutPanel.Controls.Add(nudUsedCapacity, 1, 4);
            mainLayoutPanel.Controls.Add(flowLayoutPanel1, 0, 5);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.Location = new Point(0, 0);
            mainLayoutPanel.Margin = new Padding(2);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.RowCount = 7;
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 12F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 12F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 12F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 12F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 12F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 27F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 13F));
            mainLayoutPanel.Size = new Size(458, 281);
            mainLayoutPanel.TabIndex = 0;
            // 
            // lblCommander
            // 
            lblCommander.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblCommander.AutoSize = true;
            lblCommander.Location = new Point(62, 0);
            lblCommander.Margin = new Padding(2, 0, 2, 0);
            lblCommander.Name = "lblCommander";
            lblCommander.Size = new Size(77, 33);
            lblCommander.TabIndex = 0;
            lblCommander.Text = "Commander:";
            lblCommander.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbCommanders
            // 
            cbCommanders.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbCommanders.FormattingEnabled = true;
            cbCommanders.Location = new Point(143, 6);
            cbCommanders.Margin = new Padding(2, 6, 2, 2);
            cbCommanders.Name = "cbCommanders";
            cbCommanders.Size = new Size(313, 23);
            cbCommanders.TabIndex = 1;
            cbCommanders.SelectedIndexChanged += cbCommanders_SelectedIndexChanged;
            // 
            // lblCarrier
            // 
            lblCarrier.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblCarrier.AutoSize = true;
            lblCarrier.Location = new Point(94, 33);
            lblCarrier.Margin = new Padding(2, 0, 2, 0);
            lblCarrier.Name = "lblCarrier";
            lblCarrier.Size = new Size(45, 33);
            lblCarrier.TabIndex = 2;
            lblCarrier.Text = "Carrier:";
            lblCarrier.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblCarrierNameAndId
            // 
            lblCarrierNameAndId.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblCarrierNameAndId.AutoSize = true;
            lblCarrierNameAndId.Location = new Point(143, 33);
            lblCarrierNameAndId.Margin = new Padding(2, 0, 2, 0);
            lblCarrierNameAndId.Name = "lblCarrierNameAndId";
            lblCarrierNameAndId.Size = new Size(116, 33);
            lblCarrierNameAndId.TabIndex = 3;
            lblCarrierNameAndId.Text = "(Select Commander)";
            lblCarrierNameAndId.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblStartSystem
            // 
            lblStartSystem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblStartSystem.AutoSize = true;
            lblStartSystem.Location = new Point(47, 66);
            lblStartSystem.Margin = new Padding(2, 0, 2, 0);
            lblStartSystem.Name = "lblStartSystem";
            lblStartSystem.Size = new Size(92, 33);
            lblStartSystem.TabIndex = 4;
            lblStartSystem.Text = "Starting System:";
            lblStartSystem.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtStartSystem
            // 
            txtStartSystem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtStartSystem.Location = new Point(143, 72);
            txtStartSystem.Margin = new Padding(2, 6, 7, 2);
            txtStartSystem.Name = "txtStartSystem";
            txtStartSystem.Size = new Size(308, 23);
            txtStartSystem.TabIndex = 5;
            txtStartSystem.TextChanged += txtStartSystem_TextChanged;
            // 
            // lblDestinationSystem
            // 
            lblDestinationSystem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblDestinationSystem.AutoSize = true;
            lblDestinationSystem.Location = new Point(28, 99);
            lblDestinationSystem.Margin = new Padding(2, 0, 2, 0);
            lblDestinationSystem.Name = "lblDestinationSystem";
            lblDestinationSystem.Size = new Size(111, 33);
            lblDestinationSystem.TabIndex = 6;
            lblDestinationSystem.Text = "Destination System:";
            lblDestinationSystem.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtDestSystem
            // 
            txtDestSystem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDestSystem.Location = new Point(143, 105);
            txtDestSystem.Margin = new Padding(2, 6, 7, 2);
            txtDestSystem.Name = "txtDestSystem";
            txtDestSystem.Size = new Size(308, 23);
            txtDestSystem.TabIndex = 7;
            txtDestSystem.TextChanged += txtDestSystem_TextChanged;
            // 
            // lblUsedCapacity
            // 
            lblUsedCapacity.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblUsedCapacity.AutoSize = true;
            lblUsedCapacity.Location = new Point(54, 132);
            lblUsedCapacity.Margin = new Padding(2, 0, 2, 0);
            lblUsedCapacity.Name = "lblUsedCapacity";
            lblUsedCapacity.Size = new Size(85, 33);
            lblUsedCapacity.TabIndex = 8;
            lblUsedCapacity.Text = "Used Capacity:";
            lblUsedCapacity.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtOutput
            // 
            txtOutput.Dock = DockStyle.Fill;
            txtOutput.Location = new Point(143, 167);
            txtOutput.Margin = new Padding(2);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ReadOnly = true;
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.Size = new Size(313, 71);
            txtOutput.TabIndex = 10;
            // 
            // buttonLayoutPanel
            // 
            buttonLayoutPanel.Controls.Add(btnSaveRoute);
            buttonLayoutPanel.Controls.Add(btnClearRoute);
            buttonLayoutPanel.Controls.Add(btnCancel);
            buttonLayoutPanel.Dock = DockStyle.Fill;
            buttonLayoutPanel.Location = new Point(143, 242);
            buttonLayoutPanel.Margin = new Padding(2);
            buttonLayoutPanel.Name = "buttonLayoutPanel";
            buttonLayoutPanel.Padding = new Padding(2);
            buttonLayoutPanel.Size = new Size(313, 37);
            buttonLayoutPanel.TabIndex = 11;
            // 
            // btnSaveRoute
            // 
            btnSaveRoute.AutoSize = true;
            btnSaveRoute.Enabled = false;
            btnSaveRoute.FlatAppearance.BorderSize = 0;
            btnSaveRoute.FlatStyle = FlatStyle.Flat;
            btnSaveRoute.Location = new Point(4, 4);
            btnSaveRoute.Margin = new Padding(2);
            btnSaveRoute.Name = "btnSaveRoute";
            btnSaveRoute.Size = new Size(95, 27);
            btnSaveRoute.TabIndex = 0;
            btnSaveRoute.Text = "Save and Start";
            btnSaveRoute.UseVisualStyleBackColor = true;
            btnSaveRoute.Click += btnAccept_Click;
            // 
            // btnClearRoute
            // 
            btnClearRoute.AutoSize = true;
            btnClearRoute.FlatAppearance.BorderSize = 0;
            btnClearRoute.FlatStyle = FlatStyle.Flat;
            btnClearRoute.Location = new Point(103, 4);
            btnClearRoute.Margin = new Padding(2);
            btnClearRoute.Name = "btnClearRoute";
            btnClearRoute.Size = new Size(82, 27);
            btnClearRoute.TabIndex = 14;
            btnClearRoute.Text = "Clear Route";
            btnClearRoute.UseVisualStyleBackColor = true;
            btnClearRoute.Click += btnClearRoute_Click;
            // 
            // btnCancel
            // 
            btnCancel.AutoSize = true;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(189, 4);
            btnCancel.Margin = new Padding(2);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(78, 27);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // nudUsedCapacity
            // 
            nudUsedCapacity.Location = new Point(143, 138);
            nudUsedCapacity.Margin = new Padding(2, 6, 2, 2);
            nudUsedCapacity.Maximum = new decimal(new int[] { 25000, 0, 0, 0 });
            nudUsedCapacity.Name = "nudUsedCapacity";
            nudUsedCapacity.Size = new Size(126, 23);
            nudUsedCapacity.TabIndex = 12;
            nudUsedCapacity.ValueChanged += nudUsedCapacity_ValueChanged;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnGenerateRoute);
            flowLayoutPanel1.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel1.Location = new Point(2, 167);
            flowLayoutPanel1.Margin = new Padding(2);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(137, 71);
            flowLayoutPanel1.TabIndex = 14;
            // 
            // btnGenerateRoute
            // 
            btnGenerateRoute.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGenerateRoute.AutoSize = true;
            btnGenerateRoute.Enabled = false;
            btnGenerateRoute.FlatAppearance.BorderSize = 0;
            btnGenerateRoute.FlatStyle = FlatStyle.Flat;
            btnGenerateRoute.Location = new Point(33, 2);
            btnGenerateRoute.Margin = new Padding(2);
            btnGenerateRoute.Name = "btnGenerateRoute";
            btnGenerateRoute.Size = new Size(102, 27);
            btnGenerateRoute.TabIndex = 13;
            btnGenerateRoute.Text = "Generate Route";
            btnGenerateRoute.UseVisualStyleBackColor = true;
            btnGenerateRoute.Click += btnGenerateRoute_Click;
            // 
            // SpanshCarrierRouterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(458, 281);
            Controls.Add(mainLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2);
            Name = "SpanshCarrierRouterForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Spansh Carrier Route Options";
            mainLayoutPanel.ResumeLayout(false);
            mainLayoutPanel.PerformLayout();
            buttonLayoutPanel.ResumeLayout(false);
            buttonLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudUsedCapacity).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayoutPanel;
        private Label lblCommander;
        private ComboBox cbCommanders;
        private Label lblCarrier;
        private Label lblCarrierNameAndId;
        private Label lblStartSystem;
        private TextBox txtStartSystem;
        private Label lblDestinationSystem;
        private TextBox txtDestSystem;
        private Label lblUsedCapacity;
        private TextBox txtOutput;
        private FlowLayoutPanel buttonLayoutPanel;
        private Button btnSaveRoute;
        private Button btnCancel;
        private NumericUpDown nudUsedCapacity;
        private Button btnGenerateRoute;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnClearRoute;
    }
}