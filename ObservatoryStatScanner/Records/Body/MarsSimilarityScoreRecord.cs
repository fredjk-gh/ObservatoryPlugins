using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class MarsSimilarityScoreRecord : BodySimilarityScoreRecord
    {
        public MarsSimilarityScoreRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData csvData)
            : base(BodyData.MARS, settings, recordKind, csvData)
        {
        }
    }
}
