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

        public Thing Create(string assemblyFile, string typeName,
                                       params object[] constructArgs)
        {
            return (Thing)Activator.CreateInstanceFrom(
               assemblyFile, typeName, false, bfi, null, constructArgs,
               null, null).Unwrap();
        }
    }
}
