using System;
using System.Data;

namespace DatabaseVersionControl.Core.Database
{
    /// <summary>
    /// Database interface
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Returns transaction for this database
        /// </summary>
        /// <returns></returns>
        ITransaction GetTransaction();

        /// <summary>
        /// Execute a sql statement
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int ExecuteSql(string sql, ITransaction transaction);

        /// <summary>
        /// Execute a sql statement
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="transaction"></param>
        /// <param name="paramaterLoader"></param>
        /// <returns></returns>
        int ExecuteSql(string sql, ITransaction transaction, Action<IParameterAdder> paramaterLoader);

        /// <summary>
        /// Switch to current database
        /// </summary>
        void SwitchToDatabase(string dbname, ITransaction transaction);

        event OnSqlExecuteDeligate OnSqlExecute;

        /// <summary>
        /// Executes query and returns results
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="transaction"></param>
        DataTable ExecuteQuery(string sql, ITransaction transaction);

        /// <summary>
        /// Allows the execution of  command with parameters
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="transaction"></param>
        /// <param name="paramaterLoader"></param>
        /// <returns></returns>
        DataTable ExecuteQuery(string sql, ITransaction transaction, Action<IParameterAdder> paramaterLoader);
    }
}