using com.github.fredjk_gh.ObservatoryAggregator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class TrackedData
    {
        public TrackedData() {
            Filters = new();
        }    

        public string CurrentSystem { get; set; }
        public string CurrentCommander { get; set; }
        public List<string> Filters { get; internal set; }

        public void FiltersFromSettings(AggregatorSettings settings)
        {
             Filters = settings.FilterSpec?.Split('|').ToList() ?? new();
        }
    }
}
