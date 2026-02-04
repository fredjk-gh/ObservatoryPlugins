namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI
{
    internal class PluginUpdaterPanel : Panel
    {
        private readonly UIContext _uic;
        public PluginUpdaterPanel(UIContext context)
        {
            _uic = context;

            DoubleBuffered = true;
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            AutoScroll = true;

            this.Controls.Add(_uic.UI);

            _uic.UI.Dock = DockStyle.Fill;
        }
    }
}
