using com.github.fredjk_gh.ObservatoryPluginAutoUpdater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryPlugins.Tests.AutoUpdater
{
    internal class TestableAutoUpdater : FredJKsPluginAutoUpdater
    {
        internal List<PluginVersion> LocalPluginVersions = new();

        internal override List<PluginVersion> GetLocalVersions()
        {
            return LocalPluginVersions;
        }
    }
}
