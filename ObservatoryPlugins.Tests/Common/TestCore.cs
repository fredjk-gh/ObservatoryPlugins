using Observatory.Framework.Files;
using Observatory.Framework.Interfaces;
using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryPlugins.Tests.Common
{
    internal class TestCore : IObservatoryCore
    {
        internal HttpClient _httpClient;
        internal List<string> _messages = new();
        internal Dictionary<Guid, NotificationArgs> _notifications = new();

        public TestCore() : this(null) { }
        
        public TestCore(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        internal List<object> gridItems = new List<object>();

#region IObservatoryCore implementation

        public string Version => "0.0.0.1";

        public HttpClient HttpClient { get => _httpClient; }

        public LogMonitorState CurrentLogMonitorState => LogMonitorState.Realtime;

        public bool IsLogMonitorBatchReading => false;

        public string PluginStorageFolder => "";

        public void AddGridItem(IObservatoryWorker worker, object item)
        {
            gridItems.Add(item);
        }

        public void AddGridItems(IObservatoryWorker worker, IEnumerable<object> items)
        {
            gridItems.AddRange(items);
        }

        public void CancelNotification(Guid notificationId)
        { }

        public void ClearGrid(IObservatoryWorker worker, object templateItem)
        {
            gridItems.Clear();
        }

        public void ExecuteOnUIThread(Action action)
        { }

        public Action<Exception, string> GetPluginErrorLogger(IObservatoryPlugin plugin)
        {
            return new Action<Exception, string>(ConsoleDebugLogger);
        }

        public Status GetStatus()
        {
            return new Status();
        }

        public Guid SendNotification(string title, string detail)
        {
            Guid guid = Guid.NewGuid();
            _notifications.Add(guid, new NotificationArgs
            {
                Title = title,
                Detail = detail
            });
            return guid;
        }

        public Guid SendNotification(NotificationArgs notificationEventArgs)
        {
            Guid guid = Guid.NewGuid();
            _notifications.Add(guid, notificationEventArgs);
            return guid;
        }

        public void UpdateNotification(Guid notificationId, NotificationArgs notificationEventArgs)
        {
            Debug.Assert(_notifications.ContainsKey(notificationId));
            _notifications[notificationId] = notificationEventArgs;
        }

        public void ConsoleDebugLogger(Exception ex, string context)
        {
            string message = String.Format($"{context}, {ex.ToString()}");
            _messages.Add(message);
            System.Diagnostics.Debug.WriteLine(message);
        }

        public Task PlayAudioFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public void SendPluginMessage(IObservatoryPlugin plugin, object message)
        {
            // TODO...
        }
#endregion


        public List<string> Messages
        {
            get { return _messages; }
        }

        public Dictionary<Guid, NotificationArgs> Notifications
        {
            get { return _notifications; }
        }
    }
}
