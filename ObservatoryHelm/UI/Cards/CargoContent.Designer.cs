using com.github.fredjk_gh.PluginCommon.UI;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    partial class CargoContent
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
            var listViewItem3 = new ListViewItem(new string[] { "2", "Limpets" }, -1);
            var listViewItem4 = new ListViewItem(new string[] { "3", "Tritium" }, -1);
            lvCargo = new ListView();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            pbCargoGauge = new ThemeableProgressBar();
            ttManager = new ToolTip(components);
            SuspendLayout();
            // 
            // lvCargo
            // 
            lvCargo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvCargo.Columns.AddRange(new ColumnHeader[] { columnHeader4, columnHeader5 });
            lvCargo.FullRowSelect = true;
            lvCargo.GridLines = true;
            lvCargo.Items.AddRange(new ListViewItem[] { listViewItem3, listViewItem4 });
            lvCargo.Location = new Point(9, 33);
            lvCargo.Name = "lvCargo";
            lvCargo.ShowGroups = false;
            lvCargo.Size = new Size(331, 121);
            lvCargo.TabIndex = 20;
            ttManager.SetToolTip(lvCargo, "Cargo hold contents");
            lvCargo.UseCompatibleStateImageBehavior = false;
            lvCargo.View = View.Details;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Qty";
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Item";
            columnHeader5.Width = 250;
            // 
            // pbCargoGauge
            // 
            pbCargoGauge.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbCargoGauge.Location = new Point(8, 8);
            pbCargoGauge.Maximum = 20;
            pbCargoGauge.Name = "pbCargoGauge";
            pbCargoGauge.Size = new Size(331, 21);
            pbCargoGauge.Step = 1;
            pbCargoGauge.Style = ProgressBarStyle.Continuous;
            pbCargoGauge.TabIndex = 19;
            ttManager.SetToolTip(pbCargoGauge, "5 / 20 T used");
            pbCargoGauge.Value = 5;
            // 
            // CargoContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lvCargo);
            Controls.Add(pbCargoGauge);
            Name = "CargoContent";
            Size = new Size(347, 162);
            ResumeLayout(false);
        }

        #endregion

        private ListView lvCargo;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ThemeableProgressBar pbCargoGauge;
        private ToolTip ttManager;
    }
}
