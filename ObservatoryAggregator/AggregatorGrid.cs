using com.github.fredjk_gh.ObservatoryAggregator.UI;
using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    public class AggregatorGrid
    {
        private TrackedData _allData;
        private int _cid;
        private SystemSummary _systemSummary;
        private BodySummary _bodySummary;
        private NotificationData _notificationData;
        //private DataGridViewRow _myRow;


        internal AggregatorGrid()
        {
            Title = "";
            State = VisitedState.None;
            Detail = "";
            ExtendedDetails = "";
            Flags = new();
            Sender = "";
        }

        internal AggregatorGrid(TrackedData data)
        {
            _allData = data;
            _cid = Constants.SYSTEM_COALESCING_ID;
            _systemSummary = data.CurrentSystem;
            Tag = $"{Constants.TAG_HEADER_1_PREFIX}{Constants.SYSTEM_COALESCING_ID}";
            Title = data.CurrentSystem.Name;
            State = VisitedState.None;
            Detail = data.CurrentSystem.GetDetailString();
            ExtendedDetails = data.GetCommanderAndShipString();
            Flags = data.CurrentSystem.GetFlagEmoji(data);
            Sender = "";
        }

        internal AggregatorGrid(TrackedData data, BodySummary bodySummary)
        {
            _allData = data;
            _cid = bodySummary.BodyID;
            _bodySummary = bodySummary;
            Tag = $"{Constants.TAG_HEADER_2_PREFIX}{bodySummary.BodyID}";
            Title = $"{Constants.BODY_NESTING_INDICATOR}{bodySummary.GetBodyNameDisplayString()}";
            State = VisitedState.None;
            Detail = bodySummary.GetBodyType();
            ExtendedDetails = bodySummary.GetDetailsString();
            Flags = bodySummary.GetFlagEmoji();  // TODO make this return flag specs.
            Sender = "";
        }

        internal AggregatorGrid(TrackedData data, NotificationData notification, string suppressedTitle = "") // Use the value from bodySummary.GetBodyNameDisplayString()
        {
            _allData = data;
            _cid = notification.CoalescingID;
            _notificationData = notification;
            Tag = $"{Constants.TAG_HEADER_3_PREFIX}{notification.CoalescingID}";
            Title = notification.GetTitleDisplayString(suppressedTitle);
            State = VisitedState.MarkForVisit;
            Detail = notification.Detail;
            ExtendedDetails = notification.ExtendedDetails;
            Flags = new();
            Sender = notification.Sender;
        }

        internal AggregatorGrid(TrackedData data, string coalescingGroupTitle, int coalescingId = Constants.DEFAULT_COALESCING_ID)
        {
            _allData = data;
            _cid = coalescingId;
            Tag = $"{Constants.TAG_HEADER_2_PREFIX}{coalescingId}";
            Title = coalescingGroupTitle;
            State = VisitedState.None;
            Detail = "";
            ExtendedDetails = "";
            Flags = new();
            Sender = "";
        }

        internal string Tag { get; init; }

        //internal int RowId { get => _myRow != null ? _myRow.Index : -1; }

        public string Title { get; init; }

        public VisitedState State { get; set; }

        public string Detail { get; init; }

        public string ExtendedDetails { get; init; }

        public List<EmojiSpec> Flags { get; set; }

        public string Sender { get; init; }


        public int ToRow(DataGridView dgv)
        {
            //if (_myRow != null) return _myRow.Index;

            var _myRow = new DataGridViewRow() { Tag = Tag };

            if (_systemSummary != null)
            {
                // H1 row (ie. System summary)
                DataGridViewTextBoxCell systemFlagsCell = new()
                {
                    Value = string.Join(" ", Flags.Select(f => f.Text).ToArray()),
                    ToolTipText = string.Join(Environment.NewLine, Flags.Select(f => f.ToolTipText).ToArray()),
                    Style =
                    {
                        Font = new Font(dgv.Font.FontFamily, dgv.Font.Size + 4),
                    },
                };
                _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Title, });
                _myRow.Cells.Add(new DataGridViewTextBoxCell());
                _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Detail, });
                _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = ExtendedDetails, });
                _myRow.Cells.Add(systemFlagsCell);
                _myRow.Cells.Add(new DataGridViewTextBoxCell());
                _myRow.DefaultCellStyle = new()
                {
                    Font = new Font(dgv.Font.FontFamily, dgv.Font.Size + 4, FontStyle.Bold),
                    BackColor = OffsetFromColor(dgv.BackgroundColor, 50),
                };
            }
            else if (_bodySummary != null)
            {
                // Body H2 group header
                DataGridViewTextBoxCell bodyFlagsCell = new()
                {
                    Value = string.Join(" ", Flags.Select(f => f.Text).ToArray()),
                    ToolTipText = string.Join(Environment.NewLine, Flags.Select(f => f.ToolTipText).ToArray()),
                    Style =
                    {
                        Font = new Font(dgv.Font.FontFamily, dgv.Font.Size + 2),
                    },
                };
                _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Title, });
                _myRow.Cells.Add(new DataGridViewTextBoxCell());
                _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Detail, });
                _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = ExtendedDetails, });
                _myRow.Cells.Add(bodyFlagsCell);
                _myRow.Cells.Add(new DataGridViewTextBoxCell());
                _myRow.DefaultCellStyle = new()
                {
                    Font = new Font(dgv.Font.FontFamily, dgv.Font.Size + 2, FontStyle.Bold),
                    BackColor = OffsetFromColor(dgv.BackgroundColor, 30),
                    Padding = new Padding(0, 4, 0, 1),
                };
            }
            else if (_notificationData != null)
            {
                // Basic row
                var showPluginButton = new DataGridViewButtonCell()
                {
                    Value = Sender,
                    FlatStyle = FlatStyle.Flat,
                };

                _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Title, });
                // Only add an inspect button for body notifications (based on coalescing ID).
                if (_cid > Constants.SYSTEM_COALESCING_ID && _cid < Constants.DEFAULT_COALESCING_ID)
                {
                    EmojiSpec state = State.ToEmojiSpec();
                    var inspectButton = new DataGridViewButtonCell()
                    {
                        Value = state.Text,
                        ToolTipText = state.Description,
                        FlatStyle = FlatStyle.Flat,
                    };
                    _myRow.Cells.Add(inspectButton);
                }
                else
                {
                    _myRow.Cells.Add(new DataGridViewTextBoxCell());
                }
                _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Detail, });
                _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = ExtendedDetails, });
                _myRow.Cells.Add(new DataGridViewTextBoxCell());
                _myRow.Cells.Add(showPluginButton);
                _myRow.DefaultCellStyle = new()
                {
                    BackColor = dgv.BackgroundColor,
                };
            }
            else if (_allData != null && !string.IsNullOrWhiteSpace(Tag))
            {
                // Generic H2 group header, Title only.
                _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Title });
                for (int i = 1; i < dgv.ColumnCount; i++)
                {
                    _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = "" });
                }
                _myRow.DefaultCellStyle = new()
                {
                    Font = new Font(dgv.Font.FontFamily, dgv.Font.Size + 2, FontStyle.Bold),
                    BackColor = OffsetFromColor(dgv.BackgroundColor, 20),
                    Padding = new Padding(0, 4, 0, 1),
                };
            }
            else
            {
                // Blank row. Set everything explicitly to an empty text box cell.
                for (int i = 0; i < dgv.ColumnCount; i++)
                {
                    _myRow.Cells.Add(new DataGridViewTextBoxCell() { Value = "" });
                }
                _myRow.DefaultCellStyle = new()
                {
                    BackColor = dgv.BackgroundColor,
                };
            }

            return dgv.Rows.Add(_myRow);
        }

        private Color OffsetFromColor(Color color, int offset)
        {
            if (offset == 0) return color;

            var r = color.R + (color.R > 127 ? -1 * offset : offset);
            var g = color.G + (color.G > 127 ? -1 * offset : offset);
            var b = color.B + (color.B > 127 ? -1 * offset : offset);

            return Color.FromArgb(r, g, b);
        }
    }
}
