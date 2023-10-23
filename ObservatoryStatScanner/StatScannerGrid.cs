using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner
{
    internal class StatScannerGrid
    {
        public string Timestamp { get; set; }
        public string ObjectClass { get; set; }
        public string Variable { get; set; } // From DisplayName
        public string Function { get; set; }
        public string BodyOrItem { get; set; }
        public string ObservedValue { get; set; }
        public string Units { get; set; }
        public string RecordValue { get; set; }
        public string RecordHolder { get; set; }
        public string Details { get; set; }
        public string DiscoveryStatus { get; set; }
        public string RecordKind { get; set; }
    }
}
