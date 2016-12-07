using Connector;
using System;
using NoDowntime;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleRecycleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CopyDll("FirstDependency", "FirstDependency.dll");
            CopyDll("FirstImpl", "Impl.dll");

            HostService host = new HostService();
            Console.WriteLine("Dll's will be loaded from Area1, press any key to continue...");
            Console.ReadKey(true);
            host.Initialize();
            host.DisplayName();
            Console.WriteLine("New Dll's will be loaded from Area2, press any key to continue...");
            Console.ReadKey(true);
            CopyDll("SecondImpl", "Impl.dll");
            host.Recycle();
            host.DisplayName();
            Console.WriteLine("Press any key to finish");
            Console.ReadKey(true);
        }

        private static void CopyDll(string projName, string dllName)
        {
            string targetFolder = "Staging";
            string dynamicLibPath = Path.Combine(Environment.CurrentDirectory, targetFolder, dllName);
            if (File.Exists(dynamicLibPath))
            {
                File.Delete(dynamicLibPath);
            }
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, targetFolder)))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, targetFolder));
            }
            File.Copy(@"..\..\..\" + projName + @"\bin\Debug\" + dllName, dynamicLibPath);
        }

    }
}
