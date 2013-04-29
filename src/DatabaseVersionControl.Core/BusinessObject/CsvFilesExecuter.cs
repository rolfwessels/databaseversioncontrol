using System.IO;
using DatabaseVersionControl.Core.Database;
using DatabaseVersionControl.Core.Export.BulkExporter;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// 
    /// </summary>
    public class CsvFilesExecuter : ISqlExecuter
    {
        private readonly string _fileName;
        private readonly string _table;

        public CsvFilesExecuter(string fileName, string table)
        {
            _fileName = fileName;
            _table = table;
        }

        #region Implementation of ISqlExecuter

        public void ExecuteSql(DatabaseVersionSetup.Profile profile, ITransaction transaction)
        {
            var sqlServerBulkExporterImporter = new SqlServerBulkExporterImporter(profile.RepositoryProfile.GetDatabase(), transaction);
            sqlServerBulkExporterImporter.ImportTableFromFile(_table,_fileName);
        }

        public string SqlValue
        {
            get { return string.Format("csv file [{0}]", _fileName); }
        }


        #endregion

        public string FileName
        {
            get { return _fileName; }
        }

        public string Table
        {
            get { return _table; }
        }

        public override string ToString()
        {
            return string.Format("CsvFileLoader FileName: {0}, Table: {1}", Path.GetFileName(_fileName), _table);
        }
    }
}