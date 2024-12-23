using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class EarthSimilarityScoreRecord : BodySimilarityScoreRecord
    {
        public EarthSimilarityScoreRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData csvData)
            : base(BodyData.EARTH, settings, recordKind, csvData)
        { }
    }
}
