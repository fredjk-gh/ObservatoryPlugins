namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    partial class CountdownTimerForm
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
            lblTimerValue = new Label();
            lblTimerTitle = new Label();
            ctxActions = new ContextMenuStrip(components);
            toggleBorderToolStripMenuItem = new ToolStripMenuItem();
            toggleTransparencyToolStripMenuItem = new ToolStripMenuItem();
            closeToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            ctxActions.SuspendLayout();
            SuspendLayout();
            // 
            // lblTimerValue
            // 
            lblTimerValue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblTimerValue.Location = new Point(0, 80);
            lblTimerValue.Margin = new Padding(0);
            lblTimerValue.Name = "lblTimerValue";
            lblTimerValue.Size = new Size(432, 108);
            lblTimerValue.TabIndex = 14;
            lblTimerValue.Text = "0:00:00";
            lblTimerValue.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblTimerTitle
            // 
            lblTimerTitle.AutoEllipsis = true;
            lblTimerTitle.Dock = DockStyle.Top;
            lblTimerTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTimerTitle.Location = new Point(0, 0);
            lblTimerTitle.Margin = new Padding(4, 6, 4, 3);
            lblTimerTitle.Name = "lblTimerTitle";
            lblTimerTitle.Size = new Size(432, 80);
            lblTimerTitle.TabIndex = 15;
            lblTimerTitle.Text = "{CarrierName}\r\n{Jump|Cooldown} Timer";
            lblTimerTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ctxActions
            // 
            ctxActions.Items.AddRange(new ToolStripItem[] { toggleBorderToolStripMenuItem, toggleTransparencyToolStripMenuItem, toolStripSeparator1, closeToolStripMenuItem });
            ctxActions.Name = "ctxActions";
            ctxActions.Size = new Size(181, 98);
            // 
            // toggleBorderToolStripMenuItem
            // 
            toggleBorderToolStripMenuItem.Name = "toggleBorderToolStripMenuItem";
            toggleBorderToolStripMenuItem.Size = new Size(180, 22);
            toggleBorderToolStripMenuItem.Text = "Toggle border";
            toggleBorderToolStripMenuItem.Click += ToggleBorderToolStripMenuItem_Click;
            // 
            // toggleTransparencyToolStripMenuItem
            // 
            toggleTransparencyToolStripMenuItem.Name = "toggleTransparencyToolStripMenuItem";
            toggleTransparencyToolStripMenuItem.Size = new Size(180, 22);
            toggleTransparencyToolStripMenuItem.Text = "Toggle transparency";
            toggleTransparencyToolStripMenuItem.Click += ToggleTransparencyToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(180, 22);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += CloseToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(177, 6);
            // 
            // CountdownTimerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(432, 190);
            ContextMenuStrip = ctxActions;
            Controls.Add(lblTimerTitle);
            Controls.Add(lblTimerValue);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize = new Size(250, 200);
            Name = "CountdownTimerForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Commander - Timer Popout";
            TopMost = true;
            Shown += CountdownTimerForm_Shown;
            ResizeEnd += CountdownTimerForm_ResizeEnd;
            BackColorChanged += CountdownTimerForm_BackColorChanged;
            MouseClick += CountdownTimerForm_MouseClick;
            Resize += CountdownTimerForm_Resize;
            ctxActions.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label lblTimerValue;
        private Label lblTimerTitle;
        private ContextMenuStrip ctxActions;
        private ToolStripMenuItem toggleBorderToolStripMenuItem;
        private ToolStripMenuItem toggleTransparencyToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem closeToolStripMenuItem;
    }
}