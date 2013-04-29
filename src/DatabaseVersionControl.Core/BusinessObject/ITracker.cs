using DatabaseVersionControl.Core.Database;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// Interface for all trackers
    /// </summary>
    public interface ITracker
    {
        /// <summary>
        /// Will initialize on initial setup
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="transaction"></param>
        void InitializeNewTracker(DatabaseVersionSetup.Profile profile, ITransaction transaction);

        /// <summary>
        /// Check if this tracker should be initialize
        /// </summary>
        bool AutoInitializeNewTracker { get; }

        /// <summary>
        /// Sets the version number
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="version"></param>
        void SetVersion(DatabaseVersionSetup.Profile profile, double version, ITransaction transaction);

        /// <summary>
        /// Used to get the current profile version
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="transaction"></param>
        double GetVersion(DatabaseVersionSetup.Profile profile, ITransaction transaction);
    }
}