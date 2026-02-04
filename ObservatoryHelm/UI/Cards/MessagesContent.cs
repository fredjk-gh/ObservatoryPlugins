using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.Utilities;

namespace com.github.fredjk_gh.ObservatoryHelm.UI.Cards
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    internal partial class MessagesContent : HelmContentBase
    {
        private readonly ThemeableImageButton messagesCopyButton;
        private readonly ThemeableImageButton messagesClearButton;

        internal MessagesContent(HelmContext context) : base(context)
        {
            InitializeComponent();

            ContentTitle = "Messages";

            messagesClearButton = new();
            messagesClearButton.SetOriginalImage(PluginCommon.Images.RouteClearImage);
            ttManager.SetToolTip(messagesClearButton, "Clear messages");

            messagesCopyButton = new();
            messagesCopyButton.SetOriginalImage(PluginCommon.Images.CopyImage);
            ttManager.SetToolTip(messagesCopyButton, "Copy messages to system clipboard");

            AddToolButton(messagesClearButton);
            AddToolButton(messagesCopyButton);

            // Event handlers for toolbuttons.
            messagesCopyButton.Click += CopyMessagesToClipboard;
            messagesClearButton.Click += ClearMessages;

            _c.UIMgr.PropertyChanged += UIStatePropertyChanged;
            _c.Settings.PropertyChanged += Settings_PropertyChanged;
            InternalClear();
            _suppressEvents = false;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case HelmSettings.SETTING_USEINGAMETIME:
                    InternalDraw();
                    break;
            }
        }

        protected override void InternalDraw()
        {
            if (_c.UIMgr.Messages.Count == lvMsgs.Items.Count) return; // Nothing to do.
            if (_c.UIMgr.Messages.Count == 0 && lvMsgs.Items.Count > 0)
            {
                InternalClear();
                return;
            }

            foreach (var m in _c.UIMgr.Messages.Skip(lvMsgs.Items.Count))
            {
                string timestamp = m.Timestamp.ToLocalTime().ToShortTimeString();
                if (_c.Settings.UseInGameTime)
                {
                    timestamp = m.Timestamp.ToUniversalTime().ToShortTimeString();
                }
                ListViewItem lvItem = new(timestamp)
                {
                    Tag = m
                };
                lvItem.SubItems.Add(m.Message);
                lvItem.SubItems.Add(m.Sender);

                lvMsgs.Items.Insert(0, lvItem);
            }
        }

        protected override void InternalClear()
        {
            lvMsgs.Items.Clear();
        }

        private void UIStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case UIStateManager.PROP_MESSAGES:
                    if (!Expanded)
                    {
                        // Don't add anything to the listview. Just tally up how many are new.
                        var howManyNew = "";
                        if (_c.UIMgr.Messages.Count > lvMsgs.Items.Count)
                            howManyNew = $" ({_c.UIMgr.Messages.Count - lvMsgs.Items.Count} new)";

                        ContentTitle = $"Messages{howManyNew}";
                    }
                    else
                        InternalDraw();
                    break;
            }
        }

        private void CopyMessagesToClipboard(object? sender, EventArgs e)
        {
            if (lvMsgs.Items.Count == 0) return;

            StringBuilder sb = new();

            // Copy Selection, otherwise everything.
            if (lvMsgs.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lvMsgs.SelectedItems)
                {
                    if (item.Tag is HelmMessage msgObj)
                        sb.AppendLine(msgObj.ToString());
                }
            }
            else
            {
                foreach (ListViewItem item in lvMsgs.Items)
                {
                    if (item.Tag is HelmMessage msgObj)
                        sb.AppendLine(msgObj.ToString());
                }
            }

            Misc.SetTextToClipboard(sb.ToString());
        }


        private void ClearMessages(object sender, EventArgs e)
        {
            _c.UIMgr.Messages.Clear();
        }

        protected override void OnBoxStateChanged(object sender, CollapsibleGroupBox.BoxStateEventArgs e)
        {
            ContentTitle = "Messages";
            _isExpanded = e.Expanded;
            if (_isExpanded)
            {
                InternalDraw();
            }
        }
    }
}
