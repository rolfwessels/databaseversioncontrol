using System;
using DatabaseVersionControl.Core.Database;
using MySql.Data.MySqlClient;

namespace DatabaseVersionControl.Core.BusinessObject
{
    public class MySqlRepositoryProfile : IRepositoryProfile
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _databaseRepository;
        private readonly ITracker _tracker;
        private readonly int _commandTimeout;
        private MysqlServer _database;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="databaseRepository"></param>
        /// <param name="tracker"></param>
        public MySqlRepositoryProfile(string connectionString, string databaseName, string databaseRepository, ITracker tracker)
            : this(connectionString, databaseName, databaseRepository, 60, tracker)
        {
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="databaseRepository"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="tracker"></param>
        public MySqlRepositoryProfile(string connectionString, string databaseName, string databaseRepository, int commandTimeout, ITracker tracker)
        {
            _connectionString = connectionString;
            _commandTimeout = commandTimeout;
            _databaseName = databaseName;
            _databaseRepository = databaseRepository;
            _tracker = tracker;
        }

        /// <summary>
        /// Returns the database using the connection string
        /// </summary>
        /// <returns></returns>
        public virtual IConnection GetDatabase()
        {
            if (_database == null)
            {
                _database = new MysqlServer(ConnectionString,CommandTimeout);
            }
            return _database;
        }

        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public int CommandTimeout
        {
            get { return _commandTimeout; }
        }


        /// <summary>
        /// Database name
        /// </summary>
        public string DatabaseName
        {
            get { return _databaseName; }
        }


        /// <summary>
        /// Database repository
        /// </summary>
        public string DatabaseRepository
        {
            get { return _databaseRepository; }
        }


        /// <summary>
        /// Return database tracker
        /// </summary>
        public ITracker Tracker
        {
            get { return _tracker; }
        }
    }
}