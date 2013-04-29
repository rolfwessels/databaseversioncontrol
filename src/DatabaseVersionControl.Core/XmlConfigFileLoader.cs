using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using DatabaseVersionControl.Core.BusinessObject;


namespace DatabaseVersionControl.Core
{
    public class XmlConfigFileLoader : IConfigFileLoader
    {
        public void Load(TextReader reader)
        {

            var versionSetup = new DatabaseVersionSetup();
                var profiles = XElement.Load(reader).XPathSelectElements("/Profiles/Profile");
                LoadProfiles(versionSetup, profiles);
                var databases = XElement.Load(reader).XPathSelectElements("/Databases/Database");
                LoadDatabases(versionSetup, databases);
        }

        private void LoadDatabases(DatabaseVersionSetup versionSetup, IEnumerable<XElement> enumerable)
        {
            foreach (var element in enumerable)
            {
                var db = new DatabaseVersionSetup.Database();
                //reader

                // versionSetup.Databases.Add(db);
            }

        }

        private void LoadProfiles(DatabaseVersionSetup versionSetup, IEnumerable<XElement> enumerable)
        {
            foreach (var element in enumerable)
            {
                var profile = new DatabaseVersionSetup.Profile()
                                  {
                                      Id = (string) element.Attribute("Id"),
                                      ConnectionString = element.Element("ConnectionString").Value,
                                      Tracking = LoadTracking(element.Element("Tracking"))
                                  };

                versionSetup.Profiles.Add(profile);
            }
        }

        private ITracker LoadTracking(XElement element)
        {
            ITracker value = null;
            if (element.Attribute("Style").Value == "DatabaseTable")
            {
                value = new DatabaseTracker(element.Attribute("TableName").Value, Convert.ToBoolean(element.Attribute("AutoCreate").Value));
                
            }
            return value;
        }

        public void Save(DatabaseVersion inst)
        {
            using (FileStream fs = new FileStream("some.xml", FileMode.Create))
            {
                new XmlSerializer(typeof(DatabaseVersion)).Serialize(fs, inst);
            }
        }
    }

    internal class DatabaseTracker : ITracker
    {
        private readonly string _tableName;
        private readonly bool _autocreate;

        public DatabaseTracker(string tableName, bool autocreate)
        {
            _tableName = tableName;
            _autocreate = autocreate;
        }



        public string TableName
        {
            get { return _tableName; }
        }

        public bool Autocreate
        {
            get { return _autocreate; }
        }
    }
}