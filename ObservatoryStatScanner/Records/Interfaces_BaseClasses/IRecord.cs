using Observatory.Framework.Files.Journal;
using ObservatoryStatScanner.DB;

namespace ObservatoryStatScanner.Records
{
    public enum RecordTable
    {
        Stars,
        Planets,
        Rings,
        Systems,
        Regions,
        Belts,
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
        string Units { get; }

        bool HasMax { get; }
        string MaxHolder { get; }
        long MaxCount { get; }
        double MaxValue { get; }
        Function MaxFunction { get; }

        bool HasMin { get; }
        string MinHolder { get; }
        long MinCount { get; }
        double MinValue { get; }
        Function MinFunction { get; }

        List<StatScannerGrid> Summary();
        List<StatScannerGrid> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans);
        List<StatScannerGrid> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey);
        List<StatScannerGrid> CheckScan(Scan scan, string currentSystem);
        List<StatScannerGrid> CheckCodexEntry(CodexEntry codexEntry);

        void MaybeInitForPersonalBest(PersonalBestManager manager);

        void Reset();
    }
}