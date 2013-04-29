using System;
using System.Data.SqlClient;
using System.Reflection;
using DatabaseVersionControl.Core.Database;
using DatabaseVersionControl.Core.Database.Do;
using Intercontinental.Core.Database.Do;
using log4net;
using MySql.Data.MySqlClient;

namespace DatabaseVersionControl.Core.BusinessObject
{
    public class AgnosticDatabaseTrackingTable : ITracker
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _tableName;
        private readonly bool _autoIntializeNewTracker;

        /// <summary>
        /// SQL string place holder
        /// </summary>
        public const string TablePlaceHolder = @"${ITracker.TableName}";

        /// <summary>
        /// Initialize with table name and setting to auto create
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="autoCreate"></param>
        public AgnosticDatabaseTrackingTable(string tableName, bool autoCreate)
        {
            _tableName = tableName;
            _autoIntializeNewTracker = autoCreate;
        }

        /// <summary>
        /// Table name to use
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
        }

        /// <summary>
        /// Check if this tracker should be initialize
        /// </summary>
        public bool AutoInitializeNewTracker
        {
            get { return _autoIntializeNewTracker; }
        }

        /// <summary>
        /// Sets the version number
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="version"></param>
        /// <param name="transaction"></param>
        public void SetVersion(DatabaseVersionSetup.Profile profile, double version, ITransaction transaction)
        {
            IConnection connection = profile.RepositoryProfile.GetDatabase();
            var access = new DoTrackDataAccess(() => connection, TableName);
            var track = new DoTrack { DatabaseName = profile.RepositoryProfile.DatabaseRepository, Version = version };
            try{
                access.Insert(track, transaction);
            }
            catch (Exception){
                access.Update(track, transaction);
            }
            
        }

        /// <summary>
        /// Used to get the current profile version
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="transaction"></param>
        public double GetVersion(DatabaseVersionSetup.Profile profile, ITransaction transaction)
        {
            IConnection connection = profile.RepositoryProfile.GetDatabase();

            var access = new DoTrackDataAccess(() => connection, TableName);
            IDoTrack doTrack = access.Select(profile.RepositoryProfile.DatabaseRepository, transaction);
            if (doTrack != null) return doTrack.Version;
            //
            // todo: Rolf remove this as some stage 
            //
            Log.Info("Tracker not found on repository name. Older version used database name. Trying on database name");
            doTrack = access.Select(profile.RepositoryProfile.DatabaseName, transaction);
            if (doTrack != null){
                Log.Info("Tracker found. Renaming to DatabaseRepository name");
                access.Delete(profile.RepositoryProfile.DatabaseName, transaction);
                doTrack.DatabaseName = profile.RepositoryProfile.DatabaseRepository;
                access.Insert(doTrack, transaction);
                return doTrack.Version;
            }
            
            return -1;
        }

        protected string MysqSqlCreateTracker = @"CREATE TABLE " + TablePlaceHolder + @" (
            `DatabaseName` varchar(100) NOT NULL,
            `Version` decimal(18, 3) NOT NULL,
            `CreateDate` datetime NOT NULL,
            `UpdateDate` datetime NOT NULL,
            PRIMARY KEY (`DatabaseName`)
            )";

        protected string SqlServerCreateTracker = @"CREATE TABLE " + TablePlaceHolder + @" (
            [DatabaseName] varchar(100) NOT NULL,
            [Version] decimal(18, 3) NOT NULL,
            [CreateDate] datetime NOT NULL,
            [UpdateDate] datetime NOT NULL,
            CONSTRAINT [PK_" + TablePlaceHolder + @"]
            PRIMARY KEY (DatabaseName))";
        

        #region Implementation of ITracker

        /// <summary>
        /// Will initialize on initial setup
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="transaction"></param>
        public void InitializeNewTracker(DatabaseVersionSetup.Profile profile, ITransaction transaction)
        {
            string replace = profile.VariableReplace(profile.RepositoryProfile is SqlServerDatabaseProfile? SqlServerCreateTracker : MysqSqlCreateTracker);
            replace = replace.Replace(TablePlaceHolder, TableName);
            profile.RepositoryProfile.GetDatabase().ExecuteSql(replace, transaction);
        }

        

        #endregion
    }
}