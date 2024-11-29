using com.github.fredjk_gh.ObservatoryAggregator;
using com.github.fredjk_gh.ObservatoryAggregator.UI;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
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
        private AggregatorSettings _settings;
        private AggregatorGrid _gridData;
        private string _commander;
        private ShipData _shipData;
        private Dictionary<string, Dictionary<ulong, ShipData>> _shipDataByCommander = new();
        private int _destinationBodyId = 0; // arrival

        public void Load(IObservatoryCore core, Aggregator worker, AggregatorSettings settings)
        {
            Core = core;
            Worker = worker;
            _settings = settings;
        }

        public IObservatoryCore Core { get; internal set; }
        public Aggregator Worker { get; internal set; }

        public bool IsDirty { get; internal set; }

        public AggregatorSettings Settings { get => _settings; }
        public void ApplySettings(AggregatorSettings newSettings)
        {
            // TODO Wire up events for when specific settings values change so I can trigger re-draw when those change.
            _settings = newSettings;
            IsDirty = true;
        }

        public SystemSummary CurrentSystem { get; private set; }
        public string CurrentCommander
        {
            get => _commander;
            set
            {
                _commander = value;
                IsDirty = true;
            }
        }

        public ShipData CurrentShip
        { 
            get => _shipData;
            private set
            {
                _shipData = value;
                IsDirty = true;
            }
        }

        public Dictionary<int, List<NotificationData>> Notifications { get => _notifications; }
        public Dictionary<int, BodySummary> BodyData { get => _bodies; }

        public void ChangeSystem(string newSystem, ulong systemAddress)
        {
            CurrentSystem = new(newSystem, systemAddress);
            _destinationBodyId = 0;

            _notifications.Clear();
            _bodies.Clear();
            _gridData = null;
            IsDirty = true;
        }

        public void MaybeChangeDestinationBody(Destination destination)
        {
            if (destination != null && destination.System == CurrentSystem.SystemAddress && destination.Body != _destinationBodyId && destination.Body != 0)
            {
                _destinationBodyId = destination.Body;
                MarkForVisit(_destinationBodyId);
            }
        }
        internal Dictionary<ulong, ShipData> GetShipsForCommander()
        {
            if (string.IsNullOrWhiteSpace(CurrentCommander))
            {
                return null;
            }

            if (!_shipDataByCommander.ContainsKey(CurrentCommander))
            {
                _shipDataByCommander.Add(CurrentCommander, new());
            }
            return _shipDataByCommander[CurrentCommander];
        }

        public string GetCommanderAndShipString()
        {
            return $"{CurrentCommander} - {CurrentShip.Name}";
        }


        public void ChangeShip(ulong shipId, string name = "")
        {
            var commanderShips = GetShipsForCommander();
            if (!commanderShips.ContainsKey(shipId))
            {
                AddKnownShip(shipId, name);
            }
            CurrentShip = commanderShips[shipId];
        }

        public void UpdateShip(ulong shipId, string userShipName)
        {
            var commanderShips = GetShipsForCommander();
            if (commanderShips.ContainsKey(shipId))
            {
                commanderShips[shipId].Name = userShipName;
                IsDirty = true;
            }
            else
            {
                AddKnownShip(shipId, userShipName);
            }
        }

        public ShipData AddKnownShip(ulong shipId, string name)
        {
            var commanderShips = GetShipsForCommander();
            commanderShips[shipId] = new(shipId, name);
            IsDirty = true;
            return commanderShips[shipId];
        }

        public List<AggregatorGrid> ToGrid()
        {
            bool haveCatchAllHeader = false;
            List<AggregatorGrid> gridItems = new();

            if (_gridData == null || IsDirty)
            {
                _gridData = new AggregatorGrid(this);
                IsDirty = false;
            }
            gridItems.Add(_gridData);

            List<int> cIds;
            if (Settings.ShowAllBodySummaries)
                cIds = Notifications.Keys.Union(_bodies.Keys).OrderBy(cid => cid).ToList();
            else
                cIds = Notifications.Keys.OrderBy(n => n).ToList();

            foreach (var cId in cIds)
            {
                var bodyDisplayString = "";
                if (cId >= Constants.MIN_BODY_COALESCING_ID && cId <= Constants.MAX_BODY_COALESCING_ID && BodyData.ContainsKey(cId))
                {
                    var bData = GetBody(cId);
                    bodyDisplayString = bData.GetBodyNameDisplayString();
                    gridItems.Add(bData.ToGrid());
                }
                else if (cId == Constants.SYSTEM_COALESCING_ID)
                {
                    // No H2 summary row!
                    bodyDisplayString = "System";
                }
                else if (cId == Constants.ALERT_COALESCING_ID)
                {
                    gridItems.Add(new AggregatorGrid(this, "Alerts", cId));
                }
                else if (cId >= Constants.DEFAULT_COALESCING_ID && !haveCatchAllHeader)
                {
                    gridItems.Add(new AggregatorGrid(this, "Other notifications"));
                    haveCatchAllHeader = true;
                }

                if (Notifications.ContainsKey(cId))
                {
                    foreach (var n in GetNotifications(cId))
                    {
                        gridItems.Add(n.ToGrid(bodyDisplayString));
                    }
                }
            }

            return gridItems;
        }

        public void AddNotification(NotificationData n)
        {
            NotificationData shifted = n;
            if (n.CoalescingID >= Constants.MIN_BODY_COALESCING_ID && n.CoalescingID <= Constants.MAX_BODY_COALESCING_ID
                && string.IsNullOrWhiteSpace(n.ExtendedDetails) && !n.Title.StartsWith(GetBody(n.CoalescingID).GetBodyNameDisplayString()))
            //if (string.IsNullOrWhiteSpace(n.ExtendedDetails) && !n.Title.StartsWith(GetBody(n.CoalescingID).BodyShortName))
            {
                // we have a notification that does not start with the body short name and has an empty extended details. This
                // means the title won't suppress -- let's shift values right.
                shifted = new(this, n.System, n.Sender, "", n.Title, n.Detail, n.CoalescingID);
            }
            GetNotifications(shifted.CoalescingID).Add(shifted);
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

        public void MarkForVisit(int bodyId)
        {
            if (!_notifications.ContainsKey(bodyId)) return;

            GetNotifications(bodyId)
                .ForEach(n =>
                {
                    if (n.VisitedState == VisitedState.MarkForVisit || Core.IsLogMonitorBatchReading)
                        n.VisitedState = VisitedState.Unvisited;
                });
        }

        public void MarkVisited(int bodyId)
        {
            if (!_notifications.ContainsKey(bodyId)) return;
            
            GetNotifications(bodyId)
                .ForEach(n =>
                {
                    if (n.VisitedState == VisitedState.Unvisited || Core.IsLogMonitorBatchReading)
                        n.VisitedState = VisitedState.Visited;
                });
        }

        public BodySummary GetBody(int bodyID)
        {
            if (!_bodies.ContainsKey(bodyID))
                _bodies[bodyID] = new(this);

            return _bodies[bodyID];
        }

        public void AddDiscoveryScan(FSSDiscoveryScan discoveryScan)
        {
            CurrentSystem.DiscoveryScan = discoveryScan;
            IsDirty = true;
        }

        public void AddAllBodiesFound(FSSAllBodiesFound allBodiesFound)
        {
            CurrentSystem.AllBodiesFound = allBodiesFound;
            IsDirty = true;
        }
    }
}
