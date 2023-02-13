using Observatory.Framework.Files.Journal;

namespace ObservatoryStatScanner.Records
{
    public enum RecordTable
    {
        Stars,
        Planets,
        Rings,
        Unknown,
    };

    enum Function
    {
        Min,
        Max,
        MaxCount,
        MaxSum,
    }

    enum Outcome
    {
        None,
        NearRecord,
        Tie,
        PotentialNew
    }

    internal interface IRecord
    {
        bool Enabled { get; }
        string DisplayName { get; }
        string EDAstroObjectName { get; }
        string JournalObjectName { get; }
        string MaxBody { get; }
        long MaxCount { get; }
        double MaxValue { get; }
        string MinBody { get; }
        long MinCount { get; }
        double MinValue { get; }
        RecordTable Table { get; }
        string VariableName { get; }
        string ValueFormat { get; set; }

        List<StatScannerGrid> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans);
        List<StatScannerGrid> CheckFSSBodySignals(FSSBodySignals bodySignals, FileHeader header);
        List<StatScannerGrid> CheckFSSDiscoveryScan(FSSDiscoveryScan fssDiscoveryScan);
        List<StatScannerGrid> CheckScan(Scan scan);
    }
}