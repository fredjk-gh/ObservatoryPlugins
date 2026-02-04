using System.ComponentModel;
using System.ComponentModel.Design;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.UI;
using static com.github.fredjk_gh.ObservatoryHelm.UI.UIStateManager;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    internal partial class CommanderContent : HelmContentBase
    {
        private UIStateData _state;

        internal CommanderContent(HelmContext context) : base(context)
        {
            InitializeComponent();
            ContentTitle = "Commander";

            tbtnRealtimeLock.ImageSize = new(26, 26);
            ttManager.SetToolTip(tbtnRealtimeLock, "The Helm UI is in detached \"browse\" mode as a result of changing a selection in a droplist. Click to lock back to realtime.");

            _state = _c.UIMgr.ForMode();
            tbtnRealtimeLock.Click += TbtnRealtimeLock_Click;
            _c.UIMgr.PropertyChanged += UIState_PropertyChanged;
            cbCommander.SelectedIndexChanged += CbCommander_SelectedIndexChanged;
            DrawMode();
            _suppressEvents = false;
        }

        private void TbtnRealtimeLock_Click(object sender, EventArgs e)
        {
            // Force Realtime
            _c.UIMgr.Mode = UIMode.Realtime;
        }

        protected override void InternalDraw()
        {
            _suppressEvents = true;

            cbCommander.DisplayMember = "Name";
            cbCommander.ValueMember = "Key";
            cbCommander.DataSource = _c.Data.Commanders.Values.ToList();

            var cKey = _c.UIMgr.ForMode().CommanderKey;
            if (cKey is not null
                && _c.Data.IsCommanderKnown(cKey))
            {
                var cdata = _c.Data.For(cKey);

                cbCommander.SelectedItem = cdata;
                UpdateLastActiveTime();
            }
            DrawMode();
            _suppressEvents = false;
        }

        protected override void InternalClear()
        {
            ContentTitle = "Commander";
            cbCommander.DataSource = null;
            cbCommander.Items.Clear();
            lblLastActive.Text = string.Empty;
        }

        private void DrawMode()
        {
            switch (_c.UIMgr.Mode)
            {
                case UIMode.Realtime:
                    tbtnRealtimeLock.SetOriginalImage(Images.LockImage);
                    tbtnRealtimeLock.Enabled = false;
                    ttManager.SetToolTip(lblUIMode, "The Helm UI is locked to the current game state.");
                    break;
                case UIMode.Detached:
                    tbtnRealtimeLock.SetOriginalImage(Images.UnlockImage);
                    tbtnRealtimeLock.Enabled = true;
                    ttManager.SetToolTip(lblUIMode, "The Helm UI is in detached \"browse\" mode as a result of changing a selection in a droplist. Click to lock back to realtime.");
                    break;
            }
            lblUIMode.Text = $"Mode: {_c.UIMgr.Mode}";
        }

        internal void SetTitle()
        {
            if (!_isExpanded) ContentTitle = $"Commander - {cbCommander.Text}";
            else ContentTitle = "Commander";
        }

        private void UpdateLastActiveTime()
        {
            lblLastActive.Text = _c.Data.For(_state.CommanderKey).LastActive.ToString();
            _isDirty = true;
        }

        private void CbCommander_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents || cbCommander.SelectedItem is null) return;

            var data = ((CommanderData)cbCommander.SelectedItem);
            _suppressEvents = true; // Prevent infinite recursion.

            if (data.Key.Equals(_c.UIMgr.Realtime.CommanderKey))
            {
                // Switch back to realtime mode.
                _c.UIMgr.Mode = UIMode.Realtime;
            }
            else
            {
                // Either the commander switch or the mode switch triggers re-draw via property changed event.
                _c.UIMgr.Detatched.SwitchCommander(data.Key);
                _c.UIMgr.Mode = UIMode.Detached;
            }
            _suppressEvents = false;
            _c.AddMessage($"Switched to commander: {data.Name}");
        }

        private void UIState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case UIStateManager.PROP_MODE:
                    _state = _c.UIMgr.ForMode();
                    if (_state.CommanderKey == null)
                    {
                        InternalClear();
                        return;
                    }
                    InternalDraw();
                    break;
                case UIStateManager.PROP_COMMANDERKEY:
                    if (_state.CommanderKey == null)
                    {
                        InternalClear();
                        return;
                    }
                    InternalDraw();
                    SetTitle();
                    break;
                case UIStateManager.PROP_SYSTEMID64:
                    UpdateLastActiveTime();
                    break;
            }
        }

        protected override void OnBoxStateChanged(object sender, CollapsibleGroupBox.BoxStateEventArgs e)
        {
            _isExpanded = e.Expanded;
            SetTitle();
        }
    }
}
