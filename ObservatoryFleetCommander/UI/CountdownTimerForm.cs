using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.UI
{
    public partial class CountdownTimerForm : Form
    {
        private IObservatoryCore _core;
        private CarrierData _data;
        private FormWindowState _lastWindowState;

        public CountdownTimerForm(IObservatoryCore core, CarrierData data)
        {
            InitializeComponent();
            _lastWindowState = WindowState;
            _core = core;
            _data = data;

            // Ticker running state is managed by the main UI.
            _data.Ticker.Elapsed += Countdown_Tick;

            AdjustTimerFont();
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            if ((_core.CurrentLogMonitorState & LogMonitorState.Batch) != 0) return;

            string timerDesc = "(nothing scheduled)";
            string time = "0:00:00";

            if (_data.CarrierJumpTimerScheduled && DateTime.Now.CompareTo(_data.DepartureDateTime) < 0)
            {
                timerDesc = "Jump Timer";
                time = $"{_data.DepartureDateTime.Subtract(DateTime.Now).ToString(@"h\:mm\:ss")}";
            }
            else if (DateTime.Now.CompareTo(_data.CooldownDateTime) <= 0)
            {
                timerDesc = "Cooldown Timer";
                time = $"{_data.CooldownDateTime.Subtract(DateTime.Now).ToString(@"h\:mm\:ss")}";
            }

            lblTimerTitle.Text = $"{_data.CarrierName}{Environment.NewLine}{timerDesc}";
            lblTimerValue.Text = time;
        }

        private void AdjustTimerFont()
        {
            // Adjust timer text font size to fit inside the label.
            var fontSize = GetFontSize(lblTimerValue, lblTimerValue.Text);

            Font f = lblTimerValue.Font;
            lblTimerValue.Font = new Font(f.FontFamily, fontSize);
        }

        // Adapted from https://www.csharphelper.com/howtos/howto_biggest_font_label.html
        private float GetFontSize(Label label, string text, float min_size = 36.0f, float max_size = 384.0f)
        {
            // Only bother if there's text.
            if (text.Length == 0) return min_size;

            // See how much room we have, allowing a bit for the Label's padding.
            int wid = label.DisplayRectangle.Width - (label.Padding.Left + label.Padding.Right);
            int hgt = label.DisplayRectangle.Height - (label.Padding.Top + label.Padding.Bottom);

            // Make a Graphics object to measure the text.
            using (Graphics gr = label.CreateGraphics())
            {
                while (max_size - min_size > 0.1f)
                {
                    float pt = (min_size + max_size) / 2f;
                    using (Font test_font = new Font(label.Font.FontFamily, pt, FontStyle.Bold))
                    {
                        // See if this font is too big.
                        SizeF text_size = gr.MeasureString(text, test_font);
                        if ((text_size.Width > wid) || (text_size.Height > hgt))
                            max_size = pt;
                        else
                            min_size = pt;
                    }
                }
                return min_size;
            }
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
                AdjustTimerFont();
                _lastWindowState = WindowState;
            }
        }
        private void CountdownTimerForm_ResizeEnd(object sender, EventArgs e)
        {
            AdjustTimerFont();
        }
    }
}
