using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    // TODO: Move this to PluginCommon
    internal class PluginInterop
    {
        private const string PLUGIN_EVALUATOR = "Evaluator";
        private IObservatoryCore _core;
        private IObservatoryPlugin _plugin;

        public PluginInterop(IObservatoryCore core, IObservatoryPlugin plugin)
        {
            _core = core;
            _plugin = plugin;
        }

        public void EvaluatorAddBodyToRoute(ulong systemId64, int bodyId)
        {
            EvaluatorSetWorthMapping(systemId64, bodyId, true);
        }

        public void EvaluatorRemoveBodyFromRoute(ulong systemId64, int bodyId)
        {
            EvaluatorSetWorthMapping(systemId64, bodyId, false);
        }

        public void EvaluatorSetBodyVisited(ulong systemId64, int bodyId)
        {
            // TODO?
        }

        private void EvaluatorSetWorthMapping(ulong systemId64, int bodyId, bool value)
        {
            List<object> message = new List<object>();
            message.Add("SetWorthMapping");
            message.Add(systemId64);
            message.Add(bodyId);
            message.Add(value);

            _core.SendPluginMessage(_plugin, PLUGIN_EVALUATOR, message);
        }
    }
}
