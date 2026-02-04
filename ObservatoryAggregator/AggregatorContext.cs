using com.github.fredjk_gh.ObservatoryAggregator.UI;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.PluginInterop;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using Observatory.Framework.Interfaces;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class AggregatorContext : ICoreContext<Aggregator>
    {
        private readonly List<NotificationData> _loadTimeNotifications = [];
        private readonly Dictionary<int, List<NotificationData>> _notifications = [];
        private readonly Dictionary<int, BodySummary> _bodies = [];
        private readonly Dictionary<string, RingData> _ringData = [];
        private AggregatorGrid _gridData;
        private string _commander;
        private ShipData _shipData;
        private readonly Dictionary<string, Dictionary<ulong, ShipData>> _shipDataByCommander = [];
        private int _destinationBodyId = 0; // arrival

        private IObservatoryCore _c;
        private Aggregator _w;
        private MessageDispatcher _d;
        private DebugLogger _l;
        private AggregatorSettings _settings;

        public void Load(IObservatoryCore core, Aggregator worker, AggregatorSettings settings)
        {
            _c = core;
            _w = worker;
            _d = new(core, worker, PluginType.fredjk_Aggregator);
            _l = new(core, worker);
            _settings = settings;
        }

        public IObservatoryCore Core { get => _c; }
        public Aggregator Worker { get => _w; }
        public MessageDispatcher Dispatcher { get => _d; }
        public DebugLogger Dlogger { get => _l; }

        public bool IsDirty { get; internal set; }

        public AggregatorSettings Settings { get => _settings; }

        public void ApplySettings(AggregatorSettings newSettings)
        {
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

        public List<NotificationData> LoadTimeNotifications { get => _loadTimeNotifications; }
        public Dictionary<int, List<NotificationData>> Notifications { get => _notifications; }
        public Dictionary<int, BodySummary> BodyData { get => _bodies; }
        public Dictionary<string, RingData> RingData { get => _ringData; }
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

            if (!_shipDataByCommander.TryGetValue(CurrentCommander, out Dictionary<ulong, ShipData> shipData))
            {
                shipData = [];
                _shipDataByCommander.Add(CurrentCommander, shipData);
            }
            return shipData;
        }

        public string GetCommanderAndShipString()
        {
            return $"{(CurrentCommander ?? "(unknown commander)")} - {(CurrentShip?.Name ?? "(unknown ship)")}";
        }

        public void ChangeShip(ulong shipId, string name = "")
        {
            var commanderShips = GetShipsForCommander();
            if (commanderShips == null) return;
            if (!commanderShips.ContainsKey(shipId))
            {
                AddKnownShip(shipId, name);
            }
            CurrentShip = commanderShips[shipId];
        }

        public void UpdateShip(ulong shipId, string userShipName)
        {
            var commanderShips = GetShipsForCommander();
            if (commanderShips.TryGetValue(shipId, out ShipData shipData))
            {
                shipData.Name = userShipName;
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
            List<AggregatorGrid> gridItems = [];

            if (_gridData == null || IsDirty)
            {
                _gridData = new AggregatorGrid(this);
                IsDirty = false;
            }
            gridItems.Add(_gridData);

            List<int> cIds;
            if (Settings.ShowAllBodySummaries)
                cIds = [.. Notifications.Keys.Union(_bodies.Keys).OrderBy(cid => cid)];
            else
                cIds = [.. Notifications.Keys.OrderBy(n => n)];

            foreach (var cId in cIds)
            {
                var bodyDisplayString = "";
                if (CoalescingIDs.IsBodyCoalescingId(cId) && BodyData.ContainsKey(cId))
                {
                    var bData = GetBody(cId);
                    bodyDisplayString = bData.GetBodyNameDisplayString();
                    gridItems.Add(bData.ToGrid());
                }
                else if (cId == CoalescingIDs.SYSTEM_COALESCING_ID)
                {
                    // No H2 summary row!
                    bodyDisplayString = "System";
                }
                else if (cId == CoalescingIDs.ALERT_COALESCING_ID)
                {
                    gridItems.Add(new AggregatorGrid(this, "Alerts", cId));
                }
                else if (cId >= CoalescingIDs.DEFAULT_COALESCING_ID && !haveCatchAllHeader)
                {
                    gridItems.Add(new AggregatorGrid(this, "Other notifications"));
                    haveCatchAllHeader = true;
                }

                if (Notifications.ContainsKey(cId))
                {
                    foreach (var n in GetNotifications(cId))
                    {
                        NotificationData shifted = n;
                        if (CoalescingIDs.IsBodyCoalescingId(n.CoalescingID)
                            && string.IsNullOrWhiteSpace(n.ExtendedDetails) && !n.Title.StartsWith(bodyDisplayString))
                        {
                            // we have a notification that does not start with the body short name and has an empty extended details. This
                            // means the title won't suppress -- let's shift values right.
                            shifted = new(this, n.System, n.Sender, "", n.Title, n.Detail, n.CoalescingID);
                        }
                        gridItems.Add(shifted.ToGrid(bodyDisplayString));
                    }
                }
            }

            return gridItems;
        }

        public void AddNotification(NotificationData n)
        {
            // Dedupe!
            List<NotificationData> notifs = GetNotifications(n.CoalescingID);
            if (!notifs.Contains(n))
                notifs.Add(n);
        }

        public List<NotificationData> GetNotifications(int coalescingID)
        {
            if (!_notifications.ContainsKey(coalescingID))
                _notifications[coalescingID] = [];

            return _notifications[coalescingID];
        }

        public void AddScan(Scan scan)
        {
            GetBody(scan.BodyID)
                .Scan = scan;

            if (scan.Rings != null && scan.Rings.Count > 0)
            {
                foreach (var r in scan.Rings)
                {
                    var bodyShortName = SharedLogic.GetBodyShortName(r.Name, scan.StarSystem);
                    _ringData[bodyShortName] = new()
                    {
                        ParentBodyID = scan.BodyID,
                        ReserveLevel = scan.ReserveLevel,
                        Ring = r,
                    };
                }
            }

            if (scan.Parent != null && scan.Parent.Count > 0)
            {
                // TODO: Walk up all the barycenters and add the parent/child relation??
                //var p = scan.Parent.First();
                //if (p.ParentType == ParentType.Null)
                //{
                //    // This parent is a barycentre; inform them of a child.
                //    var parentBodySummary = GetBody(p.Body);
                //    parentBodySummary.BarycentreChildren.Add(scan);
                //}

                ScanBaryCentre childScan = scan;
                foreach (var p in scan.Parent)
                {
                    if (p.ParentType != ParentType.Null || childScan == null) break;

                    var parentBodySummary = GetBody(p.Body);
                    parentBodySummary.BarycentreChildren.Add(childScan);
                    childScan = parentBodySummary.ScanBarycentre; // may be null. Not much we can do.
                }
            }
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

        internal void AddScanSignals(SAASignalsFound scanSignals)
        {
            GetBody(scanSignals.BodyID)
                .ScanSignals = scanSignals;
        }

        public void AddScanBarycentre(ScanBaryCentre scanBarycentre)
        {
            GetBody(scanBarycentre.BodyID)
                .ScanBarycentre = scanBarycentre;
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

            var msg = EvaluatorBodyRoutingMessage.NewSetWorthVisitingMessage(CurrentSystem.SystemAddress, bodyId);
            Dispatcher.SendMessage(msg, PluginType.mattg_Evaluator);
        }

        public void ResetMark(int bodyId)
        {
            if (!_notifications.ContainsKey(bodyId)) return;

            GetNotifications(bodyId)
                .ForEach(n =>
                {
                    n.VisitedState = VisitedState.MarkForVisit;
                });

            var msg = EvaluatorBodyRoutingMessage.NewSetWorthVisitingMessage(CurrentSystem.SystemAddress, bodyId, false);
            Dispatcher.SendMessage(msg, PluginType.mattg_Evaluator);
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

            var msg = EvaluatorBodyRoutingMessage.NewSetBodyVisitedMessage(CurrentSystem.SystemAddress, bodyId);
            Dispatcher.SendMessage(msg, PluginType.mattg_Evaluator);
        }

        public BodySummary GetBody(int bodyID)
        {
            if (!_bodies.ContainsKey(bodyID))
                _bodies[bodyID] = new(this, bodyID);

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
