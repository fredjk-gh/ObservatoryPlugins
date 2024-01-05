using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryPlugins.Tests
{
    internal class KnownCoords
    {
        public string StarSystem { get; set; }

        public (double, double, double) StarPos { get; set; }

        public Int64 SystemAddress { get; set; }
    }
}
