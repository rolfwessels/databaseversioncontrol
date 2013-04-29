using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using log4net;

namespace DatabaseVersionControl.Core.Database
{
    /// <summary>
    /// Used for sql server execution
    /// </summary>
    public class SqlServer : IConnection
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _sqlConnection;
        private readonly string _connectionString;
       private readonly int _commandTimeout;

       public SqlServer(string connectionString, int commandTimeout)
        {
           _connectionString = connectionString;
           _commandTimeout = commandTimeout;
        }

        public event OnSqlExecuteDeligate OnSqlExecute;

        public DataTable ExecuteQuery(string sql, ITransaction transaction)
        {
            return ExecuteQuery(sql, transaction, (x) => { });
        }

        public DataTable ExecuteQuery(string sql, ITransaction transaction, Action<IParameterAdder> paramaterLoader)
        {
            Log.Info("Sql:" + sql);
            InvokeOnSqlExecute(new OnSqlExecuteDeligateArgs(sql));
            if (sql == null) throw new ArgumentNullException("sql");

            var sqlTransactionWrapper = transaction as SqlTransactionWrapper;
            var cmd = new SqlCommand(sql,_sqlConnection,sqlTransactionWrapper != null ? sqlTransactionWrapper.Transaction : null);
            paramaterLoader(new SqlParamAdder(cmd.Parameters));
            var sqlDataAdapter = new SqlDataAdapter(cmd);
            var dtResult = new DataTable();

            sqlDataAdapter.Fill(dtResult);
            sqlDataAdapter.Dispose();
            return dtResult;
        }

        public class SqlParamAdder : IParameterAdder
        {
            private readonly SqlParameterCollection _parameters;

            public SqlParamAdder(SqlParameterCollection parameters)
            {
                _parameters = parameters;
            }

            public void AddWithValue(string name, string value)
            {
                _parameters.AddWithValue(name, value);
            }

            public void AddWithValue(string name, double value)
            {
                _parameters.AddWithValue(name, value);
            }

            public void AddWithValue(string name, DateTime value)
            {
                _parameters.AddWithValue(name, value);
            }
        }


        private void InvokeOnSqlExecute(OnSqlExecuteDeligateArgs args)
        {
            OnSqlExecuteDeligate execute = OnSqlExecute;
            if (execute != null) execute(this, args);
        }


        /// <summary>
        /// Get the current connection
        /// </summary>
        public SqlConnection Connection
        {
            get {
                if (_sqlConnection == null){
                    _sqlConnection = new SqlConnection(_connectionString);
                    _sqlConnection.Open();
                    Log.Info("Connect to " + _connectionString);
                }
                return _sqlConnection; }
        }

        public ITransaction GetTransaction()
        {
            return new SqlTransactionWrapper(Connection.BeginTransaction(IsolationLevel.ReadCommitted));
        }

        public class SqlTransactionWrapper : ITransaction
        {
            private readonly SqlTransaction _transaction;

            public SqlTransactionWrapper(SqlTransaction transaction)
            {
                _transaction = transaction;
            }

            /// <summary>
            /// Rolls back a transaction from a pending state.
            /// </summary>
            /// <exception cref="T:System.Exception">An error occurred while trying to commit the transaction. 
            ///                 </exception><exception cref="T:System.InvalidOperationException">The transaction has already been committed or rolled back.
            ///                     -or- 
            ///                     The connection is broken. 
            ///                 </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
            public void Rollback()
            {
                _transaction.Rollback();
            }

            /// <summary>
            /// Commits the database transaction.
            /// </summary>
            /// <exception cref="T:System.Exception">An error occurred while trying to commit the transaction. 
            ///                 </exception><exception cref="T:System.InvalidOperationException">The transaction has already been committed or rolled back.
            ///                     -or- 
            ///                     The connection is broken. 
            ///                 </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
            public void Commit()
            {
                _transaction.Commit();
            }

            public SqlTransaction Transaction
            {
                get { return _transaction; }
            }

            #region Implementation of IDisposable

            public void Dispose()
            {
                if (_transaction != null){
                    _transaction.Dispose();
                }
            }

            #endregion
        }

        public int ExecuteSql(string sql, ITransaction transaction)
        {
            return ExecuteSql(sql, transaction, (x) => { });
            
        }

        public int ExecuteSql(string sql, ITransaction transaction, Action<IParameterAdder> paramaterLoader)
        {
            Log.Info("Sql:" + sql);
            InvokeOnSqlExecute(new OnSqlExecuteDeligateArgs(sql));
            if (sql == null) throw new ArgumentNullException("sql");
            SqlCommand command = CreateCommand();
            command.CommandText = sql;
            var transWrapper = transaction as SqlTransactionWrapper;
            if (transWrapper != null)
            {
                command.Transaction = transWrapper.Transaction;
            }
            paramaterLoader(new SqlParamAdder(command.Parameters));
            return command.ExecuteNonQuery();
        }

        private SqlCommand CreateCommand()
       {
          var sqlCommand = Connection.CreateCommand();
          sqlCommand.CommandTimeout = _commandTimeout;
          return sqlCommand;
       }

       public void SwitchToDatabase(string dbname, ITransaction transaction)
        {
            ExecuteSql("use " + dbname, transaction);
        }
    }

    public delegate void OnSqlExecuteDeligate(object sender, OnSqlExecuteDeligateArgs args);

    public class OnSqlExecuteDeligateArgs
    {
        private readonly string _sql;

        public OnSqlExecuteDeligateArgs(string sql)
        {
            _sql = sql;
        }

        public string Sql
        {
            get { return _sql; }
        }
    }
}