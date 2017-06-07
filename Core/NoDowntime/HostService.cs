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
        #region Fields
        private AppDomainSetup _info;
        private AppDomain _currentDomain;
        private AppDomain _nextDomain;
        private IRecycableService _currentService;
        private IRecycableService _nextService;

        private HotFolders _folders;
        private string _stagingFolder;

        private string _className;
        private string _dllName;
        private bool _inPlaceRecycling;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Constructs a HostService with the specified folders, and dynamic DLL name and root class name. Folder names can be absolute or relative.
        /// </summary>
        /// <param name="stagingFolder"></param>
        /// <param name="firstFolder"></param>
        /// <param name="secondFolder"></param>
        /// <param name="dllNameAccessor"></param>
        /// <param name="classNameAccessor"></param>
        public HostService( string className = null, string dllName = null, string stagingFolder = null, string firstFolder = null, string secondFolder = null,bool inPlaceRecycling = true)
        {
            NoDowntimeConfiguration config = ConfigurationManager.GetSection("noDowntime") as NoDowntimeConfiguration;

            _stagingFolder = stagingFolder ?? config?.StagingFolder ?? NoDowntimeConfiguration.StagingFolderDefault;
            _folders = new HotFolders(firstFolder?? config?.Folder1 ?? NoDowntimeConfiguration.Folder1Default, secondFolder ?? config?.Folder2 ?? NoDowntimeConfiguration.Folder2Default);
            _className = className ?? config?.ClassName;
            _dllName = dllName ?? config?.DllName;
            _inPlaceRecycling = inPlaceRecycling || config == null ? true : config.InPlaceRecycling;

            _info = new AppDomainSetup();
        }
        #endregion

        #region Public Methods

        public void Initialize()
        {

            if (DirectoryExistsAndNotEmpty(_stagingFolder)) // default common behaviour
            {
                Load(_stagingFolder, _folders.NextDirectory);
            }
            else if (DirectoryExistsAndNotEmpty(_folders.NextDirectory))
            {
                Load(_folders.NextDirectory, _folders.NextDirectory);
            }
            else
            {
                Load(_folders.CurrentDirectory, _folders.CurrentDirectory);
            }
            SwitchServiceAndDomain();
        }

        public void Recycle()
        {
            State state = _currentService.GetState();
            if (DirectoryExistsAndNotEmpty(_stagingFolder)) // default common behaviour
            {
                Load(_stagingFolder, _folders.NextDirectory, state);
            }
            else if (_inPlaceRecycling)
            {
                Load(_folders.CurrentDirectory, _folders.NextDirectory, state);
            }

            Unload();
            SwitchServiceAndDomain();
        }

        public void Stop()
        {
            Unload();
        }

        public string GetName()
        {
            return _currentService.GetName();
        }

        #endregion

        private void Unload()
        {
            _currentService.Stop();
            _currentService = null;
            AppDomain.Unload(_currentDomain);
        }

        private void Load(string librariesDirectory, string targetDirectory, State state = null)
        {
            if (librariesDirectory != targetDirectory)
            {
                DirectoryCopyAndOverwrite(librariesDirectory, targetDirectory);
            }
            EmptyStagingFolder();
            ConfigurationManager.RefreshSection("appSettings");
            RefreshAdditionalConfigurationSections();
            _nextDomain = AppDomain.CreateDomain("Ad2", null, _info);
            RemoteFactory factory = _nextDomain.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory") as RemoteFactory;
            _nextService = factory.Create(targetDirectory, _dllName, _className);
            _folders.Swap();
            _nextService.SetState(state);
            _nextService.Start();
        }

        private void RefreshAdditionalConfigurationSections()
        {
            var config = ConfigurationManager.GetSection("noDowntime") as NoDowntimeConfiguration;
            string additionalSections = config?.RefreshedSections;
            if (string.IsNullOrWhiteSpace(additionalSections))
            {
                return;
            }
            foreach (string sectionName in additionalSections.Split(';'))
            {
                if (ConfigurationManager.GetSection(sectionName) != null)
                {
                    ConfigurationManager.RefreshSection(sectionName);
                }
            }
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
            if (!DirectoryExistsAndNotEmpty(_stagingFolder))
            {
                return;
            }
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
                file.CopyTo(Path.Combine(destDirName, file.Name), true);
            }
            foreach (DirectoryInfo subDirectory in directories)
            {
                string temppath = Path.Combine(destDirName, subDirectory.Name);
                DirectoryCopyAndOverwrite(subDirectory.FullName, temppath);
            }
        }

        private bool DirectoryExistsAndNotEmpty(string path)
        {
            return Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}
