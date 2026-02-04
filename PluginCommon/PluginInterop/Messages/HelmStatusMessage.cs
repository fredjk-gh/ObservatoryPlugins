using Observatory.Framework;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages
{
    public class HelmStatusMessage : PluginMessageWrapper
    {
        public const string MESSAGE_TYPE = "HelmStatusMessage";

        public static HelmStatusMessage New(string msg, DateTime? utcTs = null)
        {
            return new()
            {
                Message = msg,
                TimestampUtc = utcTs ?? DateTime.UtcNow,
            };
        }

        public static bool TryUnmarshal(PluginMessageWrapper message, out PluginMessageWrapper result)
        {
            result = null;
            try
            {
                result = new HelmStatusMessage(message);
            }
            catch (Exception) { }
            return result != null;
        }

        public HelmStatusMessage(MessageSender sender, PluginMessage msg) : base(sender, msg)
        { }

        protected HelmStatusMessage(Guid? inReplyTo = null) : base(MESSAGE_TYPE, inReplyTo)
        { }

        private HelmStatusMessage(PluginMessageWrapper raw) : base(raw.Sender, raw.MessageFromInit)
        {
            Unmarshal();
        }

        protected override Dictionary<string, object> Marshal()
        {
            return new()
            {
                { "timestamp_utc", TimestampUtc.ToString() },
                { "message", Message },
            };
        }

        protected override void Unmarshal()
        {
            if (MessageFromInit.MessageType != MESSAGE_TYPE)
            {
                throw new ArgumentException($"Provided raw message is not unmarshalable by {this.GetType().Name}.");
            }

            TimestampUtc = DateTime.Parse(MessageFromInit.MessagePayload["timestamp_utc"].ToString());
            Message = MessageFromInit.MessagePayload["message"].ToString();
        }

        public DateTime TimestampUtc { get; set; }
        public string Message { get; set; }
    }
}
