using NoDowntime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers.HelperMethods;

namespace FileSystemRecycler
{
    class Program
    {
        //FileSystemWatcher can fire multiple events for what is usually considered a single event.
        //As a workaround, we ignore events within this timespan.
        static readonly TimeSpan minDelayBetweenEvents = TimeSpan.FromSeconds(1);
        
        static void Main(string[] args)
        {
            CopyDll("FirstDependency", "FirstDependency.dll");
            CopyDll("FirstImpl", "Impl.dll");

            HostService host = new HostService();
            host.Initialize();
            host.DisplayName();
            Task.Run(() => RecycleOnConfigChange(host));
            Console.WriteLine("Service is running, press any key to close");
            Console.ReadKey(true);
        }

        private static void RecycleOnConfigChange(HostService host)
        {
            var fsw = new FileSystemWatcher("./","*.config");
            DateTime lastEventTime = DateTime.Now;
            fsw.Changed += (o, e) =>
             {
                 if (DateTime.Now - lastEventTime > minDelayBetweenEvents)
                 {
                     lastEventTime = DateTime.Now;
                     Console.WriteLine("Recycling!");
                     host.Recycle();
                     host.DisplayName();
                 }
             };
            fsw.EnableRaisingEvents = true;
        }
    }
}
