using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using log4net;
using MySql.Data.MySqlClient;

namespace DatabaseVersionControl.Core.Database
{
    public class MysqlServer : IConnection
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MySqlConnection _sqlConnection;
        private readonly string _connectionString;
        private readonly int _commandTimeout;

        public MysqlServer(string connectionString, int commandTimeout)
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

            var mySqlTransactionWrapper = transaction as MySqlTransactionWrapper;
            var mySqlCommand = new MySqlCommand(sql, _sqlConnection, mySqlTransactionWrapper != null ? mySqlTransactionWrapper.Transaction : null);
            paramaterLoader(new MysqlParaAdder(mySqlCommand.Parameters));
            var sqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            var dtResult = new DataTable();
            sqlDataAdapter.Fill(dtResult);
            sqlDataAdapter.Dispose();
            return dtResult;
        }


        private void InvokeOnSqlExecute(OnSqlExecuteDeligateArgs args)
        {
            OnSqlExecuteDeligate execute = OnSqlExecute;
            if (execute != null) execute(this, args);
        }


        /// <summary>
        /// Get the current connection
        /// </summary>
        public MySqlConnection Connection
        {
            get
            {
                if (_sqlConnection == null)
                {
                    _sqlConnection = new MySqlConnection(_connectionString);
                    _sqlConnection.Open();
                    Log.Info("Connect to " + _connectionString);
                }
                return _sqlConnection;
            }
        }

        public ITransaction GetTransaction()
        {
            return new MySqlTransactionWrapper(Connection.BeginTransaction(IsolationLevel.ReadCommitted));
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
                if (_transaction != null)
                {
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
            MySqlCommand command = CreateCommand();
            command.CommandText = sql;
            var transWrapper = transaction as MySqlTransactionWrapper;
            if (transWrapper != null)
            {
                command.Transaction = transWrapper.Transaction;
            }
            paramaterLoader(new MysqlParaAdder(command.Parameters));
            return command.ExecuteNonQuery();
        }

        public class MysqlParaAdder : IParameterAdder
        {
            private readonly MySqlParameterCollection _parameters;

            public MysqlParaAdder(MySqlParameterCollection parameters)
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

        private MySqlCommand CreateCommand()
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
}