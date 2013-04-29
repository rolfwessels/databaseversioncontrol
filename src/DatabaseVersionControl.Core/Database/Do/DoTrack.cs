using System;
namespace Intercontinental.Core.Database.Do
{
    public partial class DoTrack : IDoTrack, IChangedObject
    {
        private bool _changed;
        private String _databaseName;
        private double _version;
        private DateTime _createDate;
        private DateTime _updateDate;


        #region IChangedObject

        public bool IsChanged { get { return _changed; } }

        public void SetChanged(bool inChanged)
        {
            _changed = inChanged;
        }

        #endregion

        public String DatabaseName
        {
            get { return _databaseName; }
            set
            {
                if (!Equals(_databaseName, value))
                {
                    _databaseName = value;
                    _changed = true;
                }
            }
        }
        public double Version
        {
            get { return _version; }
            set
            {
                if (!Equals(_version, value))
                {
                    _version = value;
                    _changed = true;
                }
            }
        }
        public DateTime CreateDate
        {
            get { return _createDate; }
            set
            {
                if (!Equals(_createDate, value))
                {
                    _createDate = value;
                    _changed = true;
                }
            }
        }
        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set
            {
                if (!Equals(_updateDate, value))
                {
                    _updateDate = value;
                    _changed = true;
                }
            }
        }

    }
}
