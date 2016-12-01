using Connector;
using System;
using System.Collections.Generic;
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
            //string connectorAssemblyName = "Connector, Version=1.0.0.0,Culture=neutral,PublicKeyToken=null";
            CopyDll("FirstImpl");
            MakeNoise();

            CopyDll("SecondImpl");
            MakeNoise();

            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
        }
        private static void CopyDll(string projName)
        {
            string dynamicLibPath = Environment.CurrentDirectory + @"\Impl.dll";
            if (File.Exists(dynamicLibPath))
            {
                File.Delete(dynamicLibPath);
            }
            File.Copy(@"..\..\..\" + projName + @"\bin\Debug\Impl.dll", dynamicLibPath);
        }
        private static void MakeNoise()
        {

            AppDomainSetup info = new AppDomainSetup();
            AppDomain ad2 = AppDomain.CreateDomain("Ad2", null, info);
            string otherAssemblyName = "Impl, Version=1.0.0.0,Culture=neutral,PublicKeyToken=null";
            RemoteFactory factory = ad2.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory") as RemoteFactory;
            Thing obj = factory.Create("Impl.dll", "Impl.Class1");
            Console.WriteLine(obj.GetName());
            AppDomain.Unload(ad2);
        }
    }
}
