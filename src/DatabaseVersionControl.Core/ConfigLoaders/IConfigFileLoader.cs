using System;
using System.IO;
using DatabaseVersionControl.Core.BusinessObject;

namespace DatabaseVersionControl.Core
{
    public interface IConfigFileLoader
    {
        DatabaseVersionSetup Load(TextReader reader);
    }
}