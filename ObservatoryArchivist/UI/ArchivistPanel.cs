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
            _tablePanel.RowStyles.Add(new()
            {
                SizeType = SizeType.Absolute,
                Height = 50,
            });

            // First row: Main UI.
            _tablePanel.Controls.Add(_context.UI = new ArchivistUI(context));
            _context.UI.Dock = DockStyle.Fill;

            // Second row: Settings button.
            var btnSettingsCog = new Button()
            {
                FlatStyle = FlatStyle.Flat,
                Margin = new(5),
                AutoSize = true,
                Text = "Settings",
            };
            btnSettingsCog.FlatAppearance.BorderSize = 0;
            btnSettingsCog.Click += btnSettingsCog_Click;
            _tablePanel.Controls.Add(btnSettingsCog);
        }

        private void btnSettingsCog_Click(object sender, EventArgs e)
        {
            _context.Core.OpenSettings(_context.PluginWorker);
        }
    }
}
