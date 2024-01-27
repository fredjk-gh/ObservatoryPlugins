using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryHelm
{
    internal class HelmSettings
    {
        public static readonly HelmSettings DEFAULT = new()
        {
            GravityWarningThresholdx10 = 30,
            GravityAdvisoryThresholdx10 = 10,
            MaxNearbyScoopableDistance = 3000,
            WarnIncompleteUndiscoveredSystemScan = false,
        };

        private int _gravityWarning = 0;
        private int _gravityAdvisory = 0;

        [SettingDisplayName("Really High gravity warning threshold x10")]
        [SettingNumericBounds(30.0, 100, 1.0)]
        public int GravityWarningThresholdx10
        {
            get => _gravityWarning;
            set
            {
                if (_gravityAdvisory < value)
                    _gravityWarning = value;
            }
        }

        [SettingIgnore]
        public double GravityWarningThreshold { get => GravityWarningThresholdx10 / 10.0; }

        [SettingDisplayName("Gravity advisory threshold x10 (0 to disable)")]
        [SettingNumericBounds(0.0, 50.0, 1.0)]
        public int GravityAdvisoryThresholdx10
        {
            get => _gravityAdvisory;
            set
            {
                if (value < _gravityWarning)
                    _gravityAdvisory = value;
            }
        }

        [SettingIgnore]
        internal double GravityAdvisoryThreshold { get => GravityAdvisoryThresholdx10 / 10.0; }

        [SettingDisplayName("Maximum secondary scoopable star distance (Ls)")]
        [SettingNumericBounds(500, 50000, 1000)]
        public int MaxNearbyScoopableDistance { get; set; }

        [SettingDisplayName("Warn if current undiscovered system is not fully scanned")]
        public bool WarnIncompleteUndiscoveredSystemScan { get; set; }
    }
}
