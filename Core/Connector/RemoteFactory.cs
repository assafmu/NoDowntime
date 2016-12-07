using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace Connector
{
    public class RemoteFactory : MarshalByRefObject
    {
        private const BindingFlags bfi =
           BindingFlags.Instance | BindingFlags.Public |
           BindingFlags.CreateInstance;

        public IRecycableService Create(string folder, string assemblyFile, string typeName,
                                       params object[] constructArgs)
        {
            return (IRecycableService)Activator.CreateInstanceFrom(
               folder+"\\"+assemblyFile, typeName, false, bfi, null, constructArgs,
               null, null).Unwrap();
        }
    }
}
