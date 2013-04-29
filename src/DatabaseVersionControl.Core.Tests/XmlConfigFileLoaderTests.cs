using System.IO;
using DatabaseVersionControl.Core;
using NUnit.Framework;

namespace DatabaseVersionControl.UnitTests
{
	[TestFixture]
	public class XmlConfigFileLoaderTests
	{
		[Test]
		public void Load_ReadsStreamIntoBussinessObjects_ValidRead()
		{
            var configFileLoader = new XmlConfigFileLoader();
            using (TextReader file = new StreamReader(File.OpenRead(@"Resources\Sample1.xml"))) {
                configFileLoader.Load(file);
            }
		}
	}

    
}

