namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// Metadata about update
    /// </summary>
    public interface IUpdatesVersions
    {
        /// <summary>
        /// Unique decimal value for data
        /// </summary>
        double Index { get; }

        /// <summary>
        /// Data when update was made
        /// </summary>
        string CreateDate { get; }

        /// <summary>
        /// Short update description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Update made by?
        /// </summary>
        string CreateBy { get; }

        /// <summary>
        /// The update runner
        /// </summary>
        IRunner Runner { get; }

        /// <summary>
        /// Ensures that any commands in this command does not get run in a transaction
        /// </summary>
        bool SkipTransaction { get; }
    }
}