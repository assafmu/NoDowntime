using Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime
{
    public class HostService
    {
        #region fields
        private AppDomainSetup _info;
        private AppDomain _currentDomain;
        private AppDomain _nextDomain;
        private IRecycableService _currentService;
        private IRecycableService _nextService;
        private HotFolders _folders;
        private string _stagingFolder;
        private State _state;
        #endregion
        #region Constructor
        public HostService()
        {
            _folders = new HotFolders("Area1", "Area2");
            _info = new AppDomainSetup();
            _stagingFolder = "Staging";
        }
        #endregion

        public void Initialize()
        {
            _state = null;
            Load();
            SwitchServiceAndDomain();
        }

        public void Recycle()
        {
            _state = _currentService.GetState();
            Load();
            Unload();
            SwitchServiceAndDomain();
        }

        public void Close()
        {
            Unload();
        }

        public void DisplayName()
        {
            Console.WriteLine(_currentService.GetName());
        }

        internal void Unload()
        {
            _currentService.Stop();
            _currentService = null;
            AppDomain.Unload(_currentDomain);
        }

        internal void Load()
        {
            DirectoryCopyAndOverwrite(_stagingFolder, _folders.NextDirectory);
            EmptyStagingFolder();
            ConfigurationManager.RefreshSection("appSettings");
            string dllName = ConfigurationManager.AppSettings["RootDll"];
            string className = ConfigurationManager.AppSettings["RootClass"];
            _nextDomain = AppDomain.CreateDomain("Ad2", null, _info);
            RemoteFactory factory = _nextDomain.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory") as RemoteFactory;
            _nextService = factory.Create(_folders.NextDirectory, dllName, className);
            _folders.Swap();
            _nextService.SetState(_state);
            _nextService.Start();
        }

        private void SwitchServiceAndDomain()
        {
            _currentDomain = _nextDomain;
            _currentService = _nextService;
            _nextDomain = null;
            _nextService = null;
        }

        private void EmptyStagingFolder()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(_stagingFolder);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private void DirectoryCopyAndOverwrite(string sourceDirName, string destDirName)
        {
            DirectoryInfo directory = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] directories = directory.GetDirectories();
            if (Directory.Exists(destDirName))
            {
                Directory.Delete(destDirName, true);    //Delete contents recursively.
            }
            Directory.CreateDirectory(destDirName);
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destDirName, file.Name), false);
            }
            foreach (DirectoryInfo subDirectory in directories)
            {
                string temppath = Path.Combine(destDirName, subDirectory.Name);
                DirectoryCopyAndOverwrite(subDirectory.FullName, temppath);
            }
        }


    }
}
