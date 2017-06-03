using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NoDowntime.Wrappers;
using Connector;

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

        [TestInitialize]
        public void Initialize()
        {
            _directory = new Mock<IDirectory>();
            _config = new Mock<IConfiguration>();
            _domainFactory = new Mock<IAppDomainFactory>();
            _stagingDir = new Mock<DirInfo>();
            _dllInfo = new Mock<FileInfo>();
            _appDomain = new Mock<IApplicationDomain>();
            _hostService = new HostService("staging", "1", "2", "dll", "class", _directory.Object, _config.Object,_domainFactory.Object);
        }
        [TestMethod]
        public void HostService_Initialize_CreatesAppDomain()
        {
            _directory.Setup(dir => dir.Exists("staging")).Returns(true);
            _directory.Setup(dir => dir.EnumerateFileSystemEntries("staging")).Returns(new string[] { "a" });
            _directory.Setup(dir => dir.Get("staging")).Returns(_stagingDir.Object);
            _stagingDir.Setup(dir => dir.GetDirectories()).Returns(new DirInfo[0]);
            _stagingDir.Setup(dir => dir.GetFiles()).Returns(new FileInfo[] { _dllInfo.Object });
            _dllInfo.Setup(file => file.Name).Returns("a");
            var appSettings = new System.Collections.Specialized.NameValueCollection();
            appSettings["NoDowntime.RefreshedConfigurationSections"] = "";
            _config.Setup(c => c.AppSettings).Returns(appSettings);
            _domainFactory.Setup(factory => factory.CreateDomain(It.IsAny<string>(), null, It.IsAny<AppDomainSetup>())).Returns(_appDomain.Object);
            var service = new Mock<IRecycableService>();
            var remoteFactory = Mock.Of<RemoteFactory>(rf => rf.Create(It.IsAny<string>(),"dll","class",It.IsAny<object[]>()) == service.Object);
            _appDomain.Setup(domain => domain.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory")).Returns(remoteFactory);
            _hostService.Initialize();
        }
    }
}
