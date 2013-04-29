using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using DatabaseVersionControl.Core.Database;
using log4net;

namespace DatabaseVersionControl.Core.Export.BulkExporter
{
    public class SqlServerBulkExporterImporter : IBulkExporterImporter
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SqlServer.SqlTransactionWrapper _transaction;
        private readonly SqlServer _sqlServer;
        private Encoding Encoding;
        private const string Encapsulator = "\"";
        private const string Seperator = ",";
        public SqlServerBulkExporterImporter(IConnection sqlServer, ITransaction transaction)
        {
            _transaction = transaction as SqlServer.SqlTransactionWrapper;
            _sqlServer = sqlServer as SqlServer;
            Encoding = Encoding.UTF8;
        }

        public void ExportTable(string table, string filenameSql)
        {
            using (var fileStream = new FileStream(filenameSql, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding))
                {
                    var cmd = new SqlCommand("select * from " + table, _sqlServer.Connection);
                    using (IDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr != null)
                        {
                            var dateTimeType = typeof(DateTime);
                            bool isFirstLine = true;
                            while (rdr.Read())
                            {
                                if (!isFirstLine) textWriter.WriteLine();
                                for (int i = 0; i < rdr.FieldCount; i++){

                                    if (rdr[i].GetType() != dateTimeType)
                                    {
                                        textWriter.Write((i > 0 ? "," : "") + CsvReader.Csv.Escape(rdr[i].ToString()));
                                    }
                                    else{
                                        var dateTime = (DateTime) rdr[i];
                                        textWriter.Write((i > 0 ? "," : "") + CsvReader.Csv.Escape(dateTime + "." + dateTime.Millisecond));
                                    }
                                }
                                isFirstLine = false;
                            }
                        }
                    }
                }
            }

        }

        public void ImportTableFromFile(string table, string filenameSql)
        {
            using (var fileStream = new FileStream(filenameSql, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding))
                {
                    var externalTransaction = _transaction != null ? _transaction.Transaction : null;
                    var executeQuery = _sqlServer.ExecuteQuery("select top 1 * from " + table, _transaction);
                    using (var bc = new SqlBulkCopy(_sqlServer.Connection, SqlBulkCopyOptions.TableLock, externalTransaction))
                    {

                        bc.BatchSize = 10000;
                        bc.BulkCopyTimeout = 6000; //10 Minutes
                        bc.DestinationTableName = table;
                        
                        IDataReader dt = new CsvDataReader(streamReader, executeQuery.Columns);
                        bc.WriteToServer(dt);
                        bc.Close();
                    }
                }
            }

        }



      

        public class CsvDataReader : IDataReader
        {
            private static readonly ILog Log =
                LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            private readonly StreamReader _stream;
            private readonly DataColumnCollection _columnsByOrdinal;
            private bool _isClosed = true;
            private readonly CsvReader _csvReader;
            private readonly IEnumerator<String[]> _enumerator;

            public CsvDataReader(StreamReader stream, DataColumnCollection columns)
            {
                _stream = stream;
                _csvReader = new CsvReader(_stream);
                _columnsByOrdinal = columns;
                _enumerator = _csvReader.RowEnumerator.GetEnumerator();
            }

            public void Close()
            {
                if (!_isClosed)
                {
                    if (_stream != null) _stream.Close();
                    _isClosed = true;
                }
            }


            /// <summary>
            /// This is the main function that does the work - it reads in the next line of data and parses the values into ordinals.
            /// </summary>
            /// <returns>A value indicating whether the EOF was reached or not.</returns>
            public bool Read()
            {
                if (_stream == null || _stream.EndOfStream) return false;
                _enumerator.MoveNext();
                return true;
            }

            public object GetValue(int i)
            {
                var value = _enumerator.Current[i];


                if (string.IsNullOrEmpty(value) && _columnsByOrdinal[i].AllowDBNull && (_columnsByOrdinal[i].DataType != typeof(string)))
                {
                    
                    return DBNull.Value;
                }
                else if (_columnsByOrdinal[i].DataType == typeof(Guid))
                {
                    return new Guid(value);
                }
                else if (_columnsByOrdinal[i].DataType == typeof(Byte[]))
                {
                    Log.Error("CsvDataReader:GetValue Csv file does not support binary yet");
                    return "";
                }
               

                return value;
            }

            public string GetName(int i)
            {
                return _columnsByOrdinal[i].ColumnName;
            }

            public int GetOrdinal(string name)
            {
                return _columnsByOrdinal[name].Ordinal;
            }

            public int FieldCount
            {
                get { return _columnsByOrdinal.Count; }
            }

            public Type GetFieldType(int i)
            {
                return typeof(int);
            }

            public string GetDataTypeName(int i)
            {
                return GetFieldType(i).ToString();
            }


            #region Non implemented IDataReader


            object IDataRecord.this[int i]
            {
                get { throw new NotImplementedException(); }
            }

            object IDataRecord.this[string name]
            {
                get { throw new NotImplementedException(); }
            }
            public DataTable GetSchemaTable()
            {
                throw new NotImplementedException();
            }

            public bool NextResult()
            {
                throw new NotImplementedException();
            }

            public bool IsDBNull(int i)
            {
                throw new NotImplementedException();
            }


            public int Depth
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsClosed
            {
                get { throw new NotImplementedException(); }
            }

            public int RecordsAffected
            {
                get { throw new NotImplementedException(); }
            }

            



            public int GetValues(object[] values)
            {
                throw new NotImplementedException();
            }



            

            public bool GetBoolean(int i)
            {
                throw new NotImplementedException();
            }

            public byte GetByte(int i)
            {
                throw new NotImplementedException();
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public char GetChar(int i)
            {
                throw new NotImplementedException();
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public Guid GetGuid(int i)
            {
                throw new NotImplementedException();
            }

            public short GetInt16(int i)
            {
                throw new NotImplementedException();
            }

            public int GetInt32(int i)
            {
                throw new NotImplementedException();
            }

            public long GetInt64(int i)
            {
                throw new NotImplementedException();
            }

            public float GetFloat(int i)
            {
                throw new NotImplementedException();
            }

            public double GetDouble(int i)
            {
                throw new NotImplementedException();
            }

            public string GetString(int i)
            {
                throw new NotImplementedException();
            }

            public decimal GetDecimal(int i)
            {
                throw new NotImplementedException();
            }

            public DateTime GetDateTime(int i)
            {
                throw new NotImplementedException();
            }

            public IDataReader GetData(int i)
            {
                throw new NotImplementedException();
            }

            #endregion

            //Other IDataReader methods/properties here, but all throwing not implemented exceptions.

            #region Implementation of IDisposable

            public void Dispose()
            {
                Close();
            }

            #endregion
        }
    }
}