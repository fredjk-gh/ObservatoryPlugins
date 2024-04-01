using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    internal class CountdownManager
    {
        private IObservatoryCore Core;
        private IObservatoryWorker Worker;

        private DateTime _departureDateTime;
        private DateTime _cooldownDateTime;
        private int _jumpNumber = -1;
        private int _jumpsTotal = -1;
        private Guid _countdownNotificationId = Guid.Empty;
        private System.Timers.Timer _countdownTicker;

        public CountdownManager(IObservatoryCore core, IObservatoryWorker worker)
        {
            Core = core;
            Worker = worker;

            _countdownTicker = new System.Timers.Timer();
            _countdownTicker.Interval = 1000;
            _countdownTicker.Elapsed += Countdown_Tick;
        }

        public void InitCountdown(DateTime departure, DateTime cooldown, int jumpNumber = -1, int jumpsTotal = -1)
        {
            if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch)) return;

            _departureDateTime = departure;
            _cooldownDateTime = cooldown;
            _jumpNumber = jumpNumber;
            _jumpsTotal = jumpsTotal;

            RefreshDisplay();
            _countdownTicker.Start();
        }

        public void Cancel()
        {
            if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch)) return;

            _countdownTicker.Stop();
            if (_countdownNotificationId != Guid.Empty)
            {
                Core.CancelNotification(_countdownNotificationId);
                _countdownNotificationId = Guid.Empty;
            }
        }

        private void RefreshDisplay()
        {
            if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch)) return;

            NotificationArgs args = new()
            {
                Rendering = NotificationRendering.NativeVisual,
                Timeout = 0,
                XPos = 44,
                YPos = 6,
                Sender = Worker.ShortName,
            };

            string routeProgressStr = string.Empty;
            if (_jumpNumber > 0 || _jumpsTotal > 0)
            {
                routeProgressStr = $"{Environment.NewLine}Jump {_jumpNumber} of {_jumpsTotal}.";
            }

            if (DateTime.Now.CompareTo(_departureDateTime) < 0)
            {
                args.Title = "Carrier Jump";
                args.Detail = $"{_departureDateTime.Subtract(DateTime.Now).ToString(@"h\:mm\:ss")}{routeProgressStr}";
            }
            else if (DateTime.Now.CompareTo(_cooldownDateTime) <= 0)
            {
                args.Title = "Cooldown";
                args.Detail = $"{_cooldownDateTime.Subtract(DateTime.Now).ToString(@"h\:mm\:ss")}{routeProgressStr}";
            }
            else
            {
                Cancel();
                return;
            }

            if (_countdownNotificationId != Guid.Empty)
            {
                Core.UpdateNotification(_countdownNotificationId, args);
            }
            else
            {
                _countdownNotificationId = Core.SendNotification(args);
            }
        }

        private void Countdown_Tick(object sender, ElapsedEventArgs e)
        {
            RefreshDisplay();
        }
    }
}
