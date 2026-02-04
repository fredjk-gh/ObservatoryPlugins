namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    public class CarrierTradeData
    {
        public enum CarrierTradeType
        {
            None,
            Sell,
            Buy,
        }

        public string ItemID { get; set; }

        public string ItemName { get; set; }

        public int Price { get; set; }

        public long Quantity { get; set; }

        public CarrierTradeType TradeType { get; set; }

        public bool IsBlackMarket { get; set; } = false;
    }
}
