using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages.Data;
using Observatory.Framework;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages
{
    public class ArchivistPositionCacheSingle : PluginMessageWrapper
    {
        public const string MESSAGE_TYPE_RESPONSE = "ArchivistPositionCacheResp";
        public const string MESSAGE_TYPE_ADD = "ArchivistPositionCacheAdd";
        private static readonly HashSet<string> SupportedMessageTypes =
        [
            MESSAGE_TYPE_RESPONSE,
            MESSAGE_TYPE_ADD,
        ];

        private const string P_ITEM = "position_item";

        public static ArchivistPositionCacheSingle NewResponse(string sysName, ulong sysId64, double x, double y, double z, Guid? inReplyToId = null)
        {
            return new(MESSAGE_TYPE_RESPONSE, NewItem(sysName, sysId64, x, y, z), inReplyToId);
        }

        public static ArchivistPositionCacheSingle NewResponse(ArchivistPositionCacheItem position, Guid? inReplyToId = null)
        {
            return new(MESSAGE_TYPE_RESPONSE, position, inReplyToId);
        }

        public static ArchivistPositionCacheSingle NewAddToCache(string sysName, ulong sysId64, double x, double y, double z)
        {
            return new(MESSAGE_TYPE_ADD, NewItem(sysName, sysId64, x, y, z));
        }

        public static ArchivistPositionCacheSingle NewAddToCache(ArchivistPositionCacheItem position)
        {
            return new(MESSAGE_TYPE_ADD, position);
        }

        internal static ArchivistPositionCacheItem NewItem(string sysName, ulong sysId64, double x, double y, double z)
        {
            return new()
            {
                SystemName = sysName,
                SystemId64 = sysId64,
                X = x,
                Y = y,
                Z = z,
            };
        }

        public static bool TryUnmarshal(PluginMessageWrapper message, out PluginMessageWrapper result)
        {
            result = null;
            try
            {
                result = new ArchivistPositionCacheSingle(message);
            }
            catch (Exception) { }
            return result != null;
        }

        public ArchivistPositionCacheSingle(MessageSender sender, PluginMessage msg) : base(sender, msg)
        { }

        protected ArchivistPositionCacheSingle(string msgType, ArchivistPositionCacheItem position, Guid? inReplyTo = null)
            : base(msgType, inReplyTo)
        {
            Position = position;
        }

        private ArchivistPositionCacheSingle(PluginMessageWrapper raw) : base(raw.Sender, raw.MessageFromInit)
        {
            Unmarshal();
        }

        protected override Dictionary<string, object> Marshal()
        {
            return new()
            {
                { P_ITEM, Position },
            };
        }

        protected override void Unmarshal()
        {
            if (!SupportedMessageTypes.Contains(MessageFromInit.MessageType))
            {
                throw new ArgumentException($"Provided raw message type ({MessageFromInit.MessageType}) is not unmarshalable by {this.GetType().Name}.");
            }

            Position = (ArchivistPositionCacheItem)MessageFromInit.MessagePayload.GetValueOrDefault(P_ITEM);
        }

        public ArchivistPositionCacheItem Position { get; set; }
    }
}
