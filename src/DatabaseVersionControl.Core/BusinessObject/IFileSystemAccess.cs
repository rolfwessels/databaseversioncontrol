using System.IO;

namespace DatabaseVersionControl.Core.BusinessObject
{
    /// <summary>
    /// Interface for file system access
    /// </summary>
    public interface IFileSystemAccess
    {
        /// <summary>
        /// Searches multiple paths to find the file location
        /// </summary>
        /// <returns></returns>
        bool FileExists(string fileName, string[] possiblePaths, out string fullPath);
    }

    public class FileSystemAccess : IFileSystemAccess
    {
        #region Implementation of IFileSystemAccess

        public bool FileExists(string fileName, string[] possiblePaths, out string fullPath)
        {
            foreach (string possiblePath in possiblePaths){
                fullPath = Path.Combine(possiblePath, fileName);
                if (File.Exists(fullPath))
                {
                    return true;
                }
            }
            fullPath = null;
            return false;
        }

        #endregion
    }
}