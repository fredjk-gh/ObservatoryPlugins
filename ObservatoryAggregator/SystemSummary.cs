using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class SystemSummary
    {
        public SystemSummary(string systemName)
        {
            Name = systemName;
            IsUndiscovered = false;
        }

        public string Name { get; }
        public bool IsUndiscovered { get; set; }
        public FSSDiscoveryScan DiscoveryScan { get; set; }
        public FSSAllBodiesFound AllBodiesFound { get; set; }

    }
}
