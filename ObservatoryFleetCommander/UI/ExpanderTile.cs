using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public partial class ExpanderTile : UserControl
    {
        private bool _expanded = false;
        private Control _contentControl;

        public ExpanderTile(Control content, string title, bool expanded = false)
        {
            InitializeComponent();

            _contentControl = content;
            lblTileTitle.Text = title;

            tlpLayout.Controls.Add(_contentControl, 0, 1);
            tlpLayout.SetColumnSpan(_contentControl, 2);

            MinimumSize = new Size(content.Width, Math.Max(btnToggle.Height, lblTileTitle.Height));

            DoToggleView(expanded);
        }

        private void DoToggleView(bool expanded)
        {
            if (expanded)
            {
                btnToggle.Text = "➖";
                _contentControl.Visible = true;
            }
            else
            {
                btnToggle.Text = "➕";
                _contentControl.Visible = false;
            }
            _expanded = expanded;
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            DoToggleView(!_expanded);
        }

        private void lblTileTitle_DoubleClick(object sender, EventArgs e)
        {
            DoToggleView(!_expanded);
        }
    }
}
