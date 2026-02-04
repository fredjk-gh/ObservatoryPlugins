namespace com.github.fredjk_gh.PluginCommon.UI.Shared
{
    public class ImageLabelSpec
    {
        private readonly List<ImageSpec> _imageSpecs = [];
        private LabelPositionType _labelPos = LabelPositionType.Right;

        public static ImageLabelSpec New(string label)
        {
            return new() { Label = label };
        }

        public static ImageLabelSpec New(string label, ImageSpec imgSpec)
        {
            ImageLabelSpec il = new() { Label = label };
            il.ImageSpecs.Add(imgSpec);

            return il;
        }

        public static ImageLabelSpec New(string label, List<ImageSpec> imgSpecs)
        {
            ImageLabelSpec il = new() { Label = label };
            il.ImageSpecs.AddRange(imgSpecs);

            return il;
        }

        public static ImageLabelSpec New(List<ImageSpec> imgSpecs)
        {
            ImageLabelSpec il = new();
            il.ImageSpecs.AddRange(imgSpecs);

            return il;
        }

        public string Label { get; private init; }

        public LabelPositionType LabelPosition
        {
            get => _labelPos;
            set => _labelPos = value;
        }

        public List<ImageSpec> ImageSpecs { get => _imageSpecs; }
    }
}
