using DatabaseVersionControl.Core.Database;

namespace DatabaseVersionControl.Core.BusinessObject
{
    class DefaultSqlStringExecutor : ISqlExecuter
    {
        private readonly string _sql;

        public DefaultSqlStringExecutor(string sql)
        {
            _sql = sql;
        }

        #region Implementation of ISqlExecuter

        public void ExecuteSql(DatabaseVersionSetup.Profile profile, ITransaction transaction)
        {
            IConnection connection = profile.RepositoryProfile.GetDatabase();
            connection.ExecuteSql(profile.VariableReplace(_sql), transaction);
        }

        public string SqlValue
        {
            get { return _sql; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Sql: {0}", _sql);
        }
    }
}