namespace com.github.fredjk_gh.ObservatoryHelm.UI
{
    partial class HelmUI
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(HelmUI));
            flpCardPanel = new FlowLayoutPanel();
            ttip = new ToolTip(components);
            tsTools = new ToolStrip();
            tsbAbout = new ToolStripButton();
            tsbFontShrink = new ToolStripButton();
            tsbFontEnlarge = new ToolStripButton();
            tsbWrap = new ToolStripButton();
            tsbSettings = new ToolStripButton();
            tsTools.SuspendLayout();
            SuspendLayout();
            // 
            // flpCardPanel
            // 
            flpCardPanel.AllowDrop = true;
            flpCardPanel.AutoScroll = true;
            flpCardPanel.Dock = DockStyle.Fill;
            flpCardPanel.FlowDirection = FlowDirection.TopDown;
            flpCardPanel.Location = new Point(0, 31);
            flpCardPanel.Margin = new Padding(2);
            flpCardPanel.Name = "flpCardPanel";
            flpCardPanel.Size = new Size(916, 495);
            flpCardPanel.TabIndex = 0;
            // 
            // tsTools
            // 
            tsTools.ImageScalingSize = new Size(24, 24);
            tsTools.Items.AddRange(new ToolStripItem[] { tsbAbout, tsbFontShrink, tsbFontEnlarge, tsbWrap, tsbSettings });
            tsTools.Location = new Point(0, 0);
            tsTools.Name = "tsTools";
            tsTools.Size = new Size(916, 31);
            tsTools.TabIndex = 0;
            // 
            // tsbAbout
            // 
            tsbAbout.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbAbout.Image = (Image)resources.GetObject("tsbAbout.Image");
            tsbAbout.ImageTransparentColor = Color.Magenta;
            tsbAbout.Name = "tsbAbout";
            tsbAbout.Size = new Size(28, 28);
            tsbAbout.Text = "tsbAbout";
            tsbAbout.ToolTipText = "About Helm";
            tsbAbout.Click += tsbAbout_Click;
            // 
            // tsbFontShrink
            // 
            tsbFontShrink.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbFontShrink.ImageTransparentColor = Color.Magenta;
            tsbFontShrink.Name = "tsbFontShrink";
            tsbFontShrink.Size = new Size(23, 28);
            tsbFontShrink.ToolTipText = "Decrease font size";
            tsbFontShrink.Click += tsbFontShrink_Click;
            // 
            // tsbFontEnlarge
            // 
            tsbFontEnlarge.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbFontEnlarge.ImageTransparentColor = Color.Magenta;
            tsbFontEnlarge.Name = "tsbFontEnlarge";
            tsbFontEnlarge.Size = new Size(23, 28);
            tsbFontEnlarge.ToolTipText = "Increase font size";
            tsbFontEnlarge.Click += tsbFontEnlarge_Click;
            // 
            // tsbWrap
            // 
            tsbWrap.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbWrap.Image = (Image)resources.GetObject("tsbWrap.Image");
            tsbWrap.ImageTransparentColor = Color.Magenta;
            tsbWrap.Name = "tsbWrap";
            tsbWrap.Size = new Size(28, 28);
            tsbWrap.ToolTipText = "Toggle card flow wrapping";
            tsbWrap.Click += tsbWrap_Click;
            // 
            // tsbSettings
            // 
            tsbSettings.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbSettings.Image = (Image)resources.GetObject("tsbSettings.Image");
            tsbSettings.ImageTransparentColor = Color.Magenta;
            tsbSettings.Name = "tsbSettings";
            tsbSettings.Size = new Size(28, 28);
            tsbSettings.Text = "toolStripButton2";
            tsbSettings.ToolTipText = "Open settings...";
            tsbSettings.Click += tsbSettings_Click;
            // 
            // HelmUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(flpCardPanel);
            Controls.Add(tsTools);
            Margin = new Padding(2);
            Name = "HelmUI";
            Size = new Size(916, 526);
            tsTools.ResumeLayout(false);
            tsTools.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flpCardPanel;
        private ToolStrip tsTools;
        private ToolTip ttip;
        private ToolStripButton tsbFontShrink;
        private ToolStripButton tsbFontEnlarge;
        private ToolStripButton tsbWrap;
        private ToolStripButton tsbAbout;
        private ToolStripButton tsbSettings;
    }
}
