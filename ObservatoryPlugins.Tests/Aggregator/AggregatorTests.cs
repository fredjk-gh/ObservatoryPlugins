using com.github.fredjk_gh.ObservatoryAggregator;
using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;

namespace com.github.fredjk_gh.ObservatoryPlugins.Tests
{
    [TestClass]
    public class AggregatorTests
    {
        Aggregator sutAggregator;
        Common.TestCore _core;
        AggregatorSettings _settings;

        [TestInitialize]
        public void Setup()
        {
            _settings = new()
            {
                ShowCurrentSystemOnly = true,
                FilterSpec = "",
            };
            sutAggregator = new Aggregator();
            _core = new Common.TestCore();
            sutAggregator.Load(_core);
            sutAggregator.Settings = _settings;
        }

        [TestMethod]
        public void TestNotificationFiltering_Match()
        {
            _settings.FilterSpec = "Minimum Distance Reached";
            NotificationArgs args = new NotificationArgs()
            {
                Title = "Minimum Distance Reached",
                Detail = "You've travelled over 200 metres from previous sample."
            };

            sutAggregator.OnNotificationEvent(args);

            Assert.AreEqual(0, _core.gridItems.Count);
        }

        [TestMethod]
        public void TestNotificationFiltering_MultipleFiltersOnDifferentFieldsMatch()
        {
            _settings.FilterSpec = "Minimum Distance Reached|TestWorker|junk";
            sutAggregator.Settings = _settings;

            List<NotificationArgs> notifications = new()
            {
                new NotificationArgs()
                {
                    Title = "Minimum Distance Reached",
                    Detail = "This should not be aggregated."
                },
                new NotificationArgs()
                {
                    Title = "Sender name is filtered",
                    Detail = "This should not be aggregated.",
#if EXTENDED_EVENT_ARGS
                    Sender = new Common.TestWorker(),
#endif
                },
                new NotificationArgs()
                {
                    Title = "Suppressed",
                    Detail = "This should not be aggregated because the word JUNK is filtered.",
                },
                new NotificationArgs()
                {
                    Title = "Suppressed",
                    Detail = "This should not be aggregated.",
#if EXTENDED_EVENT_ARGS
                    ExtendedDetails = "This notification is also Junk.",
#endif
                },
            };

            foreach(var notification in notifications)
            {
                sutAggregator.OnNotificationEvent(notification);

                Assert.AreEqual(0, _core.gridItems.Count);
            }
        }

        [TestMethod]
        public void TestNotificationFiltering_MatchAfterSettingsChanged()
        {
            NotificationArgs args = new NotificationArgs()
            {
                Title = "Minimum Distance Reached",
                Detail = "You've travelled over 200 metres from previous sample."
            };

            sutAggregator.OnNotificationEvent(args);

            Assert.AreEqual(1, _core.gridItems.Count);

            // Change the settings so as to filter the previous event and and re-send it. This verifies that
            // changed settings are ingested correctly after the filter is updated.
            _settings.FilterSpec = "Minimum Distance Reached";

            sutAggregator.OnNotificationEvent(args);

            // No *additional* item added -- this is the same item added in response to the first event.
            Assert.AreEqual(1, _core.gridItems.Count);
        }

        [TestMethod]
        public void TestNotificationFiltering_NonMatch()
        {
            _settings.FilterSpec = "Nonmatch";
            NotificationArgs args = new NotificationArgs()
            {
                Title = "Minimum Distance Reached",
                Detail = "You've travelled over 200 metres from previous sample."
            };

            sutAggregator.OnNotificationEvent(args);

            Assert.AreEqual(1, _core.gridItems.Count);
        }

        [TestMethod]
        public void TestJournalEvents_Jumps_CurrentSystemChanges()
        {
            List<FSDJump> jumps = new()
            {
                new FSDJump()
                {
                    StarSystem = "FSD Jump",
                },
                new CarrierJump()
                {
                    Docked = true,
                    StarSystem = "Docked Carrier Jump",
                },
#if EXTENDED_EVENT_ARGS
                new CarrierJump()
                {
                    OnFoot = true,
                    StarSystem = "OnFoot Carrier Jump",
                },
#endif
            };

            foreach (var jump in jumps)
            {
                sutAggregator.JournalEvent(jump);

                Assert.AreEqual(jump.StarSystem, sutAggregator.data.CurrentSystem);
                Assert.AreEqual(1, _core.gridItems.Count); // Header row updated with new system.
                Assert.AreEqual(jump.StarSystem, ((AggregatorNotificationGrid)_core.gridItems[0]).System);
            }
        }

        [TestMethod]
        public void TestJournalEvents_Location_CurrentSystemChanges()
        {
            Location location = new() { StarSystem = "Location change " };

            sutAggregator.JournalEvent(location);

            Assert.AreEqual(location.StarSystem, sutAggregator.data.CurrentSystem);
            Assert.AreEqual(1, _core.gridItems.Count); // Header row updated with new system.
            Assert.AreEqual(location.StarSystem, ((AggregatorNotificationGrid)_core.gridItems[0]).System);
        }
    }
}