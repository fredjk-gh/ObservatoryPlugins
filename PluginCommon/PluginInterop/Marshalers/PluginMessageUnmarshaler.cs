using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using System.Diagnostics;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Marshalers
{
    public abstract class PluginMessageUnmarshaler
    {
        public static bool TryUnmarshal(PluginMessageWrapper message, out PluginMessageWrapper result)
        {
            result = null;

            switch (message.Type)
            {
                case GenericPluginReadyMessage.MESSAGE_TYPE:
                    return GenericPluginReadyMessage.TryUnmarshal(message, out result);
                case HelmStatusMessage.MESSAGE_TYPE:
                    return HelmStatusMessage.TryUnmarshal(message, out result);
                case ArchivistJournalsMessage.MESSAGE_TYPE:
                    return ArchivistJournalsMessage.TryUnmarshal(message, out result);
                case ArchivistPositionCacheSingleLookup.MESSAGE_TYPE:
                    return ArchivistPositionCacheSingleLookup.TryUnmarshal(message, out result);
                case ArchivistPositionCacheSingle.MESSAGE_TYPE_RESPONSE:
                case ArchivistPositionCacheSingle.MESSAGE_TYPE_ADD:
                    return ArchivistPositionCacheSingle.TryUnmarshal(message, out result);
                case ArchivistPositionCacheBatchLookup.MESSAGE_TYPE:
                    return ArchivistPositionCacheBatchLookup.TryUnmarshal(message, out result);
                case ArchivistPositionCacheBatch.MESSAGE_TYPE_RESPONSE:
                case ArchivistPositionCacheBatch.MESSAGE_TYPE_ADD:
                    return ArchivistPositionCacheBatch.TryUnmarshal(message, out result);
                case ArchivistNotificationsMessage.MESSAGE_TYPE:
                    return ArchivistNotificationsMessage.TryUnmarshal(message, out result);

                case EvaluatorBodyRoutingMessage.EVALUATOR_SET_WORTH_VISITING:
                case EvaluatorBodyRoutingMessage.EVALUATOR_SET_BODY_VISITED:
                    return EvaluatorBodyRoutingMessage.TryUnmarshal(message, out result);
                default:
                    // Also update MessageDispatcher.MaybeHandlePluginMessage.
                    Debug.Fail($"TryUnmarshal: Encountered unrecognized v2 message type: {message.Type}; missing registration?");
                    break;
            }

            return false;
        }
    }
}
