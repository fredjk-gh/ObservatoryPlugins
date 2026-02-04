using com.github.fredjk_gh.PluginCommon.UI.Shared;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;


namespace com.github.fredjk_gh.PluginCommon.UI
{
    public class DGVImageLabelCell : DataGridViewCell
    {
        private readonly Type _valueType = typeof(ImageLabelSpec);
        private int _spacing = 1;
        private SizeF _scaleFactor = new(1, 1);
        private FontStyle _fontStyle = FontStyle.Regular;
        private Font _initialDgvFont;

        public DGVImageLabelCell()
        {
            ValueType = _valueType;
        }

        [MaybeNull]
        public override object DefaultNewRowValue => null;

        [MaybeNull]
        public override Type EditType => null; // No editor

        public int Spacing
        {
            get => _spacing;
            set => _spacing = value;
        }

        public int FontSizeAdjustment { get; set; }

        public FontStyle FontStyle
        {
            get => _fontStyle;
            set => _fontStyle = value;
        }

        public SizeF ScaleFactor
        {
            get => _scaleFactor;
            set => _scaleFactor = value;
        }

        protected override void OnDataGridViewChanged()
        {
            if (DataGridView is null)
            {
                return;
            }

            _initialDgvFont = DataGridView.Font;
            DataGridView.FontChanged += DataGridView_FontChanged;
        }

        private void DataGridView_FontChanged(object sender, EventArgs e)
        {
            Debug.WriteIf(false, $"DGV.FontChanged: {DataGridView.Font}");
            if (DataGridView is null) return;

            if (_initialDgvFont is null) // should never happen.
            {
                _initialDgvFont = DataGridView.Font;
                return;
            }

            // The font changed -- derive a scaling factor from it for scaling the image.
            if (_initialDgvFont.Size != DataGridView.Font.Size)
            {
                var factor = DataGridView.Font.Size / _initialDgvFont.Size;
                _scaleFactor = new(factor, factor);
            }
        }

        protected override void Paint(
            Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates cellState,
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            using (Brush b = new SolidBrush(Selected ? cellStyle.SelectionBackColor : cellStyle.BackColor))
            {
                graphics.FillRectangle(b, cellBounds);
            }

            ImageLabelSpec v = (ImageLabelSpec)value;
            if (v is null
                || DataGridView is null
                || string.IsNullOrEmpty(v.Label) && !v.ImageSpecs.Select(i => i.Visible).Any())
            {
                return;
            }

            Rectangle bSize = BorderWidths(DataGridView.AdvancedCellBorderStyle);
            int horizontalSpacing = bSize.Left + bSize.Width + cellStyle.Padding.Horizontal;
            int verticalSpacing = bSize.Top + bSize.Height + cellStyle.Padding.Vertical;

            PointF pos = new(
                cellBounds.Left + bSize.Left + bSize.Width + cellStyle.Padding.Left,
                cellBounds.Top + bSize.Top + bSize.Height + cellStyle.Padding.Top);
            RectangleF paddedBounds = new(pos,
                new SizeF(Math.Max(1, cellBounds.Width - horizontalSpacing),
                        Math.Max(1, cellBounds.Height - verticalSpacing)));

            // For now -- never wrap the icons.
            Size iconSizes = new(0, 0);
            int iconSpacing = 0;
            var imgList = v.ImageSpecs.Where(i => i.Visible).ToList();
            if (imgList.Count > 0)
            {
                iconSizes = ScaleSize(new(
                        imgList.Select(i => i.Size ?? i.Image.Size).Select(s => s.Width).Sum(),
                        imgList.Select(i => i.Size ?? i.Image.Size).Select(s => s.Height).Max()),
                    _scaleFactor);
                iconSpacing = Spacing * (imgList.Count - 1);
            }

            SizeF labelSize = new(0, 0);
            if (!string.IsNullOrWhiteSpace(v.Label))
            {
                if (cellStyle.WrapMode == DataGridViewTriState.True)
                {
                    labelSize = graphics.MeasureString(v.Label, cellStyle.Font, (int)paddedBounds.Width);
                }
                else
                {
                    labelSize = graphics.MeasureString(v.Label, cellStyle.Font);
                }
            }

            bool wrappingRequired = cellStyle.WrapMode == DataGridViewTriState.True
                && labelSize.Width + iconSizes.Width > cellBounds.Width - horizontalSpacing;

            // Determine position of string
            if (v.LabelPosition == LabelPositionType.Left && !string.IsNullOrEmpty(v.Label))
            {
                // paint label
                PaintString(graphics, v.Label, cellStyle, pos, paddedBounds);

                if (wrappingRequired)
                    pos.Y += labelSize.Height + Spacing;
                else
                    pos.X += labelSize.Width + Spacing;
            }

            // Paint visible icons
            foreach (ImageSpec i in imgList)
            {
                Size size = ScaleSize(i.Size ?? i.Image.Size, _scaleFactor);
                Image img = ImageCommon.RecolorAndSizeImage(i.Image, cellStyle.ForeColor, size);

                PointF offsetPos = new(pos.X, pos.Y + 2);
                graphics.DrawImage(img, offsetPos);

                pos.X += size.Width + Spacing;
            }

            if (v.LabelPosition == LabelPositionType.Right && !string.IsNullOrEmpty(v.Label))
            {
                if (wrappingRequired && iconSizes.Height > 0)
                {
                    pos.X = cellBounds.Left + bSize.Left + bSize.Width + cellStyle.Padding.Left;
                    pos.Y += iconSizes.Height + Spacing;
                }

                paddedBounds.X = pos.X;
                paddedBounds.Y = pos.Y;
                // paint label
                PaintString(graphics, v.Label, cellStyle, pos, paddedBounds);
            }
        }

        protected override Size GetPreferredSize(
            Graphics g, DataGridViewCellStyle cellStyle, int rowIndex, Size conSize)
        {
            Size pSize = new(-1, -1);
            ImageLabelSpec v = (ImageLabelSpec)Value;

            if (DataGridView is null || v is null || cellStyle is null)
                return pSize;

            Rectangle bSize = BorderWidths(DataGridView.AdvancedCellBorderStyle);
            int horizontalSpacing = bSize.Left + bSize.Width + cellStyle.Padding.Horizontal;
            int verticalSpacing = bSize.Top + bSize.Height + cellStyle.Padding.Vertical;

            // For now -- never wrap the icons.
            Size iconSizes = new(0, 0);
            int iconSpacing = 0;
            var imgList = v.ImageSpecs.Where(i => i.Visible).ToList();
            if (imgList.Count > 0)
            {
                iconSizes = ScaleSize(new(
                        imgList.Select(i => i.Size ?? i.Image.Size).Select(s => s.Width).Sum(),
                        imgList.Select(i => i.Size ?? i.Image.Size).Select(s => s.Height).Max()),
                    _scaleFactor);
                iconSpacing = Spacing * (imgList.Count - 1);
            }
            Font f = GetFont(cellStyle);

            // No width constraint or no wrapping -- similar logic here.
            if (conSize.Width == 0 || cellStyle.WrapMode != DataGridViewTriState.True)
            {
                // Find width when rendering without wrapping.
                Size labelSize = new(0, 0); // No label
                if (!string.IsNullOrWhiteSpace(v.Label))
                {
                    var labelSizeF = g.MeasureString(v.Label, f);
                    labelSize = new((int)Math.Ceiling(labelSizeF.Width), (int)Math.Ceiling(labelSizeF.Height));
                }

                horizontalSpacing += iconSpacing;
                if (imgList.Count > 0 && !string.IsNullOrWhiteSpace(v.Label))
                    horizontalSpacing += Spacing;
                pSize = new(labelSize.Width + iconSizes.Width + horizontalSpacing, Math.Max(iconSizes.Height, labelSize.Height));
            }
            else if (conSize.Width > 0 && conSize.Height == 0 && cellStyle.WrapMode == DataGridViewTriState.True) // Width is constrained and wrapping enabled
            {
                // Find height when rendering with wrapping within the width constraint. Only text is wrapped.
                Size labelSize = new(0, 0); // No label
                if (!string.IsNullOrWhiteSpace(v.Label))
                {
                    var labelSizeF = g.MeasureString(v.Label, f, conSize.Width);
                    labelSize = new((int)Math.Ceiling(labelSizeF.Width), (int)Math.Ceiling(labelSizeF.Height));
                }
                bool wrapIconsAndText = false;
                if (imgList.Count > 0 && !string.IsNullOrWhiteSpace(v.Label))
                {
                    horizontalSpacing += iconSpacing;
                    // Both text and icons -- do they both fit within available space?
                    if (labelSize.Width + Spacing + iconSizes.Width + iconSpacing > conSize.Width - horizontalSpacing)
                    {
                        // This needs to wrap, add vertical spacing between icons and text.
                        wrapIconsAndText = true;
                        verticalSpacing += Spacing;
                    }
                    else
                        // Everything fits --- Need horizontal spacing between icons and text.
                        horizontalSpacing += Spacing;
                }
                if (wrapIconsAndText)
                    pSize = new(conSize.Width, labelSize.Height + iconSizes.Height + verticalSpacing);
                else
                    pSize = new(
                        labelSize.Width + iconSizes.Width + horizontalSpacing,
                        Math.Max(labelSize.Height, iconSizes.Height));
            }
            else // invalid case: fixed size.
            {
                Debug.WriteLine($"DGVImageLabelCell.GetPreferredSize: Unexpected conditions; returning unuseful preferred size.");
            }
            return pSize;
        }

        private void PaintString(Graphics g, string text, DataGridViewCellStyle cellStyle, PointF position, RectangleF cellBounds)
        {
            using Brush b = new SolidBrush(cellStyle.ForeColor);
            Font f = GetFont(cellStyle);
            if (cellStyle.WrapMode == DataGridViewTriState.True)
            {
                g.DrawString(text, f, b, cellBounds);
            }
            else
            {
                g.DrawString(text, f, b, position);
            }
        }

        private Font GetFont(DataGridViewCellStyle cellStyle)
        {
            Font f = cellStyle.Font;
            if (FontStyle != FontStyle.Regular || FontSizeAdjustment != 0)
            {
                f = new Font(cellStyle.Font.FontFamily, cellStyle.Font.Size + FontSizeAdjustment, FontStyle);
            }
            return f;
        }

        private Size ScaleSize(Size size, SizeF factor)
        {
            return new Size((int)(size.Width * factor.Width), (int)(size.Height * factor.Height));
        }
    }
}
