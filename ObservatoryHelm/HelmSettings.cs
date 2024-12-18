﻿using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryHelm
{
    [SettingSuggestedColumnWidth(500)]
    internal class HelmSettings
    {
        public static readonly HelmSettings DEFAULT = new()
        {
            GravityAdvisoryThreshold = 1.5,
            MaxNearbyScoopableDistance = 3000,
            WarnIncompleteUndiscoveredSystemScan = false,
            SuppressionZoneRadiusLy = 1500.0,
        };

        [SettingNewGroup("Notifications")]
        [SettingDisplayName("Notify if current undiscovered system is not fully scanned")]
        public bool WarnIncompleteUndiscoveredSystemScan { get; set; }

        [SettingNewGroup]
        [SettingDisplayName("Enable high gravity advisory on approach")]
        public bool EnableHighGravityAdvisory { get; set; }

        [SettingDisplayName("Gravity advisory threshold")]
        [SettingNumericBounds(0.0, 10.0, 0.1)]
        public double GravityAdvisoryThreshold { get; set; }

        [SettingDisplayName("Suppression zone advisory radius (Ly)")]
        [SettingNumericBounds(100.0, 2500.0, 100.0)]
        public double SuppressionZoneRadiusLy { get; set; }

        [SettingNewGroup("Neutron Highway")]
        [SettingDisplayName("Maximum secondary scoopable star distance (Ls)")]
        [SettingNumericBounds(500, 50000, 500)]
        public int MaxNearbyScoopableDistance { get; set; }
    }
}
