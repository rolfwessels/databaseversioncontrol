using System;
using DatabaseVersionControl.Core.Database;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// The sql runner will contain the command, rollback and tests statements
    /// </summary>
    public class SqlRunner : IRunner
    {
        private readonly ISqlExecuter _commandStatement;
        private readonly ISqlExecuter _rollbackStatement;
        private readonly ISqlExecuter _testsStatements;

        /// <summary>
        /// Constructor with only the create statement
        /// </summary>
        /// <param name="createStatement"></param>
        public SqlRunner(string createStatement) : this(createStatement, "")
        {
        }

        /// <summary>
        /// constructor with command and rollback statement
        /// </summary>
        /// <param name="createStatement"></param>
        /// <param name="rollbackStatement"></param>
        public SqlRunner(string createStatement, string rollbackStatement)
            : this(createStatement, rollbackStatement, "")
        {
        }

        /// <summary>
        /// Constructor with IRunnerDistribute objects as input
        /// </summary>
        /// <param name="createStatement"></param>
        /// <param name="rollbackStatement"></param>
        /// <param name="testsStatements"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SqlRunner(ISqlExecuter createStatement, ISqlExecuter rollbackStatement, ISqlExecuter testsStatements)
        {
            if (createStatement == null) throw new ArgumentNullException("createStatement");
            _commandStatement = createStatement;
            _rollbackStatement = rollbackStatement;
            _testsStatements = testsStatements;
        }

        /// <summary>
        /// Constructor with string values only. They will all be converted to <see cref="DefaultSqlStringExecutor"/> runners
        /// </summary>
        /// <param name="createStatement"></param>
        /// <param name="rollbackStatement"></param>
        /// <param name="testsStatements"></param>
        public SqlRunner(string createStatement, string rollbackStatement, string testsStatements)
        {
            if (createStatement == null) throw new ArgumentNullException("createStatement");
            if (!string.IsNullOrEmpty(createStatement))
                _commandStatement = new DefaultSqlStringExecutor(createStatement);
            if (!string.IsNullOrEmpty(rollbackStatement))
                _rollbackStatement = new DefaultSqlStringExecutor(rollbackStatement);
            if (!string.IsNullOrEmpty(testsStatements))
                _testsStatements = new DefaultSqlStringExecutor(testsStatements);
        }

        #region Implementation of IRunner

        /// <summary>
        /// Execute create statement
        /// </summary>
        public void ExecuteCommand(DatabaseVersionSetup.Profile profile, ITransaction transaction)
        {
            _commandStatement.ExecuteSql(profile, transaction);
        }


        /// <summary>
        /// Does the runner have a rollback statement
        /// </summary>
        /// <returns></returns>
        public bool HasRollback()
        {
            return _rollbackStatement != null;
        }

        /// <summary>
        /// Execute rollback
        /// </summary>
        public void ExecuteRollback(DatabaseVersionSetup.Profile profile, ITransaction transaction)
        {
            _rollbackStatement.ExecuteSql(profile, transaction);
        }

        /// <summary>
        /// Does the runner have test data statement
        /// </summary>
        /// <returns></returns>
        public bool HasTestData()
        {
            return _testsStatements != null;
        }

        /// <summary>
        /// Execute the test data 
        /// </summary>
        public void ExecuteTestData(DatabaseVersionSetup.Profile profile, ITransaction transaction)
        {
            _testsStatements.ExecuteSql(profile, transaction);
        }

        #endregion

        /// <summary>
        /// The command statement
        /// </summary>
        public ISqlExecuter CommandStatement
        {
            get
            {
                return _commandStatement;
            }
        }

        /// <summary>
        /// The rollback statement
        /// </summary>
        public ISqlExecuter RollbackStatement
        {
            get
            {
                return _rollbackStatement;
            }
        }

        /// <summary>
        /// The test statement in SQL
        /// </summary>
        public ISqlExecuter TestsStatement
        {
            get
            {
                return _testsStatements;
            }
        }
    }
}