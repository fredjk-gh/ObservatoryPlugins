using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    internal class Manager
    {
        // This is for rare corner cases where the actual squadron is unknown.
        internal const string SQUADRON_CARRIER_OWNER = "Squadron";

        private readonly Dictionary<string, CommanderData> _commandersByName = [];
        private readonly Dictionary<ulong, string> _carrierOwnerNameByCarrierId = [];
        private readonly Dictionary<string, SquadronData> _knownSquadronsByName = [];

        internal void Rehydrate(FleetCommanderDataCache cache)
        {
            foreach (CommanderData cmdrData in cache.KnownCommanders)
            {
                _commandersByName.Add(cmdrData.Name, cmdrData);
                if (cmdrData.HasCarrier)
                {
                    var junkInventory = cmdrData.Carrier.Inventory.Where(i => i.Value.Quantity <= 0).Select(i => i.Key).ToList();
                    foreach (var j in junkInventory)
                    {
                        cmdrData.Carrier.Inventory.Remove(j);
                    }
                    RegisterCarrier(cmdrData.Carrier);
                }
            }

            foreach (SquadronData squadron in cache.Squadrons)
            {
                RegisterSquadron(squadron);
                RegisterCarrier(squadron.Carrier);
            }
        }

        internal SquadronData RegisterSquadron(ulong id, string name, string commanderName = null)
        {
            SquadronData data = FindSquadron(id, name);
            if (data == null)
            {
                data = new(id, name);
                _knownSquadronsByName[name] = data;
            }

            if (data.ID == 0 && data.ID != id)
            {
                data.ID = id;
            }
            if (data.ID > 0 && data.Name != name)
            {
                // Found by ID, but squadron name has changed. Is that a thing?
                data.Name = name;
            }

            if (!string.IsNullOrWhiteSpace(commanderName))
            {
                data.MemberCommanderNames.Add(commanderName);
            }

            return data;
        }

        internal SquadronData RegisterSquadron(SquadronData deserializedData)
        {
            // We're deserializing here.
            if (deserializedData != null && !string.IsNullOrWhiteSpace(deserializedData.Name))
            {
                _knownSquadronsByName.Add(deserializedData.Name, deserializedData);
                return deserializedData;
            }

            return null;
        }

        internal SquadronData FindSquadron(ulong id, string name)
        {
            SquadronData data = null;
            if (id > 0)
            {
                data = GetSquadronById(id);
                if (data != null) return data;
            }

            if (!string.IsNullOrEmpty(name))
            {
                data = GetSquadronByName(name);
            }

            return data;
        }

        internal SquadronData GetSquadronById(ulong id)
        {
            if (id == 0) return null;

            return _knownSquadronsByName.Values.Where(s => s.ID == id).FirstOrDefault();
        }

        internal SquadronData GetSquadronByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            return _knownSquadronsByName.GetValueOrDefault(name, null);
        }

        internal SquadronData GetSquadronForCommander(string commanderName)
        {
            return _knownSquadronsByName.Values.Where(s => s.MemberCommanderNames.Contains(commanderName)).FirstOrDefault();
        }

        internal SquadronData GetAssociatedSquadron(ulong carrierId)
        {
            return _knownSquadronsByName.Values.Where(s => s.CarrierId == carrierId).FirstOrDefault();
        }

        internal CarrierData RegisterCarrier(string commander, CarrierBuy buyData)
        {
            var squadron = GetSquadronForCommander(commander);
            var owner = DetermineOwnerName(buyData.CarrierType, commander, squadron);

            CarrierData data = new(owner, buyData); // Sets fuel too.
            data.MaybeUpdateLocation(new(buyData.Location, buyData.SystemAddress));

            if (data.IsSquadronCarrier)
                MaybeAssociateSquadronCarrier(data, squadron);
            else
            {
                CommanderData cmdrData = _commandersByName[commander];
                MaybeAssociateCommanderCarrier(data, cmdrData);
            }
            return data;
        }

        internal CarrierData RegisterCarrier(string commander, CarrierStats stats, Location initialLocation = null)
        {
            var squadron = GetSquadronForCommander(commander);
            var owner = DetermineOwnerName(stats.CarrierType, commander, squadron);
            CarrierData data = new(owner, stats); // Sets fuel too.
            if (initialLocation != null && initialLocation.StationName == stats.Callsign && (initialLocation.Docked || initialLocation.OnFoot))
            {
                data.MaybeUpdateLocation(new(initialLocation));
            }

            if (data.IsSquadronCarrier)
                MaybeAssociateSquadronCarrier(data, squadron);
            else
            {
                CommanderData cmdrData = _commandersByName[commander];
                MaybeAssociateCommanderCarrier(data, cmdrData);
            }

            return data;
        }

        internal CarrierData RegisterCarrier(SquadronData squadron, CarrierLocation location)
        {
            if (location.CarrierType != CarrierType.SquadronCarrier) return null;

            var owner = DetermineOwnerName(location.CarrierType, string.Empty, squadron);
            CarrierData data = new(owner, location.CarrierID, location.CarrierType, squadron.DisplayName, squadron.Tag);

            MaybeAssociateSquadronCarrier(data, squadron);
            return data;
        }

        internal CarrierData RegisterCarrier(CarrierData deserializedData)
        {
            // We're deserializing here; let's assume we don't have > 1 carrier per commander (as that has already been
            // checked, and we don't have the benefit of time-ordering guarantees).
            if (deserializedData != null
                && !string.IsNullOrWhiteSpace(deserializedData.Owner)
                && !string.IsNullOrWhiteSpace(deserializedData.CarrierCallsign)
                && deserializedData.CarrierId > 0
                && deserializedData.CarrierFuel > 0)
            {
                _carrierOwnerNameByCarrierId[deserializedData.CarrierId] = deserializedData.Owner;
                return deserializedData;
            }

            return null;
        }

        private void MaybeAssociateSquadronCarrier(CarrierData carrierData, SquadronData squadronData)
        {
            if (squadronData != null && carrierData != null && carrierData.CarrierType == CarrierType.SquadronCarrier)
            {
                if (squadronData.HasCarrier && _carrierOwnerNameByCarrierId.ContainsKey(squadronData.CarrierId))
                {
                    // Clean up old carriers (if not already done by owner name).
                    _carrierOwnerNameByCarrierId.Remove(squadronData.CarrierId);
                }
                squadronData.Carrier = carrierData;
                squadronData.Tag = carrierData.CarrierCallsign;
                _carrierOwnerNameByCarrierId[squadronData.CarrierId] = squadronData.Name;
            }
        }

        private void MaybeAssociateCommanderCarrier(CarrierData data, CommanderData cmdrData)
        {
            MaybeRemoveExistingCarrier(cmdrData.Name);
            _carrierOwnerNameByCarrierId[data.CarrierId] = cmdrData.Name;
            cmdrData.Carrier = data;
        }

        // Deprecate this in favour of IsIdKnown?
        public bool IsCallsignKnown(string callSign)
        {
            return Carriers.Any(c => c.CarrierCallsign == callSign);
        }

        public bool IsIDKnown(ulong carrierOrMarketId)
        {
            return _carrierOwnerNameByCarrierId.ContainsKey(carrierOrMarketId);
        }

        public bool IsCommanderKnown(string commanderName)
        {
            return _commandersByName.ContainsKey(commanderName);
        }

        public CommanderData AddCommander(string fid, string name)
        {
            _commandersByName[name] = new()
            {
                FID = fid,
                Name = name,
            };

            return _commandersByName[name];
        }

        public CommanderData GetCommander(string name)
        {
            return _commandersByName[name];
        }

        public CarrierData GetById(ulong carrierOrMarketId)
        {
            if (!_carrierOwnerNameByCarrierId.TryGetValue(carrierOrMarketId, out string ownerName)) return null;

            return GetByOwner(ownerName);
        }

        public CarrierData GetByTimer(System.Timers.Timer timer)
        {
            return Carriers
                .Where(c => c.CarrierCooldownTimer == timer || c.CarrierJumpTimer == timer)
                .FirstOrDefault();
        }

        public CarrierData GetByOwner(string owner)
        {

            if (_commandersByName.TryGetValue(owner, out CommanderData cmdr))
                return cmdr.Carrier;
            if (_knownSquadronsByName.TryGetValue(owner, out SquadronData squadron))
                return squadron.Carrier;
            return null;
        }

        public CarrierData GetSquadronCarrier(string commanderName)
        {
            var squadron = GetSquadronForCommander(commanderName);
            if (squadron == null || !squadron.HasCarrier) return null;

            return GetById(squadron.CarrierId);
        }

        public bool SetCommanderAboard(string commanderName, ulong? marketId = null)
        {
            // MarketId having a value says where the commander IS docked. The commander cannot be docked anywhere else.
            bool hasChanges = false;
            foreach (var c in Carriers)
            {
                if (!marketId.HasValue) // not docked anywhere
                {
                    if (c.CommandersOnBoard.Remove(commanderName)) hasChanges = true;
                }
                else
                {
                    if (c.CarrierId == marketId.Value)
                    {
                        if (c.CommandersOnBoard.Add(commanderName)) hasChanges = true;
                    }
                    else
                    {
                        if (c.CommandersOnBoard.Remove(commanderName)) hasChanges = true;
                    }
                }
            }

            return hasChanges;
        }

        public void Clear()
        {
            _commandersByName.Clear();
            _knownSquadronsByName.Clear();
            _carrierOwnerNameByCarrierId.Clear();
        }

        public List<CarrierData> Carriers
        {
            get => [.. _commandersByName.Values.Where(c => c.Carrier is not null).Select(c => c.Carrier)
                .Union(_knownSquadronsByName.Values.Where(s => s.Carrier is not null).Select(s => s.Carrier))];
        }

        public List<CommanderData> Commanders
        {
            get => [.. _commandersByName.Values];
        }

        public List<SquadronData> Squadrons
        {
            get => [.. _knownSquadronsByName.Values];
        }

        public int CarrierCount
        {
            get => Carriers.Count;
        }

        private string DetermineOwnerName(CarrierType type, string commanderName, SquadronData squadron = null)
        {
            if (type == CarrierType.SquadronCarrier)
            {
                if (squadron != null)
                {
                    return squadron.DisplayName;
                }
                else
                {
                    Debug.Fail($"Unable to determine carrier owner name for a squadron carrier but no known squadron for the current commander {commanderName}!");
                    return SQUADRON_CARRIER_OWNER;
                }
            }
            else
            {
                return commanderName;
            }
        }

        private void MaybeRemoveExistingCarrier(string owner)
        {
            // Check if we already have a carrier for this owner. It could be we've crossed the June 2020 boundary
            // where Frontier deployed a patch that re-rolled the carrier call-signs. If we have an existing carrier for
            // this owner, remove it before adding the new one.
            // Assumptions:
            // - owners can be squadrons or commanders
            // - owners can have at most one carrier.
            // - Journals are processed in date order, oldest to latest.
            CarrierData existing = GetByOwner(owner);
            if (existing != null)
            {
                _carrierOwnerNameByCarrierId.Remove(existing.CarrierId);
            }
        }
    }
}
