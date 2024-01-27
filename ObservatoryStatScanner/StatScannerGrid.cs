using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    internal class StatScannerGrid
    {
        [ColumnSuggestedWidth(300)]
        public string Timestamp { get; set; }
        [ColumnSuggestedWidth(350)]
        public string ObjectClass { get; set; }
        [ColumnSuggestedWidth(250)]
        public string Variable { get; set; } // From DisplayName
        [ColumnSuggestedWidth(150)]
        public string Function { get; set; }
        [ColumnSuggestedWidth(350)]
        public string BodyOrItem { get; set; }
        [ColumnSuggestedWidth(200)]
        public string ObservedValue { get; set; }
        [ColumnSuggestedWidth(150)]
        public string Units { get; set; }
        [ColumnSuggestedWidth(150)]
        public string RecordValue { get; set; }
        [ColumnSuggestedWidth(350)]
        public string RecordHolder { get; set; }
        [ColumnSuggestedWidth(250)]
        public string Details { get; set; }
        [ColumnSuggestedWidth(200)]
        public string DiscoveryStatus { get; set; }
        [ColumnSuggestedWidth(150)]
        public string RecordKind { get; set; }
    }
}
