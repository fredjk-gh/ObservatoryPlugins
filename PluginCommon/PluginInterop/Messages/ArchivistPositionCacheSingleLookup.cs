using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages.Data;
using Observatory.Framework;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages
{
    public class ArchivistPositionCacheSingleLookup : PluginMessageWrapper
    {
        public const string MESSAGE_TYPE = "ArchivistPositionCacheSingleLookup";

        private const string P_SYSTEM_NAME= "system_name";
        private const string P_SYSTEM_ID64 = "system_id64";
        private const string P_EXT_FALLBACK = "ext_fallback";

        public static ArchivistPositionCacheSingleLookup New(string sysName = "", ulong sysId64 = 0, bool externalFallback = false)
        {
            return new()
            {
                // Set required properties here.
                SystemName = sysName,
                SystemId64 = sysId64,
                ExternalFallback = externalFallback,
            };
        }

        public static bool TryUnmarshal(PluginMessageWrapper message, out PluginMessageWrapper result)
        {
            result = null;
            try
            {
                result = new ArchivistPositionCacheSingleLookup(message);
            }
            catch (Exception) { }
            return result != null;
        }

        public ArchivistPositionCacheSingleLookup(MessageSender sender, PluginMessage msg) : base(sender, msg)
        { }

        protected ArchivistPositionCacheSingleLookup(Guid? inReplyTo = null) : base(MESSAGE_TYPE, inReplyTo)
        { }

        private ArchivistPositionCacheSingleLookup(PluginMessageWrapper raw) : base(raw.Sender, raw.MessageFromInit)
        {
            Unmarshal();
        }

        protected override Dictionary<string, object> Marshal()
        {
            return new()
            {
                // Create the Dictionary of name/value pairs here. Example:
                { P_SYSTEM_NAME, SystemName },
                { P_SYSTEM_ID64, SystemId64 },
                { P_EXT_FALLBACK, ExternalFallback },
            };
        }

        protected override void Unmarshal()
        {
            if (MessageFromInit.MessageType != MESSAGE_TYPE)
            {
                throw new ArgumentException($"Provided raw message type ({MessageFromInit.MessageType}) is not unmarshalable by {this.GetType().Name}.");
            }

            // Parse values out of the initial message payload.
            SystemName = MessageFromInit.MessagePayload.GetValueOrDefault(P_SYSTEM_NAME).ToString();
            SystemId64 = Convert.ToUInt64(MessageFromInit.MessagePayload.GetValueOrDefault(P_SYSTEM_ID64));
            ExternalFallback = Convert.ToBoolean(MessageFromInit.MessagePayload.GetValueOrDefault(P_EXT_FALLBACK));
        }

        public string SystemName { get; set; }
        public ulong SystemId64 { get; set; }
        public bool ExternalFallback { get; set; }

        public ArchivistPositionCacheSingle ReplyWith(string sysName, ulong sysId64, double x, double y, double z)
        {
            return ArchivistPositionCacheSingle.NewResponse(sysName, sysId64, x, y, z, SourceMessageId);
        }

        public ArchivistPositionCacheSingle ReplyWith(ArchivistPositionCacheItem item)
        {
            return ArchivistPositionCacheSingle.NewResponse(item, SourceMessageId);
        }
    }
}
