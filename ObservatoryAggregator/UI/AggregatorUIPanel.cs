using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static com.github.fredjk_gh.ObservatoryAggregator.AggregatorSettings;

namespace com.github.fredjk_gh.ObservatoryAggregator.UI
{
    internal class AggregatorUIPanel : Panel
    {
        private const string COL_TITLE = "colTitle";
        private const string COL_TRACKING = "colTracking";
        private const string COL_DETAILS = "colDetails";
        private const string COL_EXT_DETAILS = "colExtDetails";
        private const string COL_FLAGS = "colFlags";
        private const string COL_SENDER = "colSender";

        private Dictionary<string, (bool IsSizeable, int Width, int MinWidth, int FillWeight, DataGridViewAutoSizeColumnMode AutoSizeMode)> _colSizingInfo = new()
        {
            { COL_TITLE, ( true, 300, 100, 200, DataGridViewAutoSizeColumnMode.NotSet ) },
            { COL_TRACKING, ( true, 40, 30, 0, DataGridViewAutoSizeColumnMode.None ) },
            { COL_DETAILS, ( true, 350, 300, 200, DataGridViewAutoSizeColumnMode.Fill ) },
            { COL_EXT_DETAILS, ( true, 750, 200, 400, DataGridViewAutoSizeColumnMode.Fill ) },
            { COL_FLAGS, ( true, 160, 40, 0, DataGridViewAutoSizeColumnMode.NotSet ) },
            { COL_SENDER, ( true, 150, 100, 100, DataGridViewAutoSizeColumnMode.NotSet ) },
        };

        private bool _isLoadingOrApplyingSettings = true;
        private TrackedData _data;
        private List<AggregatorGrid> _readAllGridItems = new();
        private List<AggregatorGrid> gridItems = new();

        private ToolStrip _tools;
        private DataGridView _dgvGrid;

        public AggregatorUIPanel(TrackedData data)
        {
            _data = data;
            _data.Settings.PropertyChanged += Settings_PropertyChanged;

            #region Initialize Component code.
            DoubleBuffered = true;
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            _dgvGrid = new DataGridView()
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = true,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                Dock = DockStyle.Fill,
                EnableHeadersVisualStyles = false,
                Location = new Point(0, 0),
                RowHeadersWidth = 25,
                Font = new Font(this.Font.FontFamily, this.Font.Size + (float)_data.Settings.FontSizeAdjustment)
            };
            _dgvGrid.SuspendLayout();
            SuspendLayout();

            _dgvGrid.RowTemplate.Height = 25;
            _dgvGrid.ColumnHeadersDefaultCellStyle.BackColor = _dgvGrid.BackgroundColor;
            _dgvGrid.ColumnHeadersDefaultCellStyle.ForeColor = _dgvGrid.ForeColor;
            _dgvGrid.RowHeadersDefaultCellStyle.BackColor = _dgvGrid.BackgroundColor;
            _dgvGrid.RowHeadersDefaultCellStyle.ForeColor = _dgvGrid.ForeColor;
            _dgvGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

            // Set up columns.
            int titleColIndex = _dgvGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Title",
                Name = COL_TITLE,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _dgvGrid.Columns[titleColIndex].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            _dgvGrid.Columns.Add(new DataGridViewButtonColumn()
            {
                Tag = Constants.TAG_INSPECT,
                FlatStyle = FlatStyle.Flat,
                HeaderText = "✅",
                Name = COL_TRACKING,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ToolTipText = "Interest and completion tracking",
            });
            int detailsColIndex = _dgvGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Details",
                Name = COL_DETAILS,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _dgvGrid.Columns[detailsColIndex].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            int extDetailsColIndex = _dgvGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Extended Details",
                Name = COL_EXT_DETAILS,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _dgvGrid.Columns[extDetailsColIndex].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            _dgvGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Flags",
                Name = COL_FLAGS,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ToolTipText = "Icons indicating other states (hover an icon for more help)",
            });
            _dgvGrid.Columns.Add(new DataGridViewButtonColumn()
            {
                Tag = Constants.TAG_SENDER,
                HeaderText = "Sender",
                Name = COL_SENDER,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ToolTipText = "Click the name to navigate to the sending plugin",
            });

            ApplyResizeMode(true);
            ApplyColumnOrder();

            _dgvGrid.CellMouseClick += Grid_CellMouseClick;
            _dgvGrid.CellPainting += Grid_CellPainting;
            _dgvGrid.BackgroundColorChanged += Grid_BackgroundColorChanged;
            _dgvGrid.ForeColorChanged += Grid_ForeColorChanged;
            _dgvGrid.ColumnWidthChanged += Grid_ColumnWidthChanged;
            _dgvGrid.ColumnDisplayIndexChanged += Grid_ColumnDisplayIndexChanged;

            Controls.Add(_dgvGrid);

            _tools = new ToolStrip();
            _tools.Name = "_tools";
            _tools.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _tools.Dock = DockStyle.Top;

            var tsbInfo = new ToolStripButton()
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Text = "ℹ️",
                Name = "tsbInfo",
            };
            tsbInfo.Click += tsbInfo_Click;
            _tools.Items.Add(tsbInfo);
            var tsbSettings = new ToolStripButton()
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Text = "⚙️",
                Name = "tsbSettings",
            };
            tsbSettings.Click += tsbSettings_Click;
            _tools.Items.Add(tsbSettings);

            Controls.Add(_tools);

            _dgvGrid.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
            #endregion

            _isLoadingOrApplyingSettings = false;
        }

        private void ApplyResizeMode(bool initializing = false)
        {
            _isLoadingOrApplyingSettings = true;

            GridSizingMode sizingMode = _data.Settings.GridSizingModeEnum;
            var colSizes = _data.Settings.ColumnSizes;

            foreach (DataGridViewColumn col in _dgvGrid.Columns)
            {
                if (_colSizingInfo[col.Name].IsSizeable)
                {
                    col.Resizable = DataGridViewTriState.True;
                    col.AutoSizeMode = sizingMode == GridSizingMode.Manual ? DataGridViewAutoSizeColumnMode.None : _colSizingInfo[col.Name].AutoSizeMode;
                    if (initializing)
                        col.Width = sizingMode == GridSizingMode.Manual && colSizes.ContainsKey(col.Name)
                            ? colSizes[col.Name]
                            : _colSizingInfo[col.Name].Width;
                    col.MinimumWidth = Math.Max(2, _colSizingInfo[col.Name].MinWidth);
                    col.FillWeight = Math.Max(1, _colSizingInfo[col.Name].FillWeight);
                }
                else
                {
                    col.Resizable = DataGridViewTriState.False;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    col.Width = colSizes.ContainsKey(col.Name) ? colSizes[col.Name] : _colSizingInfo[col.Name].Width;
                    col.MinimumWidth = 2; // minimum valid value
                    col.FillWeight = 1; // minimum valid value
                }
            }
            _isLoadingOrApplyingSettings = false;
        }

        private void SaveGridColumnWidths()
        {
            // Only save manual column sizes.
            if (_isLoadingOrApplyingSettings || _data.Settings.GridSizingModeEnum != GridSizingMode.Manual) return;

            foreach (DataGridViewColumn col in _dgvGrid.Columns)
            {
                _data.Settings.ColumnSizes[col.Name] = col.Width;
            }
            _data.Core.SaveSettings(_data.Worker);
        }

        private void ApplyColumnOrder()
        {
            _isLoadingOrApplyingSettings = true;
            foreach (DataGridViewColumn c in _dgvGrid.Columns)
            {
                if (_data.Settings.ColumnOrder.ContainsKey(c.Name))
                {
                    c.DisplayIndex = _data.Settings.ColumnOrder[c.Name];
                }
            }
            _isLoadingOrApplyingSettings = false;
        }

        private void SaveGridColumnOrder()
        {
            // Don't do this if programmatically changing things.
            if (_isLoadingOrApplyingSettings) return;

            // If there's more events to come, punt.
            // Adapted from: https://stackoverflow.com/questions/53849396/detect-column-reordering-columndisplayindexchanged-raises-multiple-times
            var p = typeof(DataGridViewColumn).GetProperty("DisplayIndexHasChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (_dgvGrid.Columns.Cast<DataGridViewColumn>().Any(col => (bool)p.GetValue(col))) return;

            foreach (DataGridViewColumn c in _dgvGrid.Columns)
            {
                if (c.DisplayIndex != c.Index)
                    _data.Settings.ColumnOrder[c.Name] = c.DisplayIndex;
                else if (_data.Settings.ColumnOrder.ContainsKey(c.Name))
                    _data.Settings.ColumnOrder.Remove(c.Name);
            }

            _data.Core.SaveSettings(_data.Worker);
        }

        private void Grid_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            SaveGridColumnOrder();
        }

        private void Grid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            SaveGridColumnWidths();
        }

        private void tsbSettings_Click(object sender, EventArgs e)
        {
            _data.Core.OpenSettings(_data.Worker);
        }

        private void tsbInfo_Click(object sender, EventArgs e)
        {
            _data.Core.OpenAbout(_data.Worker);
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FontSizeAdjustment":
                    _dgvGrid.Font = new Font(this.Font.FontFamily, this.Font.Size + (float)_data.Settings.FontSizeAdjustment);
                    RepaintAll();
                    break;
                case "ShowAllBodySummaries":
                    SetGridItems(_data.ToGrid());
                    RepaintAll();
                    break;
                case "GridSizingMode":
                    ApplyResizeMode();
                    RepaintAll();
                    break;
            }
        }

        private void Grid_BackgroundColorChanged(object sender, EventArgs e)
        {
            _dgvGrid.ColumnHeadersDefaultCellStyle.BackColor = _dgvGrid.BackgroundColor;
            _dgvGrid.RowHeadersDefaultCellStyle.BackColor = _dgvGrid.BackgroundColor;
            // We also need to clear and re-draw the grid as many row styles depend on BG Color.
            RepaintAll();
        }
        private void Grid_ForeColorChanged(object sender, EventArgs e)
        {
            _dgvGrid.ColumnHeadersDefaultCellStyle.ForeColor = _dgvGrid.ForeColor;
            _dgvGrid.RowHeadersDefaultCellStyle.ForeColor = _dgvGrid.ForeColor;
        }

        public void Clear()
        {
            _data.Core.ExecuteOnUIThread(() =>
            {
                _dgvGrid.Rows.Clear();
            });
        }

        public void SetGridItems(List<AggregatorGrid> items)
        {
            // Naive add everything as a row, stateless.
            gridItems = items;
            RepaintAll();
        }

        private void RepaintAll()
        {
            _data.Core.ExecuteOnUIThread(() =>
            {
                Clear();
                foreach (AggregatorGrid item in gridItems)
                {
                    item.ToRow(_dgvGrid);
                }
            });
        }

        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            DataGridView? dataGridView = sender as DataGridView;
            if (dataGridView == null) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // Clicked a header; Ignore

            string colTag = (dataGridView.Columns[e.ColumnIndex].Tag ?? "").ToString();
            var row = dataGridView.Rows[e.RowIndex];
            if (e.Button == MouseButtons.Left)
            {
                if (colTag == Constants.TAG_INSPECT) // Inspect column
                {
                    // Inspect Column. Cycle to the next state.
                    var inspectCell = row.Cells[e.ColumnIndex];

                    AggregatorGrid data = row.Tag as AggregatorGrid;
                    if (data != null && data.State != VisitedState.None)
                    {
                        VisitedState currentState = data.State;
                        switch (data.State)
                        {
                            case VisitedState.MarkForVisit:
                                _data.MarkForVisit(data.CoalescingId);
                                break;
                            case VisitedState.Unvisited:
                                _data.MarkVisited(data.CoalescingId);
                                break;
                            case VisitedState.Visited:
                                _data.ResetMark(data.CoalescingId);
                                break;
                        }
                    }
                }
                else if (colTag == Constants.TAG_SENDER)
                {
                    // Sender column
                    var senderCell = row.Cells[e.ColumnIndex];
                    _data.Core.FocusPlugin((string)senderCell.Value);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (colTag == Constants.TAG_INSPECT) // Inspect column
                {
                    // Inspect Column. Reset to initial state.
                    var inspectCell = row.Cells[e.ColumnIndex];

                    AggregatorGrid data = row.Tag as AggregatorGrid;
                    if (data != null && data.State != VisitedState.None)
                    {
                        _data.ResetMark(data.CoalescingId);
                    }
                }
            }
        }

        const int BORDER_PEN_WIDTH = 4;
        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv == null) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // Header cells or last row.

            var row = dgv.Rows[e.RowIndex];
            AggregatorGrid rowData = row.Tag as AggregatorGrid;
            if (rowData != null && rowData.RowStyle == AggregatorRowStyle.H2)
            {
                // Body summary row.
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                using (Brush brush = new SolidBrush(dgv.ForeColor))
                using (Pen pen = new Pen(brush, BORDER_PEN_WIDTH))
                {
                    var r = e.CellBounds;
                    e.Graphics.DrawLine(pen, r.Left, r.Top + 2, r.Right, r.Top + 2);
                    e.Handled = true;
                }
            }
        }
    }
}
