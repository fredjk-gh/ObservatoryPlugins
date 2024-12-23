using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public class ThemeableProgressBar : ProgressBar
    {
        // Adapted from https://stackoverflow.com/questions/778678/how-to-change-the-color-of-progressbar-in-c-sharp-net-3-5
        public ThemeableProgressBar() : base()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = e.ClipRectangle;

            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            e.Graphics.FillRectangle(new SolidBrush(BackColor), 2, 2, rec.Width, rec.Height);
            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            rec.Height = rec.Height - 4;
            e.Graphics.FillRectangle(new SolidBrush(ForeColor), 2, 2, rec.Width, rec.Height);
        }
    }
}
