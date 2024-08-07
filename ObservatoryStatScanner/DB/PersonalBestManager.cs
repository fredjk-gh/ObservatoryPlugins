﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.DB
{
    public class PersonalBestManager
    {
        internal const string PERSONAL_BEST_DB_FILENAME_TEMPLATE = "{0}StatScanner_PersonalBests_{1}.db";
        private ILiteDatabase PersonalBestsDB;
        private ILiteCollection<PersonalBest> PersonalBestsCol;
        private Action<Exception, string> ErrorLogger;

        private HashSet<PersonalBest> _dirtyObjects = new();

        public PersonalBestManager(string pluginDataPath, Action<Exception, string> errorLogger, string commanderId)
        {
            ErrorLogger = errorLogger;

            string dbPath = string.Format(PERSONAL_BEST_DB_FILENAME_TEMPLATE, pluginDataPath, commanderId);

            PersonalBestsDB = new LiteDatabase($"Filename={dbPath};Connection=direct"); // Direct for performance
            PersonalBestsCol = PersonalBestsDB.GetCollection<PersonalBest>("personalbests");
            PersonalBestsCol.EnsureIndex(pb => pb._id);
            PersonalBestsCol.EnsureIndex(pb => pb.Table);
            PersonalBestsCol.EnsureIndex(pb => pb.EDAstroObjectName);
            PersonalBestsCol.EnsureIndex(pb => pb.Variable);
        }

        public bool BatchProcessingMode { get; set; }

        public void Load(PersonalBest pbMetadata)
        {
            // We're in read-all. The DB was cleared. Do nothing to avoid a read from an empty DB.
            // We also defer updates until we're done (working effectively from memory).
            if (BatchProcessingMode) return;

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
            if (BatchProcessingMode)
            {
                _dirtyObjects.Add(updatedData);
            }
            else
            {
                PersonalBestsCol.Upsert(updatedData);
            }
        }

        public void Flush()
        {
            PersonalBestsCol.Upsert(_dirtyObjects);
            _dirtyObjects.Clear();
        }

        public void Clear()
        {
            PersonalBestsCol.DeleteAll();
        }
    }
}
