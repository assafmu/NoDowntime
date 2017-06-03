using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime.Wrappers
{
    internal class DirInfo
    {
        private DirectoryInfo _inner;
        internal DirInfo() : this(null) { }
        internal DirInfo(DirectoryInfo info)
        {
            _inner = info;
        }

        internal virtual string FullName { get; private set; }
        internal virtual string Name { get; private set; }

        internal virtual DirInfo[] GetDirectories()
        {
            return _inner.GetDirectories().Select(d => new DirInfo(d)).ToArray();
        }

        internal virtual FileInfo[] GetFiles()
        {
            return _inner.GetFiles().Select(f => new FileInfo(f)).ToArray();
        }

        internal virtual void Delete(bool recursive)
        {
            _inner.Delete(recursive);
        }
    }

    internal class FileInfo
    {
        System.IO.FileInfo _inner;
        internal FileInfo() : this(null) { }
        internal FileInfo(System.IO.FileInfo info)
        {
            _inner = info;
        }
        internal virtual string Name { get; private set; }

        internal virtual FileInfo CopyTo(string destFileName, bool overwrite)
        {
            return new FileInfo(_inner.CopyTo(destFileName, overwrite));
        }

        internal virtual void Delete()
        {
            _inner.Delete();
        }
    }
}
