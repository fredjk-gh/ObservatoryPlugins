namespace com.github.fredjk_gh.PluginCommon.UI.Shared
{
    public class ImageSpec
    {
        private readonly Bitmap _img;
        private Color? _imgColor = null;
        private Size? _imgSize = null;

        readonly public Guid Guid = Guid.NewGuid();

        public ImageSpec(Bitmap img)
        {
            _img = img;
        }

        public ImageSpec(ImageSpec cloneFrom)
        {
            _img = cloneFrom.Image;
            _imgSize = cloneFrom.Size;
            Visible = cloneFrom.Visible;

            // Don't clone Tag, Tooltip or Color;
            // Guid is initialized statically.
        }

        public Bitmap Image
        {
            get => _img;
        }

        public Color? Color
        {
            get => _imgColor;
            init => _imgColor = value;
        }

        public Size? Size
        {
            get => _imgSize;
            set => _imgSize = value;
        }

        public string Tag { get; init; }

        public string ToolTip { get; set; }

        public bool Visible { get; set; }
    }
}
