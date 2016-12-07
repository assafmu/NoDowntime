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
        private AppDomain _ad2;
        private IRecycableService _currentService;
        private HotFolders _folders;
        private string _stagingFolder;
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
            Load();
        }
        
        public void Recycle()
        {
            Unload();
            Load();
        }

        public void DisplayName()
        {
            Console.WriteLine(_currentService.GetName());
        }

        internal void Unload()
        {
            _currentService.Stop();
            //Save the state
            _currentService = null;
            AppDomain.Unload(_ad2);
        }

        internal void Load()
        {
            DirectoryCopyAndOverwrite(_stagingFolder, _folders.NextDirectory);
            EmptyStagingFolder();
            ConfigurationManager.RefreshSection("appSettings");
            string dllName = ConfigurationManager.AppSettings["RootDll"];
            string className = ConfigurationManager.AppSettings["RootClass"];
            _ad2 = AppDomain.CreateDomain("Ad2", null, _info);
            RemoteFactory factory = _ad2.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory") as RemoteFactory;
            _currentService = factory.Create(_folders.NextDirectory, dllName, className);
            _folders.Swap();
            _currentService.Start();
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
