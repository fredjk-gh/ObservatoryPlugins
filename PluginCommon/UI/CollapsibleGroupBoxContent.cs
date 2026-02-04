using System.ComponentModel;
using System.Runtime.CompilerServices;
using static com.github.fredjk_gh.PluginCommon.UI.CollapsibleGroupBox;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public class CollapsibleGroupBoxContent : UserControl, INotifyPropertyChanged
    {
        public CollapsibleGroupBoxContent() : base()
        {
            DoubleBuffered = true;
        }

        protected List<Button> _toolboxButtons = [];
        protected string _title = null;
        protected bool _isExpanded = true;

        #region Used By CollapsibleGroupBoxControl
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Button> ToolboxButtons
        {
            get => _toolboxButtons;
        }

        // If set, overrides whatever the collapsible box container initializes.
        // -- when the content control is added and when the property changes.
        [Category("Data")]
        public string ContentTitle
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Tells the container to change the box state.
        /// </summary>
        [Category("Appearance")]
        public bool Expanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public virtual void Draw() { }

        public virtual void Clear() { }

        internal void BoxStateChangedHandler(object sender, BoxStateEventArgs e)
        {
            OnBoxStateChanged(sender, e);
        }
        #endregion

        #region Used by inheriting objects
        protected void AddToolButton(Button tool)
        {
            _toolboxButtons.Add(tool);
        }

        protected virtual void OnBoxStateChanged(object sender, BoxStateEventArgs e)
        {
            // Do nothing; Interested content containers should override and do whatever.
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
