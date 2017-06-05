using Connector;
using NoDowntime.Wrappers;
using System;
using System.Linq;

namespace NoDowntime
{
    public class HostService : IHostService
    {
        #region Fields
        private AppDomainSetup _info;
        private readonly IDirectory _directoryWrapper;
        private readonly IConfiguration _config;
        private readonly IAppDomainFactory _appDomainFactory;
        private IApplicationDomain _currentDomain;
        private IApplicationDomain _nextDomain;
        private IRecycableService _currentService;
        private IRecycableService _nextService;

        private HotFolders _folders;
        private string _stagingFolder;

        private string _className;
        private string _dllName;
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
        /// <summary>
        /// Constructs a HostService using the specified folders. Folder names can be absolute or relative.
        /// </summary>
        /// <param name="stagingFolder">The folder from which the library is copied.</param>
        /// <param name="firstFolder">The first folder copied to.</param>
        /// <param name="secondFolder">The second folder copied to.</param>
        public HostService(string stagingFolder, string firstFolder, string secondFolder)
            : this(stagingFolder, firstFolder, secondFolder, null, null)
        { }
        /// <summary>
        /// Constructs a HostService with the specified folders, and constant DLL name and root class name. Folder names can be absolute or relative.
        /// </summary>
        /// <param name="stagingFolder"></param>
        /// <param name="firstFolder"></param>
        /// <param name="secondFolder"></param>
        /// <param name="dllName"></param>
        /// <param name="className"></param>
        public HostService(string stagingFolder, string firstFolder, string secondFolder, string dllName, string className) 
            : this(stagingFolder, firstFolder, secondFolder, dllName, className, new Wrappers.Directory(), new Wrappers.Configuration(),new AppDomainFactory())
        {
        }

        internal HostService(string stagingFolder, string firstFolder, string secondFolder, string dllName, string className, IDirectory dw, IConfiguration config, IAppDomainFactory appDomainFactory)
        {
            _info = new AppDomainSetup();
            _stagingFolder = stagingFolder;
            _folders = new HotFolders(firstFolder, secondFolder);
            _className = className ?? GetClassNameFromConfiguration();
            _dllName = dllName ?? GetDllNameFromConfiguration();
            InPlaceRecycling = _defaultInPlaceRecycling;
            _directoryWrapper = dw;
            _config = config;
            _appDomainFactory = appDomainFactory;
        }
        #endregion

        #region Public Methods

        public void Initialize()
        {

            if (DirectoryExistsAndNotEmpty(_stagingFolder)) // default common behaviour
            {
                Load(_stagingFolder, _folders.CurrentDirectory);
            }
            else if (DirectoryExistsAndNotEmpty(_folders.CurrentDirectory))
            {
                Load(_folders.CurrentDirectory, _folders.NextDirectory);
            }
            else if (DirectoryExistsAndNotEmpty(_folders.NextDirectory))
            {
                _folders.Swap();
                Load(_folders.CurrentDirectory, _folders.NextDirectory);
            }
            else
            {
                throw new Exception("No dlls available for loading");
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
            _folders.Swap();
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
            _currentDomain.Unload();
        }

        private void Load(string librariesDirectory, string targetDirectory, State state = null)
        {
            if (librariesDirectory != targetDirectory)
            {
                DirectoryCopyAndOverwrite(librariesDirectory, targetDirectory);
            }
            EmptyDirectory(_stagingFolder);
            _config.RefreshSection("appSettings");
            RefreshAdditionalConfigurationSections();
            _nextDomain = _appDomainFactory.CreateDomain("Ad2", null, _info);
            RemoteFactory factory = _nextDomain.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory") as RemoteFactory;
            _nextService = factory.Create(targetDirectory, _dllName, _className);
            _nextService.SetState(state);
            _nextService.Start();
        }

        private void RefreshAdditionalConfigurationSections()
        {
            string additionalSections = _config.AppSettings["NoDowntime.RefreshedConfigurationSections"];
            if (string.IsNullOrWhiteSpace(additionalSections))
            {
                return;
            }
            foreach (string sectionName in additionalSections.Split(';'))
            {
                if (_config.GetSection(sectionName) != null)
                {
                    _config.RefreshSection(sectionName);
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

        private void EmptyDirectory(string directory)
        {
            if (!DirectoryExistsAndNotEmpty(directory))
            {
                return;
            }
            DirInfo di = _directoryWrapper.Get(directory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private void DirectoryCopyAndOverwrite(string sourceDirName, string destDirName)
        {
            DirInfo directory =_directoryWrapper.Get(sourceDirName);
            DirInfo[] directories = directory.GetDirectories();
            if (_directoryWrapper.Exists(destDirName))
            {
                _directoryWrapper.Delete(destDirName, true);    //Delete contents recursively.
            }
            _directoryWrapper.CreateDirectory(destDirName);
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(System.IO.Path.Combine(destDirName, file.Name), true);
            }
            foreach (DirInfo subDirectory in directories)
            {
                string temppath = System.IO.Path.Combine(destDirName, subDirectory.Name);
                DirectoryCopyAndOverwrite(subDirectory.FullName, temppath);
            }
        }

        private bool DirectoryExistsAndNotEmpty(string path)
        {
            return _directoryWrapper.Exists(path) && _directoryWrapper.EnumerateFileSystemEntries(path).Any();
        }

        #region Defaults
        private string GetDllNameFromConfiguration()
        {
            return _config.AppSettings["NoDowntime.RootDll"];
        }

        private string GetClassNameFromConfiguration()
        {
            return _config.AppSettings["NoDowntime.RootClass"];
        }
        private static string defaultStagingFolder = "Staging";
        private static string defaultArea1 = "Area1";
        private static string defaultArea2 = "Area2";
        private static bool _defaultInPlaceRecycling = false;
        #endregion
    }
}
