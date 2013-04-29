using DatabaseVersionControl.Core.Database;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// Default runner interface
    /// </summary>
    public interface IRunner
    {

        /// <summary>
        /// Execute create statement
        /// </summary>
        void ExecuteCommand(DatabaseVersionSetup.Profile profile, ITransaction transaction);

        /// <summary>
        /// Does the runner have a rollback statement
        /// </summary>
        /// <returns></returns>
        bool HasRollback();

        /// <summary>
        /// Execute rollback
        /// </summary>
        void ExecuteRollback(DatabaseVersionSetup.Profile profile, ITransaction transaction);

        /// <summary>
        /// Does the runner have test data statement
        /// </summary>
        /// <returns></returns>
        bool HasTestData();

        /// <summary>
        /// Execute the test data 
        /// </summary>
        void ExecuteTestData(DatabaseVersionSetup.Profile profile, ITransaction transaction);

    }
}