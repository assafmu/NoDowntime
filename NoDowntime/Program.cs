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
    class Program
    {
        static void Main(string[] args)
        {
            CopyDll("FirstImpl", "Impl.dll", "Area1");

            HostService host = new HostService();
            host.Initialize();
            host.DisplayName();
            Console.WriteLine("Edit the configuration to the new DLL, press any key to continue...");
            Console.ReadKey(true);
            CopyDll("SecondImpl", "SecondImpl.dll", "Area2");
            host.Recycle();
            host.DisplayName();
            Console.WriteLine("Press any key to finish");
            Console.ReadKey(true);
        }
        private static void CopyDll(string projName, string dllName, string areaFolder)
        {
            string dynamicLibPath = Path.Combine(Environment.CurrentDirectory, areaFolder, dllName);
            if (File.Exists(dynamicLibPath))
            {
                File.Delete(dynamicLibPath);
            }
            File.Copy(@"..\..\..\" + projName + @"\bin\Debug\" + dllName, dynamicLibPath);
        }

    }
}
