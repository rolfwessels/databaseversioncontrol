using System;
using MySql.Data.MySqlClient;

namespace DatabaseVersionControl.Core.Database
{
    public class MySqlTransactionWrapper : ITransaction
    {
        private readonly MySqlTransaction _transaction;

        public MySqlTransactionWrapper(MySqlTransaction transaction)
        {
            _transaction = transaction;
        }

        public MySqlTransaction Transaction
        {
            get { return _transaction; }
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Commit()
        {
            _transaction.Commit();
        }
    }
}