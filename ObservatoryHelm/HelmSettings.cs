using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryHelm
{
    internal class HelmSettings
    {
        public static readonly HelmSettings DEFAULT = new()
        {
            GravityAdvisoryThresholdx10 = 10,
            GravityWarningThresholdx10 = 30,
            MaxNearbyScoopableDistance = 3000,
        };

        [SettingDisplayName("Gravity advisory threshold x10 (0 to disable)")]
        [SettingNumericBounds(0.0, 50.0, 1.0)]
        public int GravityAdvisoryThresholdx10 { get; set; }

        [SettingDisplayName("Really High gravity warning threshold x10")]
        [SettingNumericBounds(30.0, 100, 1.0)]
        public int GravityWarningThresholdx10 { get; set; }

        [SettingDisplayName("Maximum secondary scoopable star distance (Ls)")]
        [SettingNumericBounds(500, 50000, 1000)]
        public int MaxNearbyScoopableDistance { get; set; }
    }
}
