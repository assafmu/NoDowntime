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
            CopyDll("FirstImpl", "Impl.dll");
            MakeNoise();

            Console.WriteLine("First complete, press any key to continue...");
            Console.ReadKey(true);
            CopyDll("SecondImpl", "SecondImpl.dll");
            MakeNoise();

            Console.WriteLine("Press any key to finish");
            Console.ReadKey(true);
        }
        private static void CopyDll(string projName, string dllName)
        {
            string dynamicLibPath = Path.Combine(Environment.CurrentDirectory, dllName);
            if (File.Exists(dynamicLibPath))
            {
                File.Delete(dynamicLibPath);
            }
            File.Copy(@"..\..\..\" + projName + @"\bin\Debug\"+dllName, dynamicLibPath);
        }
        private static void MakeNoise()
        {
            ConfigurationManager.RefreshSection("appSettings");
            string dllName = ConfigurationManager.AppSettings["RootDll"];
            string className = ConfigurationManager.AppSettings["RootClass"];
            AppDomainSetup info = new AppDomainSetup();
            AppDomain ad2 = AppDomain.CreateDomain("Ad2", null, info);
            RemoteFactory factory = ad2.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory") as RemoteFactory;
            Thing obj = factory.Create(dllName, className);
            Console.WriteLine(obj.GetName());
            AppDomain.Unload(ad2);
        }
    }
}
