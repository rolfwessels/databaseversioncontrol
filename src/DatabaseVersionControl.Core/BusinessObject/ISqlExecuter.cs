using System.Data.SqlClient;
using DatabaseVersionControl.Core.Database;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// Interface for executing and reporting sql values
    /// </summary>
    public interface ISqlExecuter
    {
        /// <summary>
        /// Execute the sql command
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="transaction"></param>
        void ExecuteSql(DatabaseVersionSetup.Profile profile, ITransaction transaction);

        /// <summary>
        /// Log out sql value
        /// </summary>
        string SqlValue { get;  }
    }
}