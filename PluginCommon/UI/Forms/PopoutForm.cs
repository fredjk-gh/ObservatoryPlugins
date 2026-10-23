namespace com.github.fredjk_gh.PluginCommon.UI.Forms
{
    public partial class PopoutForm : Form
    {
        private CollapsibleGroupBox _content;

        public CollapsibleGroupBox Content { get { return _content; } }

        public PopoutForm(CollapsibleGroupBox content)
        {
            InitializeComponent();
            DoubleBuffered = true;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            ShowIcon = false;

            _content = content;
            _content.Expanded = true;

            Controls.Add(_content);
            _content.Dock = DockStyle.Fill;

            _content.MouseDown += PassthruClick;
            Text = _content.ContentControl.ContentTitle;

            TransparencyKey = Color.Empty;
            FormClosing += PopoutForm_FormClosing;
        }

        private void PassthruClick(object sender, MouseEventArgs e)
        {
            PopoutForm_MouseClick(sender, e);
        }

        protected override bool ShowWithoutActivation => true;

        private void ToggleTransparency()
        {
            if (!TransparencyKey.IsEmpty)
            {
                TransparencyKey = Color.Empty;
            }
            else
            {
                TransparencyKey = BackColor;
            }
        }

        private void ToggleBorder()
        {
            if (FormBorderStyle == FormBorderStyle.None)
            {
                FormBorderStyle = FormBorderStyle.SizableToolWindow;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.None;
            }
        }

        private void ToggleBorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleBorder();
        }

        private void ToggleTransparencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleTransparency();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PopoutForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ctxActions.Show(this, e.X, e.Y);
            }
        }

        private void PopoutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _content.MouseDown -= PassthruClick;
        }
    }
}
