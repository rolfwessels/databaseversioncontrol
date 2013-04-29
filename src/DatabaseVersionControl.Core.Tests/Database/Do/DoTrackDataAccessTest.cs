using System.Data;
using System.Data.SqlClient;
using DatabaseVersionControl.Core;
using DatabaseVersionControl.Core.Database;
using DatabaseVersionControl.Core.Database.Do;
using NUnit.Framework;
using System;
using Intercontinental.Core.Database.Do;

namespace DatabaseVersionControl.Tests.Database.Do {

        [TestFixture, Category("Integration")]
		public class DoTrackDataAccessTest {
			
			private DoTrackDataAccess _doTrackDataAccess;
		    private ITransaction _transaction;
		    private DvcController _controller;
		    private SqlServer _server;

		    #region Setup

            [TestFixtureSetUp]
            public void Setup()
            {
                _controller = IntegrationTests.GetDvcControlller();
                try{
                    _controller.InitializeRollBack();
                }
                catch (Exception e) {
                    Console.Out.WriteLine("no database to rollback," + e.Message);
                    
                }
                _controller.Initialize();
            }


            [TestFixtureTearDown]
            public void TestFixtureTearDown()
            {
                _controller.InitializeRollBack();
            }

		    [SetUp]
            public void SetUp()
            {
		        var database = _controller.Profile.RepositoryProfile.GetDatabase();
		        _server = database as SqlServer;
		        database.SwitchToDatabase(_controller.Profile.RepositoryProfile.DatabaseName,null);
                _doTrackDataAccess = new DoTrackDataAccess(() => database, "_DbTracking");
                _transaction = new MysqlServer.SqlTransactionWrapper(GetConnection().BeginTransaction(IsolationLevel.ReadUncommitted));
			}

		    private SqlConnection GetConnection()
		    {
                return _server.Connection;
		    }
            
		    [TearDown]
            public void TearDown()
            {
                try
                {
                    _transaction.Rollback();
                }
                catch (Exception e){
                    Console.Out.WriteLine("Error:"+e.Message);
                }
            }

            #endregion
			
			#region IntegrationTests

            [Test,Category("Integration")]
            public void Insert_CallInsertStatement_RecordCountValid()
            {
                var count = _doTrackDataAccess.Select(_transaction).Length;
		        IDoTrack value = GetDoTrack(1);
		        _doTrackDataAccess.Insert(value, _transaction);
                Assert.That(_doTrackDataAccess.Select(_transaction).Length, Is.EqualTo(count+1));
			}
			
            [Test, Category("Integration")]
            public void Insert_CallInsertStatement_CheckDatabaseName()
            {
                IDoTrack value = GetDoTrack(1);
                _doTrackDataAccess.Insert(value, _transaction);
                IDoTrack loadValue = _doTrackDataAccess.Select(value.DatabaseName, _transaction);
                Assert.That(loadValue.DatabaseName, Is.EqualTo(value.DatabaseName));
            }
			
            [Test, Category("Integration")]
            public void Insert_CallInsertStatement_CheckIndex()
            {
                IDoTrack value = GetDoTrack(1);
                _doTrackDataAccess.Insert(value, _transaction);
                IDoTrack loadValue = _doTrackDataAccess.Select(value.DatabaseName, _transaction);
                Assert.That(loadValue.Version, Is.EqualTo(value.Version));
            }
			
            
            [Test, Category("Integration")]
            public void Update_CallUpdateStatement_CheckIndex()
            {
                IDoTrack value = GetDoTrack(1);
                _doTrackDataAccess.Insert(value, _transaction);
				value.Version = 8655007;
				_doTrackDataAccess.Update(value, _transaction);
                IDoTrack loadValue = _doTrackDataAccess.Select(value.DatabaseName, _transaction);
                Assert.That(loadValue.Version, Is.EqualTo(value.Version));
            }
								
			[Test, Category("Integration")]
            public void Delete_CallDeleteStatement_Removed()
            {
                IDoTrack value = GetDoTrack(1);
                _doTrackDataAccess.Insert(value, _transaction);
				
				_doTrackDataAccess.Delete(value.DatabaseName, _transaction);
                IDoTrack loadValue = _doTrackDataAccess.Select(value.DatabaseName, _transaction);
                Assert.That(loadValue, Is.Null);
            }

            #endregion
			
			public static IDoTrack GetDoTrack(int count) {
				switch (count) {
					
					default:
					return new DoTrack() {
					DatabaseName = "qbphdxjgzlrywgcuevvvnaqvsdiomlroamaomrnmhzelgurb",
				Version = 627372,
				CreateDate = DateTime.Parse("30/05/2010 16:26"),
				UpdateDate = DateTime.Parse("30/05/2010 16:26"),
	
					};
				}
			}
			
		}
	
}
	
	