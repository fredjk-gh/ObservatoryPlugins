namespace com.github.fredjk_gh.ObservatoryAggregator
{
    class ShipData(ulong shipId, string name = "")
    {
        public ulong ShipId { get => shipId; }

        public string Name
        {
            get => string.IsNullOrWhiteSpace(name) ? "(unknown ship)" : name;
            set => name = value;
        }
    }
}
