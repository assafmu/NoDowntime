using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime.Wrappers
{
    internal interface IDirectory
    {
        IEnumerable<string> EnumerateFileSystemEntries(string path);
        bool Exists(string path);
        void CreateDirectory(string destDirName);
        void Delete(string destDirName, bool v);
    }
}
