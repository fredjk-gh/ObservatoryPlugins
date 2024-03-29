﻿using com.github.fredjk_gh.ObservatoryHelm;
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
                EnableHighGravityAdvisory = true,
                GravityAdvisoryThreshold = 1.5,
                MaxNearbyScoopableDistance = 3000,
            };
            sutHelm = new Helm();
            _core = new Common.TestCore();
            sutHelm.Load(_core);
            sutHelm.Settings = _settings;
        }

        [TestMethod]
        public void TestHelm_ApproachBodyGravityDisabled()
        {
            string system = "TestSystem";
            string fullBodyName = $"{system} 3 a";
            _settings.EnableHighGravityAdvisory = false;

            LoadGame loadGame = new()
            {
                Odyssey = true,
                Commander = "Test_Commander",
                FuelCapacity = 16,
                FuelLevel = 12,
            };
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

            sutHelm.JournalEvent(loadGame);
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

            LoadGame loadGame = new()
            {
                Odyssey = true,
                Commander = "Test_Commander",
                FuelCapacity = 16,
                FuelLevel = 12,
            };
            Scan scan = new()
            {
                ScanType = "Detailed",
                PlanetClass = "High Metal Content",
                BodyName = fullBodyName,
                StarSystem = system,
                SurfaceGravity = 1.72f * 9.81f,
                WasDiscovered = false,
                DistanceFromArrivalLS = 234.5,
            };
            ApproachBody ab = new()
            {
                StarSystem = system,
                Body = fullBodyName,
            };

            sutHelm.JournalEvent(loadGame);
            sutHelm.JournalEvent(scan);
            sutHelm.JournalEvent(ab);

            Assert.AreEqual(1, _core.Notifications.Count);
            Assert.IsTrue(_core.Notifications.Values.First().Detail.Contains("Relatively"));
        }
    }
}
