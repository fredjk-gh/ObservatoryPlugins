using Observatory.Framework;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages
{
    public class PluginMessageWrapper
    {
        protected MessageSender _sender = null;
        protected PluginMessage _msgFromInit = null;

        protected string _type;
        protected Guid? _inReplyToMsgId;
        protected Guid? _marshalledMsgId;

        protected PluginMessageWrapper(string type, Guid? inReplyTo = null)
        {
            _type = type;
            _inReplyToMsgId = inReplyTo;
        }

        public PluginMessageWrapper(MessageSender sender, PluginMessage msg)
        {
            _sender = sender;
            _msgFromInit = msg;
        }

        /// <summary>
        /// Null for newly created messages.
        /// </summary>
        public MessageSender Sender { get => _sender; }

        // For unmarshaling.
        internal PluginMessage MessageFromInit { get => _msgFromInit; }

        public Guid? SourceMessageId { get => _msgFromInit?.MessageId; }

        public string Type { get => _msgFromInit?.MessageType ?? _type; }

        public Guid? InReplyToMessageId
        {
            get => _inReplyToMsgId ?? _msgFromInit.InReplyTo;
            set => _inReplyToMsgId = value;
        }

        public PluginMessage ToPluginMessage()
        {
            PluginMessage pm = new(Type, Marshal(), _inReplyToMsgId);

            _marshalledMsgId = pm.MessageId;
            return pm;
        }

        protected virtual Dictionary<string, object> Marshal()
        {
            throw new NotImplementedException("Inherited implementations must provide marshalling");
        }

        protected virtual void Unmarshal()
        {
            throw new NotImplementedException("Inherited implementations must provide marshalling");
        }
    }
}
