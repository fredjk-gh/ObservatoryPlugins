namespace com.github.fredjk_gh.ObservatoryArchivist.UI
{
    internal class ArchivistPanel : Panel
    {
        private readonly ArchivistContext _context;
        private readonly TableLayoutPanel _tablePanel;
        private readonly ToolTip _ttip = new();

        public ArchivistPanel(ArchivistContext context)
        {
            _context = context;

            AutoScroll = true;
            DoubleBuffered = true;
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            Controls.Add(_tablePanel = new TableLayoutPanel());
            _tablePanel.Dock = DockStyle.Fill;
            _tablePanel.ColumnStyles.Clear();
            _tablePanel.ColumnStyles.Add(new()
            {
                SizeType = SizeType.Percent,
                Width = 100,
            });

            _tablePanel.RowStyles.Clear();
            _tablePanel.RowStyles.Add(new()
            {
                SizeType = SizeType.Percent,
                Height = 100,
            });

            // First row: Main UI.
            _tablePanel.Controls.Add(_context.UI = new ArchivistUI(context));
            _context.UI.Dock = DockStyle.Fill;
        }
    }
}
