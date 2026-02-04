using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.UI.Shared;
using System.Diagnostics;
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

        private readonly Dictionary<string, (bool IsSizeable, int Width, int MinWidth, int FillWeight, DataGridViewAutoSizeColumnMode AutoSizeMode)> _colSizingInfo = new()
        {
            { COL_TITLE, ( true, 300, 100, 200, DataGridViewAutoSizeColumnMode.NotSet ) },
            { COL_TRACKING, ( true, 40, 30, 0, DataGridViewAutoSizeColumnMode.None ) },
            { COL_DETAILS, ( true, 350, 300, 200, DataGridViewAutoSizeColumnMode.Fill ) },
            { COL_EXT_DETAILS, ( true, 750, 200, 400, DataGridViewAutoSizeColumnMode.Fill ) },
            { COL_FLAGS, ( true, 160, 40, 0, DataGridViewAutoSizeColumnMode.NotSet ) },
            { COL_SENDER, ( true, 150, 100, 100, DataGridViewAutoSizeColumnMode.NotSet ) },
        };

        private bool _isLoadingOrApplyingSettings = true;
        private readonly AggregatorContext _data;
        private readonly List<AggregatorGrid> gridItems = [];

        #region Controls
        private readonly ToolStrip _tools;
        private readonly ToolStripButton _tsbInfo;
        private readonly ToolStripButton _tsbVisbility;
        private readonly ToolStripButton _tsbTextSmaller;
        private readonly ToolStripButton _tsbTextLarger;
        private readonly ToolStripButton _tsbSettings;
        private readonly DataGridView _dgvGrid;

        #endregion

        public AggregatorUIPanel(AggregatorContext data)
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
                RowHeadersWidth = 10,
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
            var colTitle = new DGVImageLabelColumn()
            {
                HeaderText = "Title",
                Name = COL_TITLE,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };
            colTitle.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            var colInspect = new DGVImageLabelColumn()
            {
                Tag = Constants.TAG_INSPECT,
                HeaderText = "Interest",
                Name = COL_TRACKING,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ToolTipText = "Interest and completion tracking",
            };

            var colDetails = new DGVImageLabelColumn()
            {
                HeaderText = "Details",
                Name = COL_DETAILS,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };
            colDetails.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            var colExtDetails = new DGVImageLabelColumn()
            {
                HeaderText = "Extended Details",
                Name = COL_EXT_DETAILS,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };
            colExtDetails.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            var colFlags = new DGVImageLabelColumn()
            {
                HeaderText = "Flags",
                Name = COL_FLAGS,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ToolTipText = "Icons indicating other states (hover an icon for more help)",
            };

            var colSender = new DataGridViewButtonColumn()
            {
                Tag = Constants.TAG_SENDER,
                HeaderText = "Sender",
                Name = COL_SENDER,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ToolTipText = "Click the name to navigate to the sending plugin",
            };

            _dgvGrid.Columns.Add(colTitle);
            _dgvGrid.Columns.Add(colInspect);
            _dgvGrid.Columns.Add(colDetails);
            _dgvGrid.Columns.Add(colExtDetails);
            _dgvGrid.Columns.Add(colFlags);
            _dgvGrid.Columns.Add(colSender);

            ApplyResizeMode(true);
            ApplyColumnOrder();

            _dgvGrid.CellMouseClick += Grid_CellMouseClick;
            _dgvGrid.CellPainting += Grid_CellPainting;
            _dgvGrid.BackgroundColorChanged += Grid_BackgroundColorChanged;
            _dgvGrid.ForeColorChanged += Grid_ForeColorChanged;
            _dgvGrid.ColumnWidthChanged += Grid_ColumnWidthChanged;
            _dgvGrid.ColumnDisplayIndexChanged += Grid_ColumnDisplayIndexChanged;

            Controls.Add(_dgvGrid);

            _tools = new()
            {
                Name = "_tools",
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Dock = DockStyle.Top
            };

            _tsbInfo = new()
            {
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                Image = Images.InfoBubbleImage,
                ToolTipText = "About Aggregator",
                Name = "tsbInfo",
            };
            _tsbInfo.Click += TsbInfo_Click;
            _tools.Items.Add(_tsbInfo);
            _tsbTextSmaller = new()
            {
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                Image = Images.TextSizeSmallerImage,
                ToolTipText = "Decrease text size.",
                Name = "tsbTextSmaller",
            };
            _tsbTextSmaller.Click += TsbTextSizeSmaller_Click;
            _tools.Items.Add(_tsbTextSmaller);
            _tsbTextLarger = new()
            {
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                Image = Images.TextSizeLargerImage,
                ToolTipText = "Increase text size.",
                Name = "tsbTextLarger",
            };
            _tsbTextLarger.Click += TsbTextSizeLarger_Click;
            _tools.Items.Add(_tsbTextLarger);

            var visibilityImage = Images.VisibilityShowImage;
            if (_data.Settings.ShowAllBodySummaries)
                visibilityImage = Images.VisibilityOffImage;
            _tsbVisbility = new()
            {
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                Image = visibilityImage,
                ToolTipText = "Toggle visibility of body summaries with no notifications.",
                Name = "tsbVisbility",
            };
            _tsbVisbility.Click += TsbVisbility_Click;
            _tools.Items.Add(_tsbVisbility);
            _tsbSettings = new()
            {
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                Image = Images.SettingsCogImage,
                ToolTipText = "Open settings...",
                Name = "tsbSettings",
            };
            _tsbSettings.Click += TsbSettings_Click;
            _tools.Items.Add(_tsbSettings);

            Controls.Add(_tools);

            ForeColorChanged += ApplyThemeForeColor;

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
                        col.Width = (sizingMode == GridSizingMode.Manual && colSizes.TryGetValue(col.Name, out int size)
                            ? size
                            : _colSizingInfo[col.Name].Width);
                    col.MinimumWidth = Math.Max(2, _colSizingInfo[col.Name].MinWidth);
                    col.FillWeight = Math.Max(1, _colSizingInfo[col.Name].FillWeight);
                }
                else
                {
                    col.Resizable = DataGridViewTriState.False;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    col.Width = colSizes.TryGetValue(col.Name, out int size) ? size : _colSizingInfo[col.Name].Width;
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
                if (_data.Settings.ColumnOrder.TryGetValue(c.Name, out int index))
                    c.DisplayIndex = index;
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

        private void TsbSettings_Click(object sender, EventArgs e)
        {
            _data.Core.OpenSettings(_data.Worker);
        }

        private void TsbInfo_Click(object sender, EventArgs e)
        {
            _data.Core.OpenAbout(_data.Worker);
        }

        private void TsbTextSizeLarger_Click(object sender, EventArgs e)
        {
            // The property changed event takes care of enacting the change.
            _data.Settings.FontSizeAdjustment++;
        }

        private void TsbTextSizeSmaller_Click(object sender, EventArgs e)
        {
            // The property changed event takes care of enacting the change.
            _data.Settings.FontSizeAdjustment--;
        }

        private void TsbVisbility_Click(object sender, EventArgs e)
        {
            // The property changed event takes care of enacting the change.
            _data.Settings.ShowAllBodySummaries = !_data.Settings.ShowAllBodySummaries;
            UpdateVisibilityImage();
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FontSizeAdjustment":
                    _dgvGrid.Font = new Font(this.Font.FontFamily, this.Font.Size + (float)_data.Settings.FontSizeAdjustment);
                    break;
                case "ShowAllBodySummaries":
                    SetGridItems(_data.ToGrid());
                    UpdateVisibilityImage();
                    break;
                case "GridSizingMode":
                    ApplyResizeMode();
                    // Invalidate instead?
                    //RepaintAll();
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

        private void ApplyThemeForeColor(object sender, EventArgs e)
        {
            // Propagate to the toolbar image buttons.
            _tsbInfo.Image = ImageCommon.RecolorAndSizeImage(Images.InfoBubbleImage, ForeColor);
            _tsbTextSmaller.Image = ImageCommon.RecolorAndSizeImage(Images.TextSizeSmallerImage, ForeColor);
            _tsbTextLarger.Image = ImageCommon.RecolorAndSizeImage(Images.TextSizeLargerImage, ForeColor);
            _tsbSettings.Image = ImageCommon.RecolorAndSizeImage(Images.SettingsCogImage, ForeColor);
            UpdateVisibilityImage();
        }

        private void UpdateVisibilityImage()
        {
            if (!_data.Settings.ShowAllBodySummaries)
                _tsbVisbility.Image = ImageCommon.RecolorAndSizeImage(Images.VisibilityShowImage, ForeColor);
            else
                _tsbVisbility.Image = ImageCommon.RecolorAndSizeImage(Images.VisibilityOffImage, ForeColor);
        }

        public void Clear()
        {
            _data.Core.ExecuteOnUIThread(() =>
            {
                _dgvGrid.Rows.Clear();
            });
        }

        private bool _repaintQueued = false;

        public void SetGridItems(List<AggregatorGrid> items, bool directPaint = false)
        {
            // Naive add everything as a row, stateless.
            // Don't re-assign to avoid changing the object a waiting thread references.
            lock(gridItems)
            {
                gridItems.Clear();
                gridItems.AddRange(items);
            }

            if (directPaint) // Use sparingly -- this impacts performance
            {
                RepaintAll();
                return;
            }
            // Throttle redraws. Option: Throw this in a task for greater reliability?
            if (!_repaintQueued)
            {
                _repaintQueued = true; // Do this immediately to prevent multiple starts.
                try
                {
                    Task.Run(() =>
                    {
                        Task.Delay(300);
                        RepaintAll();
                    });
                }
                catch
                {
                    _repaintQueued = false;
                }
            }
            else
            {
                Debug.WriteLine("Repaint skipped due to queued task.");
            }
        }

        private void RepaintAll()
        {
            _data.Core.ExecuteOnUIThread(() =>
            {
                lock(gridItems)
                {
                    Clear();
                    _dgvGrid.SuspendLayout();
                    foreach (AggregatorGrid item in gridItems)
                    {
                        item.ToRow(_dgvGrid);
                    }
                    _dgvGrid.ResumeLayout();
                }

                _repaintQueued = false;
            });
        }

        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView dataGridView) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // Clicked a header; Ignore

            string colTag = (dataGridView.Columns[e.ColumnIndex].Tag ?? "").ToString();
            var row = dataGridView.Rows[e.RowIndex];
            if (e.Button == MouseButtons.Left)
            {
                if (colTag == Constants.TAG_INSPECT) // Inspect column
                {
                    // Inspect Column. Cycle to the next state.
                    if (row.Tag is AggregatorGrid data && data.State != VisitedState.None)
                    {
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
                    if (row.Tag is AggregatorGrid data && data.State != VisitedState.None)
                    {
                        _data.ResetMark(data.CoalescingId);
                    }
                }
            }
        }

        const int BORDER_PEN_WIDTH = 4;
        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // Header cells or last row.

            var row = dgv.Rows[e.RowIndex];
            AggregatorGrid rowData = row.Tag as AggregatorGrid;
            if (rowData is not null && rowData.RowStyle == AggregatorRowStyle.H2)
            {
                // Body summary row.
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                using Brush brush = new SolidBrush(dgv.ForeColor);
                using Pen pen = new(brush, BORDER_PEN_WIDTH);
                var r = e.CellBounds;
                e.Graphics.DrawLine(pen, r.Left, r.Top + 2, r.Right, r.Top + 2);
                e.Handled = true;
            }
        }
    }
}
