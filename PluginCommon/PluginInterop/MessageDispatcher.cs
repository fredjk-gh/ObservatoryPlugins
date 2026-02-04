using com.github.fredjk_gh.PluginCommon.PluginInterop.Marshalers;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System.Diagnostics;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop
{
    public class MessageDispatcher
    {
        private readonly IObservatoryCore _oc;
        private readonly IObservatoryPlugin _p;
        private readonly PluginType _st;
        private readonly PluginTracker _pt;
        private readonly Guid _senderGuid;
        private readonly Action<Exception, string> _errLog;
        // TODO: Think about how to prune stale responses listeners from here.
        private readonly Dictionary<Guid, Action<PluginMessageWrapper>> _pendingResponse = [];
        private readonly Dictionary<Type, Action<PluginMessageWrapper>> _registeredHandlers = [];

        private static Guid GetPluginGuid(IObservatoryPlugin sender)
        {
            var guidProp = sender.GetType().GetProperty("Guid");
            var guid = guidProp?.GetValue(sender.GetType()) as Guid?;
            return guid ?? Guid.Empty;
        }

        public MessageDispatcher(IObservatoryCore core, IObservatoryPlugin sender, PluginType senderType)
        {
            _oc = core;
            _p = sender;
            _st = senderType;
            _pt = new(senderType);
            _senderGuid = GetPluginGuid(sender); // This won't change over the lifecycle of the plugin; do the reflection junk just once.
            _errLog = core.GetPluginErrorLogger(sender);

            // Register built-in handlers.
            RegisterHandler<GenericPluginReadyMessage>(HandleReadyMessage);
        }

        public PluginTracker PluginTracker { get => _pt; }

        public void RegisterHandler<T>(Action<T> handler) where T : PluginMessageWrapper
        {
            // Allow overriding default handler by using the indexer vs. Add().
            _registeredHandlers[typeof(T)] = w =>
            {
                handler(w as T);
            };
        }

        public void SendMessage(PluginMessageWrapper msg, PluginType? targetPlugin = null)
        {
            Guid? targetPluginGuid = GetTargetPluginGuid(targetPlugin);

            if (targetPluginGuid.HasValue)
                _oc.SendPluginMessage(_p, targetPluginGuid.Value, msg.ToPluginMessage());
            else
                _oc.SendPluginMessage(_p, msg.ToPluginMessage());
        }

        public void SendMessageAndAwaitResponse<TResp>(PluginMessageWrapper msg, Action<TResp> handler, PluginType? targetPlugin = null) where TResp : PluginMessageWrapper
        {
            Guid? targetPluginGuid = GetTargetPluginGuid(targetPlugin);

            PluginMessage msgToSend = msg.ToPluginMessage();
            _pendingResponse.Add(msgToSend.MessageId, w =>
            {
                handler(w as TResp);
            });
            if (targetPluginGuid.HasValue)
                _oc.SendPluginMessage(_p, targetPluginGuid.Value, msgToSend);
            else
                _oc.SendPluginMessage(_p, msgToSend);
        }

        public void SendResponse(PluginMessageWrapper origMsg, PluginMessageWrapper responseMsg)
        {
            responseMsg.InReplyToMessageId = origMsg.SourceMessageId;
            _oc.SendPluginMessage(_p, origMsg.Sender.Guid, responseMsg.ToPluginMessage());
        }

        // Returns true if message was handled.
        // Even if not handled, if the message could be unmarshaled, it is set in the out param.
        public bool MaybeHandlePluginMessage(MessageSender sender, PluginMessage messageArgs, out PluginMessageWrapper unMarshaled)
        {
            PluginMessageWrapper w = new(sender, messageArgs);

            if (!PluginMessageUnmarshaler.TryUnmarshal(w, out unMarshaled))
                return false;

            Action<PluginMessageWrapper> handler = null;
            if (unMarshaled.InReplyToMessageId.HasValue)
                _pendingResponse.TryGetValue(unMarshaled.InReplyToMessageId.Value, out handler);
            if (handler is null)
                _registeredHandlers.TryGetValue(unMarshaled.GetType(), out handler);

            // Nothing to handle this.
            if (handler is null)
                return false;

            try
            {
                handler?.Invoke(unMarshaled);
                return true;
            }
            catch (Exception ex)
            {
                _errLog(ex, $"MessageDispatcher[{_p.Name}]: Failed while handling message of type: {unMarshaled.Type}");
            }
            finally
            {
                if (unMarshaled.InReplyToMessageId.HasValue && _pendingResponse.ContainsKey(unMarshaled.InReplyToMessageId.Value))
                {
                    _pendingResponse.Remove(unMarshaled.InReplyToMessageId.Value);
                }
            }
            return false;
        }


        // Public so overriders can use it.
        public void HandleReadyMessage(GenericPluginReadyMessage readyMsg)
        {
            PluginType type = _pt.MarkActive(readyMsg.Sender.Guid, new(readyMsg.Sender.Version));
            if (type == PluginType.Unknown)
            {
                // Try by name.
                _pt.MarkActive(readyMsg.Sender.ShortName, new(readyMsg.Sender.Version));
            }
        }

        private Guid? GetTargetPluginGuid(PluginType? targetPlugin)
        {
            Guid? targetPluginGuid = null;
            if (targetPlugin.HasValue)
            {
                if (!_pt.IsActive(targetPlugin.Value))
                    Debug.WriteLine($"Targeted plugin {targetPlugin.Value} has not known to be present or available.");
                targetPluginGuid = PluginConstants.GuidByPluginType[targetPlugin.Value];
            }

            return targetPluginGuid;
        }
    }
}
