using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// Data repository 
    /// </summary>
    public class DatabaseRepository
    {

        private readonly string _id;
        private readonly UpdatesMetadata _initialRunner;
        private readonly List<UpdatesMetadata> _updates;
        private IDictionary<string,string> _properties;

        /// <summary>
        /// Constructor for new repository
        /// </summary>
        public DatabaseRepository(string id, UpdatesMetadata runner)
        {
            _initialRunner = runner;
            _updates = new List<UpdatesMetadata>();
            _properties = new Dictionary<string, string>();
            _id = id;
        }


        /// <summary>
        /// The repository id
        /// </summary>
        public string Id
        {
            get { return _id; }
            
        }

        /// <summary>
        /// Used t
        /// </summary>
        public UpdatesMetadata InitialRunner
        {
            get { return _initialRunner; }
        }


        /// <summary>
        /// List of all update version
        /// </summary>
        public List<UpdatesMetadata> Updates
        {
            get { return _updates; }
        }

        public void AddProperties(KeyValuePair<string, string>[] props)
        {
            foreach (var keyValuePair in props)
            {
                if (!_properties.ContainsKey(keyValuePair.Key))
                {
                    _properties.Add(keyValuePair);
                }
                else
                {
                    _properties[keyValuePair.Key] = keyValuePair.Value;
                }

            }
        }

        public void AddProperties(string key, string value)
        {
            AddProperties(new KeyValuePair<string, string>(key, value));
        }

        private void AddProperties(KeyValuePair<string, string> keyValuePair)
        {
            if (!_properties.ContainsKey(keyValuePair.Key))
            {
                _properties.Add(keyValuePair);
            }
            else
            {
                _properties[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        public KeyValuePair<string, string>[] Properties
        {
            get { return _properties.ToArray(); }
        }
    }
}