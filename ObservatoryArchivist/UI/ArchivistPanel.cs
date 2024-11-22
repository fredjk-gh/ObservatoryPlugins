using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace com.github.fredjk_gh.ObservatoryArchivist.UI
{
    internal class ArchivistPanel : Panel
    {
        private ArchivistContext _context;
        private TableLayoutPanel _tablePanel;
        private ToolTip _ttip = new ToolTip();

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
