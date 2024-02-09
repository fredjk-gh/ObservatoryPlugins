using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    public class AggregatorGrid
    {
        [ColumnSuggestedWidth(200)]
        public string Flags { get; set; }

        [ColumnSuggestedWidth(350)]
        public string Title { get; set; }

        [ColumnSuggestedWidth(500)]
        public string Detail { get; set; }

        [ColumnSuggestedWidth(500)]
        public string ExtendedDetails { get; set; }

        [ColumnSuggestedWidth(250)]
        public string Sender { get; set; }
    }
}
