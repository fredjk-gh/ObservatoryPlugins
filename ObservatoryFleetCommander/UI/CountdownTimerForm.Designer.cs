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
            lblTimerValue = new Label();
            lblTimerTitle = new Label();
            SuspendLayout();
            // 
            // lblTimerValue
            // 
            lblTimerValue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblTimerValue.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTimerValue.Location = new Point(0, 80);
            lblTimerValue.Margin = new Padding(0);
            lblTimerValue.Name = "lblTimerValue";
            lblTimerValue.Size = new Size(438, 120);
            lblTimerValue.TabIndex = 14;
            lblTimerValue.Text = "0:00:00";
            lblTimerValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTimerTitle
            // 
            lblTimerTitle.AutoEllipsis = true;
            lblTimerTitle.Dock = DockStyle.Top;
            lblTimerTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTimerTitle.Location = new Point(0, 0);
            lblTimerTitle.Margin = new Padding(4, 6, 4, 3);
            lblTimerTitle.Name = "lblTimerTitle";
            lblTimerTitle.Size = new Size(438, 80);
            lblTimerTitle.TabIndex = 15;
            lblTimerTitle.Text = "{CarrierName}\r\n{Jump|Cooldown} Timer";
            lblTimerTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CountdownTimerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(438, 202);
            Controls.Add(lblTimerTitle);
            Controls.Add(lblTimerValue);
            DoubleBuffered = true;
            MinimumSize = new Size(250, 200);
            Name = "CountdownTimerForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Commander - Timer Popout";
            TopMost = true;
            Shown += CountdownTimerForm_Shown;
            ResizeEnd += CountdownTimerForm_ResizeEnd;
            Resize += CountdownTimerForm_Resize;
            ResumeLayout(false);
        }

        #endregion

        private Label lblTimerValue;
        private Label lblTimerTitle;
    }
}