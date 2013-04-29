using System;
using System.IO;
using DatabaseVersionControl.Core;
using DatabaseVersionControl.Core.BusinessObject;
using NUnit.Framework;

namespace DatabaseVersionControl.Tests
{
    [TestFixture, Category("Integration")]
    public class IntegrationTests
    {
        private DvcController _controller;

        [TestFixtureSetUp]
        public void Setup()
        {
            
        }


        [TestFixtureTearDown]
        public void TearDown()
        {
            _controller.InitializeRollBack();
            
        }

        


        [Test, Category("Integration")]
        public void Initialize_InitializetheDatabase_ConnectsToSqlServerAndCreatesDb()
        {
            try
            {
                _controller = new DvcController(GetVersionSetup(), "Default", true);
                _controller.Profile.RepositoryProfile.GetDatabase().OnSqlExecute += (o, e) => Console.WriteLine("Sql:" + e.Sql);
                Console.Out.WriteLine("InitializeRollBack");
                _controller.InitializeRollBack();
            }
            catch
            {
            }
            Console.Out.WriteLine("Initialize");
            _controller.Initialize();
            Console.Out.WriteLine("BringUp2Date");
            _controller.BringUpToDate();
            var stringWriter = new StringWriter();
            _controller.PrintDatabaseVersion(stringWriter);
            Assert.That(stringWriter.ToString(), Is.StringContaining("IntegrationTestForDvc : 2"));
            
        }


        [Test, Category("Integration")]
        public void Initialize_InitializetheDatabase_ConnectToMysqlAndCreateDatabase()
        {
            _controller = new DvcController(GetVersionSetup(), "Mysql", true);
            _controller.Profile.RepositoryProfile.GetDatabase().OnSqlExecute += (o, e) => Console.WriteLine("Sql:" + e.Sql);
            try
            {
                
                Console.Out.WriteLine("InitializeRollBack");
                _controller.InitializeRollBack();
            }
            catch
            {
            }
            Console.Out.WriteLine("Initialize");
            _controller.Initialize();
            Console.Out.WriteLine("BringUp2Date");
            _controller.BringUpToDate();
            var stringWriter = new StringWriter();
            _controller.PrintDatabaseVersion(stringWriter);
            Assert.That(stringWriter.ToString(), Is.StringContaining("IntegrationTestForDvc : 2"));

        }

        public static DatabaseVersionSetup GetVersionSetup()
        {
            var versionSetup = new DatabaseVersionSetup();
            var profile = new DatabaseVersionSetup.Profile("Default",
                                                        new SqlServerDatabaseProfile(
                                                            "Data Source=zino;User Id=rolf;Password=Password123;"
                                                            , "IntegrationTestForDvc"
                                                            ,"SqlServerTests"
                                                            ,60
                                                            ,new AgnosticDatabaseTrackingTable("_DbTracking", true)
                                                        ));
                              
            versionSetup.Profiles.Add(profile);

            var mysqlProfile = new DatabaseVersionSetup.Profile("Mysql",
                                                        new MySqlRepositoryProfile(
                                                            "Server=192.168.0.48;Uid=dvc;Pwd=S7v7rs5XfadHXQrn;"
                                                            , "IntegrationTestForDvc"
                                                            , "MySqlServerTests"
                                                            , 60
                                                            , new AgnosticDatabaseTrackingTable("_DbTracking", true)
                                                        ));

            versionSetup.Profiles.Add(mysqlProfile);
            
            //sql server
            versionSetup.Repository.Add(GetSqlServerRepository());
            versionSetup.Repository.Add(GetMySqlServerRepository());


          
            return versionSetup;
        }

        private static DatabaseRepository GetSqlServerRepository()
        {
            var runner = new SqlRunner("CREATE DATABASE ${dp.DatabaseName};", "USE Master; DROP DATABASE ${dp.DatabaseName};");
            var repository = new DatabaseRepository("SqlServerTests", new UpdatesMetadata(1, "Todau", "Initialize", "rolf", runner));

            var versions = new UpdatesMetadata(1, "2010-04-16", "First update", "Rolf Wessels",
                                               new SqlRunner(@"CREATE TABLE OrderItem (
            [GmvTransactionID] int NOT NULL,
            [SellableProductID] bigint NOT NULL,
            [Title] varchar(100) NOT NULL,
            [ArtistName] varchar(100) NOT NULL,
            [Value] decimal(8, 2) NOT NULL,
            [VoucherID] int NOT NULL,
            [CreateDate] datetime NOT NULL,
            [UpdateDate] datetime NOT NULL,
            CONSTRAINT [PK__OrderItem]
            PRIMARY KEY CLUSTERED ([GmvTransactionID] ASC, [SellableProductID] ASC))", "DROP TABLE OrderItem;"));
            repository.Updates.Add(versions);


            repository.Updates.Add(new UpdatesMetadata(2, "2010-04-16", "First update", "Rolf Wessels",
                                                       new SqlRunner(new SqlFilesExecuter(new string[] { @"resources\Sample_Schema.sql" }), null, null)));
            return repository;
        }

        private static DatabaseRepository GetMySqlServerRepository()
        {
            var runner = new SqlRunner("CREATE DATABASE ${dp.DatabaseName};", "DROP DATABASE ${dp.DatabaseName};");
            var repository = new DatabaseRepository("MySqlServerTests", new UpdatesMetadata(1, "Todau", "Initialize", "rolf", runner));
            
            var versions = new UpdatesMetadata(1, "2010-04-16", "First update", "Rolf Wessels",
                                               new SqlRunner(@"CREATE TABLE `icr_finalise_key` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `key_id` int(11) NOT NULL DEFAULT '0',
  `name` varchar(25) DEFAULT NULL,
  `type` enum('Cash','Cheque','EFT','Account','Hotel XFer') DEFAULT 'Cash',
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_number` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`till_id`,`key_id`,`create_date`),
  KEY `till_id` (`till_id`,`key_id`,`create_date`,`update_date`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;", "DROP TABLE icr_finalise_key;"));
            repository.Updates.Add(versions);

            
            repository.Updates.Add(new UpdatesMetadata(2, "2010-04-16", "First update", "Rolf Wessels",
                                                       new SqlRunner(new SqlFilesExecuter(new string[] { @"resources\Mysql_Schema.sql" }), null, null)));
            return repository;
        }

        public static DvcController GetDvcControlller()
        {
            return new DvcController(GetVersionSetup(), "Default", true);
        }
    }

    
}