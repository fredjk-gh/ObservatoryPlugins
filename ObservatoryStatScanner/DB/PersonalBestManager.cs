using LiteDB;
using ObservatoryStatScanner.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.DB
{
    public class PersonalBestManager
    {
        private const string PERSONAL_BEST_DB_FILENAME = "StatScanner_PersonalBests.db";
        private ILiteDatabase PersonalBestsDB;
        private ILiteCollection<PersonalBest> PersonalBestsCol;
        private Action<Exception, string> ErrorLogger;

        public PersonalBestManager(string pluginDataPath, Action<Exception, string> errorLogger)
        {
            ErrorLogger = errorLogger;

            string dbPath = $"{pluginDataPath}{PERSONAL_BEST_DB_FILENAME}";

            PersonalBestsDB = new LiteDatabase($"Filename={dbPath};Connection=shared"); // Should be direct?
            PersonalBestsCol = PersonalBestsDB.GetCollection<PersonalBest>("personalbests");
            PersonalBestsCol.EnsureIndex(pb => pb._id);
            PersonalBestsCol.EnsureIndex(pb => pb.Table);
            PersonalBestsCol.EnsureIndex(pb => pb.EDAstroObjectName);
            PersonalBestsCol.EnsureIndex(pb => pb.Variable);
        }

        public void Load(PersonalBest pbMetadata)
        {
            PersonalBest result = null;
            if (pbMetadata._id > 0)
            {
                result = PersonalBestsCol.FindOne(pb => pb._id == pbMetadata._id);
                if (result == null) ErrorLogger(new Exception($"No record found by ID: {pbMetadata._id}"), "Loading Personal Best by id");
            }
            else
            {
                result = PersonalBestsCol
                    .FindOne(pb => pb.Table == pbMetadata.Table
                        && pb.EDAstroObjectName == pbMetadata.EDAstroObjectName
                        && pb.Variable == pbMetadata.Variable);
                // This error log is super noisy.
                // if (result == null) ErrorLogger(new Exception($"No record found by lookup: Table: {pbMetadata.Table}; EDAstroObjectName: {pbMetadata.EDAstroObjectName}; Variable: {pbMetadata.Variable}"), "Loading Personal Best by lookup");
            }
            if (result != null) pbMetadata.MergeFrom(result);
        }

        public void Upsert(PersonalBest updatedData)
        {
            PersonalBestsCol.Upsert(updatedData);
        }

        public void Clear()
        {
            PersonalBestsCol.DeleteAll();
        }

        public void RewriteAll(List<PersonalBestData> updatedRecords)
        {
            // Clear
            Clear();

            // Re-write everything.
            foreach (PersonalBestData data in updatedRecords)
            {
                data.Save();
            }
        }
    }
}
