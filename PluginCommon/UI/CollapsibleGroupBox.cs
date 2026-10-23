using com.github.fredjk_gh.PluginCommon.UI.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public partial class CollapsibleGroupBox : UserControl
    {
        private ToolTip _ttManager;
        private CollapsibleGroupBoxContent _contentControl;
        private Color _borderColor = SystemColors.ActiveBorder;
        private bool _expanded = false;
        private int _contentHeight = -1;

        private int _parentIndex = -1;
        private bool _originalExpandedState;
        private Size _originalCardSize;
        private Color _originalBGColor;
        private Control _originalParent = null;
        private PopoutForm _popout = null;
        private List<ThemeableImageButton> managedToolButtons = [];

        public CollapsibleGroupBox(ToolTip ttManager)
        {
            InitializeComponent();
            _ttManager = ttManager;

            DoubleBuffered = true;

            pBackBorder.Paint += BorderPaint;

            // For Drag & Drop
            pBackBorder.MouseDown += ChildMouseDownPassthru;
            pHeaderBox.MouseDown += ChildMouseDownPassthru;
            lblBoxTitle.MouseDown += ChildMouseDownPassthru;

            btnExpander.ImageSize = new Size(32, 32);
            _ttManager.SetToolTip(btnExpander, "Collapse the content");
            _expanded = true;
            managedToolButtons.Add(btnExpander);

            // TODO: Add tool button to remove a panel (if container supports such an action)
        }

        #region Properties
        [Description("Sets the Text property value of the title label")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => lblBoxTitle.Text;
            set => lblBoxTitle.Text = value;
        }

        [Description("Sets the color of the control's border")]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ActiveBorder")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                pBackBorder.Invalidate();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CollapsibleGroupBoxContent ContentControl
        {
            get => _contentControl;
            set
            {
                if (_contentControl != null && value != _contentControl)
                {
                    throw new ArgumentException("Only one content control is supported.");
                }
                else if (value == _contentControl)
                {
                    // Nothing to do.
                    return;
                }

                // Size to content area; dock.
                // Needs to be inserted before headerbox for proper docking.
                _contentControl = value;

                _contentControl.SuspendLayout();
                _contentControl.AutoScaleDimensions = new SizeF(7F, 15F);
                _contentControl.AutoScaleMode = AutoScaleMode.Font;
                _contentControl.ResumeLayout();

                Size = new Size(this.Width, pHeaderBox.Height + VerticalPadding + _contentControl.Height);

                if (_contentControl.EnablePopout)
                    AddToolboxButton(CreateToolButton(
                        Images.OpenInNewImage, $"btnPopout{_contentControl.ContentTitle.Replace(" ", "")}", PopoutButton_Click, "Open this card in a pop-out window"));

                foreach (Button tool in _contentControl.ToolboxButtons)
                {
                    AddToolboxButton(tool);
                }

                pBackBorder.Controls.Add(_contentControl);
                pBackBorder.Controls.SetChildIndex(_contentControl, 0);
                _contentControl.Dock = DockStyle.Fill;
                _contentHeight = _contentControl.Height;

                if (!string.IsNullOrWhiteSpace(_contentControl.ContentTitle))
                {
                    Text = _contentControl.ContentTitle;
                }
                _contentControl.PropertyChanged += ContentControl_PropertyChanged;
                _contentControl.FontChanged += ContentControl_FontChanged;

                BoxStateChanged += _contentControl.BoxStateChangedHandler;
            }
        }

        // This property has no effect until a command control is set -- which currently can't be done by designer.
        // TODO: Consider an enum for this instead to be more explicit?
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Expanded
        {
            get => _expanded;
            set
            {
                ToggleExpanded(value);
            }
        }

        [Browsable(false)]
        private int VerticalPadding
        {
            get => this.Padding.Top + this.Padding.Bottom + pBackBorder.Padding.Top + pBackBorder.Padding.Bottom;
        }

        #endregion

        #region Methods

        public void Draw()
        {
            _contentControl?.Draw();
        }

        public void Clear()
        {
            _contentControl?.Clear();
        }

        private void ToggleExpanded(bool expanded, bool requested = false)
        {
            if (_contentControl == null) return;
            if (_expanded == expanded) return; // Nothing to do.

            this.SuspendLayout();
            if (expanded)
            {
                _contentControl.Visible = true;
                Size = new Size(this.Width, pHeaderBox.Height + VerticalPadding + _contentHeight);
                btnExpander.OriginalImage = Images.CollapseImage;
                _ttManager?.SetToolTip(btnExpander, "Collapse the content");
            }
            else
            {
                _contentControl.Visible = false;
                Size = new Size(this.Width, pHeaderBox.Height + VerticalPadding + 1 /* to ensure lower border edge is drawn? */);
                btnExpander.OriginalImage = Images.ExpandImage;
                _ttManager?.SetToolTip(btnExpander, "Expand the content");
            }

            _expanded = expanded;
            // Don't fire events if the change was requested by the content control.
            if (!requested)
            {
                BoxStateChanged?.Invoke(this, new() { Expanded = expanded });
            }

            pBackBorder.Invalidate();
            this.ResumeLayout();
        }

        private void AddToolboxButton(Button tool)
        {
            tool.AutoSize = true;
            tool.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tool.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tool.Margin = new(0);
            tool.FlatStyle = FlatStyle.Flat;
            tool.FlatAppearance.BorderSize = 0;

            flpToolbox.Controls.Add(tool);
        }

        private Button CreateToolButton(Bitmap img, string name, EventHandler? clickHandler, string ttip = "")
        {
            ThemeableImageButton toolButton = new()
            {
                OriginalImage = img,
                ImageSize = new(32, 32),
                Enabled = true,
                Name = name,
            };
            _ttManager?.SetToolTip(toolButton, ttip);

            toolButton.Click += clickHandler;

            managedToolButtons.Add(toolButton);

            return toolButton;
        }

        #endregion

        #region Event handlers

        private void BorderPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control c) return;

            ControlPaint.DrawBorder(
                e.Graphics, c.ClientRectangle,
                _borderColor, 1, ButtonBorderStyle.Solid,
                _borderColor, 1, ButtonBorderStyle.Solid,
                _borderColor, 1, ButtonBorderStyle.Solid,
                _borderColor, 1, ButtonBorderStyle.Solid);
        }

        private void ContentControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ContentTitle":
                    Text = _contentControl.ContentTitle;
                    break;
                case "Expanded":
                    ToggleExpanded(_contentControl.Expanded, true);
                    break;
            }
        }

        private void BtnExpander_Click(object sender, EventArgs e)
        {
            ToggleExpanded(!_expanded);
        }


        private void ContentControl_FontChanged(object sender, EventArgs e)
        {
            if (_contentControl.Height > 0)
                _contentHeight = _contentControl.Height;
        }

        private void ChildMouseDownPassthru(object? sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        private void PopoutButton_Click(object sender, EventArgs e)
        {
            if (_popout is not null)
            {
                Debug.WriteLine("Somehow, the popout button was clicked while hidden?");
                return;
            }

            _originalParent = this.Parent; // for un-popping out.

            // Remove from the current container and hide it first.
            _parentIndex = _originalParent.Controls.IndexOf(this);
            _originalExpandedState = Expanded;
            _originalCardSize = new(Size.Width, Size.Height);
            _originalParent.Controls.Remove(this);
            _originalBGColor = BackColor;

            // Now add to the new container.
            _popout = new(this)
            {
                Size = new(_originalCardSize.Width + 20, _contentHeight + VerticalPadding + pHeaderBox.Height + 75), // expanded content size plus extra for window title bar.
                BackColor = BackColor,
                Font = _originalParent.Font, // preserve zoom.
            };
            _popout.FormClosed += Popout_FormClosed;

            // Hide these in the pop-out window.
            managedToolButtons.ForEach(b => b.Visible = false);

            // Need core to register the popout window for themeing.
            _popout.Show();
        }

        private void Popout_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_popout is null) return;

            SuspendLayout();
            _popout.Controls.Remove(this);

            _originalParent.SuspendLayout();

            // The PopoutForm Sets docking to Fill, but this breaks the layout engine of other containers such as FLP. Clear before re-adding to FLP.
            Dock = DockStyle.None;
            Expanded = _originalExpandedState;
            _originalParent.Controls.Add(this);
            Size = _originalCardSize;
            BackColor = _originalBGColor;
            if (_parentIndex >= 0 && _parentIndex < _originalParent.Controls.Count - 1)
                _originalParent.Controls.SetChildIndex(this, _parentIndex);

            managedToolButtons.ForEach(b => b.Visible = true);
            Visible = true;

            _originalParent.ResumeLayout();
            _originalParent.Invalidate();

            try
            {
                _popout.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception while closing popout: {ex})");
            }
            finally
            {
                _popout = null;
                ResumeLayout();
            }
        }
        #endregion

        #region BoxStateChanged Event
        public delegate void BoxStateEventHandler(object sender, BoxStateEventArgs e);

        public class BoxStateEventArgs : EventArgs
        {
            public bool Expanded { get; init; }
        }

        public event BoxStateEventHandler BoxStateChanged;
        #endregion
    }
}
