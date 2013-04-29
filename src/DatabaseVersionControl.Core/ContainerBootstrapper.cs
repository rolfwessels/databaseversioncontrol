using DatabaseVersionControl.Core.BusinessObject;
using StructureMap;

namespace DatabaseVersionControl.Core
{
    public static class ContainerBootstrapper
    {
        public static void BootstrapStructureMap()
        {
            // Initialize the static ObjectFactory container
            ObjectFactory.Initialize(x =>
            {
                x.For<IProductUpdate>().Use<DatabaseVersionSetup>();
                x.For<IFileSystemAccess>().Use<FileSystemAccess>();
            });
        }
    }
}