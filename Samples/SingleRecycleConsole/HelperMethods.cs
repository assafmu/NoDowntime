using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleRecycleConsole
{
    public static class HelperMethods
    {
        public static void CopyDll(string projName, string dllName)
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
