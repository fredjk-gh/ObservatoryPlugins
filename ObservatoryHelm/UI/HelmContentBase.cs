using com.github.fredjk_gh.PluginCommon.UI;

namespace com.github.fredjk_gh.ObservatoryHelm.UI
{
    internal class HelmContentBase : CollapsibleGroupBoxContent
    {
        protected HelmContext _c;
        protected bool _suppressEvents = true;
        protected bool _isDirty = true;

        [Obsolete("Designer only", true)]
        private HelmContentBase()
        {
            // Constructor has to be present to prevent breaking Designer mode, but this is not valid for general
            // use. Throw if not in designer mode.
            // https://stackoverflow.com/questions/1166226/detecting-design-mode-from-a-controls-constructor
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName != "devenv"
                && System.Diagnostics.Process.GetCurrentProcess().ProcessName != "DesignToolsServer")
                throw new InvalidOperationException($"Default Constructor is only for Designer mode; {System.Diagnostics.Process.GetCurrentProcess().ProcessName}");
        }

        public HelmContentBase(HelmContext context)
        {
            _c = context;
            DoubleBuffered = true;
        }

        public override void Clear()
        {
            if (!_isDirty) return;
            _isDirty = false; // Allow clearing once during read-all.
            _c.Core.ExecuteOnUIThread(() =>
            {
                InternalClear();
            });
        }

        public override void Draw()
        {
            if (_c.Core.IsLogMonitorBatchReading || _c.UIMgr.ReplayMode) return;
            _isDirty = true;
            _c.Core.ExecuteOnUIThread(() =>
            {
                InternalDraw();
            });
        }

        protected virtual void InternalDraw() { }
        protected virtual void InternalClear() { }
    }
}
