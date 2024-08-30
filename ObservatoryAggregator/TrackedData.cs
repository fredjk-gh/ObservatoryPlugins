using com.github.fredjk_gh.ObservatoryAggregator;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class TrackedData
    {
        private Dictionary<int, List<NotificationData>> _notifications = new();
        private Dictionary<int, BodySummary> _bodies = new();
        private Aggregator _worker;
        private AggregatorSettings _settings;

        public void Load(IObservatoryCore core, Aggregator worker, AggregatorSettings settings)
        {
            Core = core;
            _worker = worker;
            _settings = settings;
        }

        public IObservatoryCore Core { get; private set; }
        public bool IsDirty { get; internal set; }

        public AggregatorSettings Settings { get => _settings; }
        public void ApplySettings(AggregatorSettings newSettings)
        {
            // TODO Wire up events for when specific settings values change so I can trigger re-draw when those change.
            _settings = newSettings;
            IsDirty = true;
        }

        public SystemSummary CurrentSystem { get; private set; }
        public string CurrentCommander { get; set; }
        public string CurrentShip { get; set; }

        public Dictionary<int, List<NotificationData>> Notifications { get => _notifications; }
        public Dictionary<int, BodySummary> BodyData { get => _bodies; }

        public void ChangeSystem(string newSystem)
        {
            CurrentSystem = new(newSystem);

            _notifications.Clear();
            _bodies.Clear();
            IsDirty = true;
        }

        public string GetCommanderAndShipString()
        {
            return $"{CurrentCommander} - {CurrentShip}";
        }

        public List<AggregatorGrid> ToGrid(bool isBatchMode)
        {
            bool haveCatchAllHeader = false;
            List<AggregatorGrid> gridItems = new();

            gridItems.Add(new AggregatorGrid(this));

            List<int> cIds;
            if (Settings.ShowAllBodySummaries)
                cIds = Notifications.Keys.Union(_bodies.Keys).OrderBy(cid => cid).ToList();
            else
                cIds = Notifications.Keys.OrderBy(n => n).ToList();

            foreach (var cId in cIds)
            {
                var bodyDisplayString = "";
                if (BodyData.ContainsKey(cId))
                {
                    var bData = GetBody(cId);
                    bodyDisplayString = bData.GetBodyNameDisplayString();
                    gridItems.Add(new AggregatorGrid(this, bData));
                }
                else if (cId == Constants.SYSTEM_COALESCING_ID)
                {
                    // No H2 summary row!
                    bodyDisplayString = "System";
                }
                else if (!haveCatchAllHeader)
                {
                    gridItems.Add(new AggregatorGrid(this, "Other notifications"));
                    haveCatchAllHeader = true;
                }

                if (Notifications.ContainsKey(cId))
                {
                    foreach (var n in GetNotifications(cId))
                    {
                        gridItems.Add(new AggregatorGrid(this, n, bodyDisplayString));
                    }
                }
            }

            if (isBatchMode) gridItems.Add(new()); // Blank line for readability.
            return gridItems;
        }

        public void AddNotification(NotificationData n)
        {
            GetNotifications(n.CoalescingID).Add(n);
        }

        public List<NotificationData> GetNotifications(int coalescingID)
        {
            if (!_notifications.ContainsKey(coalescingID))
                _notifications[coalescingID] = new();

            return _notifications[coalescingID];
        }

        public void AddScan(Scan scan)
        {
            GetBody(scan.BodyID)
                .Scan = scan;
        }

        public void AddBodySignals(FSSBodySignals bodySignals)
        {
            GetBody(bodySignals.BodyID)
                .BodySignals = bodySignals;
        }

        public void AddScanComplete(SAAScanComplete scanComplete)
        {
            GetBody(scanComplete.BodyID)
                .ScanComplete = scanComplete;
        }

        public BodySummary GetBody(int bodyID)
        {
            if (!_bodies.ContainsKey(bodyID))
                _bodies[bodyID] = new();

            return _bodies[bodyID];
        }
    }
}
