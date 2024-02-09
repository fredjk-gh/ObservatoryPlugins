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

        public SystemSummary CurrentSystem { get; private set; }
        public string CurrentCommander { get; set; }
        public string CurrentShip { get; set; }
        public AggregatorSettings Settings { get; set; }

        public bool IsDirty { get; internal set; }

        public Dictionary<int, List<NotificationData>> Notifications { get => _notifications; }
        public Dictionary<int, BodySummary> BodyData { get => _bodies; }

        public void ChangeSystem(string newSystem)
        {
            CurrentSystem = new(newSystem);

            _notifications.Clear();
            _bodies.Clear();
            IsDirty = true;
        }
        
        public List<AggregatorGrid> ToGridItem()
        {
            List<AggregatorGrid> items = new() {
                new AggregatorGrid()
                {
                    Flags = GetFlagsString(),
                    Title = CurrentSystem.Name,
                    Detail = GetDetailString(),
                    ExtendedDetails = $"{CurrentCommander} - {CurrentShip}",
                    Sender = Constants.PLUGIN_SHORT_NAME,
                }
            };
            return items;
        }

        public string GetFlagsString()
        {
            List<string> parts = new();

            if (CurrentSystem.IsUndiscovered) parts.Add("🆕"); // or 🥇??
            // ⚛, 💯, ✔, 💫 or 🎇 as alternatives?
            if (CurrentSystem.AllBodiesFound != null) parts.Add("💯");
            if (_bodies.Values.Any(b => b.IsScoopableStar)) parts.Add("⛽");

            return string.Join(Constants.DETAIL_SEP, parts);
        }

        public string GetDetailString()
        {
            if (CurrentSystem.DiscoveryScan != null)
                return $"{CurrentSystem.DiscoveryScan.BodyCount} bodies";

            return "";
        }

        public List<AggregatorGrid> ToGrid(bool isBatchMode)
        {
            List<AggregatorGrid> gridItems = new();

            gridItems.AddRange(ToGridItem());

            //if (_bodies.Count == 0) return gridItems;

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
                    gridItems.Add(bData.ToGridItem());
                } else if (cId == Constants.SYSTEM_COALESCING_ID)
                {
                    bodyDisplayString = "System";
                }

                if (Notifications.ContainsKey(cId))
                {
                    foreach (var n in GetNotifications(cId))
                    {
                        gridItems.Add(n.ToGridItem(bodyDisplayString));
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
