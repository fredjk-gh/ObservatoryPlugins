using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryArchivist.UI
{
    internal class ThemeableIconButton : Button
    {
        private Bitmap _originalIcon = null;
        private Size? _resize = null;

        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                MaybeRecolorImage(value);
            }
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                FlatAppearance.MouseOverBackColor = AdjustColor(value, 75);
            }
        }

        public void SetIcon(Bitmap icon, Size? resize = null)
        {
            _originalIcon = icon;
            _resize = resize;
            MaybeRecolorImage(ForeColor);
        }

        internal Color AdjustColor(Color color, int offset = 20)
        {
            var r = color.R + (color.R > 127 ? -1 * offset : offset);
            var g = color.G + (color.G > 127 ? -1 * offset : offset);
            var b = color.B + (color.B > 127 ? -1 * offset : offset);
            return Color.FromArgb(r, g, b);
        }

        internal void MaybeRecolorImage(Color c)
        {
            if (_originalIcon == null) return;

            // derived from https://stackoverflow.com/questions/4699762/how-do-i-recolor-an-image-see-images
            ColorMatrix cm = new ColorMatrix(new float[][]
            {
                new float[] {r(c.R), 0, 0, 0, 0},
                new float[] {0, r(c.G), 0, 0, 0},
                new float[] {0, 0, r(c.B), 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });
            var ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            var colored = new Bitmap(_resize?.Width ?? _originalIcon.Width, _resize?.Height ?? _originalIcon.Height);
            var rect = new Rectangle(new(0, 0), _resize ?? _originalIcon.Size);
            var gfx = Graphics.FromImage(colored);

            gfx.DrawImage(_originalIcon, rect, 0, 0, _originalIcon.Width, _originalIcon.Height, GraphicsUnit.Pixel, ia);

            Image = colored;
        }

        private float r(byte component)
        {
            return component > 0 ? (component / 255.0f) : 0;
        }
    }
}
