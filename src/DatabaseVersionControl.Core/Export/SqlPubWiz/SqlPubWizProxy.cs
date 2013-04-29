using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;

namespace DatabaseVersionControl.Core.Export.SqlPubWiz
{
    public class SqlPubWizProxy : ISchemaExtractor
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _executableLocation;

        public SqlPubWizProxy(string executableLocation)
        {
            _executableLocation = executableLocation;
        }

        public TextReader GetSchema(string connectionString)
        {
            string tempFileName = Path.GetFileName(Path.GetTempFileName())+".sql";

            var arguments = string.Format("script -C \"{0}\" \"{1}\" -schemaonly -nodropexisting  -noschemaqualify -f -q", connectionString, tempFileName);
            Log.Info("exe:"+_executableLocation);
            Log.Info("arguments:" + arguments);
            Log.Error("SqlPubWizProxy:GetSchema " + _executableLocation + " " + arguments);
            var p = new System.Diagnostics.Process
                        {
                            StartInfo =
                                {
                                    FileName = _executableLocation,
                                    Arguments = arguments ,
                                    CreateNoWindow = false,
                                    RedirectStandardOutput= true,
                                    UseShellExecute = false,
                                    WindowStyle = ProcessWindowStyle.Hidden
                                },
                                EnableRaisingEvents=false

                        };
            
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            Log.Info("output:" + output);
            return new StreamReader(new TempFile(tempFileName));
        }

        public class TempFile : FileStream , IDisposable
        {
            private readonly string _tempFileName;


            public TempFile(string path) : base(path, FileMode.Open, FileAccess.Read, FileShare.Read)
            {
                _tempFileName = path;
            }

            public void Dispose()
            {
                base.Dispose();
                try{
                    File.Delete(_tempFileName);
                }
                catch{
                    
                }
            }
        }

        public string[] GetListOfTables(TextReader inputFileName)
        {
            var tables = new List<string>();
            string readLine;
            do
            {
                readLine = inputFileName.ReadLine();
                if (readLine != null){
                    Match match = Regex.Match(readLine, @"CREATE TABLE ([\[\]A-z_]*)\(");
                    if (match.Success){
                        tables.Add(match.Groups[1].ToString());
                    }
                }

            } while (readLine != null);
            return tables.ToArray();
        }
    }
}