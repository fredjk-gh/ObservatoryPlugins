using System.ComponentModel;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public class ScalableLabel : Label
    {
        private float _initialFontSize;
        private float _adjustment;

        public ScalableLabel()
        {
            _initialFontSize = Font.Size;

            ParentChanged += ScalableLabel_ParentChanged;
        }

        private void ScalableLabel_ParentChanged(object sender, EventArgs e)
        {
            if (Parent == null) return;
            _initialFontSize = Parent.Font.Size;
            Parent.FontChanged += Parent_FontChanged;
        }

        private void Parent_FontChanged(object sender, EventArgs e)
        {
            if (sender is not Control parentCtrl) return;

            _initialFontSize = parentCtrl.Font.Size;
            this.Font = new Font(parentCtrl.Font.FontFamily, parentCtrl.Font.Size + _adjustment);
        }

        [Category("Appearance")]
        public float FontSizeAdjustment
        {
            get => _adjustment;
            set
            {
                _adjustment = value;
                Font = new Font(this.Font.FontFamily, _initialFontSize + _adjustment);
            }
        }
    }
}
