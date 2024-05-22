using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    internal class AutoUpdaterSettings
    {
        public static readonly AutoUpdaterSettings DEFAULT = new()
        {
            UseBeta = false,
        };

        [SettingDisplayName("Use beta versions")]
        public bool UseBeta { get; set; }

    }
}
