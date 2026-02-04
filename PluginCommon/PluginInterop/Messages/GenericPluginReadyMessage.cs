using Observatory.Framework;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages
{
    public class GenericPluginReadyMessage : PluginMessageWrapper
    {
        internal const string MESSAGE_TYPE = "Ready";

        public static GenericPluginReadyMessage New(Guid? inReplyTo = null)
        {
            return new(inReplyTo);
        }

        public static bool TryUnmarshal(PluginMessageWrapper msg, out PluginMessageWrapper result)
        {
            result = null;
            try
            {
                result = new GenericPluginReadyMessage(msg.Sender, msg.MessageFromInit);
            }
            catch (Exception) { }
            return result != null;
        }

        protected GenericPluginReadyMessage(Guid? inReplyTo = null) : base(MESSAGE_TYPE, inReplyTo)
        { }

        public GenericPluginReadyMessage(MessageSender sender, PluginMessage msg) : base(sender, msg)
        { }


        protected override Dictionary<string, object> Marshal()
        {
            return []; // Nothing else needed.
        }

        protected override void Unmarshal()
        {
            // Nothing to do.
        }

    }
}
