using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using DatabaseVersionControl.Cmd.Templates;
using DatabaseVersionControl.Core;
using DatabaseVersionControl.Core.BusinessObject;
using DatabaseVersionControl.Core.ConfigLoaders;
using DatabaseVersionControl.Core.Database;
using DatabaseVersionControl.Core.Export.BulkExporter;
using DatabaseVersionControl.Core.Export.SqlPubWiz;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Plossum.CommandLine;

namespace DatabaseVersionControl.Cmd
{
    class Program
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        [STAThread]
        private static int Main(string[] args)
        {
            string log4NetFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                              "loggingSettings.xml");
            XmlConfigurator.Configure(new FileInfo(log4NetFile));
            var programParams = new ProgramParams();
            var parser = new CommandLineParser(programParams);
            parser.Parse();

            if (programParams.Help)
            {
                Console.WriteLine(parser.UsageInfo.ToString(78, false));
            }
            else if (parser.HasErrors)
            {
                Console.WriteLine(parser.UsageInfo.ToString(78, true));
                return -1;
            }

            try
            {
                new Program(programParams);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
                Console.Out.WriteLine("ERROR: " + e.Message);
                return -1;
            }
            return 0;
        }


        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private DvcController _controller;
        private ProgramParams AppParam { get; set; }

        private Program(ProgramParams appParams)
        {
            AppParam = appParams;
            Initialize();
        }

        private void Initialize()
        {
            if (AppParam.Verbose)
            {
                var repository = (Hierarchy) LogManager.GetRepository();
                var appender = new ConsoleAppender();
                appender.Layout =
                    new PatternLayout("%date %-5level  [%ndc] - %message%newline");

                repository.Root.AddAppender(appender);
                repository.Configured = true;
                repository.RaiseConfigurationChanged(EventArgs.Empty);
                appender.Threshold = Level.Debug;
            }

            if (AppParam.Export){
                if (string.IsNullOrEmpty(AppParam.ConnectionString)) throw new Exception("Please specify a connection string");
                if (string.IsNullOrEmpty(AppParam.OutputFileName)) throw new Exception("Please specify output file");
                var sqlPubWizProxy = new SqlPubWizProxy(Settings.Default.Executable);

                OutputAndLog(string.Format("Create template file [{0}] ",AppParam.OutputFileName ));
                string exportSchemaFileName = CalculateFileName("_schema.sql");
                
                OutputAndLog(string.Format("Load schema from [{0}] ",AppParam.ConnectionString ));
                if (!AppParam.SkipSchemaExport || !File.Exists(exportSchemaFileName)){
                    using (TextReader textReader = sqlPubWizProxy.GetSchema(AppParam.ConnectionString)){
                        using (var output = File.Open(exportSchemaFileName, FileMode.Create, FileAccess.Write)){
                            var streamWriter = new StreamWriter(output);
                            streamWriter.Write(textReader.ReadToEnd());
                            streamWriter.Flush();
                        }
                    }
                }
                OutputAndLog(string.Format("Load file data [{0}]", AppParam.ConnectionString ));
                var sqlAdditions = new List<string>();
                using (var sr = new StreamReader(File.OpenRead(exportSchemaFileName))){
                    Console.Out.WriteLine("Please select what you would like to do to each tables data");
                    Console.Out.WriteLine("s : Save data in csv export");
                    Console.Out.WriteLine("t : Store csv data in test note");
                    Console.Out.WriteLine("t[0-9] : Save limited records as test data eg. T100 will store 100 records in the test data");
                    Console.Out.WriteLine("m : Display more table information");
                    Console.Out.WriteLine("i : Ignore table data");
                    string[] tables = sqlPubWizProxy.GetListOfTables(sr);
                    IConnection sqlServer = new SqlServer(AppParam.ConnectionString, 30);
                    int index = 3;
                    IOptionReader optionMemoryFile = new OptionMemoryFile(AppParam.ExportSettingsFile);
                    foreach (var table in tables){

                        try{
                            var readExportInput = ReadExportInput(sqlServer, table, index, optionMemoryFile);
                            if (!string.IsNullOrEmpty(readExportInput)){
                                sqlAdditions.Add(readExportInput);
                                index++;
                            }
                        }
                        catch (Exception e){
                            Log.Error("Program:Initialize "+e.Message);
                        }
                    }
                }
                string template = TemplateHelper.GetFileTemplate(exportSchemaFileName, AppParam.ConnectionString, AppParam.OutputFileName, exportSchemaFileName, sqlAdditions.ToArray());
                using (var output = File.Open(AppParam.OutputFileName, FileMode.Create, FileAccess.Write))
                {
                    var streamWriter = new StreamWriter(output);
                    streamWriter.Write(template);
                    streamWriter.Flush();
                }
            }

            if (AppParam.InitializeRollback)
            {
                Log.Info("Initialize rollback");
                DvcController controller = GetController();
                controller.InitializeRollBack();
                
            }

            if (AppParam.Initialize){
                Log.Info("Initialize system");
                DvcController controller = GetController();
                controller.Initialize();
            }
            if (AppParam.LoadTracker)
            {
                Log.Info("Initialize tracker");
                DvcController controller = GetController();
                controller.InitializeTracker();
            }
            
            
            if (AppParam.Rollback > 0)
            {
                Log.Info("Bring database up to date");
                DvcController controller = GetController();
                controller.RollbackIndexToIndex(AppParam.Rollback);
            }


            if (AppParam.RollbackIndex > 0)
            {
                Log.Info(string.Format("Rollback command at index [{0}]", 0));
                DvcController controller = GetController();
                controller.RollbackIndex(AppParam.RollbackIndex);
            }

            if (AppParam.RunIndex > 0)
            {
                Log.Info(string.Format("Run command at index [{0}]", 0));
                DvcController controller = GetController();
                controller.RunIndex(AppParam.RunIndex);
            }

            if (AppParam.Update)
            {
                Log.Info("Bring database up to date");
                DvcController controller = GetController();
                controller.BringUpToDate();
            }

            if (AppParam.SetVersion > 0)
            {
                Log.Info("Set the version");
                DvcController controller = GetController();
                controller.SetProfileVersion(AppParam.SetVersion);
            }


            if (AppParam.Version)
            {
                Log.Info("Get the current repository version");
                DvcController controller = GetController();
                controller.PrintDatabaseVersion(Console.Out);
            }


            
           
        }

        private string CalculateFileName(string filename)
        {
            return Path.Combine(Path.GetDirectoryName(AppParam.OutputFileName),Path.GetFileNameWithoutExtension(AppParam.OutputFileName)+filename);
        }


        private string ReadExportInput(IConnection sqlServer, string table, int index, IOptionReader optionMemoryFile)
        {
           
            
            var key = "TableSelection"+table;
            if (!optionMemoryFile.ContainsOption(key)){
                long count = -1;
                try{
                    DataTable executeQuery = sqlServer.ExecuteQuery("Select count(*) from " + table, null);
                    count = Convert.ToInt64(executeQuery.Rows[0][0]);
                    Console.Out.Write("Table {0} [{1}]", table, count);
                }
                catch (Exception e){
                    Log.Error("Program:ReadExportInput Could not determine table size [" + e.Message + "]");
                    Console.Out.Write("Table {0} ", table);
                }
                
            }
            string readLine = optionMemoryFile.ReadLine(key); 
            if (readLine.ToUpper() == "M"){
                DataTable sampleData = sqlServer.ExecuteQuery("Select top 5 * from " + table, null);
                foreach (DataColumn column in sampleData.Columns){
                    string[] results = GetResults(sampleData, column.ColumnName).Distinct().ToArray();
                    Console.Out.WriteLine("Column : {0} [{1}] ", column.ColumnName, String.Join(",", results));
                }
                return ReadExportInput(sqlServer, table, index, optionMemoryFile);
            }
            if (readLine.ToUpper() == "T")
            {
                optionMemoryFile.SaveValue(key, readLine);
                Console.Out.WriteLine("Saving to test sql");
                IBulkExporterImporter exporter = new SqlServerBulkExporterImporter(sqlServer, null);
                var filenameSql = CalculateFileName("_TD_" + Regex.Replace(table,@"[^A-z^0-9^_]","") + ".csv");
                if (!AppParam.SkipSchemaExport || !File.Exists(filenameSql))
                {
                    exporter.ExportTable(table, filenameSql);
                }
                return TemplateHelper.GetTestDataTemplate(index, table, filenameSql, table);
            }
            if (readLine.ToUpper() == "S")
            {
                optionMemoryFile.SaveValue(key, readLine);
                Console.Out.WriteLine("Saving to command sql");
                IBulkExporterImporter exporter = new SqlServerBulkExporterImporter(sqlServer, null);
                var filenameSql = CalculateFileName("_Data_" + Regex.Replace(table, @"[^A-z^0-9^_]", "") + ".csv");
                if (!AppParam.SkipSchemaExport || !File.Exists(filenameSql))
                {
                    exporter.ExportTable(table, filenameSql);
                }
                return TemplateHelper.GetCommandDataTemplate(index, table, filenameSql,table);
            }
            if (readLine.ToUpper() == "I")
            {
                optionMemoryFile.SaveValue(key, readLine);
                return null;
            }
            return ReadExportInput(sqlServer, table, index, optionMemoryFile);
        }

        private IEnumerable<String> GetResults(DataTable sampleData, string columnName)
        {
            return from DataRow row in sampleData.Rows select row[columnName].ToString();
        }

        private DvcController GetController()
        {
            if (_controller == null){
                var loader = new XmlConfigFileLoader(XmlError,new FileSystemAccess());
                var setup = loader.Load(GetDefaultConfigFile(AppParam.UpdateFileName));
                var profileId = GetValueOrDefault(AppParam.ProfileId, setup.DefaultProfile);
                _controller = new DvcController(setup, profileId, AppParam.IncludeTest);
                _controller.CommitAfterEveryUpdate = AppParam.CommitAfterEveryUpdate;
                if (!AppParam.SuperSilent){
                    _controller.OnUpdateExecute += OutputEvent;
                    if (!AppParam.Silent){
                        
                        _controller.Profile.RepositoryProfile.GetDatabase().OnSqlExecute += OutputSql;
                    }
                }
            }
            return _controller;
        }

        private void OutputSql(object sender, OnSqlExecuteDeligateArgs args)
        {
            Console.Out.WriteLine("Sql: "+args.Sql);
        }

        private void OutputEvent(object sender, OnUpdateExecuteDeligateArgs args)
        {
            Console.Out.WriteLine(string.Format("{3} - {0} [{2} - {1} {4}]", args.UpdatesMetadata.Description,args.UpdatesMetadata.CreateBy,args.UpdatesMetadata.Index, args.Executecommand, args.UpdatesMetadata.CreateDate) );
        }

        private void OutputAndLog(string message)
        {
            Log.Info(message);
            if (!AppParam.Silent){
                Console.Out.WriteLine(message);
            }
        }

        private string GetValueOrDefault(string value, string @default)
        {
            if (!string.IsNullOrEmpty(value)){
                return value;
            }
            return @default;
        }

        private string GetDefaultConfigFile(string name)
        {
            if (string.IsNullOrEmpty(name)){
                name = "dbupdate.xml";
            }
            if (File.Exists(name))
            {
                Log.Info(string.Format("Reading file [{0}]", name));
                return name;
            }
            
            throw new Exception(string.Format("File [{0}] does not exist", name));
        }

        private void XmlError(ValidationEventArgs obj)
        {
            throw new Exception(obj.Message);
        }


        internal class ProgramException : Exception
        {
            public ProgramException()
                : base()
            {
            }

            public ProgramException(string message)
                : base(message)
            {
            }

            public ProgramException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }

        
    }

    
}