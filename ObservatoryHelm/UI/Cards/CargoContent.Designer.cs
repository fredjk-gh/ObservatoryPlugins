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
            var listViewItem1 = new ListViewItem(new string[] { "2", "Limpets" }, -1);
            var listViewItem2 = new ListViewItem(new string[] { "3", "Tritium" }, -1);
            var listViewItem3 = new ListViewItem(new string[] { "2", "Limpets" }, -1);
            var listViewItem4 = new ListViewItem(new string[] { "3", "Tritium" }, -1);
            lvCargoShip = new ListView();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            pbCargoGaugeShip = new ThemeableProgressBar();
            lvCargoSRV = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            pbCargoGaugeSRV = new ThemeableProgressBar();
            tabCargos = new TabControl();
            tabCargoShip = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            tabCargoSRV = new TabPage();
            tableLayoutPanel2 = new TableLayoutPanel();
            tabCargos.SuspendLayout();
            tabCargoShip.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tabCargoSRV.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // lvCargoShip
            // 
            lvCargoShip.Columns.AddRange(new ColumnHeader[] { columnHeader4, columnHeader5 });
            lvCargoShip.Dock = DockStyle.Fill;
            lvCargoShip.FullRowSelect = true;
            lvCargoShip.GridLines = true;
            lvCargoShip.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2 });
            lvCargoShip.Location = new Point(3, 33);
            lvCargoShip.Name = "lvCargoShip";
            lvCargoShip.ShowGroups = false;
            lvCargoShip.Size = new Size(473, 112);
            lvCargoShip.TabIndex = 20;
            lvCargoShip.UseCompatibleStateImageBehavior = false;
            lvCargoShip.View = View.Details;
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
            // pbCargoGaugeShip
            // 
            pbCargoGaugeShip.Dock = DockStyle.Fill;
            pbCargoGaugeShip.Location = new Point(3, 3);
            pbCargoGaugeShip.Maximum = 20;
            pbCargoGaugeShip.Name = "pbCargoGaugeShip";
            pbCargoGaugeShip.Size = new Size(473, 24);
            pbCargoGaugeShip.Step = 1;
            pbCargoGaugeShip.Style = ProgressBarStyle.Continuous;
            pbCargoGaugeShip.TabIndex = 19;
            pbCargoGaugeShip.Value = 5;
            // 
            // lvCargoSRV
            // 
            lvCargoSRV.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            lvCargoSRV.Dock = DockStyle.Fill;
            lvCargoSRV.FullRowSelect = true;
            lvCargoSRV.GridLines = true;
            lvCargoSRV.Items.AddRange(new ListViewItem[] { listViewItem3, listViewItem4 });
            lvCargoSRV.Location = new Point(3, 33);
            lvCargoSRV.Name = "lvCargoSRV";
            lvCargoSRV.ShowGroups = false;
            lvCargoSRV.Size = new Size(473, 112);
            lvCargoSRV.TabIndex = 21;
            lvCargoSRV.UseCompatibleStateImageBehavior = false;
            lvCargoSRV.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Qty";
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Item";
            columnHeader2.Width = 250;
            // 
            // pbCargoGaugeSRV
            // 
            pbCargoGaugeSRV.Dock = DockStyle.Fill;
            pbCargoGaugeSRV.Location = new Point(3, 3);
            pbCargoGaugeSRV.Maximum = 20;
            pbCargoGaugeSRV.Name = "pbCargoGaugeSRV";
            pbCargoGaugeSRV.Size = new Size(473, 24);
            pbCargoGaugeSRV.Step = 1;
            pbCargoGaugeSRV.Style = ProgressBarStyle.Continuous;
            pbCargoGaugeSRV.TabIndex = 22;
            pbCargoGaugeSRV.Value = 5;
            // 
            // tabCargos
            // 
            tabCargos.Controls.Add(tabCargoShip);
            tabCargos.Controls.Add(tabCargoSRV);
            tabCargos.Dock = DockStyle.Fill;
            tabCargos.Location = new Point(0, 0);
            tabCargos.Name = "tabCargos";
            tabCargos.SelectedIndex = 0;
            tabCargos.Size = new Size(493, 182);
            tabCargos.TabIndex = 21;
            // 
            // tabCargoShip
            // 
            tabCargoShip.Controls.Add(tableLayoutPanel1);
            tabCargoShip.Location = new Point(4, 24);
            tabCargoShip.Name = "tabCargoShip";
            tabCargoShip.Padding = new Padding(3);
            tabCargoShip.Size = new Size(485, 154);
            tabCargoShip.TabIndex = 0;
            tabCargoShip.Text = "Ship";
            tabCargoShip.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lvCargoShip, 0, 1);
            tableLayoutPanel1.Controls.Add(pbCargoGaugeShip, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(479, 148);
            tableLayoutPanel1.TabIndex = 22;
            // 
            // tabCargoSRV
            // 
            tabCargoSRV.Controls.Add(tableLayoutPanel2);
            tabCargoSRV.Location = new Point(4, 24);
            tabCargoSRV.Name = "tabCargoSRV";
            tabCargoSRV.Padding = new Padding(3);
            tabCargoSRV.Size = new Size(485, 154);
            tabCargoSRV.TabIndex = 1;
            tabCargoSRV.Text = "SRV";
            tabCargoSRV.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Controls.Add(pbCargoGaugeSRV, 0, 0);
            tableLayoutPanel2.Controls.Add(lvCargoSRV, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(479, 148);
            tableLayoutPanel2.TabIndex = 22;
            // 
            // CargoContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabCargos);
            Name = "CargoContent";
            Size = new Size(493, 182);
            tabCargos.ResumeLayout(false);
            tabCargoShip.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tabCargoSRV.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListView lvCargoShip;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ThemeableProgressBar pbCargoGaugeShip;
        private TabControl tabCargos;
        private TabPage tabCargoShip;
        private TabPage tabCargoSRV;
        private ListView lvCargoSRV;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ThemeableProgressBar pbCargoGaugeSRV;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
    }
}
