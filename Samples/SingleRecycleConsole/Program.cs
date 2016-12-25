using System;
using NoDowntime;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recyclers;
using static SingleRecycleConsole.HelperMethods;

namespace SingleRecycleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunFileSystemRecycler();
            RunSingleRecycle();
        }

        static void RunSingleRecycle()
        {
            CopyDll("FirstDependency", "FirstDependency.dll");
            CopyDll("FirstImpl", "Impl.dll");
            CopyDll("ImplCommon", "ImplCommon.dll");
            CopyDll(@"..\Core\Connector", "Connector.dll");

            HostService host = new HostService("Impl.dll", "Impl.Service");
            Console.WriteLine("Dll's will be loaded from Area1, press any key to continue...");
            Console.ReadKey(true);
            host.Initialize();
            Console.WriteLine(host.GetName());
            Console.WriteLine("New Dll's will be loaded from Area2, press any key to continue...");
            Console.ReadKey(true);

            CopyDll("SecondImpl", "Impl.dll");
            CopyDll("ImplCommon", "ImplCommon.dll");
            CopyDll(@"..\Core\Connector", "Connector.dll");
            host.Recycle();
            Console.WriteLine(host.GetName());
            Console.WriteLine("Press any key to finish");
            Console.ReadKey(true);
            host.Stop();
        }

        static void RunFileSystemRecycler()
        {
            CopyDll("FirstDependency", "FirstDependency.dll");
            CopyDll("FirstImpl", "Impl.dll");
            CopyDll("ImplCommon", "ImplCommon.dll");
            CopyDll(@"..\Core\Connector", "Connector.dll");
            HostService host = new HostService("Impl.dll","Impl.Service");
            FileSystemRecycler recycler = new FileSystemRecycler(host)
            {
                NameCallback = Console.WriteLine
            };
            Console.WriteLine("Service initializing, press any key to continue...");
            Console.ReadKey(true);
            recycler.Start();
            Console.WriteLine("Service will be reloaded on change to configuration files.");
            Console.WriteLine("Press any key to finish");
            Console.ReadKey(true);
            recycler.Stop();
        }
    }
}
