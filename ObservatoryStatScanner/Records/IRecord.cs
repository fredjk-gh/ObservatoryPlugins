using Observatory.Framework.Files.Journal;

namespace ObservatoryStatScanner.Records
{
    public enum RecordTable
    {
        Stars,
        Planets,
        Rings,
        Systems,
        Unknown,
    };

    enum Function
    {
        Min,
        Max,
        MaxCount, // No Min -- would all be 0.
        MaxSum,  // No Min -- would all be 0.
        Count, // Currently used only for header stats.
    }

    enum Outcome
    {
        None,
        NearRecord,
        Tie,
        PotentialNew,
        PersonalNew
    }

    enum RecordKind
    {
        Galactic,
        GalacticProcGen,
        Personal,
    }

    internal interface IRecord
    {
        bool Enabled { get; }
        RecordTable Table { get; }
        RecordKind RecordKind { get; }

        string VariableName { get; }
        string DisplayName { get; }
        string EDAstroObjectName { get; }
        string JournalObjectName { get; }
        string ValueFormat { get; }

        bool HasMax { get; }
        string MaxBody { get; }
        long MaxCount { get; }
        double MaxValue { get; }

        bool HasMin { get; }
        string MinBody { get; }
        long MinCount { get; }
        double MinValue { get; }

        List<StatScannerGrid> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans);
        List<StatScannerGrid> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey);
        List<StatScannerGrid> CheckScan(Scan scan);

        void Reset();
    }
}