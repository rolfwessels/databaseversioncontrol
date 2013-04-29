using System.IO;

namespace DatabaseVersionControl.Core
{
    public interface IConfigFileLoader
    {
        void Load(TextReader reader);
    }
}