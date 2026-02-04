using com.github.fredjk_gh.PluginCommon.UI.Shared;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public class ThemeableImageButton : ScalableImageButton
    {
        public static ThemeableImageButton New(Bitmap img, EventHandler clickHandler, Size? size = null)
        {
            ThemeableImageButton btn = new()
            {
                Size = size ?? new(32, 32),
                FlatStyle = FlatStyle.Flat,
            };

            btn.SetOriginalImage(img);
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += clickHandler;
            return btn;
        }

        public ThemeableImageButton() : base()
        {
            DoubleBuffered = true;
        }

        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                UpdateImage();
            }
        }

        protected override void UpdateImage()
        {
            if (_originalImage == null) return;

            Image = ImageCommon.RecolorAndSizeImage(_originalImage, ForeColor, _initialImageSize ?? _originalImage.Size);
        }
    }
}
