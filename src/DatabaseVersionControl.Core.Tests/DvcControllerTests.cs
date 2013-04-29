using System;
using System.IO;
using DatabaseVersionControl.Core;
using DatabaseVersionControl.Core.BusinessObject;
using DatabaseVersionControl.Core.Database;
using Moq;
using NUnit.Framework;

namespace DatabaseVersionControl.Tests
{
    [TestFixture]
    public class DvcControllerTests
    {
        private const string DefaultProfile = "Default";
        private DvcController _dvcController;
        private DatabaseVersionSetup _databaseVersionSetup;
        private Mock<IRepositoryProfile> _mockIRepositoryProfile;
        private Mock<IRunner> _mockIRunner;
        private Mock<ITracker> _mockITracker;
        private Mock<IConnection> _mockIDatabase;
        private Mock<ITransaction> _mockITransaction;
        private static Mock<SqlServerDatabaseProfile> mockSqlServerDatabaseProfile;

        [SetUp]
        public void Setup()
        {
            _databaseVersionSetup = new DatabaseVersionSetup();
            _mockIRepositoryProfile = new Mock<IRepositoryProfile>(MockBehavior.Strict);
            _databaseVersionSetup.DefaultProfile = DefaultProfile;
            var profile = new DatabaseVersionSetup.Profile(DefaultProfile, _mockIRepositoryProfile.Object);
            _mockITracker = new Mock<ITracker>(MockBehavior.Strict);
            _mockIDatabase = new Mock<IConnection>(MockBehavior.Strict);
            _mockITransaction = new Mock<ITransaction>(MockBehavior.Strict);
            _databaseVersionSetup.Profiles.Add(profile);
            _mockIRunner = new Mock<IRunner>(MockBehavior.Strict);
            var runner = new UpdatesMetadata(1, DateTime.MinValue.ToShortDateString(), "FirstUpdate", "Rolf", _mockIRunner.Object);
            _databaseVersionSetup.Repository.Add(new DatabaseRepository("Repo",runner));
            _mockIRepositoryProfile.Setup(mc => mc.DatabaseRepository)
                .Returns("Repo");
            _dvcController = new DvcController(_databaseVersionSetup, DefaultProfile, true);
        }

        [TearDown]
        public void TearDown()
        {
            _mockITracker.VerifyAll();
            _mockIRunner.VerifyAll();
            _mockIRepositoryProfile.VerifyAll();
            _mockIDatabase.VerifyAll();
            _mockITransaction.VerifyAll();
        }

        [Test]
        public void Initialize_ShouldCallVariouseThings_DoesNotCallTrackerInitialize()
        {
            
            _mockIRunner.Setup(mc => mc.ExecuteCommand(_databaseVersionSetup.GetProfile(DefaultProfile), null));
            _mockIRepositoryProfile.Setup(mc => mc.DatabaseRepository)
                .Returns("Repo");
            _mockIRepositoryProfile.Setup(mc => mc.Tracker)
                .Returns(_mockITracker.Object);
            _mockITracker.Setup(mc => mc.AutoInitializeNewTracker)
                .Returns(false);
            _dvcController.Initialize();
            

        }

        [Test]
        public void Initialize_ShouldCallVariouseThings_DoesCallTrackerInitialize()
        {
            _mockIRepositoryProfile.Setup(mc => mc.DatabaseRepository)
                .Returns("Repo");
            var profile = _databaseVersionSetup.GetProfile(DefaultProfile);
            _mockIRunner.Setup(mc => mc.ExecuteCommand(profile, null));
            _mockIRepositoryProfile.Setup(mc => mc.Tracker)
                .Returns(_mockITracker.Object);
            _mockITracker.Setup(mc => mc.AutoInitializeNewTracker)
                .Returns(true);

            _mockIRepositoryProfile.Setup(mc => mc.DatabaseName)
                .Returns("Test");
            _mockIRepositoryProfile.Setup(mc => mc.GetDatabase())
                .Returns(_mockIDatabase.Object);
            _mockIDatabase.Setup(mc => mc.GetTransaction())
                .Returns(_mockITransaction.Object);
            _mockIDatabase.Setup(mc => mc.SwitchToDatabase("Test", _mockITransaction.Object));
            _mockITransaction.Setup(mc => mc.Dispose());
            _mockITracker.Setup(mc => mc.InitializeNewTracker(profile, _mockITransaction.Object));
            _mockITracker.Setup(mc => mc.SetVersion(profile,0 ,_mockITransaction.Object));
            _mockITransaction.Setup(mc => mc.Commit());
            _dvcController.Initialize();
        }

        [Test]
        public void InitializeRollBack_ShouldRollbackTheInitializeScript_Done()
        {
            _mockIRepositoryProfile.Setup(mc => mc.DatabaseRepository)
                .Returns("Repo");
            _mockIRunner.Setup(mc => mc.HasRollback()).Returns(true);
            _mockIRunner.Setup(mc => mc.ExecuteRollback(_databaseVersionSetup.GetProfile(DefaultProfile), null));
            _dvcController.InitializeRollBack();
        }

        [Test]
        public void InitializeRollBack_ShouldRollbackTheInitializeScript_NoRollBack()
        {
            _mockIRepositoryProfile.Setup(mc => mc.DatabaseRepository)
                .Returns("Repo");
            _mockIRunner.Setup(mc => mc.HasRollback()).Returns(false);
            
            _dvcController.InitializeRollBack();
        }

        [Test]
        public void BringUpToDate_RunFullUpdate_UntilLastCall()
        {
            var databaseVersionSetup = GetVersionSetup();
            var mockIDatabase = new Mock<IConnection>(MockBehavior.Strict);
            var mockITransaction = new Mock<ITransaction>(MockBehavior.Strict);
            mockIDatabase.Setup(mc => mc.GetTransaction()).Returns(mockITransaction.Object);
            mockIDatabase.Setup(mc => mc.SwitchToDatabase("Intercontinental4", mockITransaction.Object));
            _mockITracker.Setup(mc => mc.GetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), mockITransaction.Object)).Returns(0);
            //mockIDatabase.Setup(mc => mc.ExecuteSql("valie ${test0} ${test1} ${test2} ${test3} ${test4} $test ", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql(It.IsAny<string>(), mockITransaction.Object)).Returns(1);
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 2, mockITransaction.Object));
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 3, mockITransaction.Object));
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 4, mockITransaction.Object));
            mockITransaction.Setup(mc => mc.Commit());
            mockITransaction.Setup(mc => mc.Dispose());
            mockSqlServerDatabaseProfile.Setup(mc => mc.GetDatabase()).Returns(mockIDatabase.Object);
            
            //act
            _dvcController = new DvcController(databaseVersionSetup, DefaultProfile, true);      
            _dvcController.BringUpToDate();
            mockSqlServerDatabaseProfile.VerifyAll();
            mockIDatabase.VerifyAll();
            mockITransaction.VerifyAll();
            _mockITracker.VerifyAll();
        }

        [Test]
        public void InitializeRollBack_InitializeCreate_ContainsVariableReplaces()
        {
            var databaseVersionSetup = GetVersionSetup();
            var mockIDatabase = new Mock<IConnection>(MockBehavior.Strict);
            var mockITransaction = new Mock<ITransaction>(MockBehavior.Strict);
            mockIDatabase.Setup(mc => mc.ExecuteSql("USE Master; DROP DATABASE Intercontinental4;", null)).Returns(1);
            mockSqlServerDatabaseProfile.Setup(mc => mc.GetDatabase()).Returns(mockIDatabase.Object);

            //act
            _dvcController = new DvcController(databaseVersionSetup, DefaultProfile, true);
            _dvcController.InitializeRollBack();
            mockSqlServerDatabaseProfile.VerifyAll();
            mockIDatabase.VerifyAll();
            mockITransaction.VerifyAll();
            _mockITracker.VerifyAll();
        }

        [Test]
        public void Initialize_InitializeCreate_ContainsVariableReplaces()
        {
            var databaseVersionSetup = GetVersionSetup();
            var mockIDatabase = new Mock<IConnection>(MockBehavior.Strict);
            var mockITransaction = new Mock<ITransaction>(MockBehavior.Strict);
            mockIDatabase.Setup(mc => mc.ExecuteSql("CREATE DATABASE Intercontinental4;", null)).Returns(1);
            //mockIDatabase.Setup(mc => mc.ExecuteSql(It.IsAny<string>(), mockITransaction.Object)).Returns(1).Callback((string o, ITransaction t) => Console.WriteLine(o));
            _mockITracker.Setup(mc => mc.AutoInitializeNewTracker).Returns(false);
            mockSqlServerDatabaseProfile.Setup(mc => mc.GetDatabase()).Returns(mockIDatabase.Object);

            //act
            _dvcController = new DvcController(databaseVersionSetup, DefaultProfile, true);
            _dvcController.Initialize();
            mockSqlServerDatabaseProfile.VerifyAll();
            mockIDatabase.VerifyAll();
            mockITransaction.VerifyAll();
            _mockITracker.VerifyAll();
        }


        [Test]
        public void BringUpToDate_RunFullUpdate_ValuesAreReplaced()
        {
            var databaseVersionSetup = GetVersionSetup();
            var mockIDatabase = new Mock<IConnection>(MockBehavior.Strict);
            var mockITransaction = new Mock<ITransaction>(MockBehavior.Strict);
            mockIDatabase.Setup(mc => mc.GetTransaction()).Returns(mockITransaction.Object);
            mockIDatabase.Setup(mc => mc.SwitchToDatabase("Intercontinental4", mockITransaction.Object));
            _mockITracker.Setup(mc => mc.GetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), mockITransaction.Object)).Returns(0);
            mockIDatabase.Setup(mc => mc.ExecuteSql("Create2 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql("Create3 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql("Create4 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 2, mockITransaction.Object));
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 3, mockITransaction.Object));
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 4, mockITransaction.Object));
            mockITransaction.Setup(mc => mc.Commit());
            mockITransaction.Setup(mc => mc.Dispose());
            mockSqlServerDatabaseProfile.Setup(mc => mc.GetDatabase()).Returns(mockIDatabase.Object);

            //act
            _dvcController = new DvcController(databaseVersionSetup, DefaultProfile, false);
            _dvcController.OnUpdateExecute += ((o, a) => Console.WriteLine(a.UpdatesMetadata.Description));
            _dvcController.BringUpToDate();
            mockSqlServerDatabaseProfile.VerifyAll();
            mockIDatabase.VerifyAll();
            mockITransaction.VerifyAll();
            _mockITracker.VerifyAll();
            
        }

        [Test]
        public void BringUpToDate_RunFullUpdate_ValuesAreReplacedAlsoCheckTestData()
        {
            var databaseVersionSetup = GetVersionSetup();
            var mockIDatabase = new Mock<IConnection>(MockBehavior.Strict);
            var mockITransaction = new Mock<ITransaction>(MockBehavior.Strict);
            mockIDatabase.Setup(mc => mc.GetTransaction()).Returns(mockITransaction.Object);
            mockIDatabase.Setup(mc => mc.SwitchToDatabase("Intercontinental4", mockITransaction.Object));
            _mockITracker.Setup(mc => mc.GetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), mockITransaction.Object)).Returns(0);
            mockIDatabase.Setup(mc => mc.ExecuteSql("Create2 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql("TestData2 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql("Create3 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql("TestData3 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql("Create4 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql("TestData4 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 2, mockITransaction.Object));
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 3, mockITransaction.Object));
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 4, mockITransaction.Object));
            mockITransaction.Setup(mc => mc.Commit());
            mockITransaction.Setup(mc => mc.Dispose());
            mockSqlServerDatabaseProfile.Setup(mc => mc.GetDatabase()).Returns(mockIDatabase.Object);

            //act
            _dvcController = new DvcController(databaseVersionSetup, DefaultProfile, true);
            _dvcController.BringUpToDate();
            mockSqlServerDatabaseProfile.VerifyAll();
            mockIDatabase.VerifyAll();
            mockITransaction.VerifyAll();
            _mockITracker.VerifyAll();

        }

        [Test]
        public void RollbackIndexToIndex_RunFullUpdate_ValuesAreReplacedRoleBack()
        {
            var databaseVersionSetup = GetVersionSetup();
            var mockIDatabase = new Mock<IConnection>(MockBehavior.Strict);
            var mockITransaction = new Mock<ITransaction>(MockBehavior.Strict);
            mockIDatabase.Setup(mc => mc.GetTransaction()).Returns(mockITransaction.Object);
            mockIDatabase.Setup(mc => mc.SwitchToDatabase("Intercontinental4", mockITransaction.Object));
            _mockITracker.Setup(mc => mc.GetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), mockITransaction.Object)).Returns(4);
            mockIDatabase.Setup(mc => mc.ExecuteSql("RoleBack4 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql("RoleBack3 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql("RoleBack2 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 1, mockITransaction.Object));
            mockITransaction.Setup(mc => mc.Commit());
            mockITransaction.Setup(mc => mc.Dispose());
            mockSqlServerDatabaseProfile.Setup(mc => mc.GetDatabase()).Returns(mockIDatabase.Object);

            //act
            _dvcController = new DvcController(databaseVersionSetup, DefaultProfile, true);
            _dvcController.RollbackIndexToIndex(2);
            mockSqlServerDatabaseProfile.VerifyAll();
            mockIDatabase.VerifyAll();
            mockITransaction.VerifyAll();
            _mockITracker.VerifyAll();

        }

       

        [Test]
        public void RollbackIndexToIndex_RunFullUpdate_ValuesAreReplacedRoleBackPartial()
        {
            var databaseVersionSetup = GetVersionSetup();
            var mockIDatabase = new Mock<IConnection>(MockBehavior.Strict);
            var mockITransaction = new Mock<ITransaction>(MockBehavior.Strict);
            mockIDatabase.Setup(mc => mc.GetTransaction()).Returns(mockITransaction.Object);
            mockIDatabase.Setup(mc => mc.SwitchToDatabase("Intercontinental4", mockITransaction.Object));
            _mockITracker.Setup(mc => mc.GetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), mockITransaction.Object)).Returns(4);
            mockIDatabase.Setup(mc => mc.ExecuteSql("RoleBack4 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            mockIDatabase.Setup(mc => mc.ExecuteSql("RoleBack3 test0 test1Prof|withvar|Intercontinental4| test2|over ${test3}", mockITransaction.Object)).Returns(1);
            _mockITracker.Setup(mc => mc.SetVersion(It.IsAny<DatabaseVersionSetup.Profile>(), 2, mockITransaction.Object));
            mockITransaction.Setup(mc => mc.Commit());
            mockITransaction.Setup(mc => mc.Dispose());
            mockSqlServerDatabaseProfile.Setup(mc => mc.GetDatabase()).Returns(mockIDatabase.Object);

            //act
            _dvcController = new DvcController(databaseVersionSetup, DefaultProfile, true);
            _dvcController.RollbackIndexToIndex(3);
            mockSqlServerDatabaseProfile.VerifyAll();
            mockIDatabase.VerifyAll();
            mockITransaction.VerifyAll();
            _mockITracker.VerifyAll();

        }

        public DatabaseVersionSetup GetVersionSetup()
        {
            var versionSetup = new DatabaseVersionSetup();
            mockSqlServerDatabaseProfile = new Mock<SqlServerDatabaseProfile>(MockBehavior.Strict, "Data Source=GMV-RW-LT;Initial Catalog=Intercontinental;Integrated Security=True"
                , "Intercontinental4"
                , "Intercontinental"
                , _mockITracker.Object);
            
            
            
            var profile = new DatabaseVersionSetup.Profile("Default",
                                                        mockSqlServerDatabaseProfile.Object);
            profile.AddProperties("test1", "test1Prof|withvar|${dp.DatabaseName}|");
            profile.AddProperties("test2", "test2");
            profile.AddProperties("test2", "test2|over");
            profile.AddProperties("insert", "insert.sql");
            profile.AddProperties("update", "update.sql");
            profile.AddProperties("delete", "delete.sql");
            versionSetup.Profiles.Add(profile);

            var runner = new SqlRunner("CREATE DATABASE ${dp.DatabaseName};", "USE Master; DROP DATABASE ${dp.DatabaseName};");
            var repository = new DatabaseRepository("Intercontinental", new UpdatesMetadata(1, "Todau", "Initialize", "rolf", runner));
            repository.AddProperties("test0", "test0");
            repository.AddProperties("test1", "test1");
            var versions = new UpdatesMetadata(2, "2010-04-16", "First update", "Rolf Wessels",
                new SqlRunner(
                    @"Create2 ${test0} ${test1} ${test2} ${test3}",
                    @"RoleBack2 ${test0} ${test1} ${test2} ${test3}",
                    @"TestData2 ${test0} ${test1} ${test2} ${test3}"
                    )
                );
            repository.Updates.Add(versions);

            versions = new UpdatesMetadata(3, "2010-04-16", "First update", "Rolf Wessels",
                new SqlRunner(
                    @"Create3 ${test0} ${test1} ${test2} ${test3}",
                    @"RoleBack3 ${test0} ${test1} ${test2} ${test3}",
                    @"TestData3 ${test0} ${test1} ${test2} ${test3}"
                    )
                );
            repository.Updates.Add(versions);
            File.WriteAllText(@"resources\insert.sql", @"Create4 ${test0} ${test1} ${test2} ${test3}");
            File.WriteAllText(@"resources\update.sql", @"RoleBack4 ${test0} ${test1} ${test2} ${test3}");
            File.WriteAllText(@"resources\delete.sql", @"TestData4 ${test0} ${test1} ${test2} ${test3}");
            repository.Updates.Add(new UpdatesMetadata(4, "2010-04-16", "First update", "Rolf Wessels",
                new SqlRunner(new SqlFilesExecuter(new[] { @"resources\${insert}" }),
                    new SqlFilesExecuter(new[] { @"resources\${update}" }),
                    new SqlFilesExecuter(new[] { @"resources\${delete}" }))));

            versionSetup.Repository.Add(repository);


            return versionSetup;
        }

    }

}