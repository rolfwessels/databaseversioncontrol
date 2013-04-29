using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DatabaseVersionControl.Core.BusinessObject;
using DatabaseVersionControl.Core.Database;
using log4net;

namespace DatabaseVersionControl.Core
{
    
    /// <summary>
    /// Manage the maintenance of versions
    /// </summary>
    public class DvcController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DatabaseVersionSetup _setup;
        private readonly DatabaseVersionSetup.Profile _profile;
        //private DatabaseRepository _repository;
        private readonly bool _runTestData;
        public event OnUpdateExecuteDeligate OnUpdateExecute;

        private void InvokeOnUpdateExecute(OnUpdateExecuteDeligateArgs args)
        {
            OnUpdateExecuteDeligate execute = OnUpdateExecute;
            if (execute != null) execute(this, args);
        }

        /// <summary>
        /// Public constructor with read only database version
        /// </summary>
        /// <param name="setup"></param>
        /// <param name="profileId"></param>
        /// <param name="repositoryId"></param>
        /// <param name="runTestData"></param>
        public DvcController(DatabaseVersionSetup setup, string profileId, bool runTestData)
        {
            _setup = setup;
            _runTestData = runTestData;
            Log.Info(string.Format("Loading profile {0}", profileId));
            _profile = _setup.GetProfile(profileId);
            if (_profile == null){
                throw new Exception(string.Format("Could not find profile {0} in list of profiles", profileId));
            }
            //add variable from repository to profile
            _profile.AddRepositoryProperties(GetDefaultRepository().Properties);

        }

        
        /// <summary>
        /// Connects to database. Tries to run the InitialRunner script and installs any <see cref="ITracker"/> requirements.
        /// </summary>
        public void Initialize()
        {
            Log.Info("Load initialize script");

            InvokeOnUpdateExecute(new OnUpdateExecuteDeligateArgs(GetDefaultRepository().InitialRunner, "ExecuteCommand"));
            GetDefaultRepository().InitialRunner.Runner.ExecuteCommand(_profile,null);
            if (_profile.RepositoryProfile.Tracker.AutoInitializeNewTracker){
                InitializeTracker();
            }
        }

        private void SwitchToDefaultDatase(ITransaction transaction)
        {
            _profile.RepositoryProfile.GetDatabase().SwitchToDatabase(_profile.RepositoryProfile.DatabaseName , transaction);
        }

        public void InitializeTracker()
        {
            using (ITransaction transaction = _profile.RepositoryProfile.GetDatabase().GetTransaction())
            {
                SwitchToDefaultDatase(transaction);
                InitializeTracker(transaction);
                transaction.Commit();
            }

            
        }

        private void InitializeTracker(ITransaction transaction)
        {
            InvokeOnUpdateExecute(new OnUpdateExecuteDeligateArgs(GetDefaultRepository().InitialRunner, "Initialize tracker"));
            _profile.RepositoryProfile.Tracker.InitializeNewTracker(_profile, transaction);
            _profile.RepositoryProfile.Tracker.SetVersion(_profile, 0, transaction);
        }

        private DatabaseRepository GetDefaultRepository()
        {
            return GetRepository(_profile.RepositoryProfile.DatabaseRepository);
        }

        private DatabaseRepository GetRepository(string repositoryName)
        {
            Log.Info(string.Format("Loading repository {0}", repositoryName));
            var repository = _setup.GetRepository(repositoryName);
            if (repository == null)
            {
                throw new Exception(string.Format("Could not find repository {0} in list of repositories", repositoryName));
            }
            return repository;
        }


        public void InitializeRollBack()
        {
            if (GetDefaultRepository().InitialRunner.Runner.HasRollback())
            {
                //ERROR: DROP DATABASE statement cannot be used inside a user transaction.
                Log.Info("Rollback initialize script");
                InvokeOnUpdateExecute(new OnUpdateExecuteDeligateArgs(GetDefaultRepository().InitialRunner,
                                                                      "ExecuteRollback"));
                GetDefaultRepository().InitialRunner.Runner.ExecuteRollback(_profile, null);
                
            }
            else{
                Log.Info("Could not rollback initialize script database does not exist");
            }
        }

        public void BringUpToDate()
        {

            using (NDC.Push(string.Format("BringUpToDate:{0}|", _profile.Id)))
            {
                ITransaction transaction = GetTransactionIfOneDoesNotExistAlready(null);
                SwitchToDefaultDatase(transaction);
                double version = _profile.RepositoryProfile.Tracker.GetVersion(_profile, transaction);
                Log.Info(string.Format("Starting version {0}", version));
                IOrderedEnumerable<UpdatesMetadata> enumerable = (from r in GetDefaultRepository().Updates orderby r.Index select r);
                Log.Info(string.Format("{0} Updates found", GetDefaultRepository().Updates.Count));

                foreach (var updatesVersions in enumerable){
                    if (updatesVersions.Index > version){
                        using (NDC.Push(string.Format("Index:{0}|", updatesVersions.Index)))
                        {
                            version = updatesVersions.Index;
                            transaction = updatesVersions.SkipTransaction? DisposeTransaction(transaction) : GetTransactionIfOneDoesNotExistAlready(transaction);

                            if (transaction == null){
                                Log.Info("data");
                            }
                            ExecuteRunner(updatesVersions, transaction);
                            _profile.RepositoryProfile.Tracker.SetVersion(_profile, version, transaction);
                            if (CommitAfterEveryUpdate){
                                transaction = DisposeTransaction(transaction);
                                transaction = GetTransactionIfOneDoesNotExistAlready(transaction);
                            }
                        }
                    }
                }

                Log.Info("Commit all");
                DisposeTransaction(transaction);
            }

        }

        private ITransaction GetTransactionIfOneDoesNotExistAlready(ITransaction transaction)
        {
            if (transaction == null){
                return _profile.RepositoryProfile.GetDatabase().GetTransaction();
            }
            return transaction;
        }

        private ITransaction DisposeTransaction(ITransaction transaction)
        {
            if (transaction != null)
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
            return transaction;
        }

        public void RunIndex(int index)
        {
            ExecuteSingleUpdate(index,ExecuteRunner);
        }

        private void ExecuteSingleUpdate(int index, Action<UpdatesMetadata, ITransaction> executeRunner)
        {
            UpdatesMetadata[] enumerable = (from r in GetDefaultRepository().Updates where r.Index == index select r).ToArray();
            if (enumerable.Length > 1)
            {
                throw new Exception(string.Format("Index [{0}] has more than related update. This should not be possible.", index));
            }
            if (enumerable.Length == 0)
            {
                throw new Exception(string.Format("Index [{0}] not found", index));
            }

            foreach (var updatesVersions in enumerable)
            {
                ITransaction transaction = null;
                if (!updatesVersions.SkipTransaction){
                    transaction = _profile.RepositoryProfile.GetDatabase().GetTransaction();
                }
                SwitchToDefaultDatase(transaction);
                executeRunner(updatesVersions, transaction);
                if (transaction != null)
                {
                    Log.Info("Commit all");
                    transaction.Commit();
                    transaction.Dispose();
                }
            }
        }

        public void RollbackIndex(int index)
        {
            ExecuteSingleUpdate(index, ExecuteRollBack);
        }


        public void SetProfileVersion(int version)
        {
            using (ITransaction transaction = _profile.RepositoryProfile.GetDatabase().GetTransaction())
            {
                SwitchToDefaultDatase(transaction);
                _profile.RepositoryProfile.Tracker.SetVersion(_profile, version , transaction);
                transaction.Commit();
            }
        }

        public void PrintDatabaseVersion(TextWriter writer)
        {
            using (ITransaction transaction = _profile.RepositoryProfile.GetDatabase().GetTransaction())
            {
                SwitchToDefaultDatase(transaction);
                var version = _profile.RepositoryProfile.Tracker.GetVersion(_profile, transaction);
                writer.WriteLine(_profile.RepositoryProfile.DatabaseName + " : " + version);
            }
            
        }


        public void RollbackIndexToIndex(int rollback)
        {
            using (ITransaction transaction = _profile.RepositoryProfile.GetDatabase().GetTransaction())
            {
                SwitchToDefaultDatase(transaction);
                double version = _profile.RepositoryProfile.Tracker.GetVersion(_profile, transaction);
                Log.Info(string.Format("Starting version {0}", version));
                IOrderedEnumerable<UpdatesMetadata> enumerable = (from r in GetDefaultRepository().Updates orderby r.Index select r);
                Log.Info(string.Format("{0} Updates found", GetDefaultRepository().Updates));
                foreach (var updatesVersions in enumerable.Reverse())
                {
                    if (updatesVersions.Index <= version && updatesVersions.Index >= rollback)
                    {
                        version = updatesVersions.Index;
                        ExecuteRollBack(updatesVersions, transaction);
                    }
                }
                _profile.RepositoryProfile.Tracker.SetVersion(_profile, rollback-1, transaction);
                Log.Info("Commit all");
                transaction.Commit();
            }
        }

        private void ExecuteRollBack(UpdatesMetadata updatesMetadata, ITransaction transaction)
        {
            using (NDC.Push(string.Format("RollBack:{0}|", updatesMetadata.Index)))
            {
                if (updatesMetadata.Runner.HasRollback()){
                    InvokeOnUpdateExecute(new OnUpdateExecuteDeligateArgs(updatesMetadata, "ExecuteRollback"));
                    updatesMetadata.Runner.ExecuteRollback(_profile, transaction);
                }
            }
        }

        private void ExecuteRunner(UpdatesMetadata updatesMetadata, ITransaction transaction)
        {
            using (NDC.Push(string.Format("Update:{0}|", updatesMetadata.Index)))
            {
                InvokeOnUpdateExecute(new OnUpdateExecuteDeligateArgs(updatesMetadata, "ExecuteCommand"));
                Log.Info("Running: " + updatesMetadata);
                updatesMetadata.Runner.ExecuteCommand(_profile, transaction);
                if (_runTestData && updatesMetadata.Runner.HasTestData()){
                    InvokeOnUpdateExecute(new OnUpdateExecuteDeligateArgs(updatesMetadata, "ExecuteTestData"));
                    updatesMetadata.Runner.ExecuteTestData(_profile, transaction);
                }
            }
            
        }

        public DatabaseVersionSetup.Profile Profile
        {
            get { return _profile; }
        }

        public DatabaseVersionSetup Setup
        {
            get { return _setup; }
        }

        public bool CommitAfterEveryUpdate { get; set; }
    }

    public delegate void OnUpdateExecuteDeligate(object sender, OnUpdateExecuteDeligateArgs args);

    public class OnUpdateExecuteDeligateArgs
    {
        private readonly UpdatesMetadata _updatesMetadata;
        private readonly string _executecommand;

        public OnUpdateExecuteDeligateArgs(UpdatesMetadata metadata, string executecommand)
        {
            _updatesMetadata = metadata;
            _executecommand = executecommand;
        }

        public UpdatesMetadata UpdatesMetadata
        {
            get { return _updatesMetadata; }
        }

        public string Executecommand
        {
            get { return _executecommand; }
        }
    }
}