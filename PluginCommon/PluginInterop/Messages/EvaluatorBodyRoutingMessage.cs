using Observatory.Framework;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages
{
    public class EvaluatorBodyRoutingMessage : PluginMessageWrapper
    {
        internal const string EVALUATOR_SET_WORTH_VISITING = "SetWorthVisiting";
        internal const string EVALUATOR_SET_BODY_VISITED = "SetBodyVisited";
        private static readonly HashSet<string> SupportedMessageTypes =
        [
            EVALUATOR_SET_WORTH_VISITING,
            EVALUATOR_SET_BODY_VISITED,
        ];

        public static bool TryUnmarshal(PluginMessageWrapper message, out PluginMessageWrapper result)
        {
            result = null;
            try
            {
                result = new EvaluatorBodyRoutingMessage(message);
            }
            catch (Exception) { }
            return result != null;
        }

        public static EvaluatorBodyRoutingMessage NewSetWorthVisitingMessage(ulong id64, int bodyId, bool isWorth = true)
        {
            return new(EVALUATOR_SET_WORTH_VISITING)
            {
                SystemId64 = id64,
                BodyId = bodyId,
                WorthVisitingOrIsVisited = isWorth,
            };
        }

        public static EvaluatorBodyRoutingMessage NewSetBodyVisitedMessage(ulong id64, int bodyId, bool isVisited = true)
        {
            return new(EVALUATOR_SET_BODY_VISITED)
            {
                SystemId64 = id64,
                BodyId = bodyId,
                WorthVisitingOrIsVisited = isVisited,
            };
        }

        protected EvaluatorBodyRoutingMessage(string type, Guid? inReplyTo = null) : base(type, inReplyTo)
        { }

        public EvaluatorBodyRoutingMessage(MessageSender sender, PluginMessage msg) : base(sender, msg)
        { }

        private EvaluatorBodyRoutingMessage(PluginMessageWrapper raw) : base(raw.Sender, raw.MessageFromInit)
        {
            Unmarshal();
        }

        protected override Dictionary<string, object> Marshal()
        {
            return new()
            {
                { "ID64", SystemId64 },
                { "BodyID", BodyId },
                { (Type == EVALUATOR_SET_WORTH_VISITING ? "IsWorthMapping" : "IsVisited"), WorthVisitingOrIsVisited },
            };
        }

        protected override void Unmarshal()
        {
            var msgPluginType = PluginConstants.PluginTypeByGuid.GetValueOrDefault(Sender.Guid, PluginTracker.PluginType.Unknown);
            if (!SupportedMessageTypes.Contains(MessageFromInit.MessageType)
                || msgPluginType == PluginTracker.PluginType.mattg_Evaluator)
            {
                throw new ArgumentException($"Provided raw message is not unmarshalable by {this.GetType().Name}.");
            }

            SystemId64 = Convert.ToUInt64(MessageFromInit.MessagePayload["systemId64"]);
            BodyId = Convert.ToInt32(MessageFromInit.MessagePayload["bodyId"]);
            WorthVisitingOrIsVisited = this.Type switch
            {
                EVALUATOR_SET_WORTH_VISITING => Convert.ToBoolean(MessageFromInit.MessagePayload["worthVisiting"]),
                EVALUATOR_SET_BODY_VISITED => Convert.ToBoolean(MessageFromInit.MessagePayload["isVisited"]),
                _ => throw new ArgumentException("Unsupported raw message."),
            };

            // Force PayloadDict to marshal.
            _msgFromInit = null;
        }

        public ulong SystemId64 { get; set; }
        public int BodyId { get; set; }
        public bool WorthVisitingOrIsVisited { get; set; }
    }
}
