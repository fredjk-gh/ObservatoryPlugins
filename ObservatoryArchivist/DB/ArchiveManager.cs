using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    enum ConnectionMode
    {
        Shared,
        Direct
    }

    internal class ArchiveManager
    {
        private const string ARCHIVE_DB_FILENAME = "Archivist_Main.db";
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private Action<Exception, string> ErrorLogger;

        private ILiteDatabase ArchivistMainDB;
        private ILiteCollection<VisitedSystem> VisitedSystemsCol;
        string dbPath;
#if DEBUG
        private ConnectionMode connectionMode = ConnectionMode.Direct; //.Shared;
#else
        private ConnectionMode connectionMode = ConnectionMode.Direct;
#endif

        public ArchiveManager(string pluginDataPath, Action<Exception, string> errorLogger)
        {
            ErrorLogger = errorLogger;
            dbPath = $"{pluginDataPath}{ARCHIVE_DB_FILENAME}";

            Connect(connectionMode);
        }

        public void Connect(ConnectionMode newConnectionMode)
        {
            if (Connected)
            {
                if (newConnectionMode != CurrentConnectionMode)
                {
                    VisitedSystemsCol = null;
                    ArchivistMainDB.Dispose();
                    ArchivistMainDB = null;
                }
                else
                {
                    return; // nothing to do -- already connected in the desired mode.
                }
            }

            ArchivistMainDB = new LiteDatabase($"Filename={dbPath};Connection={(CurrentConnectionMode == ConnectionMode.Direct ? "direct" : "shared")}");
            ArchivistMainDB.Pragma("UTC_DATE", true); // required to ensure dates read back from DB are still in UTC (to ensure old/dupe filtering works)

            VisitedSystemsCol = ArchivistMainDB.GetCollection<VisitedSystem>("visitedSystems");
            // https://github.com/mbdavid/LiteDB/issues/739 - fake multi-column unique key.
            VisitedSystemsCol.EnsureIndex("unique_cmdr_sysname", "{cmdr:$.Commander, sysname:$.SystemName}", true);
            VisitedSystemsCol.EnsureIndex(sys => sys.SystemId64);
            VisitedSystemsCol.EnsureIndex(sys => sys.SystemName);
            VisitedSystemsCol.EnsureIndex(sys => sys.FirstVisitDateTime);
            VisitedSystemsCol.EnsureIndex(sys => sys.Commander);
        }

        public bool Connected { get => ArchivistMainDB != null; }

        public ConnectionMode CurrentConnectionMode { get => connectionMode; }

        public int CountSystems(string commanderName = "")
        {
            if (!Connected) throw new DBNotConnectedException();

            if (string.IsNullOrEmpty(commanderName))
            {
                return VisitedSystemsCol.Count();
            }
            else
            {
                return VisitedSystemsCol.Count(sys => sys.Commander == commanderName);
            }
        }

        public List<VisitedSystem> Get(string systemName)
        {
            if (!Connected) throw new DBNotConnectedException();

            return VisitedSystemsCol.Find(sys => sys.SystemName == systemName).ToList();
        }

        public List<VisitedSystem> Get(UInt64 systemId64)
        {
            if (!Connected) throw new DBNotConnectedException();

            return VisitedSystemsCol.Find(sys => sys.SystemId64 == systemId64).ToList();
        }

        public VisitedSystem Get(string systemName, string commanderName)
        {
            if (!Connected) throw new DBNotConnectedException();

            //var bsonKeyDoc = new BsonDocument() { ["cmdr"] = commanderName, ["sysname"] = systemName };
            //var bsonExpr = Query.EQ("unique_cmdr_sysname", bsonKeyDoc);
            //return VisitedSystemsCol.FindOne(bsonExpr);
            return VisitedSystemsCol.FindOne(sys => sys.SystemName == systemName && sys.Commander == commanderName);
        }

        public VisitedSystem Get(UInt64 systemId64, string commanderName)
        {
            if (!Connected) throw new DBNotConnectedException();

            return VisitedSystemsCol.FindOne(sys => sys.SystemId64 == systemId64 && sys.Commander == commanderName);
        }

        public void Upsert(VisitedSystem system)
        {
            if (system.PreambleJournalEntries.Count == 0 || system.SystemJournalEntries.Count == 0)
                return;

            VisitedSystemsCol.Upsert(system);
        }

        public void Clear()
        {
            VisitedSystemsCol.DeleteAll();
        }
    }

    class DBNotConnectedException : Exception
    {
        public DBNotConnectedException() : base("Archivist internal DB is not connected") { }
    }
}
