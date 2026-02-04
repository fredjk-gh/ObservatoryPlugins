namespace com.github.fredjk_gh.PluginCommon.UI
{
    partial class CollapsibleGroupBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollapsibleGroupBox));
            pBackBorder = new Panel();
            pHeaderBox = new Panel();
            flpToolbox = new FlowLayoutPanel();
            btnExpander = new ThemeableImageButton();
            lblBoxTitle = new ScalableLabel();
            ttManager = new ToolTip(components);
            pBackBorder.SuspendLayout();
            pHeaderBox.SuspendLayout();
            SuspendLayout();
            // 
            // pBackBorder
            // 
            pBackBorder.AutoSize = true;
            pBackBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pBackBorder.Controls.Add(pHeaderBox);
            pBackBorder.Dock = DockStyle.Fill;
            pBackBorder.Location = new Point(0, 0);
            pBackBorder.Margin = new Padding(0);
            pBackBorder.Name = "pBackBorder";
            pBackBorder.Padding = new Padding(1);
            pBackBorder.Size = new Size(250, 201);
            pBackBorder.TabIndex = 0;
            // 
            // pHeaderBox
            // 
            pHeaderBox.AutoSize = true;
            pHeaderBox.Controls.Add(flpToolbox);
            pHeaderBox.Controls.Add(btnExpander);
            pHeaderBox.Controls.Add(lblBoxTitle);
            pHeaderBox.Dock = DockStyle.Top;
            pHeaderBox.Location = new Point(1, 1);
            pHeaderBox.Margin = new Padding(0);
            pHeaderBox.Name = "pHeaderBox";
            pHeaderBox.Padding = new Padding(1);
            pHeaderBox.Size = new Size(248, 176);
            pHeaderBox.TabIndex = 3;
            // 
            // flpToolbox
            // 
            flpToolbox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            flpToolbox.AutoSize = true;
            flpToolbox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flpToolbox.FlowDirection = FlowDirection.RightToLeft;
            flpToolbox.Location = new Point(247, 1);
            flpToolbox.Margin = new Padding(0);
            flpToolbox.Name = "flpToolbox";
            flpToolbox.Size = new Size(0, 0);
            flpToolbox.TabIndex = 4;
            flpToolbox.WrapContents = false;
            // 
            // btnExpander
            // 
            btnExpander.AutoSize = true;
            btnExpander.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnExpander.FlatAppearance.BorderSize = 0;
            btnExpander.FlatStyle = FlatStyle.Flat;
            btnExpander.Image = (Image)resources.GetObject("btnExpander.Image");
            btnExpander.ImageSize = null;
            btnExpander.Location = new Point(1, 1);
            btnExpander.Margin = new Padding(0);
            btnExpander.Name = "btnExpander";
            btnExpander.OriginalImage = Images.CollapseImage;
            btnExpander.Size = new Size(174, 174);
            btnExpander.TabIndex = 5;
            btnExpander.UseVisualStyleBackColor = true;
            btnExpander.Click += BtnExpander_Click;
            // 
            // lblBoxTitle
            // 
            lblBoxTitle.AutoSize = true;
            lblBoxTitle.FontSizeAdjustment = 5F;
            lblBoxTitle.Location = new Point(31, 1);
            lblBoxTitle.Margin = new Padding(3);
            lblBoxTitle.Name = "lblBoxTitle";
            lblBoxTitle.Size = new Size(33, 15);
            lblBoxTitle.TabIndex = 3;
            lblBoxTitle.Text = "title1";
            // 
            // CollapsibleGroupBox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pBackBorder);
            Margin = new Padding(0);
            Name = "CollapsibleGroupBox";
            Size = new Size(250, 201);
            pBackBorder.ResumeLayout(false);
            pBackBorder.PerformLayout();
            pHeaderBox.ResumeLayout(false);
            pHeaderBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pBackBorder;
        private Panel pHeaderBox;
        private FlowLayoutPanel flpToolbox;
        private ThemeableImageButton btnExpander;
        private ScalableLabel lblBoxTitle;
        private ToolTip ttManager;
    }
}
