using System.ComponentModel;
using System.Runtime.InteropServices;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public class ThemeableProgressBar : ProgressBar
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern uint SendMessage(IntPtr hWnd,
              uint Msg,
              uint wParam,
              uint lParam);

        private Color _borderColor = SystemColors.ActiveBorder;

        // Adapted from:
        // - https://stackoverflow.com/questions/778678/how-to-change-the-color-of-progressbar-in-c-sharp-net-3-5
        // - https://stackoverflow.com/questions/2834761/disable-winforms-progressbar-animation
        public ThemeableProgressBar() : base()
        {
            Style = ProgressBarStyle.Blocks;
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            SendMessage(this.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0003, //PBST_PAUSED
                0);

            SendMessage(this.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0002, //PBST_ERROR
                0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int borderThickness = 2;
            // Clone clip rectangle for modification.
            Rectangle rec = new(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);

            // Background
            using SolidBrush fillBackBrush = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(fillBackBrush, e.ClipRectangle);

            // Progress bar
            rec.X += borderThickness;
            rec.Width = (int)(rec.Width * (Maximum > 0 ? ((double)Value / Maximum) : 0)) - (2 * borderThickness);
            using SolidBrush fillForeBrush = new SolidBrush(ForeColor);
            e.Graphics.FillRectangle(fillForeBrush, rec);

            // Border
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                _borderColor, borderThickness, ButtonBorderStyle.Solid,
                _borderColor, borderThickness, ButtonBorderStyle.Solid,
                _borderColor, borderThickness, ButtonBorderStyle.Solid,
                _borderColor, borderThickness, ButtonBorderStyle.Solid);

            base.OnPaint(e);
        }

        [Description("Sets the color of the control's border")]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ActiveBorder")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        public new int Value
        {
            get { return base.Value; }
            set { base.Value = Math.Min(value, Maximum); Invalidate(); }    // Remember to invalidate.
        }
    }
}
