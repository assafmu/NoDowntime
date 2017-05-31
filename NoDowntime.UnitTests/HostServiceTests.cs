using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NoDowntime.UnitTests
{
    [TestClass]
    public class HostServiceTests
    {
        private HostService _hostService;

        [TestInitialize]
        public void Initialize()
        {
            _hostService = new HostService();
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
