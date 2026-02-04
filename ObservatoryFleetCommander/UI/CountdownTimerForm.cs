using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using com.github.fredjk_gh.PluginCommon.UI;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System.Drawing.Text;
using System.Timers;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    internal partial class CountdownTimerForm : Form
    {
        private readonly IObservatoryCore _core;
        private readonly CarrierData _data;
        private FormWindowState _lastWindowState;

        internal CountdownTimerForm(IObservatoryCore core, CarrierData data)
        {
            InitializeComponent();
            TransparencyKey = BackColor;

            _lastWindowState = WindowState;
            _core = core;
            _data = data;

            // Ticker running state is managed by the main UI.
            _data.Ticker.Elapsed += Countdown_Tick;

            RefreshDisplay();
            AdjustLabelFont(lblTimerValue);
        }

        internal void RefreshDisplay()
        {
            if ((_core.CurrentLogMonitorState & LogMonitorState.Batch) != 0) return;

            string timerDesc = "(nothing scheduled)";
            string time = "0:00:00";

            if (_data.CarrierJumpTimerScheduled && DateTime.Now.CompareTo(_data.DepartureDateTime) < 0)
            {
                timerDesc = "Jump Timer";
                time = UIFormatter.Timehmmss(_data.DepartureDateTime.Subtract(DateTime.Now));
            }
            else if (DateTime.Now.CompareTo(_data.CooldownDateTime) <= 0)
            {
                timerDesc = "Cooldown Timer";
                time = UIFormatter.Timehmmss(_data.CooldownDateTime.Subtract(DateTime.Now));
            }

            lblTimerTitle.Text = $"{_data.CarrierName}{Environment.NewLine}{timerDesc}";
            lblTimerValue.Text = time;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
        }

        private void AdjustLabelFont(Label label)
        {
            // Adjust text font size to fit inside the label.
            var fontSize = GetFontSize(label, label.Text);

            Font f = label.Font;
            label.Font = new Font(f.FontFamily, fontSize);
        }

        // Adapted from https://www.csharphelper.com/howtos/howto_biggest_font_label.html
        private float GetFontSize(Label label, string text, float min_size = 36.0f, float max_size = 384.0f)
        {
            // Only bother if there's text.
            if (text.Length == 0) return min_size;

            // See how much room we have, allowing a bit for the Label's padding.
            int wid = label.DisplayRectangle.Width - (label.Padding.Left + label.Padding.Right);
            int hgt = label.DisplayRectangle.Height - (label.Padding.Top + label.Padding.Bottom);
            Size proposed = new(wid, hgt);

            // Make a Graphics object to measure the text.
            using Graphics gr = label.CreateGraphics();
            gr.TextRenderingHint = TextRenderingHint.AntiAlias;
            while (max_size - min_size > 1.0f)
            {
                float pt = (min_size + max_size) / 2f;
                using Font test_font = new(label.Font.FontFamily, pt);
                // See if this font is too big.
                SizeF text_size = TextRenderer.MeasureText(gr, text, test_font, proposed);
                if ((text_size.Width > wid) || (text_size.Height > hgt))
                    max_size = pt;
                else
                    min_size = pt;
            }
            return min_size;
        }

        private void Countdown_Tick(object sender, ElapsedEventArgs e)
        {
            _core.ExecuteOnUIThread(() =>
            {
                RefreshDisplay();
            });
        }

        // Adapted from https://stackoverflow.com/questions/3083146/winforms-action-after-resize-event
        // to throttle the amount of repainting (prefer resize end to trigger resize, but it doesn't detect maximizing).
        private void CountdownTimerForm_Resize(object sender, EventArgs e)
        {
            if (WindowState != _lastWindowState)
            {
                AdjustLabelFont(lblTimerValue);
                _lastWindowState = WindowState;
            }
        }

        private void CountdownTimerForm_ResizeEnd(object sender, EventArgs e)
        {
            AdjustLabelFont(lblTimerValue);
        }

        private void CountdownTimerForm_Shown(object sender, EventArgs e)
        {
            AdjustLabelFont(lblTimerValue);
        }

        private void CountdownTimerForm_BackColorChanged(object sender, EventArgs e)
        {
            if (!TransparencyKey.IsEmpty)
            {
                TransparencyKey = BackColor;
            }
        }

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

        private void CountdownTimerForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ctxActions.Show();
            }
        }

        protected override bool ShowWithoutActivation => true;
    }
}
