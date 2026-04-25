using com.github.fredjk_gh.PluginCommon.UI.Shared;
using System.ComponentModel;
using System.Data;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public partial class ThemeableImageLabel : UserControl
    {
        private readonly Dictionary<Guid, (ImageSpec Spec, PictureBox PB)> _imgs = [];
        protected Color _imgDefaultColor = Color.Transparent;
        private LabelPositionType _position = LabelPositionType.Right;
        private ToolTip _toolTips = null;

        public ThemeableImageLabel()
        {
            InitializeComponent();
            DoubleBuffered = true;

            if (lblText.Text.Length > 0)
            {
                lblText.Visible = true;
            }

            lblText.Click += ClickFowarder;
        }

        [Description("Sets the Text property value of the title label")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => lblText.Text;
            set
            {
                lblText.Visible = !string.IsNullOrWhiteSpace(value);
                lblText.Text = value;
            }
        }

        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;

                lblText.ForeColor = value;

                // Use this color for the images also, unless explicitly specified.
                ColorizeImages(ImageColor == Color.Transparent ? value : ImageColor);
            }
        }

        [Description("Overrides the default image color (if not explicitly provided by way of ImageSpec). If set to Color.Transparent (the default), ForeColor is used.")]
        [Category("Appearance")]
        [Browsable(true)]
        public Color ImageColor
        {
            get => _imgDefaultColor;
            set
            {
                _imgDefaultColor = value;

                // Default back to ForeColor if unset (null)
                ColorizeImages(value == Color.Transparent ? ForeColor : value);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolTip ToolTipManager
        {
            get => _toolTips;
            set
            {
                _toolTips = value;
                if (value is not null) // In case images were set first.
                {
                    foreach (var (Spec, PB) in _imgs.Values)
                    {
                        SetTooltipInternal(PB, Spec.ToolTip);
                    }
                }
            }
        }

        [Description("Sets the position of the text label portion of the control")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public LabelPositionType LabelPosition
        {
            get => _position;
            set
            {
                _position = value;
                switch (value)
                {
                    case LabelPositionType.Left:
                        flpContent.Controls.SetChildIndex(lblText, 0);
                        break;
                    case LabelPositionType.Right:
                        flpContent.Controls.SetChildIndex(lblText, flpContent.Controls.Count - 1);
                        break;
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<ImageSpec> Images
        {
            get => [.. _imgs.Values.Select(v => v.Spec)];
        }

        public void AddImages(List<ImageSpec> imgs)
        {
            foreach (ImageSpec img in imgs)
            {
                AddImage(img);
            }
        }

        public void AddImage(ImageSpec spec)
        {
            Color effectiveColor = spec.Color ?? (ImageColor == Color.Transparent ? ForeColor : ImageColor);
            Image imgToSet = ImageCommon.RecolorAndSizeImage(spec.Image, effectiveColor, spec.Size ?? spec.Image.Size);

            PictureBox pb = new()
            {
                Image = imgToSet,
                Visible = spec.Visible,
                Size = spec.Size ?? spec.Image.Size,
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new(0),
            };
            pb.Click += ClickFowarder;
            pb.SizeChanged += PBSizeChanged;

            var labelIndex = flpContent.Controls.IndexOf(lblText);
            flpContent.Controls.Add(pb);
            if (LabelPosition == LabelPositionType.Right)
            {
                // Move the new control in the second-to-last position (where the label is before adding the new control).
                flpContent.Controls.SetChildIndex(pb, labelIndex);
            }
            _imgs.Add(spec.Guid, (spec, pb));
            SetTooltipInternal(pb, spec.ToolTip);
        }

        public void ClearImages()
        {
            SuspendLayout();
            // Clear the tool tip, remove from the flow panel before clearing the image specs entirely.
            foreach (var img in _imgs)
            {
                img.Value.PB.Click -= ClickFowarder;
                SetTooltipInternal(img.Value.PB, null);
                flpContent.Controls.Remove(img.Value.PB);
            }
            _imgs.Clear();
            ResumeLayout(true);
        }

        private void ColorizeImages(Color value)
        {
            foreach (var (Spec, PB) in _imgs.Values)
            {
                ColorAndSizeImageToPB(PB, Spec, Spec.Color ?? value);
            }
        }

        private void ColorAndSizeImageToPB(PictureBox pb, ImageSpec spec, Color c)
        {
            // So the PB may not preserve its original aspect ratio. Find the scale factor to preserve the
            // aspect ratio and resize.
            var imgSize = spec.Size ?? spec.Image.Size;
            float widthScale = (float)pb.Size.Width / imgSize.Width;
            float heightScale = (float)pb.Size.Height / imgSize.Height;
            float minScale = Math.Min(widthScale, heightScale);
            Size newSize = new(Convert.ToInt32(imgSize.Width * minScale), Convert.ToInt32(imgSize.Height * minScale));

            pb.Image = ImageCommon.RecolorAndSizeImage(spec.Image, spec.Color ?? c, newSize);
            pb.Invalidate();
        }

        public void SetVisibility(Guid id, bool newIsVisible)
        {
            if (!_imgs.TryGetValue(id, out (ImageSpec Spec, PictureBox PB) img)) return;
            img.Spec.Visible = newIsVisible;
            img.PB.Visible = newIsVisible;
        }

        public void SetTooltip(Guid id, string ttip)
        {
            if (!_imgs.TryGetValue(id, out (ImageSpec Spec, PictureBox PB) img)) return;
            img.Spec.ToolTip = ttip;
            SetTooltipInternal(img.PB, ttip);
        }

        private void SetTooltipInternal(PictureBox pb, string? ttip)
        {
            ToolTipManager?.SetToolTip(pb, ttip);
        }

        private void ClickFowarder(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void PBSizeChanged(object sender, EventArgs e)
        {
            var matches = _imgs.Where(e => e.Value.PB == sender).ToList();

            if (matches.Count == 0) return;
            var img = matches[0];

            ColorAndSizeImageToPB(img.Value.PB, img.Value.Spec, ImageColor == Color.Transparent ? ForeColor : ImageColor);
        }
    }
}
