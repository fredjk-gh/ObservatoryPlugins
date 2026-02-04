using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Exceptions;
using com.github.fredjk_gh.PluginCommon.Utilities;
using LiteDB;
using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryStatScanner.DB
{
    public class PersonalBestManager
    {
        private const string DB_NAME = "StatScanner_PersonalBests";
        internal const string PERSONAL_BEST_DB_FILENAME_TEMPLATE = "{0}" + DB_NAME + "_{1}.db";
        private readonly Action<Exception, string> ErrorLogger;
        private readonly HashSet<PersonalBest> _dirtyObjects = [];
        private readonly string _dbPath;

        private ILiteDatabase PersonalBestsDB;
        private ILiteCollection<PersonalBest> PersonalBestsTable;
#if DEBUG
        private ConnectionMode _connectionMode = ConnectionMode.Shared;
#else
        private ConnectionMode _connectionMode = ConnectionMode.Direct;
#endif

        public PersonalBestManager(string pluginDataPath, Action<Exception, string> errorLogger, string commanderId)
        {
            ErrorLogger = errorLogger;

            _dbPath = string.Format(PERSONAL_BEST_DB_FILENAME_TEMPLATE, pluginDataPath, commanderId);
            Connect(_connectionMode);
            BatchProcessingMode = false;
        }

        public void Connect(ConnectionMode newConnectionMode)
        {
            if (Connected)
            {
                if (newConnectionMode == CurrentConnectionMode) return;

                PersonalBestsTable = null;
                PersonalBestsDB.Dispose();
                PersonalBestsDB = null;
                _connectionMode = newConnectionMode;
            }

            bool hasOpened = false;
            try
            {
                PersonalBestsDB = new LiteDatabase($"Filename={_dbPath};Connection={CurrentConnectionMode.ToString().ToLower()}");
                PersonalBestsDB.Mapper.RegisterType<RecordTable>(
                    (RecordTable e) => e.ToString(),
                    (BsonValue b) => {
                        if (Enum.TryParse<RecordTable>(b?.RawValue?.ToString(), true, out RecordTable e))
                            return e;
                        if (Int32.TryParse(b?.RawValue?.ToString(), out int ordinal))
                            return (RecordTable) ordinal;
                        return RecordTable.Unknown;
                    });
                PersonalBestsTable = PersonalBestsDB.GetCollection<PersonalBest>("personalbests");
                hasOpened = true;

                PersonalBestsTable.EnsureIndex("unique_edobj_variable", "{edobj:$.EDAstroObjectName, variable:$.Variable}", true);
                PersonalBestsTable.EnsureIndex(pb => pb._id, true);
                PersonalBestsTable.EnsureIndex(pb => pb.Table);
                PersonalBestsTable.EnsureIndex(pb => pb.EDAstroObjectName);
                PersonalBestsTable.EnsureIndex(pb => pb.Variable);
            }
            catch (Exception ex)
            {
                ErrorLogger(ex, $"While connecting to {DB_NAME} database (hasOpened? {hasOpened}). If 'hasOpened' is true, this could be a corrupt database or bad index and it may be deleted automatically. Please run restart Observatory and run Read-all!");
                Debug.WriteLine($"Failed to connect to {DB_NAME} database (hasOpened? {hasOpened})! {ex.Message}");

                // Make sure we appear NOT connected later on.
                PersonalBestsTable = null;
                PersonalBestsDB?.Dispose();
                PersonalBestsDB = null;

                if (hasOpened)
                {
                    // Failed creating indexes -- likely the unique ones. Torch the file and request a read-all.
                    // Consider just copying this out to a graveyard for analysis?
                    Debug.WriteLine($"Deleting {_dbPath}... We believe it is corrupted.");
                    File.Delete(_dbPath);
                    throw new DBCorruptedException(DB_NAME, ex);
                }
                else throw new DBNotConnectedException(DB_NAME, ex);
            }
        }

        public bool Connected { get => PersonalBestsDB is not null; }
        public bool BatchProcessingMode { get; set; }
        public ConnectionMode CurrentConnectionMode { get => _connectionMode; }

        public void Load(PersonalBest pbMetadata)
        {
            //if (!Connected) throw DBNotConnectedException()
            // We're in read-all. The DB was cleared. Do nothing to avoid a read from an empty DB.
            // We also defer updates until we're done (working effectively from memory).
            if (BatchProcessingMode) return;

            PersonalBest result = null;
            if (pbMetadata._id > 0)
            {
                result = PersonalBestsTable.FindOne(pb => pb._id == pbMetadata._id);
                if (result == null) ErrorLogger(new Exception($"No record found by ID: {pbMetadata._id}"), "Loading Personal Best by id");
            }
            else
            {
                result = PersonalBestsTable
                    .FindOne(pb => pb.EDAstroObjectName == pbMetadata.EDAstroObjectName
                        && pb.Variable == pbMetadata.Variable);
                // This error log is super noisy.
                // if (result == null) ErrorLogger(new Exception($"No record found by lookup: EDAstroObjectName: {pbMetadata.EDAstroObjectName}; Variable: {pbMetadata.Variable}"), "Loading Personal Best by lookup");
            }
            if (result != null) pbMetadata.MergeFrom(result);
        }

        public void Upsert(PersonalBest updatedData)
        {
            if (BatchProcessingMode)
            {
                _dirtyObjects.Add(updatedData);
            }
            else
            {
                PersonalBestsTable.Upsert(updatedData);
            }
        }

        public void Flush()
        {
            PersonalBestsTable.Upsert(_dirtyObjects);
            _dirtyObjects.Clear();
        }

        public void Clear()
        {
            PersonalBestsTable.DeleteAll();
        }
    }
}
