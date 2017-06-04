using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NoDowntime.Wrappers;
using Connector;
using FluentAssertions;

namespace NoDowntime.UnitTests
{
    [TestClass]
    public class HostServiceTests
    {
        private HostService _hostService;
        private Mock<IDirectory> _directory;
        private Mock<IConfiguration> _config;
        private Mock<IAppDomainFactory> _domainFactory;
        private Mock<DirInfo> _stagingDir;
        private Mock<FileInfo> _dllInfo;
        private Mock<IApplicationDomain> _appDomain;
        private string stagingFolder = "staging";
        private string className;
        private string dllName;
        private string dllFileName;

        [TestInitialize]
        public void Initialize()
        {
            className = "class";
            dllName = "dll";
            dllFileName = dllName + ".dll";
            _directory = new Mock<IDirectory>();
            _config = new Mock<IConfiguration>();
            _domainFactory = new Mock<IAppDomainFactory>();
            _stagingDir = new Mock<DirInfo>();
            _dllInfo = new Mock<FileInfo>();
            _appDomain = new Mock<IApplicationDomain>();
            InitializeConfiguration();
            InitializeFileSystem();
            InitializeRecycableService();
            _hostService = new HostService(stagingFolder, "1", "2", dllName, className, _directory.Object, _config.Object, _domainFactory.Object);
        }

        public void InitializeConfiguration()
        {
            var appSettings = new System.Collections.Specialized.NameValueCollection();
            appSettings["NoDowntime.RefreshedConfigurationSections"] = "";
            _config.Setup(c => c.AppSettings).Returns(appSettings);
        }

        public void InitializeFileSystem()
        {
            _directory.Setup(dir => dir.Exists(stagingFolder)).Returns(true);
            _directory.Setup(dir => dir.EnumerateFileSystemEntries(stagingFolder)).Returns(new string[] { dllFileName });
            _directory.Setup(dir => dir.Get(stagingFolder)).Returns(_stagingDir.Object);
            _stagingDir.Setup(dir => dir.GetDirectories()).Returns(new DirInfo[0]);
            _stagingDir.Setup(dir => dir.GetFiles()).Returns(new FileInfo[] { _dllInfo.Object });
            _dllInfo.Setup(file => file.Name).Returns(dllFileName);
        }

        public void InitializeRecycableService()
        {
            _domainFactory.Setup(factory => factory.CreateDomain(It.IsAny<string>(), null, It.IsAny<AppDomainSetup>())).Returns(_appDomain.Object);
            var service = new Mock<IRecycableService>();
            var remoteFactory = Mock.Of<RemoteFactory>(rf => rf.Create(It.IsAny<string>(), dllName, className, It.IsAny<object[]>()) == service.Object);
            _appDomain.Setup(domain => domain.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory")).Returns(remoteFactory);
        }

        [TestMethod]
        public void HostService_Initialize_CreatesAppDomain()
        {
            Action serviceInitialization = () => _hostService.Initialize();
            serviceInitialization.ShouldNotThrow();
        }

        [TestMethod]
        public void HostService_Initialize_RefreshesMultipleConfigurationSections()
        {
            var appSettings = new System.Collections.Specialized.NameValueCollection();
            appSettings["NoDowntime.RefreshedConfigurationSections"] = "a;b";
            _config.Setup(c => c.AppSettings).Returns(appSettings).Verifiable();
            _config.Setup(c => c.GetSection("a")).Returns(new object());
            _config.Setup(c => c.GetSection("b")).Returns(new object());
            _config.Setup(c => c.RefreshSection("a")).Verifiable();
            _config.Setup(c => c.RefreshSection("b")).Verifiable();
            _hostService.Initialize();
            _config.VerifyAll();

        }

        [TestMethod]
        public void HostService_Initialize_StagingEmptyLoadsFromFirstFolder()
        {
            throw new NotImplementedException();
        }


        [TestMethod]
        public void HostService_Initialize_StagingAndFirstEmptyLoadsFromSecondFolder()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void HostService_Initialize_AllFolderEmptyExceptionThrown()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void HostService_Initialize_ClearsStagingFolder()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void HostService_Initialize_ServiceStarted()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void HostService_GetName_ReturnsCurrentServiceName()
        {
            throw new NotImplementedException();
        }
    }
}
