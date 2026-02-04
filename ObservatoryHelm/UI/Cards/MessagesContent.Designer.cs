namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    partial class MessagesContent
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
            lvMsgs = new ListView();
            colTimestamp = new ColumnHeader();
            colMessage = new ColumnHeader();
            colSender = new ColumnHeader();
            ttManager = new ToolTip(components);
            SuspendLayout();
            // 
            // lvMsgs
            // 
            lvMsgs.Columns.AddRange(new ColumnHeader[] { colTimestamp, colMessage, colSender });
            lvMsgs.Dock = DockStyle.Fill;
            lvMsgs.FullRowSelect = true;
            lvMsgs.Location = new Point(0, 0);
            lvMsgs.Margin = new Padding(8);
            lvMsgs.Name = "lvMsgs";
            lvMsgs.ShowItemToolTips = true;
            lvMsgs.Size = new Size(461, 137);
            lvMsgs.TabIndex = 73;
            lvMsgs.UseCompatibleStateImageBehavior = false;
            lvMsgs.View = View.Details;
            // 
            // colTimestamp
            // 
            colTimestamp.Text = "Time";
            colTimestamp.Width = 125;
            // 
            // colMessage
            // 
            colMessage.Text = "Message";
            colMessage.Width = 600;
            // 
            // colSender
            // 
            colSender.Text = "Sender";
            colSender.Width = 150;
            // 
            // MessagesContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lvMsgs);
            Name = "MessagesContent";
            Size = new Size(461, 137);
            ResumeLayout(false);
        }

        #endregion

        private ListView lvMsgs;
        private ColumnHeader colTimestamp;
        private ColumnHeader colMessage;
        private ToolTip ttManager;
        private ColumnHeader colSender;
    }
}
