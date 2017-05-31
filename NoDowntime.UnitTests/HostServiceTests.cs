using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NoDowntime.Wrappers;

namespace NoDowntime.UnitTests
{
    [TestClass]
    public class HostServiceTests
    {
        private HostService _hostService;
        private Mock<IDirectory> _directory;
        private Mock<IConfiguration> _config;

        [TestInitialize]
        public void Initialize()
        {
            _directory = new Mock<IDirectory>();
            _config = new Mock<IConfiguration>();
            _hostService = new HostService("staging", "1", "2", "dll", "class", _directory.Object, _config.Object);
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
