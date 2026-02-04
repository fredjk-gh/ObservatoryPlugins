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
            components = new System.ComponentModel.Container();
            mainLayoutPanel = new TableLayoutPanel();
            lblCommander = new Label();
            cbCarriers = new ComboBox();
            lblCarrierType = new Label();
            lblCarrierTypeValue = new Label();
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
            ttipSpanshRouter = new ToolTip(components);
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
            mainLayoutPanel.Controls.Add(cbCarriers, 1, 0);
            mainLayoutPanel.Controls.Add(lblCarrierType, 0, 1);
            mainLayoutPanel.Controls.Add(lblCarrierTypeValue, 1, 1);
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
            lblCommander.Location = new Point(94, 0);
            lblCommander.Margin = new Padding(2, 0, 2, 0);
            lblCommander.Name = "lblCommander";
            lblCommander.Size = new Size(45, 33);
            lblCommander.TabIndex = 0;
            lblCommander.Text = "Carrier:";
            lblCommander.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbCarriers
            // 
            cbCarriers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbCarriers.FormattingEnabled = true;
            cbCarriers.Location = new Point(143, 6);
            cbCarriers.Margin = new Padding(2, 6, 2, 2);
            cbCarriers.Name = "cbCarriers";
            cbCarriers.Size = new Size(313, 23);
            cbCarriers.TabIndex = 1;
            cbCarriers.SelectedIndexChanged += CbCarriers_SelectedIndexChanged;
            // 
            // lblCarrierType
            // 
            lblCarrierType.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblCarrierType.AutoSize = true;
            lblCarrierType.Location = new Point(67, 33);
            lblCarrierType.Margin = new Padding(2, 0, 2, 0);
            lblCarrierType.Name = "lblCarrierType";
            lblCarrierType.Size = new Size(72, 33);
            lblCarrierType.TabIndex = 2;
            lblCarrierType.Text = "Carrier Type:";
            lblCarrierType.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblCarrierTypeValue
            // 
            lblCarrierTypeValue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblCarrierTypeValue.AutoSize = true;
            lblCarrierTypeValue.Location = new Point(143, 33);
            lblCarrierTypeValue.Margin = new Padding(2, 0, 2, 0);
            lblCarrierTypeValue.Name = "lblCarrierTypeValue";
            lblCarrierTypeValue.Size = new Size(82, 33);
            lblCarrierTypeValue.TabIndex = 3;
            lblCarrierTypeValue.Text = "(Select carrier)";
            lblCarrierTypeValue.TextAlign = ContentAlignment.MiddleLeft;
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
            txtStartSystem.TextChanged += TxtStartSystem_TextChanged;
            // 
            // lblDestinationSystem
            // 
            lblDestinationSystem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblDestinationSystem.AutoSize = true;
            lblDestinationSystem.Location = new Point(15, 99);
            lblDestinationSystem.Margin = new Padding(2, 0, 2, 0);
            lblDestinationSystem.Name = "lblDestinationSystem";
            lblDestinationSystem.Size = new Size(124, 33);
            lblDestinationSystem.TabIndex = 6;
            lblDestinationSystem.Text = "Destination System(s):";
            lblDestinationSystem.TextAlign = ContentAlignment.MiddleRight;
            ttipSpanshRouter.SetToolTip(lblDestinationSystem, "Separate destinations with commas");
            // 
            // txtDestSystem
            // 
            txtDestSystem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDestSystem.Location = new Point(143, 105);
            txtDestSystem.Margin = new Padding(2, 6, 7, 2);
            txtDestSystem.Name = "txtDestSystem";
            txtDestSystem.Size = new Size(308, 23);
            txtDestSystem.TabIndex = 7;
            txtDestSystem.TextChanged += TxtDestSystem_TextChanged;
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
            btnSaveRoute.Click += BtnAccept_Click;
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
            btnClearRoute.Click += BtnClearRoute_Click;
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
            btnCancel.Click += BtnCancel_Click;
            // 
            // nudUsedCapacity
            // 
            nudUsedCapacity.Location = new Point(143, 138);
            nudUsedCapacity.Margin = new Padding(2, 6, 2, 2);
            nudUsedCapacity.Maximum = new decimal(new int[] { 25000, 0, 0, 0 });
            nudUsedCapacity.Name = "nudUsedCapacity";
            nudUsedCapacity.Size = new Size(126, 23);
            nudUsedCapacity.TabIndex = 12;
            nudUsedCapacity.ValueChanged += NudUsedCapacity_ValueChanged;
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
            btnGenerateRoute.Click += BtnGenerateRoute_Click;
            // 
            // SpanshCarrierRouterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(458, 281);
            Controls.Add(mainLayoutPanel);
            DoubleBuffered = true;
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
        private ComboBox cbCarriers;
        private Label lblCarrierType;
        private Label lblCarrierTypeValue;
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
        private ToolTip ttipSpanshRouter;
    }
}