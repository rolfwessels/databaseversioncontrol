using System;
using System.IO;
using DatabaseVersionControl.Core.Export.SqlPubWiz;
using log4net.Config;
using NUnit.Framework;

namespace DatabaseVersionControl.Tests.Export
{
    [TestFixture]
    public class SqlPubWizProxyTests
    {
        private SqlPubWizProxy _sqlPubWizProxy;

        [Test,Category("Integration"),Explicit]
        public void GetSchema_ReadsTheSchema_FromTheDatabas()
        {
            XmlConfigurator.Configure();
            _sqlPubWizProxy = new SqlPubWizProxy(Settings.Default.Executable);
            TextReader textReader = _sqlPubWizProxy.GetSchema("Data Source=localhost;Initial Catalog=AMS_SHEETS;Integrated Security=True");
            Console.Out.WriteLine("textReader.ReadToEnd()" + textReader.ReadToEnd());
        }

    }

}