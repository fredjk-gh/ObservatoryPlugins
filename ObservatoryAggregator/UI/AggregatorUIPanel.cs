using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator.UI
{
    internal class AggregatorUIPanel : Panel
    {
        private const string TAG_INSPECT = "ColInspect";
        private const string TAG_SENDER = "ColSender";
        private TrackedData _data;
        private List<AggregatorGrid> _readAllGridItems = new();
        private List<AggregatorGrid> gridItems = new();

        private DataGridView _dgvGrid;

        public AggregatorUIPanel(TrackedData data)
        {
            _data = data;

            #region Initialize Component code.

            _dgvGrid = new DataGridView()
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                Dock = DockStyle.Fill,
                EnableHeadersVisualStyles = false,
                Location = new Point(0, 0),
                RowHeadersWidth = 25,

            };
            _dgvGrid.SuspendLayout();
            SuspendLayout();

            _dgvGrid.RowTemplate.Height = 25;
            _dgvGrid.ColumnHeadersDefaultCellStyle.BackColor = _dgvGrid.BackgroundColor;
            _dgvGrid.ColumnHeadersDefaultCellStyle.ForeColor = _dgvGrid.ForeColor;
            _dgvGrid.RowHeadersDefaultCellStyle.BackColor = _dgvGrid.BackgroundColor;
            _dgvGrid.RowHeadersDefaultCellStyle.ForeColor = _dgvGrid.ForeColor;


            DoubleBuffered = true;
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // TODO: Set up columns.

            _dgvGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Title",
                MinimumWidth = 100,
                Name = "colTitle",
                ReadOnly = true,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Width = 200,
            });
            _dgvGrid.Columns.Add(new DataGridViewButtonColumn()
            {
                Tag = TAG_INSPECT,
                FlatStyle = FlatStyle.Flat,
                HeaderText = "✅",
                MinimumWidth = 30,
                Name = "colTracking",
                ReadOnly = true,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ToolTipText = "Interest and completion tracking",
                Width = 40,
            });
            _dgvGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Details",
                MinimumWidth = 200,
                Name = "colDetails",
                ReadOnly = true,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Width = 250,
            });
            _dgvGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Extended Details",
                MinimumWidth = 200,
                Name = "colExtDetails",
                ReadOnly = true,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Width = 500,
            });
            _dgvGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Flags",
                MinimumWidth = 40,
                Name = "colFlags",
                ReadOnly = true,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ToolTipText = "Icons indicating other states (hover an icon for more help)",
                Width = 160,

            });
            _dgvGrid.Columns.Add(new DataGridViewButtonColumn()
            {
                Tag = TAG_SENDER,
                HeaderText = "Sender",
                MinimumWidth = 100,
                Name = "colSender",
                ReadOnly = true,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ToolTipText = "Click the name to navigate to the sending plugin",
                Width = 150,
            });

            _dgvGrid.CellClick += Grid_CellClick;
            _dgvGrid.CellPainting += Grid_CellPainting;
            _dgvGrid.BackgroundColorChanged += Grid_BackgroundColorChanged;
            _dgvGrid.ForeColorChanged += Grid_ForeColorChanged;

            Controls.Add(_dgvGrid);

            _dgvGrid.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
            #endregion
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

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // TODO: Reflect this in the underlying data structure and have the appropriate AggregatorGrid 
            DataGridView? dataGridView = sender as DataGridView;
            if (dataGridView == null) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // Clicked a header; Ignore

            string colTag = (dataGridView.Columns[e.ColumnIndex].Tag ?? "").ToString();
            if (colTag == TAG_INSPECT) // Inspect column
            {
                // Inspect Column.
                var inspectCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                VisitedState currentState = VisitedStates.FromString((string)inspectCell.Value);
                var newState = currentState.NextState().ToEmojiSpec();
                inspectCell.Value = newState.Text;
                inspectCell.ToolTipText = newState.Description;
            }
            else if (colTag == TAG_SENDER)
            {
                // Sender column
                var senderCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                _data.Core.FocusPlugin((string)senderCell.Value);
            }
        }

        const int BORDER_PEN_WIDTH = 4;
        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv == null) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // Header cells or last row.

            var row = dgv.Rows[e.RowIndex];
            string rowTag = (row.Tag ?? "").ToString();
            if (rowTag.StartsWith(Constants.TAG_HEADER_2_PREFIX))
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
