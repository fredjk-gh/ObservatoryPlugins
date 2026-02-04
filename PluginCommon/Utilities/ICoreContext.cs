using com.github.fredjk_gh.PluginCommon.PluginInterop;
using Observatory.Framework.Interfaces;

namespace com.github.fredjk_gh.PluginCommon.Utilities
{
    // A simple class that provides the context for interacting with ObservatoryCore which includes:
    // - IObservatoryCore: Core provides key functionality like HTTPClient, error reporting, plugin messaging, storage and notifications, etc.
    // - IObservatoryPlugin: The calling plugin which is required for many of these calls.
    // - MessageDispatcher: The thing that makes plugin messaging easy.
    public interface ICoreContext<out P> where P : IObservatoryWorker
    {
        public abstract IObservatoryCore Core { get; }
        public abstract P Worker { get; }
        public abstract MessageDispatcher Dispatcher { get; }
        public abstract DebugLogger Dlogger { get; }
    }
}
