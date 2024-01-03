using com.github.fredjk_gh.ObservatoryAggregator;
using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Interfaces;

namespace com.github.fredjk_gh.ObservatoryPlugins.Tests
{
    [TestClass]
    public class AggregatorTests
    {
        Aggregator aggregator;
        Common.TestCore core;
        AggregatorSettings settings;

        public AggregatorTests()
        {
            aggregator = new Aggregator();
            core = new Common.TestCore();
            aggregator.Load(core);
        }

        [TestMethod]
        public void TestNotificationFiltering_Match()
        {
            settings = new AggregatorSettings()
            {
                ShowCurrentSystemOnly = true,
                FilterSpec = "Minimum Distance Reached",
            };
            aggregator.Settings = settings;
            NotificationArgs args = new NotificationArgs()
            {
                Title = "Minimum Distance Reached",
                Detail = "You've travelled over 200 metres from previous sample."
            };

            aggregator.OnNotificationEvent(args);

            Assert.AreEqual(0, core.gridItems.Count);
        }

        [TestMethod]
        public void TestNotificationFiltering_NonMatch()
        {
            settings = new AggregatorSettings()
            {
                ShowCurrentSystemOnly = true,
                FilterSpec = "Nonmatch",
            };
            aggregator.Settings = settings;
            NotificationArgs args = new NotificationArgs()
            {
                Title = "Minimum Distance Reached",
                Detail = "You've travelled over 200 metres from previous sample."
            };

            aggregator.OnNotificationEvent(args);

            Assert.AreEqual(1, core.gridItems.Count);
        }
    }
}