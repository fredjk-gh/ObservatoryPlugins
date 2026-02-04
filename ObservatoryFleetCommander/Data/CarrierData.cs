using com.github.fredjk_gh.PluginCommon.Data.EdGIS;
using com.github.fredjk_gh.PluginCommon.Data.Id64;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using com.github.fredjk_gh.PluginCommon.PluginInterop;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using System.Diagnostics;
using System.Text.Json.Serialization;
using static com.github.fredjk_gh.ObservatoryFleetCommander.Data.CarrierTradeData;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    public class CarrierData
    {
        internal const int JUMP_COOLDOWN_MINUTES = 5;
        internal const int CANCEL_COOLDOWN_MINUTES = 1;

        private const string UNNAMED = "(unnamed)";

        private bool _deserialized = false;
        private CarrierStats _lastStats;
        private CarrierJumpRequest _lastJumpRequest;
        private CarrierJumpCancelled _lastJumpCancelledRequest;
        private System.Timers.Timer _jumpTimer;
        private System.Timers.Timer _cooldownTimer;
        private readonly System.Timers.Timer _ticker = new(1000);
        private DateTime _lastCooldownDateTime = DateTime.MinValue;
        private int _carrierFuel;
        private string _signalName = null;

        // Only to be used for JSON Deserialization.
        public CarrierData()
        {
            CarrierId = 0;
            CarrierType = CarrierType.FleetCarrier;
            CarrierName = UNNAMED;
            _deserialized = true;
        }

        internal CarrierData(string owner, CarrierBuy buy)
        {
            Owner = owner;
            CarrierId = buy.CarrierID;
            CarrierType = buy.CarrierType;
            CarrierCallsign = buy.Callsign;
            CarrierName = UNNAMED;
            CarrierFuel = 500;
        }

        internal CarrierData(string owner, CarrierStats stats)
        {
            Owner = owner;
            CarrierId = stats.CarrierID;
            CarrierType = stats.CarrierType;
            CarrierCallsign = stats.Callsign;
            CarrierName = stats.Name;
            CarrierFuel = stats.FuelLevel;
            _lastStats = stats;
        }

        internal CarrierData(string owner, ulong carrierId, CarrierType type, string name = "", string callsign = "", int fuel = 0)
        {
            // This is for the awful way we have to discover squadron carriers.
            Owner = owner;
            CarrierId = carrierId;
            CarrierType = type;
            CarrierName = string.IsNullOrEmpty(name) ? UNNAMED : name; // Not available through CarrierLocation, etc.
            CarrierCallsign = string.IsNullOrEmpty(callsign) ? UNNAMED : callsign; // Not available through CarrierLocation, etc.
            CarrierFuel = fuel;
        }

        public string Owner { get; set; }
        public HashSet<string> CommandersOnBoard { get; set; } = [];
        public CarrierDockingAccess DockingAccess { get; set; }
        public string CarrierName { get; set; }
        public ulong CarrierId { get; set; }
        public CarrierType CarrierType { get; set; }
        [JsonIgnore]
        public bool IsSquadronCarrier
        {
            get => CarrierType == CarrierType.SquadronCarrier;
        }
        [JsonIgnore]
        public int CarrierCapacity
        {
            get => IsSquadronCarrier ? 60000 : 25000;
        }
        public string CarrierCallsign { get; set; }
        public int CarrierFuel
        {
            get => _carrierFuel;
            set
            {
                _carrierFuel = Math.Clamp(value, 0, 1000);
            }
        }
        public Dictionary<string, InventoryItem> Inventory { get; set; } = [];
        public InventoryItem InventoryAdjust(string itemId, int quantityChange, string itemName = "")
        {
            if (quantityChange == 0)
            {
                if (Inventory.TryGetValue(itemId, out InventoryItem inventory))
                    return inventory;
                else
                    return new()
                    {
                        ItemId = itemId,
                        ItemName = itemName,
                        Quantity = 0,
                    };
            }

            itemName = GetItemName(itemId, itemName);

            InventoryItem item;
            if (!Inventory.TryGetValue(itemId, out InventoryItem existingItem))
            {
                item = new()
                {
                    ItemId = itemId,
                    ItemName = itemName,
                    Quantity = quantityChange,
                };
                Inventory.Add(itemId, item);
            }
            else
            {
                item = existingItem;
                if (item.Quantity + quantityChange <= 0)
                {
                    item.Quantity = 0;
                    Inventory.Remove(itemId);
                }
                else
                {
                    item.Quantity += quantityChange;
                }
            }
            return item;
        }

        private static string GetItemName(string itemId, string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                if (FDevIDs.CommodityBySymbol.ContainsKey(itemId.ToLower()))
                {
                    itemName = FDevIDs.CommodityBySymbol[itemId].Name;
                }
                else if (FDevIDs.RareCommodityBySymbol.ContainsKey(itemId.ToLower()))
                {
                    itemName = FDevIDs.RareCommodityBySymbol[itemId].Name;
                }
                else
                {
                    //Debug.Fail($"Cannot find commodity in FDevIDs: {itemId}");
                    itemName = itemId;
                }
            }

            return itemName;
        }

        public Dictionary<string, CarrierTradeData> TradeOrders { get; set; } = [];

        public CarrierTradeData TradeOrderAdjust(CarrierTradeOrder to)
        {
            // Will have one of:
            // .PurchaseOrder
            // .SaleOrder
            // .CancelTrade
            CarrierTradeData td = null;
            var key = to.Commodity.ToLower();
            if (TradeOrders.TryGetValue(key, out CarrierTradeData existingTradeOrder)) td = existingTradeOrder;

            if (td is not null)
            {
                if (to.CancelTrade)
                {
                    TradeOrders.Remove(key);
                }
                else
                {
                    // Item ID is already set.
                    td.Price = to.Price;
                    td.IsBlackMarket = to.BlackMarket;
                    td.Quantity = (to.PurchaseOrder > 0 ? to.PurchaseOrder : to.SaleOrder);
                    td.TradeType = (to.PurchaseOrder > 0 ? CarrierTradeType.Buy : CarrierTradeType.Sell);
                }
            }
            else if (!to.CancelTrade)
            {
                var c = FDevIDs.AllCommoditiesBySymbol[key];
                td = new()
                {
                    ItemID = key,
                    ItemName = c.Name,
                    Price = to.Price,
                    IsBlackMarket = to.BlackMarket,
                    Quantity = (to.PurchaseOrder > 0 ? to.PurchaseOrder : to.SaleOrder),
                    TradeType = (to.PurchaseOrder > 0 ? CarrierTradeType.Buy : CarrierTradeType.Sell),
                };
                TradeOrders.Add(td.ItemID, td);
            }
            return td;
        }

        public long CarrierBalance { get; set; }
        public DateTime StatsAsOfDate { get; set; }
        public CarrierStats LastCarrierStats
        {
            get => _lastStats;
            set
            {
                if (CarrierId == 0 && _deserialized || CarrierId == value.CarrierID)
                {
                    CarrierName = value.Name;
                    CarrierBalance = value.Finance.CarrierBalance;
                    StatsAsOfDate = value.TimestampDateTime;

                    _lastStats = value;
                    _deserialized = false;
                }
            }
        }

        public float DistanceTravelledLy { get; set; }

        public int TotalJumps { get; set; }

        public CarrierJumpRequest LastCarrierJumpRequest
        {
            get => _lastJumpRequest;
            set
            {
                _lastCooldownDateTime = DateTime.MinValue; // Force update of cached value.
                _lastJumpRequest = value;
            }
        }

        public CarrierJumpCancelled LastCarrierJumpCancelled
        {
            get => _lastJumpCancelledRequest;
            set
            {
                _lastJumpRequest = null;
                _lastCooldownDateTime = DateTime.MinValue;
                _lastJumpCancelledRequest = value;
            }
        }

        [JsonIgnore]
        public DateTime DepartureDateTime
        {
            get
            {
                if (LastCarrierJumpRequest != null
                    && !string.IsNullOrEmpty(LastCarrierJumpRequest.DepartureTime))
                {
                    return LastCarrierJumpRequest.DepartureTimeDateTime;
                }
                return DateTime.MinValue;
            }
        }

        [JsonIgnore]
        public double DepartureTimeMinutes
        {
            get
            {
                if (DepartureDateTime == DateTime.MinValue) return Double.MinValue;
                return DepartureDateTime.Subtract(DateTime.Now).TotalMinutes;
            }
        }

        [JsonIgnore]
        public DateTime CooldownDateTime
        {
            get
            {
                if (LastCarrierJumpRequest != null
                    && !string.IsNullOrEmpty(LastCarrierJumpRequest.DepartureTime)
                    && _lastCooldownDateTime == DateTime.MinValue)
                {
                    // We have a jump request. Ensure our cached cooldown is up-to-date
                    _lastCooldownDateTime = LastCarrierJumpRequest.DepartureTimeDateTime.AddMinutes(JUMP_COOLDOWN_MINUTES);
                }
                else if (LastCarrierJumpCancelled != null && _lastCooldownDateTime == DateTime.MinValue)
                {
                    _lastCooldownDateTime = LastCarrierJumpCancelled.TimestampDateTime.AddMinutes(CANCEL_COOLDOWN_MINUTES);
                }
                return _lastCooldownDateTime;
            }
        }

        public CarrierPositionData Position { get; set; }

        [JsonIgnore]
        public bool IsPositionKnown
        {
            get => Position != null;
        }

        [JsonIgnore]
        public bool CooldownNotifyScheduled { get => CarrierCooldownTimer != null; }

        [JsonIgnore]
        internal System.Timers.Timer CarrierCooldownTimer
        {
            get => _cooldownTimer;
            set => _cooldownTimer = value;
        }

        [JsonIgnore]
        public bool CarrierJumpTimerScheduled { get => CarrierJumpTimer != null; }

        [JsonIgnore]
        internal System.Timers.Timer CarrierJumpTimer
        {
            get => _jumpTimer;
            set => _jumpTimer = value;
        }

        [JsonIgnore]
        public System.Timers.Timer Ticker
        {
            get => _ticker;
        }

        public void ClearJumpState()
        {
            LastCarrierJumpRequest = null;
            LastCarrierJumpCancelled = null;
            ClearTimers();
        }

        public void ClearTimers()
        {
            if (CarrierCooldownTimer != null)
            {
                CarrierCooldownTimer.Stop();
                CarrierCooldownTimer = null;
            }
            if (CarrierJumpTimer != null)
            {
                CarrierJumpTimer.Stop();
                CarrierJumpTimer = null;
            }
            _lastCooldownDateTime = DateTime.MinValue;
            _ticker.Stop();
        }

        public bool MaybeUpdateLocation(CarrierPositionData newPosition)
        {
            if (newPosition == null
                || (Position?.IsSamePosition(newPosition) ?? false)) return false;

            Position = newPosition;
            return true;
        }

        public FleetCarrierRouteJobResult.RouteResult Route { get; set; }

        [JsonIgnore]
        public bool HasRoute { get => Route != null; }

        public bool MatchesSignalName(string signalName)
        {
            var expectedSignalName = MakeSignalName();
            return (!string.IsNullOrWhiteSpace(expectedSignalName)
                && signalName.Equals(expectedSignalName, StringComparison.OrdinalIgnoreCase));
        }

        internal string MakeSignalName()
        {
            if (!string.IsNullOrWhiteSpace(_signalName))
                return _signalName; // Use cached value.

            if (string.IsNullOrEmpty(CarrierName) && string.IsNullOrEmpty(CarrierCallsign))
                return string.Empty;

            if (IsSquadronCarrier)
                return _signalName = $"{CarrierName} | {CarrierCallsign}"; // Guessing order; only example I have has same value for these.
            else
                return _signalName = $"{CarrierName} {CarrierCallsign}";
        }

        public override string ToString()
        {
            return $"{CarrierName} (type: {CarrierType})";
        }

        internal int EstimateFuelForJumpFromCurrentPosition(CommanderContext ctx, CarrierPositionData newPosition)
        {
            if (!IsPositionKnown || newPosition == null || LastCarrierStats == null) return 0; // Not enough data.

            // These may trigger an external lookup and should be done on a separate thread.
            Position.StarPos ??= MaybeFetchStarPos(Position, ctx);
            newPosition.StarPos ??= MaybeFetchStarPos(newPosition, ctx);

            // Ideal case: two detailed positions.
            int estFuelUsage = 0;
            StarPosition pos1 = Position.StarPos;
            StarPosition pos2 = newPosition.StarPos;
            if (pos1 != null && pos2 != null)
            {
                double distanceLy = Id64CoordHelper.Distance(pos1, pos2);
                long capacityUsage = LastCarrierStats.SpaceUsage.TotalCapacity - LastCarrierStats.SpaceUsage.FreeSpace;
                double fuelCost = 5 + (distanceLy / 8.0) * (1.0 + ((capacityUsage + CarrierFuel) / CarrierCapacity));

                estFuelUsage = Convert.ToInt32(Math.Round(fuelCost, 0));
            }
            else if (!ctx.Core.IsLogMonitorBatchReading)
            {
                Debug.WriteLineIf(Position.StarPos != null, $"No coordinates for current system (after fetching!): {Position.SystemName}");
                Debug.WriteLineIf(newPosition.StarPos != null, $"No coordinates for new system (after fetching!): {newPosition.SystemName}");
            }

            // Fallback to the spansh estimate from the route.
            if (estFuelUsage == 0 && HasRoute)
            {
                var jumpInfo = Route.Find(newPosition.SystemName);
                if (jumpInfo != null) estFuelUsage = jumpInfo.FuelUsed;
            }

            return estFuelUsage;
        }

        private StarPosition MaybeFetchStarPos(CarrierPositionData pos, CommanderContext ctx)
        {
            // We'll get rate-limited during batch-read. Don'to do it.
            if (ctx.Core.IsLogMonitorBatchReading
                || (string.IsNullOrEmpty(pos.SystemName) && pos.SystemAddress == 0))
                return null;

            // Call Archivist, if installed, falling back to EdGIS.
            // To do this properly, we need to send the lookup and handle the requesting action (ie. fuel estimate)
            // when that operation completes.

            if (ctx.Dispatcher.PluginTracker.IsActive(PluginTracker.PluginType.fredjk_Archivist))
            {
                StarPosition result = null;
                var posCacheLookup = ArchivistPositionCacheSingleLookup.New(pos.SystemName, pos.SystemAddress, /*externalFallback=*/ true);
                void respHandler(ArchivistPositionCacheSingle resp)
                {
                    result = new()
                    {
                        x = resp.Position.X,
                        y = resp.Position.Y,
                        z = resp.Position.Z,
                    };
                }
                ctx.Dispatcher.SendMessageAndAwaitResponse<ArchivistPositionCacheSingle>(posCacheLookup, respHandler, PluginType.fredjk_Archivist);

                return result;
            }

            CancellationTokenSource cts = new(2000);

            Task<StarPosition> fetchTask = Task.Run(() =>
            {
                CoordsResponse result = EdGISHelper.LookupCoords(ctx.Core.HttpClient, cts.Token, pos.SystemName, pos.SystemAddress);
                if (result != null)
                {
                    var cacheAddMsg = ArchivistPositionCacheSingle.NewAddToCache(pos.SystemName, result.SystemId64, result.Coords.X, result.Coords.Y, result.Coords.Z);
                    ctx.Dispatcher.SendMessage(cacheAddMsg, PluginTracker.PluginType.fredjk_Archivist);
                    return EdGISHelper.CoordsToStarPosition(result.Coords);
                }
                return null;
            }, cts.Token);
            try
            {
                return fetchTask.Result;
            }
            catch (Exception ex)
            {
                ctx.ErrorLogger(ex, $"Failed to fetch data from edgis for system: {pos.SystemName}");
                return null;
            }
        }
    }
}
