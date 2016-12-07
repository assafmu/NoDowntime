using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime
{
    internal class HotFolders
    {
        #region Fields
        private string _area1;
        private string _area2;
        private bool _isArea1 = false;
        #endregion
        public HotFolders(string initialFolder,string swapFolder)
        {
            _area1 = initialFolder;
            _area2 = swapFolder;
        }
        public string CurrentDirectory
        {
            get
            {
                return _isArea1 ? _area1 : _area2;
            }
        }
        public string NextDirectory
        {
            get
            {
                return _isArea1 ? _area2 : _area1;
            }
        }
        public bool Swap()
        {
            _isArea1 = !_isArea1;
            return _isArea1;
        }
    }
}
