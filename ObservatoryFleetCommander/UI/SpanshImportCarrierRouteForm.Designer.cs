namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    partial class SpanshImportCarrierRouteForm
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
            txtSpanshResultsUrl = new TextBox();
            txtOutput = new TextBox();
            buttonLayoutPanel = new FlowLayoutPanel();
            btnSaveRoute = new Button();
            btnClearRoute = new Button();
            btnCancel = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnImportRoute = new Button();
            mainLayoutPanel.SuspendLayout();
            buttonLayoutPanel.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.ColumnCount = 2;
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.95975F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 69.04025F));
            mainLayoutPanel.Controls.Add(lblCommander, 0, 0);
            mainLayoutPanel.Controls.Add(cbCommanders, 1, 0);
            mainLayoutPanel.Controls.Add(lblCarrier, 0, 1);
            mainLayoutPanel.Controls.Add(lblCarrierNameAndId, 1, 1);
            mainLayoutPanel.Controls.Add(lblStartSystem, 0, 2);
            mainLayoutPanel.Controls.Add(txtSpanshResultsUrl, 1, 2);
            mainLayoutPanel.Controls.Add(txtOutput, 1, 3);
            mainLayoutPanel.Controls.Add(buttonLayoutPanel, 1, 4);
            mainLayoutPanel.Controls.Add(flowLayoutPanel1, 0, 3);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.Location = new Point(0, 0);
            mainLayoutPanel.Margin = new Padding(2);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.RowCount = 5;
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7894735F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7894735F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7894735F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35.5263176F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 17.1052628F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            mainLayoutPanel.Size = new Size(518, 219);
            mainLayoutPanel.TabIndex = 1;
            // 
            // lblCommander
            // 
            lblCommander.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblCommander.AutoSize = true;
            lblCommander.Location = new Point(81, 0);
            lblCommander.Margin = new Padding(2, 0, 2, 0);
            lblCommander.Name = "lblCommander";
            lblCommander.Size = new Size(77, 34);
            lblCommander.TabIndex = 0;
            lblCommander.Text = "Commander:";
            lblCommander.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbCommanders
            // 
            cbCommanders.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbCommanders.FormattingEnabled = true;
            cbCommanders.Location = new Point(162, 6);
            cbCommanders.Margin = new Padding(2, 6, 2, 2);
            cbCommanders.Name = "cbCommanders";
            cbCommanders.Size = new Size(354, 23);
            cbCommanders.TabIndex = 1;
            cbCommanders.SelectedIndexChanged += cbCommanders_SelectedIndexChanged;
            // 
            // lblCarrier
            // 
            lblCarrier.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblCarrier.AutoSize = true;
            lblCarrier.Location = new Point(113, 34);
            lblCarrier.Margin = new Padding(2, 0, 2, 0);
            lblCarrier.Name = "lblCarrier";
            lblCarrier.Size = new Size(45, 34);
            lblCarrier.TabIndex = 2;
            lblCarrier.Text = "Carrier:";
            lblCarrier.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblCarrierNameAndId
            // 
            lblCarrierNameAndId.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblCarrierNameAndId.AutoSize = true;
            lblCarrierNameAndId.Location = new Point(162, 34);
            lblCarrierNameAndId.Margin = new Padding(2, 0, 2, 0);
            lblCarrierNameAndId.Name = "lblCarrierNameAndId";
            lblCarrierNameAndId.Size = new Size(116, 34);
            lblCarrierNameAndId.TabIndex = 3;
            lblCarrierNameAndId.Text = "(Select Commander)";
            lblCarrierNameAndId.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblStartSystem
            // 
            lblStartSystem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblStartSystem.AutoSize = true;
            lblStartSystem.Location = new Point(46, 68);
            lblStartSystem.Margin = new Padding(2, 0, 2, 0);
            lblStartSystem.Name = "lblStartSystem";
            lblStartSystem.Size = new Size(112, 34);
            lblStartSystem.TabIndex = 4;
            lblStartSystem.Text = "Spansh Results URL:";
            lblStartSystem.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtSpanshResultsUrl
            // 
            txtSpanshResultsUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSpanshResultsUrl.Location = new Point(162, 74);
            txtSpanshResultsUrl.Margin = new Padding(2, 6, 7, 2);
            txtSpanshResultsUrl.Name = "txtSpanshResultsUrl";
            txtSpanshResultsUrl.Size = new Size(349, 23);
            txtSpanshResultsUrl.TabIndex = 5;
            txtSpanshResultsUrl.TextChanged += txtSpanshResultsUrl_TextChanged;
            // 
            // txtOutput
            // 
            txtOutput.Dock = DockStyle.Fill;
            txtOutput.Location = new Point(162, 104);
            txtOutput.Margin = new Padding(2);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ReadOnly = true;
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.Size = new Size(354, 73);
            txtOutput.TabIndex = 10;
            // 
            // buttonLayoutPanel
            // 
            buttonLayoutPanel.Controls.Add(btnSaveRoute);
            buttonLayoutPanel.Controls.Add(btnClearRoute);
            buttonLayoutPanel.Controls.Add(btnCancel);
            buttonLayoutPanel.Dock = DockStyle.Fill;
            buttonLayoutPanel.Location = new Point(162, 181);
            buttonLayoutPanel.Margin = new Padding(2);
            buttonLayoutPanel.Name = "buttonLayoutPanel";
            buttonLayoutPanel.Padding = new Padding(2);
            buttonLayoutPanel.Size = new Size(354, 36);
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
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnImportRoute);
            flowLayoutPanel1.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel1.Location = new Point(2, 104);
            flowLayoutPanel1.Margin = new Padding(2);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(156, 73);
            flowLayoutPanel1.TabIndex = 14;
            // 
            // btnImportRoute
            // 
            btnImportRoute.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnImportRoute.AutoSize = true;
            btnImportRoute.Enabled = false;
            btnImportRoute.FlatAppearance.BorderSize = 0;
            btnImportRoute.FlatStyle = FlatStyle.Flat;
            btnImportRoute.Location = new Point(52, 2);
            btnImportRoute.Margin = new Padding(2);
            btnImportRoute.Name = "btnImportRoute";
            btnImportRoute.Size = new Size(102, 27);
            btnImportRoute.TabIndex = 13;
            btnImportRoute.Text = "Import Route";
            btnImportRoute.UseVisualStyleBackColor = true;
            btnImportRoute.Click += btnImportRoute_Click;
            // 
            // SpanshImportCarrierRouteForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(518, 219);
            Controls.Add(mainLayoutPanel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "SpanshImportCarrierRouteForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Spansh Import Carrier Route";
            mainLayoutPanel.ResumeLayout(false);
            mainLayoutPanel.PerformLayout();
            buttonLayoutPanel.ResumeLayout(false);
            buttonLayoutPanel.PerformLayout();
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
        private TextBox txtSpanshResultsUrl;
        private TextBox txtOutput;
        private FlowLayoutPanel buttonLayoutPanel;
        private Button btnSaveRoute;
        private Button btnClearRoute;
        private Button btnCancel;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnImportRoute;
    }
}