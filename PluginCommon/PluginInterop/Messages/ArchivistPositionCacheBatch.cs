using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages.Data;
using Observatory.Framework;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages
{
    public class ArchivistPositionCacheBatch : PluginMessageWrapper
    {
        public const string MESSAGE_TYPE_ADD = "ArchivistPositionCacheBatchAdd";
        public const string MESSAGE_TYPE_RESPONSE = "ArchivistPositionCacheBatchResp";
        private static readonly HashSet<string> SupportedMessageTypes =
        [
            MESSAGE_TYPE_RESPONSE,
            MESSAGE_TYPE_ADD,
        ];

        private const string P_ITEM_LIST = "ITEMS";

        public static ArchivistPositionCacheBatch NewAddToCache(List<ArchivistPositionCacheItem> items)
        {
            return new(MESSAGE_TYPE_ADD, items);
        }

        public static ArchivistPositionCacheBatch NewResponse(List<ArchivistPositionCacheItem> items, Guid? inReplyToId = null)
        {
            return new(MESSAGE_TYPE_RESPONSE, items, inReplyToId);
        }

        public static bool TryUnmarshal(PluginMessageWrapper message, out PluginMessageWrapper result)
        {
            result = null;
            try
            {
                result = new ArchivistPositionCacheBatch(message);
            }
            catch (Exception) { }
            return result != null;
        }

        public ArchivistPositionCacheBatch(MessageSender sender, PluginMessage msg) : base(sender, msg)
        { }

        protected ArchivistPositionCacheBatch(string msgType, List<ArchivistPositionCacheItem> items, Guid? inReplyTo = null) : base(msgType, inReplyTo)
        {
            Items = items;
        }

        private ArchivistPositionCacheBatch(PluginMessageWrapper raw) : base(raw.Sender, raw.MessageFromInit)
        {
            Unmarshal();
        }

        protected override Dictionary<string, object> Marshal()
        {
            return new()
            {
                { P_ITEM_LIST, Items },
            };
        }

        protected override void Unmarshal()
        {
            if (!SupportedMessageTypes.Contains(MessageFromInit.MessageType))
            {
                throw new ArgumentException($"Provided raw message type ({MessageFromInit.MessageType}) is not unmarshalable by {this.GetType().Name}.");
            }

            Items = (List<ArchivistPositionCacheItem>)MessageFromInit.MessagePayload.GetValueOrDefault(P_ITEM_LIST);
        }

        public List<ArchivistPositionCacheItem> Items { get; set; }
    }
}
