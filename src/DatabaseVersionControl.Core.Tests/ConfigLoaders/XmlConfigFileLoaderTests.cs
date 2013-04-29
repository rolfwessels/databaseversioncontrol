using System;
using System.IO;
using System.Xml.Schema;
using DatabaseVersionControl.Core.BusinessObject;
using DatabaseVersionControl.Core.ConfigLoaders;
using DatabaseVersionControl.UnitTests.ConfigLoaders;
using NUnit.Framework;

namespace DatabaseVersionControl.Tests.ConfigLoaders
{
	[TestFixture]
	public class XmlConfigFileLoaderTests
	{
	    private XmlConfigFileLoader configFileLoader;

	    #region Setup/Teardown

	    [SetUp]
	    public void Setup()
	    {
            configFileLoader = new XmlConfigFileLoader(Exception, new FileSystemAccess());
	    }

	    [TearDown]
	    public void TearDown()
	    {

	    }

	    #endregion

		[Test]
        public void Load_ReadsSample1_ValidateProfile()
		{
		    DatabaseVersionSetup dvs = null;
		    using (TextReader file = new StreamReader(File.OpenRead(@"Resources\Sample1.xml"))){
		        dvs = configFileLoader.Load(file);
            }
            Assert.That(dvs.Profiles.Count , Is.EqualTo(1));
		    DatabaseVersionSetup.Profile profile = dvs.Profiles[0];
		    Assert.That(profile.Id, Is.EqualTo("Default"));
		    IRepositoryProfile repositoryProfile = profile.RepositoryProfile;
            Assert.That(repositoryProfile,Is.Not.Null);
            Assert.That(repositoryProfile.ConnectionString, Is.EqualTo("data source=Localhost;Integrated Security=True;"));
            Assert.That(repositoryProfile.DatabaseName, Is.EqualTo("DvcTest"));
            Assert.That(repositoryProfile.DatabaseRepository, Is.EqualTo("MainDatabase"));
		    var databaseTrackingTable = repositoryProfile.Tracker as AgnosticDatabaseTrackingTable;
            Assert.That(databaseTrackingTable.TableName, Is.EqualTo("DVC_Tracking"));
            Assert.That(databaseTrackingTable.AutoInitializeNewTracker, Is.True);
		}

	    private void Exception(ValidationEventArgs obj)
	    {
            throw new Exception(obj.Message);
	    }

	    [Test]
        public void Load_ReadsSample1_ValidateRepository()
        {
            DatabaseVersionSetup dvs = null;
            using (TextReader file = new StreamReader(File.OpenRead(@"Resources\Sample1.xml")))
            {
                dvs = configFileLoader.Load(file);
            }
            Assert.That(dvs.Repository.Count, Is.EqualTo(1));
            Assert.That(dvs.Repository[0].Id, Is.EqualTo("MainDatabase"));
            var sqlRunner = dvs.Repository[0].InitialRunner ;
            var runner = sqlRunner.Runner as SqlRunner;
            Assert.That(runner.CommandStatement.SqlValue, Is.EqualTo("create database ${dp.DatabaseName}"), "CommandStatement");
            Assert.That(runner.HasRollback(), Is.True, "HasRollback");
            Assert.That(runner.RollbackStatement.SqlValue, Is.EqualTo("drop database ${dp.DatabaseName}"), "RollbackStatement");
            Assert.That(runner.HasTestData(), Is.False, "HasTestData");

            Assert.That(dvs.Repository[0].Updates.Count, Is.EqualTo(7));
            Assert.That(dvs.Repository[0].Updates[0].Index, Is.EqualTo(2));
            Assert.That(dvs.Repository[0].Updates[0].CreateBy, Is.EqualTo("Rolf"));
            Assert.That(dvs.Repository[0].Updates[0].Description, Is.EqualTo("Create Product table"));
            var sqlRunner1 = dvs.Repository[0].Updates[0].Runner as SqlRunner;
            Assert.That(sqlRunner1.CommandStatement.SqlValue, Is.EqualTo("CREATE TABLE [Product] (\n              [SellableProductID] bigint NOT NULL,\n              [Title] varchar(200) NOT NULL,\n              [ArtistName] varchar(200) NOT NULL,\n              [ImageName] varchar(200) NOT NULL,\n              [Price] Decimal(8,2) NOT NULL,\n              [CreateDate] datetime NOT NULL,\n              [UpdateDate] datetime NOT NULL\n              CONSTRAINT [PK__Product] PRIMARY KEY ([SellableProductID])\n              )\n              ON [PRIMARY]"));
            Assert.That(sqlRunner1.RollbackStatement.SqlValue, Is.EqualTo("drop table Product"));
            Assert.That(sqlRunner1.TestsStatement.SqlValue, Is.EqualTo("insert into Product\n\t\t\t\t\t(SellableProductID, Title, ArtistName, ImageName, Price, CreateDate, UpdateDate) VALUES\n\t\t\t\t\t('4', 'Alpha Beta Gaga - Mark Ronson Remix (Instrumental)', 'Air', '1-0724386120458_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),\n\t\t\t\t\t('14', 'Alpha Beta Gaga (Edit 91)', 'Air', '1-0724386120458_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),\n\t\t\t\t\t('30', 'Le Soleil Est Près De Moi', 'Air', '1-0724386188458_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),\n\t\t\t\t\t('38', 'Give My Love To Rose', 'Johnny Cash', '114-0829410977685_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),\n\t\t\t\t\t('62', 'You''re The Nearest Thing To Heaven', 'Johnny Cash', '114-0829410977685_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),\n\t\t\t\t\t('78', 'Raag Bageshree', 'Hari Prasad Chaurasia', '114-0803680556009_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),\n\t\t\t\t\t('123', 'Raga Hameer', 'Ravi Shankar', '114-0803680583807_L_F_W.jpg', '123.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),\n\t\t\t\t\t('142', 'Raga Hameer', 'Ravi Shankar', '114-0803680583807_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31')"));
            
        }

        [Test]
        public void Load_ReadsSample2_ShouldLoadSqlFromFiles()
        {
            DatabaseVersionSetup dvs = null;
            using (TextReader file = new StreamReader(File.OpenRead(@"Resources\Sample2.xml")))
            {
                dvs = configFileLoader.Load(file);
            }
            Assert.That(dvs.Repository.Count, Is.EqualTo(1));
            Assert.That(dvs.Repository[0].Id, Is.EqualTo("MainDatabase"));
            var sqlRunner = dvs.Repository[0].InitialRunner;
            var runner = sqlRunner.Runner as SqlRunner;
            Assert.That(runner.CommandStatement.SqlValue, Is.EqualTo("create database ${dp.DatabaseName}"), "CommandStatement");
            Assert.That(runner.HasRollback(), Is.True, "HasRollback");
            Assert.That(runner.RollbackStatement.SqlValue, Is.EqualTo("drop database ${dp.DatabaseName}"), "RollbackStatement");
            Assert.That(runner.HasTestData(), Is.False, "HasTestData");

            Assert.That(dvs.Repository[0].Updates.Count, Is.EqualTo(8));
            Assert.That(dvs.Repository[0].Updates[0].Index, Is.EqualTo(2));
            Assert.That(dvs.Repository[0].Updates[0].CreateBy, Is.EqualTo("Rolf"));
            Assert.That(dvs.Repository[0].Updates[0].Description, Is.EqualTo("Create Product table"));


            var sqlRunner1 = dvs.Repository[0].Updates[6].Runner as SqlRunner;
            Assert.That(dvs.Repository[0].Updates[6].Index, Is.EqualTo(8m));
            Assert.That(sqlRunner1.CommandStatement.SqlValue, Is.EqualTo("CREATE TABLE [OrderItem2] (\r\n      [GmvTransactionID] int NOT NULL,\r\n      [SellableProductID] bigint NOT NULL,\r\n      [Title] varchar(100) NOT NULL,\r\n      [ArtistName] varchar(100) NOT NULL,\r\n      [Value] Decimal(8,2) NOT NULL,\r\n      [VoucherCode] varchar(20) NOT NULL,\r\n      [CreateDate] datetime NOT NULL,\r\n      [UpdateDate] datetime NOT NULL\r\n      CONSTRAINT [PK__OrderItem] PRIMARY KEY CLUSTERED ([GmvTransactionID],[SellableProductID])\r\n      )\r\n      ON [PRIMARY]"));
            Assert.That(sqlRunner1.RollbackStatement.SqlValue, Is.EqualTo("drop table OrderItem"));
            Assert.That(sqlRunner1.TestsStatement, Is.Null);


            var sqlRunner2 = dvs.Repository[0].Updates[7].Runner as SqlRunner;
            Assert.That(sqlRunner2.CommandStatement.SqlValue, Is.EqualTo("CREATE TABLE [OrderItem2] (\r\n      [GmvTransactionID] int NOT NULL,\r\n      [SellableProductID] bigint NOT NULL,\r\n      [Title] varchar(100) NOT NULL,\r\n      [ArtistName] varchar(100) NOT NULL,\r\n      [Value] Decimal(8,2) NOT NULL,\r\n      [VoucherCode] varchar(20) NOT NULL,\r\n      [CreateDate] datetime NOT NULL,\r\n      [UpdateDate] datetime NOT NULL\r\n      CONSTRAINT [PK__OrderItem] PRIMARY KEY CLUSTERED ([GmvTransactionID],[SellableProductID])\r\n      )\r\n      ON [PRIMARY]CREATE TABLE [OrderItem2] (\r\n      [GmvTransactionID] int NOT NULL,\r\n      [SellableProductID] bigint NOT NULL,\r\n      [Title] varchar(100) NOT NULL,\r\n      [ArtistName] varchar(100) NOT NULL,\r\n      [Value] Decimal(8,2) NOT NULL,\r\n      [VoucherCode] varchar(20) NOT NULL,\r\n      [CreateDate] datetime NOT NULL,\r\n      [UpdateDate] datetime NOT NULL\r\n      CONSTRAINT [PK__OrderItem] PRIMARY KEY CLUSTERED ([GmvTransactionID],[SellableProductID])\r\n      )\r\n      ON [PRIMARY]"));
            Assert.That(sqlRunner2.RollbackStatement, Is.Null);
            Assert.That(sqlRunner2.TestsStatement, Is.Null);

        }

        [Test]
        public void Load_ReadsSample3_TestingForCsvFiles()
        {
            DatabaseVersionSetup dvs = configFileLoader.Load(@"Resources\Sample3.xml");
            Assert.That(dvs.Profiles.Count,Is.EqualTo(2));
            var sqlRunner = dvs.Repository[0].Updates[0].Runner as SqlRunner;
            Assert.That(sqlRunner,Is.Not.Null);
            Assert.That(sqlRunner.TestsStatement.ToString(), Is.StringStarting("CsvFileLoader FileName: dbupdate_TestData_[Order].csv"));
            Assert.That(sqlRunner.CommandStatement.ToString(), Is.StringStarting("Sql: CREATE TABLE "));
            Assert.That(sqlRunner.RollbackStatement.ToString(), Is.EqualTo("Sql: drop table Product"));
            var csvFilesExecuter = sqlRunner.TestsStatement as CsvFilesExecuter;
            Assert.That(csvFilesExecuter, Is.Not.Null);

            Assert.That(csvFilesExecuter.SqlValue, Is.Not.Null);
            Assert.That(csvFilesExecuter.Table, Is.EqualTo("[Order]"));
            Assert.That(csvFilesExecuter.FileName, Is.StringContaining("Order"));
        }

        [Test]
        public void Load_ReadsSample4_ShouldLoadVariables()
        {
            
            DatabaseVersionSetup dvs = configFileLoader.Load(@"Resources\Sample4.xml");
            
            Assert.That(dvs.Profiles.Count, Is.EqualTo(2));
            Assert.That(dvs.Profiles[0].Properties.Length, Is.EqualTo(0));
            Assert.That(dvs.Profiles[0].RepositoryProfile.CommandTimeout, Is.EqualTo(30));
            Assert.That(dvs.Profiles[1].Properties.Length, Is.EqualTo(2));
            Assert.That(dvs.Profiles[1].RepositoryProfile.CommandTimeout, Is.EqualTo(300));

            Assert.That(dvs.Profiles[1].Properties.Length, Is.EqualTo(2), "invalid value for int dvs.Profiles[1].Properties.Length");
            Assert.That(dvs.Profiles[1].Properties[0].Key, Is.EqualTo("defaultProp3"), "invalid value for dvs.Profiles[1].Properties.SyncRoot[0].Key");
            Assert.That(dvs.Profiles[1].Properties[0].Value, Is.EqualTo("defaultProp1Value3OverRide"), "invalid value for dvs.Profiles[1].Properties.SyncRoot[0].Value");
            Assert.That(dvs.Profiles[1].Properties[1].Key, Is.EqualTo("defaultProp4"), "invalid value for dvs.Profiles[1].Properties.SyncRoot[1].Key");
            Assert.That(dvs.Profiles[1].Properties[1].Value, Is.EqualTo("defaultProp1Value4Duplicate"), "invalid value for dvs.Profiles[1].Properties.SyncRoot[1].Value");

            Assert.That(dvs.Repository.Count, Is.EqualTo(1));
            Assert.That(dvs.Repository[0].Properties.Length, Is.EqualTo(3));
            TestHelper.BuildAssert("dvs.Repository[0].Properties", dvs.Repository[0].Properties);
            Assert.That(dvs.Repository[0].Properties.Length, Is.EqualTo(3), "invalid value for int dvs.Repository[0].Properties.Length");
            Assert.That(dvs.Repository[0].Properties[0].Key, Is.EqualTo("defaultProp1"), "invalid value for dvs.Repository[0].Properties.SyncRoot[0].Key");
            Assert.That(dvs.Repository[0].Properties[0].Value, Is.EqualTo("defaultProp1Value1"), "invalid value for dvs.Repository[0].Properties.SyncRoot[0].Value");
            Assert.That(dvs.Repository[0].Properties[1].Key, Is.EqualTo("defaultProp2"), "invalid value for dvs.Repository[0].Properties.SyncRoot[1].Key");
            Assert.That(dvs.Repository[0].Properties[1].Value, Is.EqualTo("defaultProp1Value2"), "invalid value for dvs.Repository[0].Properties.SyncRoot[1].Value");
            Assert.That(dvs.Repository[0].Properties[2].Key, Is.EqualTo("defaultProp3"), "invalid value for dvs.Repository[0].Properties.SyncRoot[2].Key");
            Assert.That(dvs.Repository[0].Properties[2].Value, Is.EqualTo("defaultProp1Value3Duplicate"), "invalid value for dvs.Repository[0].Properties.SyncRoot[2].Value");

        }

        [Test]
        public void Load_ReadsSample5_ShouldFailBecauseXsdIsInvalid()
        {
            var exception = Assert.Throws<Exception>(() => configFileLoader.Load(@"Resources\Sample5.xml"));
            Assert.That(exception.Message, Is.EqualTo("The 'unknownProperty' attribute is not declared."));
        }


        [Test]
        public void Load_ReadsSample6_ThirdUpdateShouldBeRunOutsideOfATransction()
        {
            
            DatabaseVersionSetup dvs = configFileLoader.Load(@"Resources\Sample6.xml");

            Assert.That(dvs.Profiles.Count, Is.EqualTo(2));
            Assert.That(dvs.Repository[0].Updates[0].SkipTransaction, Is.False);
            Assert.That(dvs.Repository[0].Updates[1].SkipTransaction, Is.True);

        }

        [Test]
        public void Load_ReadsSample7_ShouldBeMysqlServerProfile()
        {
            DatabaseVersionSetup dvs = configFileLoader.Load(@"Resources\Sample7.xml");
            Assert.That(dvs.Profiles.Count, Is.EqualTo(2));
            Assert.That(dvs.Profiles[0].RepositoryProfile, Is.TypeOf<MySqlRepositoryProfile>());
            Assert.That(dvs.Profiles[1].RepositoryProfile, Is.TypeOf<SqlServerDatabaseProfile>());
        }

        [Test]
        public void Load_ReadsSample8_MultipleRepositories()
        {
            DatabaseVersionSetup dvs = configFileLoader.Load(@"Resources\Sample8.xml");
            
            Assert.That(dvs.Profiles.Count, Is.EqualTo(2));
            Assert.That(dvs.Profiles[0].RepositoryProfile, Is.TypeOf<MySqlRepositoryProfile>());
            Assert.That(dvs.Profiles[1].RepositoryProfile, Is.TypeOf<SqlServerDatabaseProfile>());

            Assert.That(dvs.Profiles[0].RepositoryProfile.DatabaseRepository, Is.EqualTo("MainDatabase"));
            Assert.That(dvs.Profiles[1].RepositoryProfile.DatabaseRepository, Is.EqualTo("SecondaryDatabase"));


            Assert.That(dvs.Repository.Count, Is.EqualTo(2));
            Assert.That(dvs.Repository[0].Id, Is.EqualTo("MainDatabase"));
            Assert.That(dvs.Repository[1].Id, Is.EqualTo("SecondaryDatabase"));

        }

        [Test]
        public void Load_ReadsSample9_DuplicateRepositoryNames()
        {
            var exception = Assert.Throws<Exception>(() =>
                                                                      {
                                                                          configFileLoader.Load(@"Resources\Sample9.xml");
                                                                      }
                );
            Assert.That(exception.Message, Is.EqualTo("Duplicate repository ID names (MainDatabase, MainDatabase)"));
        }
	}
}

