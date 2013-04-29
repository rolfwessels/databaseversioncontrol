using DatabaseVersionControl.Core.Database;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// The database profile
    /// </summary>
    public interface IRepositoryProfile
    {
        string ConnectionString { get;  }
        string DatabaseName { get;  }
        string DatabaseRepository { get; }
        ITracker Tracker { get; }
        int CommandTimeout { get; }
        IConnection GetDatabase();

    }
}