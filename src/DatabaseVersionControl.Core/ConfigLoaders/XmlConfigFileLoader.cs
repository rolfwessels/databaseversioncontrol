using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using DatabaseVersionControl.Core.BusinessObject;
using log4net;

namespace DatabaseVersionControl.Core.ConfigLoaders
{
    public class XmlConfigFileLoader : IConfigFileLoader
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Action<ValidationEventArgs> _exception;
        private readonly IFileSystemAccess _fileSystemAccess;
        private List<string> _possiblePaths;

       

        public XmlConfigFileLoader(Action<ValidationEventArgs> exception, IFileSystemAccess fileSystemAccess)
        {
            _exception = exception;
            _fileSystemAccess = fileSystemAccess;
            _possiblePaths = new List<string> { Path.GetFullPath(".") };
        }

        private void LoadRepositories(DatabaseVersionSetup versionSetup, IEnumerable<XElement> enumerable)
        {
            foreach (var element in enumerable){
                string value = element.Attribute("id").Value;
                var initializeRunners = ReadUpdates(element.Element("initialRunner"));
                XElement properties = element.Element("properties");
                KeyValuePair<string, string>[] props = LoadProperties(properties);
                var databaseRepository = new DatabaseRepository(value, initializeRunners[0]);
                databaseRepository.AddProperties(props);
                databaseRepository.Updates.AddRange(ReadUpdates(element.Element("updates")));
                versionSetup.Repository.Add(databaseRepository);
            }
            //check for duplicate repository ID
            if (versionSetup.Repository.Select(x => x.Id).Distinct().Count() != versionSetup.Repository.Count)
            {
                throw new Exception(string.Format("Duplicate repository ID names ({0})", string.Join(", ", versionSetup.Repository.Select(x => x.Id).ToArray())));
            }
        }

        private string ReadAttribute(XElement element, string name)
        {
            if (element != null){
                var attribute = element.Attribute(name);
                if (attribute != null) return attribute.Value;
            }
            return null;
        }

        private UpdatesMetadata[] ReadUpdates(XNode xNode)
        {
            var runners = new List<UpdatesMetadata>();
            // Add all sql runners 
            foreach (var sqlRunnerXMl in xNode.XPathSelectElements("sqlRunner")){
                var command = sqlRunnerXMl.Element("command");
                var rollback = sqlRunnerXMl.Element("rollback");
                var testData = sqlRunnerXMl.Element("testData");
                var runner = new SqlRunner(GetSqlElement(command),
                                           GetSqlElement(rollback),
                                           GetSqlElement(testData));

                var versions = new UpdatesMetadata(
                    Convert.ToInt32(ReadAttribute(sqlRunnerXMl, "index")),
                    ReadAttribute(sqlRunnerXMl, "createDate"),
                    ReadAttribute(sqlRunnerXMl, "description"),
                    ReadAttribute(sqlRunnerXMl, "createBy"), runner)
                    {
                        SkipTransaction = Convert.ToBoolean(ReadAttribute(sqlRunnerXMl, "skipTransaction") ?? "False")
                    };
                runners.Add(versions);
            }
            
            return runners.ToArray();
        }

        private ISqlExecuter GetSqlElement(XElement statement)
        {
            if (statement != null){
                var sql = statement.Element("sql");
                var csv = statement.Element("csv");
                if (sql != null){
                    var hasFile = sql.Attribute("file") != null && sql.Attribute("file").Value.Length > 0;
                    var hasFiles = sql.Attribute("files") != null && sql.Attribute("files").Value.Length > 0;

                    var hasSql = sql.Value.Trim().Length > 0;
                    var totalInput = ((hasSql ? 1 : 0) + (hasFiles ? 1 : 0) + (hasFile ? 1 : 0));
                    if (totalInput > 1){
                        throw new Exception(
                            "Sql entry can only have either file,files or sql value. Not more than one. [" +
                            statement.ToString() + "]");
                    }
                    if (hasFile){
                        var fileName = sql.Attribute("file").Value;
                        string fullPath;
                        if (!_fileSystemAccess.FileExists(fileName, _possiblePaths.ToArray(), out fullPath))
                        {
                            throw new Exception("Could not find file [" + fileName + "] as required by update!");
                        }
                        Log.Info("Reading file [" + fileName + "]");
                        return new SqlFilesExecuter(new string[] { fullPath });
                    }


                    if (hasFiles){
                        var fileNames = sql.Attribute("files").Value;
                        var strings = fileNames.Split('\\');
                        var pattern = strings[strings.Length - 1];
                        var path = Path.GetDirectoryName(Path.GetFullPath(fileNames.Replace(pattern, "")));
                        Console.Out.WriteLine("Reading files from [" + pattern + "] in [" + path + "] ");
                        var files = Directory.GetFiles(path, pattern);

                        var fileList = new List<string>();
                        foreach (var file in files){
                            fileList.Add(file);
                        }

                        return new SqlFilesExecuter(fileList.ToArray());
                    }
                    else{
                        if (sql != null && !string.IsNullOrEmpty(sql.Value)){
                            return new DefaultSqlStringExecutor(sql.Value.Trim());
                        }
                    }
                }
                else if (csv != null){
                    string fileName = csv.Attribute("datafile").Value;
                    string table = csv.Attribute("table") != null ? csv.Attribute("table").Value : null;
                    if (string.IsNullOrEmpty(table))
                    {
                        throw new Exception("Cannot load the bulk data because 'buildLoadTable' is not specified");
                    }
                    string fullPath;
                    if (!_fileSystemAccess.FileExists(fileName,_possiblePaths.ToArray(),out fullPath))
                    {
                        throw new Exception("Could not find file [" + fileName + "] in [" + string.Join(",",_possiblePaths.ToArray()) + "] as required by update!");
                    }
                    
                    Log.Info("Create bulk load from [" + fileName + "] to table [" + table + "]");
                    return new CsvFilesExecuter(fullPath, table);
                }

                else{
                    throw new Exception("Expect a csv or sql node within the command/testData/rollback node");
                }
            }
            return null;
        }

        private void LoadProfiles(DatabaseVersionSetup versionSetup, IEnumerable<XElement> enumerable)
        {
            foreach (var element in enumerable)
            {
                string value = element.Attribute("id").Value;
                IRepositoryProfile profile = null;
                XElement databaseProfile = element.Element("repositoryProfile");

                KeyValuePair<string,string>[] props;
                if (databaseProfile != null)
                {
                    XElement trackingByTable = databaseProfile.Element("trackingByTable");
                    
                    ITracker databaseTrackingTable = null;
                    if (trackingByTable != null)
                    {
                        databaseTrackingTable = new AgnosticDatabaseTrackingTable(trackingByTable.Attribute("tableName").Value, Convert.ToBoolean(trackingByTable.Attribute("autoCreate").Value));
                    }

                    var databaseProfileType = databaseProfile.Attribute("type").Value;
                    if (databaseProfileType == "sqlServer")
                    {
                       int commandTimeoutValue = 30;
                       var commandTimeoutString = databaseProfile.Attribute("commandTimeout");
                       if(commandTimeoutString != null)
                          commandTimeoutValue = int.Parse(commandTimeoutString.Value);

                        profile = new SqlServerDatabaseProfile(
                                databaseProfile.Attribute("connectionString").Value,
                                databaseProfile.Attribute("databaseName").Value,
                                databaseProfile.Attribute("repositoryID").Value,
                                commandTimeoutValue,
                                databaseTrackingTable);
                    }
                    else if (databaseProfileType == "mySql")
                    {
                        int commandTimeoutValue = 30;
                        var commandTimeoutString = databaseProfile.Attribute("commandTimeout");
                        if (commandTimeoutString != null)
                            commandTimeoutValue = int.Parse(commandTimeoutString.Value);

                        profile = new MySqlRepositoryProfile(
                                databaseProfile.Attribute("connectionString").Value,
                                databaseProfile.Attribute("databaseName").Value,
                                databaseProfile.Attribute("repositoryID").Value,
                                commandTimeoutValue,
                                databaseTrackingTable);
                    }
                    else
                    {
                        throw new Exception(string.Format("Unknown databaseProfile type {0}", databaseProfileType));
                    }
                    XElement properties = databaseProfile.Element("properties");
                    props = LoadProperties(properties);
                }
                else{
                    throw new Exception("repositoryProfile missing from profile");
                }
                var newProfile = new DatabaseVersionSetup.Profile(value,profile);
                if (props != null) newProfile.AddProperties(props);


                versionSetup.Profiles.Add(newProfile);
            }
        }

        private KeyValuePair<string, string>[] LoadProperties(XElement properties)
        {
            var list = new List<KeyValuePair<string, string>>();
            if (properties != null){
                foreach (var xElement in properties.Elements("property"))
                {
                    string propvalue;
                    if (xElement.Attribute("value") != null)
                    {
                        propvalue = xElement.Attribute("value").Value;
                    }
                    else{
                        propvalue = xElement.Value.Trim();
                    }
                    var value = new KeyValuePair<string, string>(xElement.Attribute("name").Value, propvalue);
                    list.Add(value);
                }
            }
            return list.ToArray();
        }

        #region Implementation of IConfigFileLoader

        public DatabaseVersionSetup Load(TextReader reader)
        {
            var versionSetup = new DatabaseVersionSetup();
            
            var shemas = new XmlSchemaSet();
            Assembly assem = typeof(DatabaseVersionSetup).Assembly;
            using (Stream stream = assem.GetManifestResourceStream("DatabaseVersionControl.Core.Xsd.DVCSchema.xsd"))
            {
                if (stream != null){
                    shemas.Add("", XmlReader.Create(stream));
                }
                else{
                    throw new Exception("Could not load embedded resource Xsd.DVCSchema.xsd");
                }
            }
            XDocument element = XDocument.Load(reader);            
            element.Validate(shemas, (sender, e) =>
                                         { if (_exception != null) _exception(e); }, true);
            var profiles = element.XPathSelectElements("//profiles/profile");
            versionSetup.DefaultProfile = element.Root.Attribute("defaultProfile").Value;
            LoadProfiles(versionSetup, profiles);
            var databases = element.XPathSelectElements("//repositories/repository");
            LoadRepositories(versionSetup, databases);
            return versionSetup;
        }

        #endregion

        public DatabaseVersionSetup Load(string filename)
        {
            using (FileStream fileStream = File.OpenRead(filename)){
                _possiblePaths.Add(Path.GetDirectoryName(Path.GetFullPath(filename)));
                using (TextReader file = new StreamReader(fileStream)){
                    return Load(file);
                }
            }
        }
    }

    
}