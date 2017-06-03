using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime.Wrappers
{
    internal class Directory : IDirectory
    {
        public void CreateDirectory(string destDirName)
        {
            System.IO.Directory.CreateDirectory(destDirName);
        }

        public void Delete(string destDirName, bool v)
        {
            System.IO.Directory.Delete(destDirName, v);
        }

        public IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            return System.IO.Directory.EnumerateFileSystemEntries(path);
        }

        public bool Exists(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public DirInfo Get(string sourceDirName)
        {
            return new DirInfo(new DirectoryInfo(sourceDirName));
        }
    }
}
