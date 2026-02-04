namespace com.github.fredjk_gh.PluginCommon.UI
{
    partial class ThemeableImageLabel
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
            flpContent = new FlowLayoutPanel();
            lblText = new Label();
            flpContent.SuspendLayout();
            SuspendLayout();
            // 
            // flpContent
            // 
            flpContent.AutoSize = true;
            flpContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flpContent.Controls.Add(lblText);
            flpContent.Dock = DockStyle.Fill;
            flpContent.Location = new Point(0, 0);
            flpContent.Margin = new Padding(0);
            flpContent.Name = "flpContent";
            flpContent.Size = new Size(0, 15);
            flpContent.TabIndex = 0;
            // 
            // lblText
            // 
            lblText.AutoSize = true;
            lblText.Location = new Point(0, 0);
            lblText.Margin = new Padding(0);
            lblText.Name = "lblText";
            lblText.Size = new Size(0, 15);
            lblText.TabIndex = 0;
            lblText.Visible = false;
            // 
            // ThemeableImageLabel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(flpContent);
            Name = "ThemeableImageLabel";
            Size = new Size(0, 15);
            flpContent.ResumeLayout(false);
            flpContent.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flpContent;
        private Label lblText;
    }
}
