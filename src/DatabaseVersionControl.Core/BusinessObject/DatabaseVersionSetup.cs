using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// Main setup for database version
    /// </summary>
    public class DatabaseVersionSetup : IProductUpdate
    {
        private readonly List<Profile> _profiles;
        private readonly List<DatabaseRepository> _repository;

        /// <summary>
        /// Constructor for database version setup
        /// </summary>
        public DatabaseVersionSetup()
        {
            _profiles = new List<Profile>();
            _repository = new List<DatabaseRepository>();
        }

        /// <summary>
        /// Contains list of all settings profiles
        /// </summary>
        public List<Profile> Profiles
        {
            get { return _profiles; }
        }

        /// <summary>
        /// Database repository 
        /// </summary>
        public List<DatabaseRepository> Repository
        {
            get { return _repository; }
        }

        /// <summary>
        /// The name of the default profile
        /// </summary>
        public string DefaultProfile { get; set; }


        /// <summary>
        /// Profile data class
        /// </summary>
        public class Profile
        {
            private readonly string _id;
            private readonly IRepositoryProfile _repositoryProfile;
            private readonly IDictionary<string, string> _properties;
            /// <summary>
            /// Constructor with profile settings
            /// </summary>
            public Profile(string id, IRepositoryProfile repositoryProfile)
            {
                _id = id;
                _repositoryProfile = repositoryProfile;
                _properties = new Dictionary<string, string>();
            }

            /// <summary>
            /// Profile Id
            /// </summary>
            public String Id
            {
                get { return _id; }
            }

            /// <summary>
            /// Database profile
            /// </summary>
            public IRepositoryProfile RepositoryProfile
            {
                get { return _repositoryProfile; }
            }

            /// <summary>
            /// Used to replace the variable in the sql statement if it exists
            /// </summary>
            /// <param name="statement"></param>
            /// <returns></returns>
            public string VariableReplace(string statement)
            {
                bool found = false;
                do
                {
                    statement = GetReplaceAll(statement, ref found);
                } while (found);
                return statement;
            }

            private string GetReplaceAll(string statement, ref bool found)
            {
                found = false;
                foreach (var allParam in AllParams){
                    var oldValue = "${" + allParam.Key + "}";
                    if (statement.Contains(oldValue))
                    {
                        statement = statement.Replace(oldValue, allParam.Value);
                        found = true;
                    }                    
                }
                return statement;
            }

            protected IDictionary<String,String> AllParams
            {
                get {
                    var dictionary = _properties.ToDictionary(property => property.Key, property => property.Value);
                    dictionary.Add("dp.DatabaseName", RepositoryProfile.DatabaseName);
                    dictionary.Add("dp.ConnectionString", RepositoryProfile.ConnectionString);
                    dictionary.Add("dp.DatabaseRepository", RepositoryProfile.DatabaseRepository);
                    return dictionary ; }
            }

            internal void AddProperties(KeyValuePair<string, string>[] props)
            {
                foreach (var keyValuePair in props){
                    AddProperties(keyValuePair);
                }
            }

            public void AddProperties(string key, string value)
            {
                AddProperties(new KeyValuePair<string, string>(key, value));
            }

            internal void AddProperties(KeyValuePair<string, string> keyValuePair)
            {
                if (!_properties.ContainsKey(keyValuePair.Key)){
                    _properties.Add(keyValuePair);
                }
                else{
                    _properties[keyValuePair.Key] = keyValuePair.Value;
                }
            }

            public KeyValuePair<string, string>[] Properties
            {
                get { return _properties.ToArray(); }
            }


            internal void AddRepositoryProperties(KeyValuePair<string, string>[] properties)
            {
                foreach (var keyValuePair in properties){
                    if (!_properties.ContainsKey(keyValuePair.Key))
                    {
                        _properties.Add(keyValuePair);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the profile in the profile list with matching ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Profile GetProfile(string id)
        {
            return Profiles.FirstOrDefault(profile => profile.Id == id);
        }

        /// <summary>
        /// Looks up the repository id in repository list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DatabaseRepository GetRepository(string id)
        {
            return Repository.FirstOrDefault(databaseRepository => databaseRepository.Id == id);
        }
    }
}