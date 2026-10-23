namespace com.github.fredjk_gh.PluginCommon.UI.Forms
{
    partial class PopoutForm
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
            ctxActions = new ContextMenuStrip(components);
            toggleBorderToolStripMenuItem = new ToolStripMenuItem();
            toggleTransparencyToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            closeToolStripMenuItem = new ToolStripMenuItem();
            ctxActions.SuspendLayout();
            SuspendLayout();
            // 
            // ctxActions
            // 
            ctxActions.Items.AddRange(new ToolStripItem[] { toggleBorderToolStripMenuItem, toggleTransparencyToolStripMenuItem, toolStripSeparator1, closeToolStripMenuItem });
            ctxActions.Name = "ctxActions";
            ctxActions.ShowImageMargin = false;
            ctxActions.Size = new Size(156, 76);
            // 
            // toggleBorderToolStripMenuItem
            // 
            toggleBorderToolStripMenuItem.Name = "toggleBorderToolStripMenuItem";
            toggleBorderToolStripMenuItem.Size = new Size(155, 22);
            toggleBorderToolStripMenuItem.Text = "Toggle border";
            toggleBorderToolStripMenuItem.Click += ToggleBorderToolStripMenuItem_Click;
            // 
            // toggleTransparencyToolStripMenuItem
            // 
            toggleTransparencyToolStripMenuItem.Name = "toggleTransparencyToolStripMenuItem";
            toggleTransparencyToolStripMenuItem.Size = new Size(155, 22);
            toggleTransparencyToolStripMenuItem.Text = "Toggle transparency";
            toggleTransparencyToolStripMenuItem.Click += ToggleTransparencyToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(152, 6);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(155, 22);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += CloseToolStripMenuItem_Click;
            // 
            // PopoutForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(527, 263);
            Name = "PopoutForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PopoutForm";
            TopMost = true;
            MouseClick += PopoutForm_MouseClick;
            ctxActions.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ContextMenuStrip ctxActions;
        private ToolStripMenuItem toggleBorderToolStripMenuItem;
        private ToolStripMenuItem toggleTransparencyToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem closeToolStripMenuItem;
    }
}