using System.Drawing.Imaging;

namespace com.github.fredjk_gh.PluginCommon.UI.Shared
{
    public class ImageCommon
    {
        public static Image RecolorAndSizeImage(Image original, Color c, Size? resize = null)
        {
            // derived from https://stackoverflow.com/questions/4699762/how-do-i-recolor-an-image-see-images
            ColorMatrix cm = new (
            [
                [r(c.R), 0, 0, 0, 0],
                [0, r(c.G), 0, 0, 0],
                [0, 0, r(c.B), 0, 0],
                [0, 0, 0, 1, 0],
                [0, 0, 0, 0, 1]
            ]);
            var ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            var colored = new Bitmap(resize?.Width ?? original.Width, resize?.Height ?? original.Height);
            var rect = new Rectangle(new(0, 0), resize ?? original.Size);
            var gfx = Graphics.FromImage(colored);

            gfx.DrawImage(original, rect, 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, ia);

            return colored;
        }

        private static float r(byte component)
        {
            return component > 0 ? (component / 255.0f) : 0;
        }

        public static Color AdjustColor(Color color, int offset = 20)
        {
            var r = color.R + (color.R > 127 ? -1 * offset : offset);
            var g = color.G + (color.G > 127 ? -1 * offset : offset);
            var b = color.B + (color.B > 127 ? -1 * offset : offset);
            return Color.FromArgb(r, g, b);
        }

        public static Size ScaleSize(SizeF factor, Size originalSize, Size? overriddenSize = null)
        {
            return new Size(
                (int)Math.Round((overriddenSize?.Width ?? originalSize.Width) * factor.Width),
                (int)Math.Round((overriddenSize?.Height ?? originalSize.Height) * factor.Height));
        }

        public static Image ResizeImage(Image original, Size newSize)
        {
            var resized = new Bitmap(newSize.Width, newSize.Height);
            var rect = new Rectangle(new(0, 0), newSize);
            var gfx = Graphics.FromImage(resized);

            gfx.DrawImage(original, rect, 0, 0, original.Width, original.Height, GraphicsUnit.Pixel);

            return resized;
        }
    }
}
