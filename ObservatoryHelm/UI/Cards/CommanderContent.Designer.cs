namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    partial class CommanderContent
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
            lblLastActive = new Label();
            label2 = new Label();
            cbCommander = new ComboBox();
            tbtnRealtimeLock = new com.github.fredjk_gh.PluginCommon.UI.ThemeableImageButton();
            lblUIMode = new Label();
            ttManager = new ToolTip(components);
            SuspendLayout();
            // 
            // lblLastActive
            // 
            lblLastActive.AutoSize = true;
            lblLastActive.Location = new Point(144, 40);
            lblLastActive.Name = "lblLastActive";
            lblLastActive.Size = new Size(110, 15);
            lblLastActive.TabIndex = 10;
            lblLastActive.Text = "2024-11-28 15:18:03";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 40);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 9;
            label2.Text = "Last Active:";
            // 
            // cbCommander
            // 
            cbCommander.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbCommander.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCommander.FlatStyle = FlatStyle.Flat;
            cbCommander.FormattingEnabled = true;
            cbCommander.Location = new Point(8, 8);
            cbCommander.Name = "cbCommander";
            cbCommander.Size = new Size(439, 23);
            cbCommander.TabIndex = 8;
            // 
            // tbtnRealtimeLock
            // 
            tbtnRealtimeLock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tbtnRealtimeLock.FlatAppearance.BorderSize = 0;
            tbtnRealtimeLock.FlatStyle = FlatStyle.Flat;
            tbtnRealtimeLock.ImageSize = null;
            tbtnRealtimeLock.Location = new Point(421, 36);
            tbtnRealtimeLock.Name = "tbtnRealtimeLock";
            tbtnRealtimeLock.OriginalImage = null;
            tbtnRealtimeLock.Size = new Size(26, 26);
            tbtnRealtimeLock.TabIndex = 11;
            tbtnRealtimeLock.UseVisualStyleBackColor = true;
            // 
            // lblUIMode
            // 
            lblUIMode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUIMode.Location = new Point(293, 40);
            lblUIMode.Name = "lblUIMode";
            lblUIMode.Size = new Size(116, 16);
            lblUIMode.TabIndex = 12;
            lblUIMode.Text = "Mode: Realtime";
            lblUIMode.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ttManager
            // 
            ttManager.ShowAlways = true;
            ttManager.ToolTipTitle = "Helm";
            // 
            // CommanderContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblUIMode);
            Controls.Add(tbtnRealtimeLock);
            Controls.Add(lblLastActive);
            Controls.Add(label2);
            Controls.Add(cbCommander);
            Name = "CommanderContent";
            Size = new Size(454, 67);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLastActive;
        private Label label2;
        private ComboBox cbCommander;
        private PluginCommon.UI.ThemeableImageButton tbtnRealtimeLock;
        private Label lblUIMode;
        private ToolTip ttManager;
    }
}
