using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages.Data;
using Observatory.Framework;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages
{
    public class ArchivistPositionCacheBatchLookup : PluginMessageWrapper
    {
        public const string MESSAGE_TYPE = "ArchivistPositionCacheBatchLookupRequest";

        private const string P_ITEMS_LIST = "items_list";
        private const string P_EXT_FALLBACK = "ext_fallback";

        public static ArchivistPositionCacheBatchLookup New(List<ArchivistPositionCacheRequestItem> requestedItems, bool externalFallback = false)
        {
            return new()
            {
                RequestItems = requestedItems,
                ExternalFallback = externalFallback,
            };
        }

        public static bool TryUnmarshal(PluginMessageWrapper message, out PluginMessageWrapper result)
        {
            result = null;
            try
            {
                result = new ArchivistPositionCacheBatchLookup(message);
            }
            catch (Exception) { }
            return result != null;
        }

        public ArchivistPositionCacheBatchLookup(MessageSender sender, PluginMessage msg) : base(sender, msg)
        { }

        protected ArchivistPositionCacheBatchLookup(Guid? inReplyTo = null) : base(MESSAGE_TYPE, inReplyTo)
        { }

        private ArchivistPositionCacheBatchLookup(PluginMessageWrapper raw) : base(raw.Sender, raw.MessageFromInit)
        {
            Unmarshal();
        }

        protected override Dictionary<string, object> Marshal()
        {
            return new()
            {
                { P_ITEMS_LIST, RequestItems },
                { P_EXT_FALLBACK, ExternalFallback },
            };
        }

        protected override void Unmarshal()
        {
            if (MessageFromInit.MessageType != MESSAGE_TYPE)
            {
                throw new ArgumentException($"Provided raw message type ({MessageFromInit.MessageType}) is not unmarshalable by {this.GetType().Name}.");
            }

            RequestItems = (List< ArchivistPositionCacheRequestItem>)MessageFromInit.MessagePayload.GetValueOrDefault(P_ITEMS_LIST);
            ExternalFallback = Convert.ToBoolean(MessageFromInit.MessagePayload.GetValueOrDefault(P_EXT_FALLBACK));
        }

        public List<ArchivistPositionCacheRequestItem> RequestItems { get; set; }

        public bool ExternalFallback { get; set; }

        public ArchivistPositionCacheBatch ReplyWith(List<ArchivistPositionCacheItem> items)
        {
            return ArchivistPositionCacheBatch.NewResponse(items, SourceMessageId);
        }
    }
}
