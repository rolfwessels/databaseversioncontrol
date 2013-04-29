using System;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// Metadata about update
    /// </summary>
    public class UpdatesMetadata : IUpdatesVersions
    {
        readonly double _index;
        readonly string _createDate;
        readonly string _description;
        readonly string _createBy;
        readonly IRunner _runner;
        private bool _skipTransaction;

        /// <summary>
        /// Constructor for the updater
        /// </summary>
        /// <param name="index"></param>
        /// <param name="createDate"></param>
        /// <param name="description"></param>
        /// <param name="createBy"></param>
        /// <param name="runner"></param>
        public UpdatesMetadata(double index, string createDate, string description, string createBy, IRunner runner)
        {
            _index = index;
            _createDate = createDate;
            _description = description;
            _createBy = createBy;
            _runner = runner;
        }

        /// <summary>
        /// Unique decimal value for data
        /// </summary>
        public double Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Data when update was made
        /// </summary>
        public string CreateDate
        {
            get { return _createDate; }
        }

        /// <summary>
        /// Short update description
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Update made by?
        /// </summary>
        public string CreateBy
        {
            get { return _createBy; }
        }

        /// <summary>
        /// The update runner
        /// </summary>
        public IRunner Runner
        {
            get { return _runner; }
        }

        public bool SkipTransaction
        {
            get {
                return _skipTransaction;
            }
            internal set {
                _skipTransaction = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("Index: {0}, Description: {1}, CreateDate: {2}, CreateBy: {3}", Index, Description, CreateDate, CreateBy);
        }
    }
}