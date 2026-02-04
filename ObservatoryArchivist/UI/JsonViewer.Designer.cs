using com.github.fredjk_gh.PluginCommon.UI;

namespace com.github.fredjk_gh.ObservatoryArchivist.UI
{
    partial class JsonViewer
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
            tableLayoutPanel1 = new TableLayoutPanel();
            txtJson = new TextBox();
            btnClose = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnFontSizeIcon = new PluginCommon.UI.ThemeableImageButton();
            tbFontSize = new TrackBar();
            ttip = new ToolTip(components);
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbFontSize).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(txtJson, 0, 0);
            tableLayoutPanel1.Controls.Add(btnClose, 1, 1);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel1.Size = new Size(800, 450);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // txtJson
            // 
            tableLayoutPanel1.SetColumnSpan(txtJson, 2);
            txtJson.Dock = DockStyle.Fill;
            txtJson.Font = new Font("Consolas", 9F);
            txtJson.Location = new Point(3, 3);
            txtJson.Multiline = true;
            txtJson.Name = "txtJson";
            txtJson.ReadOnly = true;
            txtJson.ScrollBars = ScrollBars.Vertical;
            txtJson.Size = new Size(794, 409);
            txtJson.TabIndex = 0;
            txtJson.Text = "{\r\n    \"timestamp\":\"2024-04-11T17:39:33\"\r\n    \"event\":\"Scan\"\r\n    ...\r\n}";
            txtJson.WordWrap = false;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(722, 418);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 23);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += BtnClose_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnFontSizeIcon);
            flowLayoutPanel1.Controls.Add(tbFontSize);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 415);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(400, 35);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // btnFontSizeIcon
            // 
            btnFontSizeIcon.FlatAppearance.BorderSize = 0;
            btnFontSizeIcon.FlatStyle = FlatStyle.Flat;
            btnFontSizeIcon.Location = new Point(3, 3);
            btnFontSizeIcon.Name = "btnFontSizeIcon";
            btnFontSizeIcon.Size = new Size(27, 27);
            btnFontSizeIcon.TabIndex = 0;
            btnFontSizeIcon.UseVisualStyleBackColor = true;
            // 
            // tbFontSize
            // 
            tbFontSize.Dock = DockStyle.Fill;
            tbFontSize.Location = new Point(36, 3);
            tbFontSize.Maximum = 24;
            tbFontSize.Minimum = 5;
            tbFontSize.Name = "tbFontSize";
            tbFontSize.Size = new Size(356, 27);
            tbFontSize.TabIndex = 1;
            tbFontSize.Value = 9;
            tbFontSize.Scroll += TbFontSize_Scroll;
            // 
            // JsonViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            DoubleBuffered = true;
            Name = "JsonViewer";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Json Viewer";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbFontSize).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox txtJson;
        private Button btnClose;
        private FlowLayoutPanel flowLayoutPanel1;
        private ThemeableImageButton btnFontSizeIcon;
        private TrackBar tbFontSize;
        private ToolTip ttip;
    }
}