using com.github.fredjk_gh.PluginCommon.UI.Shared;
using System.ComponentModel;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public class ScalableImageButton : Button
    {
        public static readonly Size MEDIUM_BUTTON_SIZE = new(32, 32);
        protected Image _originalImage;
        protected Size? _initialImageSize;

        public ScalableImageButton() : base()
        {
            DoubleBuffered = true;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                FlatAppearance.MouseOverBackColor = ImageCommon.AdjustColor(value, 75);
            }
        }

        [Description("Sets the size of the control's image")]
        [Category("Appearance")]
        [DefaultValue(typeof(Size), "24, 24")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size? ImageSize
        {
            get => _initialImageSize;
            set
            {
                _initialImageSize = value;
                if (_originalImage != null)
                {
                    UpdateImage();
                }
            }
        }

        [Description("Sets the control's untouched source image")]
        [Category("Appearance")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image OriginalImage
        {
            get => _originalImage;

            set
            {
                _originalImage = value;
                UpdateImage();
            }
        }

        public virtual void SetOriginalImage(Image originalImage)
        {
            SetOriginalImage(originalImage, MEDIUM_BUTTON_SIZE);
        }

        public virtual void SetOriginalImage(Image originalImage, Size? size = null, Color? foreColor = null)
        {
            if (size.HasValue) ImageSize = size.Value;
            if (foreColor.HasValue) ForeColor = foreColor.Value;
            OriginalImage = originalImage;
        }

        protected virtual void UpdateImage()
        {
            if (_originalImage == null) return;

            Image = ImageCommon.ResizeImage(_originalImage, _initialImageSize ?? _originalImage.Size);
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            if (_originalImage == null) return;

            ImageSize = ImageCommon.ScaleSize(factor, _originalImage.Size, Image.Size);
        }
    }
}
