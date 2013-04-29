using System;
using System.Collections.Generic;
using System.Data;
using Intercontinental.Core.Database.Do;

namespace DatabaseVersionControl.Core.Database.Do
{
   public class DoTrackDataAccess
   {
      #region All Sql

      private readonly string _sqlDelete;
      private readonly string _sqlInsert;
      private readonly string _sqlSelectAll;
      private readonly string _sqlSelectInstance;
      private readonly string _sqlUpdate;

      #endregion

      private readonly Func<IConnection> _sqlConnection;
      private readonly string _tableName;

      public DoTrackDataAccess(Func<IConnection> getSqlConnectionDeligate, string tableName)
      {
         _sqlConnection = getSqlConnectionDeligate;
         _tableName = tableName;
         _sqlDelete = @"
							DELETE FROM " + _tableName + @" 
							WHERE DatabaseName = @DatabaseName";

         _sqlUpdate = @"
						        UPDATE " + _tableName +
                     @" 
						        SET
						        Version = @Version,
				        UpdateDate = @Date	WHERE DatabaseName = @DatabaseName";

         _sqlInsert = @"
						        INSERT INTO " + _tableName +
                     @" 
						        (DatabaseName,Version,CreateDate,UpdateDate)
						        VALUES (@DatabaseName , @Version , @Date , @Date)";

         _sqlSelectInstance = @"
						        SELECT * FROM " + _tableName +
                             @" 
						        WHERE DatabaseName = @DatabaseName";

         _sqlSelectAll = @"
						        SELECT * FROM " + _tableName + @" ";
      }

      public IDoTrack Select(String databaseName, ITransaction transaction)
      {
         
         var command = _sqlConnection().ExecuteQuery(_sqlSelectInstance, transaction, x => x.AddWithValue("@DatabaseName", databaseName));



         if (command.Rows.Count > 0)
             return BuildItem(command.Rows[0]);
         

         return null;
      }

      public IDoTrack[] Select(ITransaction transaction)
      {
          var command = _sqlConnection().ExecuteQuery(_sqlSelectAll, transaction);
         var list = new List<IDoTrack>();
         foreach (DataRow dataRow in command.Rows)
          {
              list.Add(BuildItem(dataRow)); 
          }
             
         return list.ToArray();
      }

      public int Update(IDoTrack value, ITransaction transaction)
      {



          return _sqlConnection().ExecuteSql(_sqlUpdate, transaction, paramater =>
                                                                          {
                                                                              paramater.AddWithValue("@DatabaseName", value.DatabaseName);
                                                                              paramater.AddWithValue("@Version", value.Version);
                                                                              paramater.AddWithValue("@Date",
                                                                                               DateTime.Now);
                                                                          }
                                                                          );
      }



      public int Insert(IDoTrack value, ITransaction transaction)
      {



          return _sqlConnection().ExecuteSql(_sqlInsert, transaction, paramater =>
                                                                          {
                                                                              paramater.AddWithValue("@DatabaseName",
                                                                                                 value.DatabaseName);
                                                                              paramater.AddWithValue("@Version",
                                                                                                 value.Version);
                                                                              paramater.AddWithValue("@Date",
                                                                                                DateTime.Now);
                                                                          }

              );
      }

       public DoTrack BuildItem(DataRow reader)
      {
         var item = new DoTrack
                        {
                           DatabaseName = Convert.ToString(reader["DatabaseName"]) ,
                           Version = Convert.ToDouble(reader["Version"]),
                           CreateDate = Convert.ToDateTime(reader["CreateDate"]),
                           UpdateDate = Convert.ToDateTime(reader["UpdateDate"])
                        };
         item.SetChanged(false);
         return item;
      }

      public void Delete(String databaseName, ITransaction transaction)
      {
         
         _sqlConnection().ExecuteSql(_sqlDelete, transaction, param => param.AddWithValue("@DatabaseName", databaseName));
      }
   }
}