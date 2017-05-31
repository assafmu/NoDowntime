using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoDowntime;
using FluentAssertions;

namespace NoDowntime.UnitTests
{
    [TestClass]
    public class HotFoldersTests
    {
        private HotFolders _folders;

        [TestInitialize]
        public void Initialize()
        {
            _folders = new HotFolders("a", "b");
        }
        [TestMethod]
        public void HotFolders_InitialFolder()
        {
            _folders.CurrentDirectory.Should().Be("a");
            _folders.NextDirectory.Should().Be("b");
        }
        [TestMethod]
        public void HotFolders_FolderAfterSwitch()
        {
            _folders.Swap();
            _folders.CurrentDirectory.Should().Be("b");
            _folders.NextDirectory.Should().Be("a");
        }
        [TestMethod]
        public void HotFolders_FolderAfterTwoSwitches()
        {
            _folders.Swap();
            _folders.Swap();
            _folders.CurrentDirectory.Should().Be("a");
            _folders.NextDirectory.Should().Be("b");
        }

    }
}
