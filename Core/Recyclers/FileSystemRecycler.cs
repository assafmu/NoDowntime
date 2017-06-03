using NoDowntime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Recyclers
{
    public class FileSystemRecycler
    {

        #region Private Fields
        private readonly FileSystemWatcher fsw;
        private readonly IHostService _service;
        private Action<string> _nameCallback = null;

        #endregion
        #region Public Members

        /// <summary>
        /// Defines the minimal time between recycles fired by changes to the file system. Since the file system fires multiple events for each change, it is important to keep this non-zero.
        /// 1 second by default.
        /// </summary>
        public TimeSpan MinTimeBetweenRecycles = TimeSpan.FromSeconds(1);
        /// <summary>
        /// An optional delay between the change to the file system and the actual recycle. Zero by default.
        /// </summary>
        public TimeSpan RecycleDelay = TimeSpan.Zero;
        /// <summary>
        /// Called on recycling, with the name of the loaded service. Can be used for logging recycles, for example. No action by default.
        /// </summary>
        public Action<string> NameCallback
        {
            get
            {
                return _nameCallback ?? NoAction;
            }
            set
            {
                _nameCallback = value;
            }
        }

        #endregion

        #region Ctors
        public FileSystemRecycler(HostService service)
        {
            _service = service;
            fsw = new FileSystemWatcher("./", "*.config");
        }
        public FileSystemRecycler() : this(new HostService())
        {
        } 
        #endregion

        public void Start()
        {
            _service.Initialize();
            if (NameCallback != null)
            {
                NameCallback(_service.GetName());
            };
            RecycleOnConfigChange(_service);
        }

        public void Stop()
        {
            fsw.EnableRaisingEvents = false;
            _service.Stop();
        }

        private void RecycleOnConfigChange(IHostService host)
        {
            DateTime lastEventTime = DateTime.Now;

            fsw.Changed += (o, e) =>
             {
                 if (DateTime.Now - lastEventTime > MinTimeBetweenRecycles)
                 {
                     if (RecycleDelay != TimeSpan.Zero)
                     {
                         Thread.Sleep(RecycleDelay);
                     }
                     lastEventTime = DateTime.Now;
                     host.Recycle();
                     if (NameCallback != null)
                     {
                         NameCallback(_service.GetName());
                     };
                 }
             };
            fsw.EnableRaisingEvents = true;
        }

        private void NoAction(string s) { }
    }
}
