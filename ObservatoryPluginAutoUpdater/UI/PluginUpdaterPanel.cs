using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI
{
    internal class PluginUpdaterPanel : Panel
    {
        private UIContext _context;
        public PluginUpdaterPanel(UIContext context)
        {
            _context = context;

            DoubleBuffered = true;
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            this.Controls.Add(_context.UI);

            _context.UI.Dock = DockStyle.Fill;
        }
    }
}
