using NoDowntime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recyclers
{
    public class FileSystemRecycler
    {
        //FileSystemWatcher can fire multiple events for what is usually considered a single event.
        //As a workaround, we ignore events within this timespan.
        static readonly TimeSpan minDelayBetweenEvents = TimeSpan.FromSeconds(1);
        private readonly FileSystemWatcher fsw;
        private readonly HostService _service;
        private Action<string> _nameCallback = null;

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

        public FileSystemRecycler(HostService service)
        {
            _service = service;
            fsw = new FileSystemWatcher("./", "*.config");
        }
        public FileSystemRecycler():this(new HostService())
        {
        }
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

        private void RecycleOnConfigChange(HostService host)
        {
            DateTime lastEventTime = DateTime.Now;
            fsw.Changed += (o, e) =>
             {
                 if (DateTime.Now - lastEventTime > minDelayBetweenEvents)
                 {
                     lastEventTime = DateTime.Now;
                     Console.WriteLine("Recycling!");
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
