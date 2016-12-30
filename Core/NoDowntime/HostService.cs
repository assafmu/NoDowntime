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

        private Func<string> _classNameAccessor;
        private Func<string> _dllNameAccessor;
        #endregion

        #region Properties
        /// <summary>
        /// Enables recycling with an empty staging folder. If the staging folder is empty, the libraries will be copied from the current directory to the next.
        /// False by default.
        /// </summary>
        public bool InPlaceRecycling { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a HostService using the default folders (Staging, Area1, Area2).
        /// </summary>
        public HostService() : this(defaultStagingFolder, defaultArea1, defaultArea2)
        {
        }
        public HostService(string dllName, string className) :
            this(defaultStagingFolder, defaultArea1, defaultArea2, dllName, className)
        { }

        public HostService(Func<string> dllNameAccessor, Func<string> classNameAccessor) :
            this(defaultStagingFolder, defaultArea1, defaultArea2, dllNameAccessor, classNameAccessor)
        { }
        /// <summary>
        /// Constructs a HostService using the specified folders. Folder names can be absolute or relative.
        /// </summary>
        /// <param name="stagingFolder">The folder from which the library is copied.</param>
        /// <param name="firstFolder">The first folder copied to.</param>
        /// <param name="secondFolder">The second folder copied to.</param>
        public HostService(string stagingFolder, string firstFolder, string secondFolder)
            : this(stagingFolder, firstFolder, secondFolder, GetDllNameFromConfiguration, GetClassNameFromConfiguration)
        { }
        /// <summary>
        /// Constructs a HostService with the specified folders, and constant DLL name and root class name. Folder names can be absolute or relative.
        /// </summary>
        /// <param name="stagingFolder"></param>
        /// <param name="firstFolder"></param>
        /// <param name="secondFolder"></param>
        /// <param name="dllName"></param>
        /// <param name="className"></param>
        public HostService(string stagingFolder, string firstFolder, string secondFolder, string dllName, string className) :
            this(stagingFolder, firstFolder, secondFolder, () => dllName, () => className)
        { }

        /// <summary>
        /// Constructs a HostService with the specified folders, and dynamic DLL name and root class name. Folder names can be absolute or relative.
        /// </summary>
        /// <param name="stagingFolder"></param>
        /// <param name="firstFolder"></param>
        /// <param name="secondFolder"></param>
        /// <param name="dllNameAccessor"></param>
        /// <param name="classNameAccessor"></param>
        public HostService(string stagingFolder, string firstFolder, string secondFolder, Func<string> dllNameAccessor, Func<string> classNameAccessor)
        {
            _info = new AppDomainSetup();
            _stagingFolder = stagingFolder;
            _folders = new HotFolders(firstFolder, secondFolder);
            _classNameAccessor = classNameAccessor;
            _dllNameAccessor = dllNameAccessor;
            InPlaceRecycling = _defaultInPlaceRecycling;
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
            else if (InPlaceRecycling)
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
            string dllName = _dllNameAccessor();
            string className = _classNameAccessor();
            _nextDomain = AppDomain.CreateDomain("Ad2", null, _info);
            RemoteFactory factory = _nextDomain.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory") as RemoteFactory;
            _nextService = factory.Create(targetDirectory, dllName, className);
            _folders.Swap();
            _nextService.SetState(state);
            _nextService.Start();
        }

        private void RefreshAdditionalConfigurationSections()
        {
            string additionalSections = ConfigurationManager.AppSettings["NoDowntime.RefreshedConfigurationSections"];
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

        #region Defaults
        private static string GetDllNameFromConfiguration()
        {
            return ConfigurationManager.AppSettings["NoDowntime.RootDll"];
        }

        private static string GetClassNameFromConfiguration()
        {
            return ConfigurationManager.AppSettings["NoDowntime.RootClass"];
        }
        private static string defaultStagingFolder = "Staging";
        private static string defaultArea1 = "Area1";
        private static string defaultArea2 = "Area2";
        private static bool _defaultInPlaceRecycling = false;
        #endregion
    }
}
