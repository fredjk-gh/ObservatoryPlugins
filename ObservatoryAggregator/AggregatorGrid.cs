using com.github.fredjk_gh.ObservatoryAggregator.UI;
using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using static com.github.fredjk_gh.ObservatoryAggregator.BodySummary;


namespace com.github.fredjk_gh.ObservatoryAggregator
{
    enum AggregatorRowStyle
    {
        Default,
        H1,
        H2,
    }

    public enum AggregatorRowContentType
    {
        SystemSummary,
        BodySummary,
        GenericHeader,
        Notification,
        Blank,
    }

    public class AggregatorGrid
    {
        private readonly Size _h1IconSize = new(44, 44);
        private readonly Padding _h1Padding = new(0, 5, 0, 5);
        private readonly Size _h2IconSize = new(30, 30);
        private readonly Padding _generalPadding = new(0, 2, 0, 2);

        private readonly AggregatorContext _allData;
        private readonly int _cid;
        private readonly SystemSummary _systemSummary;
        private readonly BodySummary _bodySummary;
        private readonly NotificationData _notificationData;
        private readonly string _notifSuppressedTitle = "";
        private readonly string _groupTitle = "";
        private VisitedState _visitedState = VisitedState.None;
        private DataGridViewRow _myRow;


        internal AggregatorGrid()
        {
            RowStyle = AggregatorRowStyle.Default;
            RowContentType = AggregatorRowContentType.Blank;
        }

        internal AggregatorGrid(AggregatorContext data)
        {
            _allData = data;
            _cid = CoalescingIDs.SYSTEM_COALESCING_ID;
            _systemSummary = data.CurrentSystem;
            RowStyle = AggregatorRowStyle.H1;
            RowContentType = AggregatorRowContentType.SystemSummary;
        }

        internal AggregatorGrid(AggregatorContext data, BodySummary bodySummary)
        {
            _allData = data;
            _cid = bodySummary.BodyID;
            _bodySummary = bodySummary;
            RowStyle = AggregatorRowStyle.H2;
            RowContentType = AggregatorRowContentType.BodySummary;
        }

        internal AggregatorGrid(AggregatorContext data, NotificationData notification, string suppressedTitle = "") // Use the value from bodySummary.GetBodyDisplayName()
        {
            _allData = data;
            _cid = notification.CoalescingID;
            _notificationData = notification;
            _notifSuppressedTitle = suppressedTitle;
            RowStyle = AggregatorRowStyle.Default;
            RowContentType = AggregatorRowContentType.Notification;
            _visitedState = notification.VisitedState;
        }

        internal AggregatorGrid(AggregatorContext data, string coalescingGroupTitle, int coalescingId = CoalescingIDs.DEFAULT_COALESCING_ID)
        {
            _allData = data;
            _cid = coalescingId;
            RowStyle = AggregatorRowStyle.H2;
            RowContentType = AggregatorRowContentType.GenericHeader;

            _groupTitle = coalescingGroupTitle;
        }

        internal int CoalescingId { get => _cid; }
        internal AggregatorRowStyle RowStyle { get; init; }
        internal AggregatorRowContentType RowContentType { get; init; }
        public VisitedState State
        {
            get => _visitedState;
            set
            {
                bool changed = _visitedState != value;
                _visitedState = value;
                if (_notificationData != null && _notificationData.VisitedState != value)
                {
                    // Note that setting this may call back in a re-entrant way. Hence there's a bunch of checks for
                    // if the value has actually changed so as to avoid infinite recursion and/or unnecessary UI updates.
                    _notificationData.VisitedState = value;
                }
                if (!changed) return;

                _allData.Core.ExecuteOnUIThread(() =>
                {
                    if (_myRow.Cells["colTracking"] is not DGVImageLabelCell cell) return;
                    if (cell.Value is not ImageLabelSpec til) return;

                    string asString = value.ToString();
                    foreach (var img in til.ImageSpecs)
                    {
                        img.Visible = (img.Tag == asString);
                    }
                    _myRow.DataGridView?.InvalidateCell(cell);
                });
            }
        }

        public int ToRow(DataGridView dgv)
        {
            _myRow = new DataGridViewRow() { Tag = this };

            switch (RowContentType)
            {
                case AggregatorRowContentType.SystemSummary:
                    ToSystemSummaryRow(dgv);
                    break;
                case AggregatorRowContentType.BodySummary:
                    ToBodySummaryRow(dgv);
                    break;
                case AggregatorRowContentType.GenericHeader:
                    ToGenericHeaderRow(dgv);
                    break;
                case AggregatorRowContentType.Notification:
                    ToNotificationRow(dgv);
                    break;
                default: // blank
                    ToBlankRow(dgv);
                    break;
            }

            return dgv.Rows.Add(_myRow);
        }

        private void ToSystemSummaryRow(DataGridView dgv)
        {
            // H1 row (ie. System summary)
            _myRow.DefaultCellStyle.BackColor = OffsetFromColor(dgv.BackgroundColor, 50);

            List<ImageSpec> systemFlagIcons =
            [
                new(Images.NewDiscoveryImage) { ToolTip = "System is undiscovered", Size = _h1IconSize, Visible = _systemSummary?.IsUndiscovered ?? false, },
                new(Images.DoneAllImage) { ToolTip = "System is fully scanned", Size = _h1IconSize, Visible = (_systemSummary?.AllBodiesFound is not null), },
                new(Images.FuelImage) { ToolTip = "A scoopable fuel star is present", Size = _h1IconSize, Visible = _systemSummary?.HasScoopableStar(_allData) ?? false, }
            ];
            _myRow.Cells.Add(NewImageLabelCell(_allData.CurrentSystem?.Name));
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(NewImageLabelCell(_allData.CurrentSystem?.GetDetailString()));
            _myRow.Cells.Add(NewImageLabelCell(_allData.GetCommanderAndShipString()));
            _myRow.Cells.Add(NewImageLabelCell(systemFlagIcons));
            _myRow.Cells.Add(new DataGridViewTextBoxCell()); // No button.
        }

        private void ToBodySummaryRow(DataGridView dgv)
        {
            // Body H2 group header
            _myRow.DefaultCellStyle.BackColor = OffsetFromColor(dgv.BackgroundColor, 30);
            _myRow.DefaultCellStyle.Padding = _h1Padding;

            List<ImageSpec> bodyTitleImages =
            [
                new(Images.SubItemIndentImage) { Size = _h2IconSize, Visible = true, },
            ];
            var bodyType = _bodySummary.GetBodyType();
            var detailTooltip = string.Empty;
            if (_bodySummary.IsRing)
                detailTooltip = _bodySummary.RingHotspotDetails();

            List<ImageSpec> bodyTypeImages = GetImageForBodyType(bodyType, _bodySummary.GetBodyTypeDetail());
            List<ImageSpec> bodyFlagImages = GetImagesForBodyFlags(_bodySummary);

            _myRow.Cells.Add(NewImageLabelCell(_bodySummary.GetBodyNameDisplayString(), bodyTitleImages));
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(NewImageLabelCell(_bodySummary.GetBodyTypeLabel(), bodyTypeImages));
            _myRow.Cells.Add(NewImageLabelCell(_bodySummary.GetDetailsString(), detailTooltip));
            _myRow.Cells.Add(NewImageLabelCell(bodyFlagImages));
            _myRow.Cells.Add(new DataGridViewTextBoxCell()); // no button
        }

        private List<ImageSpec> GetImagesForBodyFlags(BodySummary bodySummary)
        {
            List<ImageSpec> images = [];

            if (bodySummary.Scan?.StarType != null)
            {
                if (bodySummary.IsScoopableStar)
                    images.Add(new(Images.FuelImage)
                    {
                        ToolTip = "This is a scoopable fuel star.",
                        Size = _h2IconSize,
                        Visible = bodySummary.IsScoopableStar,
                    });
            }
            else if (bodySummary.Scan?.PlanetClass != null)
            {
                if (bodySummary.IsValuable)
                    images.Add(new(Images.BodyValuableImage)
                    {
                        ToolTip = "Body has high mapping value",
                        Size = _h2IconSize,
                        Visible = bodySummary.IsValuable,
                    });
                if (bodySummary.IsMapped)
                    images.Add(new(Images.BodyMappedImage)
                    {
                        ToolTip = "Body has been mapped by you",
                        Size = _h2IconSize,
                        Visible = bodySummary.IsMapped,
                    });
                bool isLandable = (bodySummary.Scan?.Landable ?? false);
                if (isLandable)
                    images.Add(new(Images.BodyLandableImage)
                    {
                        ToolTip = "Body is landable",
                        Size = _h2IconSize,
                        Visible = isLandable,
                    });

                if (bodySummary.BodySignals != null)
                {
                    foreach (var signal in bodySummary.BodySignals.Signals)
                    {
                        switch (signal.Type)
                        {
                            case "$SAA_SignalType_Biological;":
                                if (signal.Count > 0)
                                    images.Add(new(Images.BodyBioSignalsImage)
                                    {
                                        Size = _h2IconSize,
                                        ToolTip = $"Body has {signal.Count} biological signals",
                                        Visible = signal.Count > 0
                                    });
                                break;
                            case "$SAA_SignalType_Geological;":
                                if (signal.Count > 0)
                                    images.Add(new(Images.BodyGeoSignalsImage)
                                    {
                                        Size = new(32, 32),
                                        ToolTip = $"Body has {signal.Count} geological signals",
                                        Visible = signal.Count > 0,
                                    });
                                break;
                        }
                    }
                }
            }
            else if (bodySummary.IsRing)
            {
                if (bodySummary.IsMapped)
                    images.Add(new(Images.BodyMappedImage)
                    {
                        ToolTip = "Ring has been mapped by you",
                        Size = _h2IconSize,
                        Visible = bodySummary.IsMapped,
                    });
            }
            return images;
        }

        private List<ImageSpec> GetImageForBodyType(GeneralizedBodyType bodyType, string toolTip = "")
        {
            Bitmap img = null;
            switch (bodyType)
            {
                case GeneralizedBodyType.Barycentre:
                    img = Images.BodyTypeBarycenterImage;
                    break;
                case GeneralizedBodyType.Star:
                    img = Images.BodyTypeStarImage;
                    break;
                case GeneralizedBodyType.Giant:
                    img = Images.BodyTypeGGImage;
                    break;
                case GeneralizedBodyType.Earthlike:
                    img = Images.BodyTypeELWImage;
                    break;
                case GeneralizedBodyType.Water:
                    img = Images.BodyTypeWWImage;
                    break;
                case GeneralizedBodyType.Ring:
                    img = Images.RingsImage;
                    break;
                case GeneralizedBodyType.Other:
                    img = Images.BodyTypeMiscImage;
                    break;
                default:
                    break;
            }

            List<ImageSpec> images = [];
            if (img is null) return images;

            images.Add(new ImageSpec(img)
            {
                Size = _h2IconSize,
                Visible = true,
                ToolTip = toolTip,
            });
            return images;
        }

        private void ToGenericHeaderRow(DataGridView dgv)
        {
            // Generic H2 group header, Title only.
            _myRow.DefaultCellStyle.BackColor = OffsetFromColor(dgv.BackgroundColor, 20);
            _myRow.DefaultCellStyle.Padding = _h1Padding;

            _myRow.Cells.Add(NewImageLabelCell(_groupTitle));
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(new DataGridViewTextBoxCell()); // no button
        }

        private void ToNotificationRow(DataGridView dgv)
        {
            // Basic row
            _myRow.DefaultCellStyle.BackColor = dgv.BackgroundColor;
            _myRow.DefaultCellStyle.Padding = _generalPadding;

            _myRow.Cells.Add(NewImageLabelCell(_notificationData.GetTitleDisplayString(_notifSuppressedTitle)));
            // Only add an interest button for body notifications (based on coalescing ID).
            if (CoalescingIDs.IsBodyCoalescingId(_cid))
                _myRow.Cells.Add(NewImageLabelCell(GetImagesForInterest()));
            else
                _myRow.Cells.Add(NewImageLabelCell());

            _myRow.Cells.Add(NewImageLabelCell(_notificationData.Detail));
            _myRow.Cells.Add(NewImageLabelCell(_notificationData.ExtendedDetails));
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(new DataGridViewButtonCell()
            {
                Value = _notificationData.Sender,
                FlatStyle = FlatStyle.Flat,
            });
        }

        private List<ImageSpec> GetImagesForInterest()
        {
            List<ImageSpec> images =
            [
                new(Images.InspectImage)
                {
                    Size = _h2IconSize,
                    ToolTip = "Mark as interesting for a closer look",
                    Visible = _visitedState == VisitedState.MarkForVisit,
                    Tag = VisitedState.MarkForVisit.ToString(),
                },
                new(Images.InspectIncompleteImage)
                {
                    Size = _h2IconSize,
                    ToolTip = "Not yet visited",
                    Visible = _visitedState == VisitedState.Unvisited,
                    Tag = VisitedState.Unvisited.ToString(),
                },
                new(Images.InspectCompleteImage)
                {
                    Size = _h2IconSize,
                    ToolTip = "Completed",
                    Visible = _visitedState == VisitedState.Visited,
                    Tag = VisitedState.Visited.ToString(),
                }
            ];

            return images;
        }

        private void ToBlankRow(DataGridView dgv)
        {
            _myRow.DefaultCellStyle.BackColor = dgv.BackgroundColor;
            _myRow.DefaultCellStyle.Padding = _generalPadding;

            // Blank row. Set everything to an empty value.
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(NewImageLabelCell());
            _myRow.Cells.Add(new DataGridViewTextBoxCell()); // no button

        }

        private DGVImageLabelCell NewImageLabelCell(string text = "", string tooltip = "")
        {
            return ApplyFontOverrides(new DGVImageLabelCell()
            {
                Value = ImageLabelSpec.New(text),
                ToolTipText = tooltip,
            });
        }

        private DGVImageLabelCell NewImageLabelCell(string text, List<ImageSpec> images, LabelPositionType labelPos = LabelPositionType.Right)
        {
            ImageLabelSpec il = ImageLabelSpec.New(text, images);
            il.LabelPosition = labelPos;

            var cell = new DGVImageLabelCell()
            {
                ToolTipText = string.Join(Environment.NewLine, [.. images.Where(f => f.Visible).Select(f => f.ToolTip)]),
                Value = il,
            };
            return ApplyFontOverrides(cell);
        }

        private DGVImageLabelCell NewImageLabelCell(List<ImageSpec> images)
        {
            return NewImageLabelCell("", images);
        }

        private DGVImageLabelCell ApplyFontOverrides(DGVImageLabelCell cell)
        {
            switch (RowStyle)
            {
                case AggregatorRowStyle.H1:
                    cell.FontSizeAdjustment = 4;
                    cell.FontStyle = FontStyle.Bold;
                    break;
                case AggregatorRowStyle.H2:
                    cell.FontSizeAdjustment = 2;
                    cell.FontStyle = FontStyle.Bold;
                    break;
            }
            return cell;
        }

        private static Color OffsetFromColor(Color color, int offset)
        {
            if (offset == 0) return color;

            var r = color.R + (color.R > 127 ? -1 * offset : offset);
            var g = color.G + (color.G > 127 ? -1 * offset : offset);
            var b = color.B + (color.B > 127 ? -1 * offset : offset);

            return Color.FromArgb(r, g, b);
        }
    }
}
