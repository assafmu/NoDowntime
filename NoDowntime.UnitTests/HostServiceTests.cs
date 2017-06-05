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
        private Mock<IRecycableService> _service;
        private string _stagingFolder = "staging";
        private string _area1 = "1";
        private string _area2 = "2";
        private string _className;
        private string _dllName;
        private string _dllFileName;

        [TestInitialize]
        public void Initialize()
        {
            _className = "class";
            _dllName = "dll";
            _dllFileName = _dllName + ".dll";
            _directory = new Mock<IDirectory>();
            _config = new Mock<IConfiguration>();
            _domainFactory = new Mock<IAppDomainFactory>();
            _stagingDir = new Mock<DirInfo>();
            _dllInfo = new Mock<FileInfo>();
            _appDomain = new Mock<IApplicationDomain>();
            InitializeConfiguration();
            InitializeFileSystem();
            InitializeRecycableService();
            _hostService = new HostService(_stagingFolder, "1", "2", _dllName, _className, _directory.Object, _config.Object, _domainFactory.Object);
        }

        public void InitializeConfiguration()
        {
            var appSettings = new System.Collections.Specialized.NameValueCollection();
            appSettings["NoDowntime.RefreshedConfigurationSections"] = "";
            _config.Setup(c => c.AppSettings).Returns(appSettings);
        }

        public void InitializeFileSystem()
        {
            _directory.Setup(dir => dir.Exists(_stagingFolder)).Returns(true);
            _directory.Setup(dir => dir.EnumerateFileSystemEntries(_stagingFolder)).Returns(new string[] { _dllFileName });
            _directory.Setup(dir => dir.Get(_stagingFolder)).Returns(_stagingDir.Object);
            _stagingDir.Setup(dir => dir.GetDirectories()).Returns(new DirInfo[0]);
            _stagingDir.Setup(dir => dir.GetFiles()).Returns(new FileInfo[] { _dllInfo.Object });
            _dllInfo.Setup(file => file.Name).Returns(_dllFileName);
        }

        public void InitializeRecycableService()
        {
            _domainFactory.Setup(factory => factory.CreateDomain(It.IsAny<string>(), null, It.IsAny<AppDomainSetup>())).Returns(_appDomain.Object);
            _service = new Mock<IRecycableService>();
            var remoteFactory = Mock.Of<RemoteFactory>(rf => rf.Create(It.IsAny<string>(), _dllName, _className, It.IsAny<object[]>()) == _service.Object);
            _appDomain.Setup(domain => domain.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory")).Returns(remoteFactory);
        }

        [TestMethod]
        public void HostService_Initialize_NoException()
        {
            Action serviceInitialization = () => _hostService.Initialize();
            serviceInitialization.ShouldNotThrow();
        }

        [TestMethod]
        public void HostService_Initialize_RefreshesMultipleConfigurationSections()
        {
            var appSettings = new System.Collections.Specialized.NameValueCollection();
            appSettings["NoDowntime.RefreshedConfigurationSections"] = "a;b";
            _config.Setup(c => c.AppSettings).Returns(appSettings);
            _config.Setup(c => c.GetSection("a")).Returns(new object());
            _config.Setup(c => c.GetSection("b")).Returns(new object());
            _config.Setup(c => c.RefreshSection("a"));
            _config.Setup(c => c.RefreshSection("b"));
            _hostService.Initialize();
            _config.VerifyAll();

        }

        [TestMethod]
        public void HostService_Initiailize_LoadsFromStagingFolder()
        {
            string area1 = "1";
            _dllInfo.Setup(info => info.CopyTo(area1+"\\"+_dllFileName, true));
            _directory.Setup(dir => dir.Exists(area1)).Returns(true);
            _directory.Setup(dir => dir.Delete(area1, true));
            _directory.Setup(dir => dir.CreateDirectory(area1));

            _hostService.Initialize();
            _dllInfo.VerifyAll();
            _directory.VerifyAll();
        }

        [TestMethod]
        public void HostService_Initialize_StagingEmptyLoadsFromFirstFolder()
        {
            _directory.Setup(dir => dir.Exists(_stagingFolder)).Returns(false);
            _directory.Setup(dir => dir.EnumerateFileSystemEntries(_stagingFolder)).Returns(new string[0]);
            _directory.Setup(dir => dir.Exists(_area1)).Returns(true);
            _directory.Setup(dir => dir.EnumerateFileSystemEntries(_area1)).Returns(new string[] { _dllFileName });
            _stagingDir.Setup(dir => dir.GetFiles()).Returns(new FileInfo[0]);

            var area1Dir = new Mock<DirInfo>();
            _directory.Setup(dir => dir.Get(_area1)).Returns(area1Dir.Object).Verifiable();
            area1Dir.Setup(dir => dir.GetDirectories()).Returns(new DirInfo[0]).Verifiable();
            area1Dir.Setup(dir => dir.GetFiles()).Returns(new FileInfo[] { _dllInfo.Object }).Verifiable();
            _dllInfo.Setup(file => file.Name).Returns(_dllFileName).Verifiable();

            _hostService.Initialize();
            _directory.Verify();
            _stagingDir.Verify();
            area1Dir.VerifyAll();
        }
        
        [TestMethod]
        public void HostService_Initialize_StagingAndFirstEmptyLoadsFromSecondFolder()
        {
            _directory.Setup(dir => dir.Exists(_stagingFolder)).Returns(false);
            _directory.Setup(dir => dir.EnumerateFileSystemEntries(_stagingFolder)).Returns(new string[0]);
            _directory.Setup(dir => dir.Exists(_area1)).Returns(true);
            _directory.Setup(dir => dir.EnumerateFileSystemEntries(_area1)).Returns(new string[0]);
            _directory.Setup(dir => dir.Exists(_area2)).Returns(true);
            _directory.Setup(dir => dir.EnumerateFileSystemEntries(_area2)).Returns(new string[] { _dllFileName });

            var area2Dir = new Mock<DirInfo>();
            _directory.Setup(dir => dir.Get(_area2)).Returns(area2Dir.Object).Verifiable();
            area2Dir.Setup(dir => dir.GetDirectories()).Returns(new DirInfo[0]).Verifiable();
            area2Dir.Setup(dir => dir.GetFiles()).Returns(new FileInfo[] { _dllInfo.Object }).Verifiable();
            _dllInfo.Setup(file => file.Name).Returns(_dllFileName).Verifiable();

            _hostService.Initialize();
            _directory.Verify();
            _stagingDir.Verify();
            area2Dir.VerifyAll();
        }

        [TestMethod]
        public void HostService_Initialize_AllFolderEmptyExceptionThrown()
        {
            _directory.Reset();
            _directory.Setup(dir => dir.Exists(_stagingFolder)).Returns(false);
            _directory.Setup(dir => dir.Exists(_area1)).Returns(true);
            _directory.Setup(dir => dir.EnumerateFileSystemEntries(_area1)).Returns(new string[0]);
            _directory.Setup(dir => dir.Exists(_area2)).Returns(true);
            _directory.Setup(dir => dir.EnumerateFileSystemEntries(_area2)).Returns(new string[0]);

            Action initialization = _hostService.Initialize;
            initialization.ShouldThrow<Exception>().And.Message.Should().Contain("available for loading");
            _directory.VerifyAll();
        }

        [TestMethod]
        public void HostService_Initialize_ClearsStagingFolder()
        {
            var additionalDllFile = new Mock<FileInfo>();
            additionalDllFile.Setup(file => file.Name).Returns("blabla.dll");
            _stagingDir.Setup(dir => dir.GetFiles()).Returns(new FileInfo[] { _dllInfo.Object,additionalDllFile.Object });
            _hostService.Initialize();
            _dllInfo.Verify(info => info.Delete());
            additionalDllFile.Verify(info => info.Delete());
        }

        [TestMethod]
        public void HostService_Initialize_ServiceStarted()
        {
            _hostService.Initialize();
            _service.Verify(s => s.Start());
            _service.Verify(s => s.SetState(null));
        }

        [TestMethod]
        public void HostService_GetName_ReturnsCurrentServiceName()
        {
            string serviceName = "The service";
            _service.Setup(s => s.GetName()).Returns(serviceName);
            _hostService.Initialize();
            string name = _hostService.GetName();
            name.Should().Be(serviceName);
            _service.VerifyAll();
        }
    }
}
