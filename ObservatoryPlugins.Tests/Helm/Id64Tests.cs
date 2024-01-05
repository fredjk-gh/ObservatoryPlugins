using com.github.fredjk_gh.ObservatoryHelm.Id64;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace com.github.fredjk_gh.ObservatoryPlugins.Tests
{
    [TestClass]
    public class Id64Tests
    {
        [TestInitialize]
        public void Setup()
        {

        }

        [TestMethod]
        public void TestDistance_Id64Estimate()
        {
            foreach (KnownDistance dist in Id64Data.KNOWN_DISTANCES)
            {
                KnownCoords system1 = Id64Data.KNOWN_SYSTEMS[dist.SystemName1];
                KnownCoords system2 = Id64Data.KNOWN_SYSTEMS[dist.SystemName2];

                double distancePos = Id64CoordHelper.Distance(system1.StarPos, system2.StarPos);
                double distanceId64 = Id64CoordHelper.Distance(system1.SystemAddress, system2.SystemAddress);

                Debug.WriteLine($"Distances between {system1.StarSystem} and {system2.StarSystem} (known, starpos, id64): {dist.Distance:n2}, {distancePos:n2}, {distanceId64:n2}");

                // Check: actual - err <= estimated <= actual + err
                // ... where err: +/- 0.1 (for StarPos-based distances)
                // ... where err: +/- ??? (for Id64-based distances)
                double err = 0.1;
                Assert.IsTrue(distancePos <= dist.Distance + err && distancePos >= dist.Distance - err);
                // err = ???
                // Assert.IsTrue(distanceId64 <= dist.Distance + err && distanceId64 >= dist.Distance - err);
            }
        }

        [TestMethod]
        public void TestEstimatedCoords()
        {
            foreach (KnownCoords kc in Id64Data.KNOWN_SYSTEMS.Values)
            {
                Id64Coords estimated = Id64CoordHelper.EstimatedCoords(kc.SystemAddress);

                Debug.WriteLine($"Results for system: {kc.StarSystem} with address: {kc.SystemAddress}:");
                Debug.WriteLine($"Actual Coords: ({kc.StarPos.Item1}, {kc.StarPos.Item2}, {kc.StarPos.Item3})");
                Debug.WriteLine($"Estimated coords: ({estimated.x}, {estimated.y}, {estimated.z}, +/- {estimated.ErrLy} ly)");

                // Check: actual - err <= estimated <= actual + err
                Assert.IsTrue(estimated.x <= kc.StarPos.Item1 + estimated.ErrLy && estimated.x >= kc.StarPos.Item1 - estimated.ErrLy);
                Assert.IsTrue(estimated.y <= kc.StarPos.Item2 + estimated.ErrLy && estimated.y >= kc.StarPos.Item2 - estimated.ErrLy);
                Assert.IsTrue(estimated.z <= kc.StarPos.Item3 + estimated.ErrLy && estimated.z >= kc.StarPos.Item3 - estimated.ErrLy);
            }
        }
    }
}
