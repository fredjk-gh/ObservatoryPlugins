using com.github.fredjk_gh.ObservatoryHelm;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryPlugins.Tests
{
    [TestClass]
    public class HelmTests
    {
        Helm sutHelm;
        Common.TestCore _core;
        HelmSettings _settings;

        [TestInitialize]
        public void Setup()
        {
            _settings = new()
            {
                GravityWarningThresholdx10 = 30,
                GravityAdvisoryThresholdx10 = 10,
                MaxNearbyScoopableDistance = 3000,
            };
            sutHelm = new Helm();
            _core = new Common.TestCore();
            sutHelm.Load(_core);
            sutHelm.Settings = _settings;
        }

        [TestMethod]
        public void TestSetting_GravityThresholds()
        {
            HelmSettings sutSettings = new()
            {
                GravityWarningThresholdx10 = 30,
                GravityAdvisoryThresholdx10 = 10,
                MaxNearbyScoopableDistance = 3000,
            };

            // Check that the /10 helpers are correct based on initializers.
            Assert.AreEqual(3.0, sutSettings.GravityWarningThreshold);
            Assert.AreEqual(1.0, sutSettings.GravityAdvisoryThreshold);

            // Increasing Advisory > Warning is ignored
            sutSettings.GravityAdvisoryThresholdx10 = 50;
            Assert.AreEqual(1.0, sutSettings.GravityAdvisoryThreshold);

            // Increasing Advisory < Warning is accepted
            sutSettings.GravityAdvisoryThresholdx10 = 25;
            Assert.AreEqual(2.5, sutSettings.GravityAdvisoryThreshold);

            // Decreasing Warning < Advisory is ignored
            sutSettings.GravityWarningThresholdx10 = 15;
            Assert.AreEqual(3.0, sutSettings.GravityWarningThreshold);

            // Changing Warning > Advisory is accepted
            sutSettings.GravityWarningThresholdx10 = 55;
            Assert.AreEqual(5.5, sutSettings.GravityWarningThreshold);
        }

        [TestMethod]
        public void TestHelm_ApproachBodyGravityDisabled()
        {
            string system = "TestSystem";
            string fullBodyName = $"{system} 3 a";
            _settings.GravityAdvisoryThresholdx10 = 0;

            Scan scan = new()
            {
                ScanType = "Detailed",
                PlanetClass = "High Metal Content",
                BodyName = fullBodyName,
                StarSystem = system,
                SurfaceGravity = 0.32f,
                WasDiscovered = false,
                DistanceFromArrivalLS = 234.5,
            };
            ApproachBody ab = new()
            {
                StarSystem = system,
                Body = fullBodyName,
            };

            sutHelm.JournalEvent(scan);
            sutHelm.JournalEvent(ab);

            Assert.AreEqual(0, _core.Notifications.Count);
        }

        [TestMethod]
        public void TestHelm_ApproachBodyGravityNoScan()
        {
            string system = "TestSystem";
            string fullBodyName = $"{system} 3 a";

            ApproachBody ab = new()
            {
                StarSystem = system,
                Body = fullBodyName,
            };

            sutHelm.JournalEvent(ab);

            Assert.AreEqual(0, _core.Notifications.Count);
        }

        [TestMethod]
        public void TestHelm_ApproachBodyGravityAdvisory()
        {
            string system = "TestSystem";
            string fullBodyName = $"{system} 3 a";

            Scan scan = new()
            {
                ScanType = "Detailed",
                PlanetClass = "High Metal Content",
                BodyName = fullBodyName,
                StarSystem = system,
                SurfaceGravity = 1.32f * 9.81f,
                WasDiscovered = false,
                DistanceFromArrivalLS = 234.5,
            };
            ApproachBody ab = new()
            {
                StarSystem = system,
                Body = fullBodyName,
            };

            sutHelm.JournalEvent(scan);
            sutHelm.JournalEvent(ab);

            Assert.AreEqual(1, _core.Notifications.Count);
            Assert.IsTrue(_core.Notifications.Values.First().Detail.Contains("Relatively"));
        }

        [TestMethod]
        public void TestHelm_ApproachBodyGravityWarning()
        {
            string system = "TestSystem";
            string fullBodyName = $"{system} 3 a";

            Scan scan = new()
            {
                ScanType = "Detailed",
                PlanetClass = "High Metal Content",
                BodyName = fullBodyName,
                StarSystem = system,
                SurfaceGravity = 3.32f * 9.81f,
                WasDiscovered = false,
                DistanceFromArrivalLS = 234.5,
            };
            ApproachBody ab = new()
            {
                StarSystem = system,
                Body = fullBodyName,
            };

            sutHelm.JournalEvent(scan);
            sutHelm.JournalEvent(ab);

            Assert.AreEqual(1, _core.Notifications.Count);
            Assert.IsTrue(_core.Notifications.Values.First().Detail.Contains("Warning"));
        }
    }
}
