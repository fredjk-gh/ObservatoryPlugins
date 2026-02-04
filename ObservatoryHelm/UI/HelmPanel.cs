namespace com.github.fredjk_gh.ObservatoryHelm.UI
{
    internal class HelmPanel : Panel
    {
        private readonly HelmContext _c;

        public HelmPanel(HelmContext context)
        {
            _c = context;

            AutoScroll = true;
            DoubleBuffered = true;

            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _c.UI = new HelmUI(_c)
            {
                Dock = DockStyle.Fill,
            };
            Controls.Add(_c.UI);
        }
    }
}
