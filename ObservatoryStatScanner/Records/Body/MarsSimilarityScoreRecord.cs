using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class MarsSimilarityScoreRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData csvData)
        : BodySimilarityScoreRecord(BodyData.MARS, settings, recordKind, csvData)
    { }
}
