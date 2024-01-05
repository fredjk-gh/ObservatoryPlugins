using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryPlugins.Tests.Common
{
    internal class TestWorker : IObservatoryWorker
    {
        private object _settings = new();

        public string Name => "ObservatoryPlugins.Test.TestWorker";

        public string ShortName => "TestWorker";

        public string Version => "1.2.3.4";

        public PluginUI PluginUI => null;

        public object Settings {
            get => _settings;
            set => _settings = value;
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            // Do nothing.
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            // Do nothing...
        }
    }
}
