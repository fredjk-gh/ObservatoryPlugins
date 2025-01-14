﻿using com.github.fredjk_gh.PluginCommon.UI;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    partial class CarrierUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tlpCarrierLayout = new TableLayoutPanel();
            lblCapacityValue = new Label();
            lblCapacityTitle = new Label();
            lblNameAndCallsign = new Label();
            lblOwningCommander = new Label();
            lblFuelTitle = new Label();
            lblBalanceTitle = new Label();
            lblBalanceValue = new Label();
            lblPositionTitle = new Label();
            lblPositionValue = new Label();
            lblCommanderStateTitle = new Label();
            lblCommanderStateValue = new Label();
            lblNextJumpTitle = new Label();
            lblNextJumpValue = new Label();
            lblTimerTitle = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnNewRoute = new ThemeableIconButton();
            btnOpenFromSpansh = new ThemeableIconButton();
            btnClearRoute = new ThemeableIconButton();
            btnOpenSpansh = new ThemeableIconButton();
            clbRoute = new CheckedListBox();
            ctxRouteMenu = new ContextMenuStrip(components);
            ctxRouteMenu_CopySystemName = new ToolStripMenuItem();
            ctxRouteMenu_SetCurrentPosition = new ToolStripMenuItem();
            txtMessages = new TextBox();
            lblMessages = new Label();
            pbFuelLevel = new ThemeableProgressBar();
            tableLayoutPanel1 = new TableLayoutPanel();
            lblTimerValue = new Label();
            btnPopOutTimer = new ThemeableIconButton();
            ttipCarrierUI = new ToolTip(components);
            tlpCarrierLayout.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            ctxRouteMenu.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tlpCarrierLayout
            // 
            tlpCarrierLayout.BackColor = SystemColors.Control;
            tlpCarrierLayout.ColumnCount = 3;
            tlpCarrierLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tlpCarrierLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tlpCarrierLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tlpCarrierLayout.Controls.Add(lblCapacityValue, 1, 2);
            tlpCarrierLayout.Controls.Add(lblCapacityTitle, 0, 2);
            tlpCarrierLayout.Controls.Add(lblNameAndCallsign, 0, 0);
            tlpCarrierLayout.Controls.Add(lblOwningCommander, 2, 0);
            tlpCarrierLayout.Controls.Add(lblFuelTitle, 0, 1);
            tlpCarrierLayout.Controls.Add(lblBalanceTitle, 0, 3);
            tlpCarrierLayout.Controls.Add(lblBalanceValue, 1, 3);
            tlpCarrierLayout.Controls.Add(lblPositionTitle, 0, 4);
            tlpCarrierLayout.Controls.Add(lblPositionValue, 1, 4);
            tlpCarrierLayout.Controls.Add(lblCommanderStateTitle, 0, 5);
            tlpCarrierLayout.Controls.Add(lblCommanderStateValue, 1, 5);
            tlpCarrierLayout.Controls.Add(lblNextJumpTitle, 0, 6);
            tlpCarrierLayout.Controls.Add(lblNextJumpValue, 1, 6);
            tlpCarrierLayout.Controls.Add(lblTimerTitle, 0, 7);
            tlpCarrierLayout.Controls.Add(flowLayoutPanel1, 2, 1);
            tlpCarrierLayout.Controls.Add(clbRoute, 2, 2);
            tlpCarrierLayout.Controls.Add(txtMessages, 1, 8);
            tlpCarrierLayout.Controls.Add(lblMessages, 0, 8);
            tlpCarrierLayout.Controls.Add(pbFuelLevel, 1, 1);
            tlpCarrierLayout.Controls.Add(tableLayoutPanel1, 1, 7);
            tlpCarrierLayout.Dock = DockStyle.Fill;
            tlpCarrierLayout.Location = new Point(0, 0);
            tlpCarrierLayout.Margin = new Padding(7, 6, 7, 6);
            tlpCarrierLayout.Name = "tlpCarrierLayout";
            tlpCarrierLayout.RowCount = 9;
            tlpCarrierLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpCarrierLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tlpCarrierLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpCarrierLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpCarrierLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpCarrierLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpCarrierLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpCarrierLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpCarrierLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 75F));
            tlpCarrierLayout.Size = new Size(873, 388);
            tlpCarrierLayout.TabIndex = 0;
            // 
            // lblCapacityValue
            // 
            lblCapacityValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblCapacityValue.AutoSize = true;
            lblCapacityValue.Location = new Point(178, 59);
            lblCapacityValue.Margin = new Padding(4, 3, 4, 3);
            lblCapacityValue.Name = "lblCapacityValue";
            lblCapacityValue.Size = new Size(341, 15);
            lblCapacityValue.TabIndex = 21;
            lblCapacityValue.Text = "[xx,xxx T]";
            // 
            // lblCapacityTitle
            // 
            lblCapacityTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCapacityTitle.AutoSize = true;
            lblCapacityTitle.Location = new Point(85, 59);
            lblCapacityTitle.Margin = new Padding(4, 3, 4, 3);
            lblCapacityTitle.Name = "lblCapacityTitle";
            lblCapacityTitle.Size = new Size(85, 15);
            lblCapacityTitle.TabIndex = 20;
            lblCapacityTitle.Text = "Capacity Used:";
            // 
            // lblNameAndCallsign
            // 
            lblNameAndCallsign.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblNameAndCallsign.AutoSize = true;
            tlpCarrierLayout.SetColumnSpan(lblNameAndCallsign, 2);
            lblNameAndCallsign.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblNameAndCallsign.Location = new Point(2, 0);
            lblNameAndCallsign.Margin = new Padding(2, 0, 2, 0);
            lblNameAndCallsign.Name = "lblNameAndCallsign";
            lblNameAndCallsign.Size = new Size(519, 21);
            lblNameAndCallsign.TabIndex = 0;
            lblNameAndCallsign.Text = "[carrier name (callsign)]";
            lblNameAndCallsign.DoubleClick += Label_DoubleClickToCopy;
            // 
            // lblOwningCommander
            // 
            lblOwningCommander.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblOwningCommander.AutoSize = true;
            lblOwningCommander.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblOwningCommander.Location = new Point(525, 0);
            lblOwningCommander.Margin = new Padding(2, 0, 2, 0);
            lblOwningCommander.Name = "lblOwningCommander";
            lblOwningCommander.Size = new Size(346, 20);
            lblOwningCommander.TabIndex = 1;
            lblOwningCommander.Text = "Owned by: [commander]";
            // 
            // lblFuelTitle
            // 
            lblFuelTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblFuelTitle.AutoSize = true;
            lblFuelTitle.Location = new Point(90, 33);
            lblFuelTitle.Margin = new Padding(4, 9, 4, 3);
            lblFuelTitle.Name = "lblFuelTitle";
            lblFuelTitle.Size = new Size(80, 15);
            lblFuelTitle.TabIndex = 2;
            lblFuelTitle.Text = "Fuel in Depot:";
            // 
            // lblBalanceTitle
            // 
            lblBalanceTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblBalanceTitle.AutoSize = true;
            lblBalanceTitle.Location = new Point(81, 83);
            lblBalanceTitle.Margin = new Padding(4, 3, 4, 3);
            lblBalanceTitle.Name = "lblBalanceTitle";
            lblBalanceTitle.Size = new Size(89, 15);
            lblBalanceTitle.TabIndex = 4;
            lblBalanceTitle.Text = "Carrier Balance:";
            // 
            // lblBalanceValue
            // 
            lblBalanceValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblBalanceValue.AutoSize = true;
            lblBalanceValue.Location = new Point(178, 83);
            lblBalanceValue.Margin = new Padding(4, 3, 4, 3);
            lblBalanceValue.Name = "lblBalanceValue";
            lblBalanceValue.Size = new Size(341, 15);
            lblBalanceValue.TabIndex = 5;
            lblBalanceValue.Text = "[x,xxx,xxx,xxx Cr]";
            // 
            // lblPositionTitle
            // 
            lblPositionTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPositionTitle.AutoSize = true;
            lblPositionTitle.Location = new Point(117, 107);
            lblPositionTitle.Margin = new Padding(4, 3, 4, 3);
            lblPositionTitle.Name = "lblPositionTitle";
            lblPositionTitle.Size = new Size(53, 15);
            lblPositionTitle.TabIndex = 6;
            lblPositionTitle.Text = "Position:";
            // 
            // lblPositionValue
            // 
            lblPositionValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblPositionValue.AutoEllipsis = true;
            lblPositionValue.AutoSize = true;
            lblPositionValue.Location = new Point(178, 107);
            lblPositionValue.Margin = new Padding(4, 3, 4, 3);
            lblPositionValue.Name = "lblPositionValue";
            lblPositionValue.Size = new Size(341, 15);
            lblPositionValue.TabIndex = 7;
            lblPositionValue.Text = "[system or body]";
            lblPositionValue.DoubleClick += Label_DoubleClickToCopy;
            // 
            // lblCommanderStateTitle
            // 
            lblCommanderStateTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCommanderStateTitle.AutoSize = true;
            lblCommanderStateTitle.Location = new Point(82, 131);
            lblCommanderStateTitle.Margin = new Padding(4, 3, 4, 3);
            lblCommanderStateTitle.Name = "lblCommanderStateTitle";
            lblCommanderStateTitle.Size = new Size(88, 15);
            lblCommanderStateTitle.TabIndex = 8;
            lblCommanderStateTitle.Text = "Commander is:";
            // 
            // lblCommanderStateValue
            // 
            lblCommanderStateValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblCommanderStateValue.AutoEllipsis = true;
            lblCommanderStateValue.AutoSize = true;
            lblCommanderStateValue.Location = new Point(178, 131);
            lblCommanderStateValue.Margin = new Padding(4, 3, 4, 3);
            lblCommanderStateValue.Name = "lblCommanderStateValue";
            lblCommanderStateValue.Size = new Size(341, 15);
            lblCommanderStateValue.TabIndex = 9;
            lblCommanderStateValue.Text = "[On-foot|Docked|Away|Unknown]";
            // 
            // lblNextJumpTitle
            // 
            lblNextJumpTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblNextJumpTitle.AutoSize = true;
            lblNextJumpTitle.Location = new Point(104, 155);
            lblNextJumpTitle.Margin = new Padding(4, 3, 4, 3);
            lblNextJumpTitle.Name = "lblNextJumpTitle";
            lblNextJumpTitle.Size = new Size(66, 15);
            lblNextJumpTitle.TabIndex = 10;
            lblNextJumpTitle.Text = "Next jump:";
            // 
            // lblNextJumpValue
            // 
            lblNextJumpValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblNextJumpValue.AutoEllipsis = true;
            lblNextJumpValue.AutoSize = true;
            lblNextJumpValue.Location = new Point(178, 155);
            lblNextJumpValue.Margin = new Padding(4, 3, 4, 3);
            lblNextJumpValue.Name = "lblNextJumpValue";
            lblNextJumpValue.Size = new Size(341, 15);
            lblNextJumpValue.TabIndex = 11;
            lblNextJumpValue.Text = "(none planned)";
            lblNextJumpValue.DoubleClick += Label_DoubleClickToCopy;
            // 
            // lblTimerTitle
            // 
            lblTimerTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTimerTitle.AutoSize = true;
            lblTimerTitle.Location = new Point(40, 206);
            lblTimerTitle.Margin = new Padding(4, 6, 4, 3);
            lblTimerTitle.Name = "lblTimerTitle";
            lblTimerTitle.Size = new Size(130, 15);
            lblTimerTitle.TabIndex = 12;
            lblTimerTitle.Text = "Jump|Cooldown Timer:";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnNewRoute);
            flowLayoutPanel1.Controls.Add(btnOpenFromSpansh);
            flowLayoutPanel1.Controls.Add(btnClearRoute);
            flowLayoutPanel1.Controls.Add(btnOpenSpansh);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(525, 26);
            flowLayoutPanel1.Margin = new Padding(2);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(346, 28);
            flowLayoutPanel1.TabIndex = 14;
            // 
            // btnNewRoute
            // 
            btnNewRoute.FlatAppearance.BorderSize = 0;
            btnNewRoute.FlatStyle = FlatStyle.Flat;
            btnNewRoute.Location = new Point(3, 3);
            btnNewRoute.Name = "btnNewRoute";
            btnNewRoute.Padding = new Padding(1);
            btnNewRoute.Size = new Size(26, 26);
            btnNewRoute.TabIndex = 3;
            ttipCarrierUI.SetToolTip(btnNewRoute, "Create a new route via Spansh");
            btnNewRoute.UseVisualStyleBackColor = true;
            btnNewRoute.Click += btnNewRoute_Click;
            // 
            // btnOpenFromSpansh
            // 
            btnOpenFromSpansh.FlatAppearance.BorderSize = 0;
            btnOpenFromSpansh.FlatStyle = FlatStyle.Flat;
            btnOpenFromSpansh.Location = new Point(35, 3);
            btnOpenFromSpansh.Name = "btnOpenFromSpansh";
            btnOpenFromSpansh.Size = new Size(26, 26);
            btnOpenFromSpansh.TabIndex = 4;
            ttipCarrierUI.SetToolTip(btnOpenFromSpansh, "Import route from Spansh URL");
            btnOpenFromSpansh.UseVisualStyleBackColor = true;
            btnOpenFromSpansh.Visible = false;
            btnOpenFromSpansh.Click += btnOpenFromSpansh_Click;
            // 
            // btnClearRoute
            // 
            btnClearRoute.FlatAppearance.BorderSize = 0;
            btnClearRoute.FlatStyle = FlatStyle.Flat;
            btnClearRoute.Location = new Point(67, 3);
            btnClearRoute.Name = "btnClearRoute";
            btnClearRoute.Padding = new Padding(1);
            btnClearRoute.Size = new Size(26, 26);
            btnClearRoute.TabIndex = 5;
            ttipCarrierUI.SetToolTip(btnClearRoute, "Clear route");
            btnClearRoute.UseVisualStyleBackColor = true;
            btnClearRoute.Click += btnClearRoute_Click;
            // 
            // btnOpenSpansh
            // 
            btnOpenSpansh.FlatAppearance.BorderSize = 0;
            btnOpenSpansh.FlatStyle = FlatStyle.Flat;
            btnOpenSpansh.Location = new Point(99, 3);
            btnOpenSpansh.Name = "btnOpenSpansh";
            btnOpenSpansh.Padding = new Padding(1);
            btnOpenSpansh.Size = new Size(26, 26);
            btnOpenSpansh.TabIndex = 6;
            ttipCarrierUI.SetToolTip(btnOpenSpansh, "Open route on Spansh");
            btnOpenSpansh.UseVisualStyleBackColor = true;
            btnOpenSpansh.Click += btnOpenSpansh_Click;
            // 
            // clbRoute
            // 
            clbRoute.ContextMenuStrip = ctxRouteMenu;
            clbRoute.Dock = DockStyle.Fill;
            clbRoute.FormattingEnabled = true;
            clbRoute.Location = new Point(527, 59);
            clbRoute.Margin = new Padding(4, 3, 4, 3);
            clbRoute.Name = "clbRoute";
            tlpCarrierLayout.SetRowSpan(clbRoute, 6);
            clbRoute.Size = new Size(342, 251);
            clbRoute.TabIndex = 15;
            clbRoute.MouseDown += clbRoute_MouseDown;
            // 
            // ctxRouteMenu
            // 
            ctxRouteMenu.ImageScalingSize = new Size(24, 24);
            ctxRouteMenu.Items.AddRange(new ToolStripItem[] { ctxRouteMenu_CopySystemName, ctxRouteMenu_SetCurrentPosition });
            ctxRouteMenu.Name = "ctxRouteMenu";
            ctxRouteMenu.Size = new Size(178, 48);
            // 
            // ctxRouteMenu_CopySystemName
            // 
            ctxRouteMenu_CopySystemName.Name = "ctxRouteMenu_CopySystemName";
            ctxRouteMenu_CopySystemName.Size = new Size(177, 22);
            ctxRouteMenu_CopySystemName.Text = "Copy system name";
            ctxRouteMenu_CopySystemName.Click += ctxRouteMenu_CopySystemName_Click;
            // 
            // ctxRouteMenu_SetCurrentPosition
            // 
            ctxRouteMenu_SetCurrentPosition.Name = "ctxRouteMenu_SetCurrentPosition";
            ctxRouteMenu_SetCurrentPosition.Size = new Size(177, 22);
            ctxRouteMenu_SetCurrentPosition.Text = "Set current position";
            ctxRouteMenu_SetCurrentPosition.Click += ctxRouteMenu_SetCurrentPosition_Click;
            // 
            // txtMessages
            // 
            tlpCarrierLayout.SetColumnSpan(txtMessages, 2);
            txtMessages.Dock = DockStyle.Fill;
            txtMessages.Location = new Point(178, 316);
            txtMessages.Margin = new Padding(4, 3, 4, 3);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ReadOnly = true;
            txtMessages.Size = new Size(691, 69);
            txtMessages.TabIndex = 16;
            // 
            // lblMessages
            // 
            lblMessages.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblMessages.AutoSize = true;
            lblMessages.Location = new Point(109, 316);
            lblMessages.Margin = new Padding(4, 3, 4, 3);
            lblMessages.Name = "lblMessages";
            lblMessages.Size = new Size(61, 15);
            lblMessages.TabIndex = 17;
            lblMessages.Text = "Messages:";
            // 
            // pbFuelLevel
            // 
            pbFuelLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbFuelLevel.BackColor = SystemColors.Control;
            pbFuelLevel.Location = new Point(181, 31);
            pbFuelLevel.Margin = new Padding(7);
            pbFuelLevel.MarqueeAnimationSpeed = 0;
            pbFuelLevel.Maximum = 1000;
            pbFuelLevel.Name = "pbFuelLevel";
            pbFuelLevel.Size = new Size(335, 18);
            pbFuelLevel.Style = ProgressBarStyle.Continuous;
            pbFuelLevel.TabIndex = 18;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36F));
            tableLayoutPanel1.Controls.Add(lblTimerValue, 0, 0);
            tableLayoutPanel1.Controls.Add(btnPopOutTimer, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(177, 203);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(343, 107);
            tableLayoutPanel1.TabIndex = 19;
            // 
            // lblTimerValue
            // 
            lblTimerValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTimerValue.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTimerValue.Location = new Point(7, 6);
            lblTimerValue.Margin = new Padding(7, 6, 7, 6);
            lblTimerValue.Name = "lblTimerValue";
            lblTimerValue.Size = new Size(293, 25);
            lblTimerValue.TabIndex = 16;
            lblTimerValue.Text = "0:00:00";
            lblTimerValue.TextAlign = ContentAlignment.TopCenter;
            // 
            // btnPopOutTimer
            // 
            btnPopOutTimer.FlatAppearance.BorderSize = 0;
            btnPopOutTimer.FlatStyle = FlatStyle.Flat;
            btnPopOutTimer.ImageAlign = ContentAlignment.TopCenter;
            btnPopOutTimer.Location = new Point(310, 3);
            btnPopOutTimer.Name = "btnPopOutTimer";
            btnPopOutTimer.Padding = new Padding(1);
            btnPopOutTimer.Size = new Size(30, 30);
            btnPopOutTimer.TabIndex = 17;
            ttipCarrierUI.SetToolTip(btnPopOutTimer, "Open Timer in pop-out window");
            btnPopOutTimer.UseVisualStyleBackColor = true;
            btnPopOutTimer.Click += btnPopOutTimer_Click;
            // 
            // CarrierUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            Controls.Add(tlpCarrierLayout);
            DoubleBuffered = true;
            Margin = new Padding(2);
            Name = "CarrierUI";
            Size = new Size(873, 388);
            tlpCarrierLayout.ResumeLayout(false);
            tlpCarrierLayout.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            ctxRouteMenu.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpCarrierLayout;
        private Label lblNameAndCallsign;
        private Label lblOwningCommander;
        private Label lblFuelTitle;
        private Label lblBalanceTitle;
        private Label lblBalanceValue;
        private Label lblPositionTitle;
        private Label lblPositionValue;
        private Label lblCommanderStateTitle;
        private Label lblCommanderStateValue;
        private Label lblNextJumpTitle;
        private Label lblNextJumpValue;
        private Label lblTimerTitle;
        private FlowLayoutPanel flowLayoutPanel1;
        private CheckedListBox clbRoute;
        private ContextMenuStrip ctxRouteMenu;
        private ToolStripMenuItem ctxRouteMenu_CopySystemName;
        private ToolStripMenuItem ctxRouteMenu_SetCurrentPosition;
        private TextBox txtMessages;
        private Label lblMessages;
        private ThemeableProgressBar pbFuelLevel;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblTimerValue;
        private ThemeableIconButton btnPopOutTimer;
        private ToolTip ttipCarrierUI;
        private ThemeableIconButton btnNewRoute;
        private ThemeableIconButton btnClearRoute;
        private ThemeableIconButton btnOpenSpansh;
        private Label lblCapacityTitle;
        private Label lblCapacityValue;
        private ThemeableIconButton btnOpenFromSpansh;
    }
}
