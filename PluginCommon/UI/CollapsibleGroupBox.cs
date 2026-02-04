using System.ComponentModel;

namespace com.github.fredjk_gh.PluginCommon.UI
{
    public partial class CollapsibleGroupBox : UserControl
    {
        private CollapsibleGroupBoxContent _contentControl;
        private Color _borderColor = SystemColors.ActiveBorder;
        private bool _expanded = false;
        private int _contentHeight = -1;

        public CollapsibleGroupBox()
        {
            InitializeComponent();

            DoubleBuffered = true;

            pBackBorder.Paint += BorderPaint;
            pBackBorder.MouseDown += ChildMouseDownPassthru;
            pHeaderBox.MouseDown += ChildMouseDownPassthru;
            lblBoxTitle.MouseDown += ChildMouseDownPassthru;

            btnExpander.ImageSize = new Size(32, 32);
            ttManager.SetToolTip(btnExpander, "Collapse the content");
            _expanded = true;
        }

        private void ChildMouseDownPassthru(object? sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
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
                ttManager.SetToolTip(btnExpander, "Collapse the content");
            }
            else
            {
                _contentControl.Visible = false;
                Size = new Size(this.Width, pHeaderBox.Height + VerticalPadding + 1 /* to ensure lower border edge is drawn? */);
                btnExpander.OriginalImage = Images.ExpandImage;
                ttManager.SetToolTip(btnExpander, "Expand the content");
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
