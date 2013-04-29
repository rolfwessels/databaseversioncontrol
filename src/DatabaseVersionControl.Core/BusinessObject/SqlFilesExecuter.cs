using System.IO;
using System.Text;
using DatabaseVersionControl.Core.Database;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// Runs sql from multiple files
    /// </summary>
    public class SqlFilesExecuter : ISqlExecuter
    {
        private readonly string[] _files;

        /// <summary>
        /// Create file reader from file array
        /// </summary>
        /// <param name="files"></param>
        public SqlFilesExecuter(string[] files)
        {
            _files = files;
        }

        #region Implementation of ISqlExecuter

        /// <summary>
        /// Execute the sql command by slitting file by lines that contain 'GO'
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="transaction"></param>
        public void ExecuteSql(DatabaseVersionSetup.Profile profile, ITransaction transaction)
        {
            var stringBuilder = new StringBuilder();
            foreach (var file in _files){
                using (var stream = File.OpenRead(profile.VariableReplace(file)))
                {
                    using (var streamReader = new StreamReader(stream)){
                        while (!streamReader.EndOfStream){
                            var readLine = streamReader.ReadLine();
                            if (readLine.Trim().Equals("GO")){
                                Execute(profile, stringBuilder, transaction);
                                stringBuilder = new StringBuilder();
                            }
                            else{
                                stringBuilder.AppendLine(readLine);
                            }
                        }
                        //make sure we execute everything
                        Execute(profile, stringBuilder, transaction);
                    }
                }
            }
        }

        private void Execute(DatabaseVersionSetup.Profile profile, StringBuilder stringBuilder, ITransaction transaction)
        {
            var trim = stringBuilder.ToString().Trim();
            if (trim.Length > 0){
                profile.RepositoryProfile.GetDatabase().ExecuteSql(profile.VariableReplace(trim), transaction);
            }
        }

        /// <summary>
        /// Log out sql value
        /// </summary>
        public string SqlValue
        {
            get
            {
                var builder = new StringBuilder();
                foreach (var file in _files)
                {
                    var text = File.ReadAllText(file);
                    builder.Append(text);
                }
                return builder.ToString();
            }
        }

        #endregion
    }
}