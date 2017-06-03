using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime.Wrappers
{
    internal class ApplicationDomain : IApplicationDomain
    {
        private AppDomain appDomain;

        public ApplicationDomain(AppDomain appDomain)
        {
            this.appDomain = appDomain;
        }

        public virtual void Unload()
        {
            AppDomain.Unload(appDomain);
        }

        public virtual object CreateInstanceAndUnwrap(string assemblyName, string typeName)
        {
            return appDomain.CreateInstanceAndUnwrap(assemblyName, typeName);
        }
    }

    internal interface IApplicationDomain
    {
        void Unload();
        object CreateInstanceAndUnwrap(string assemblyName, string typeName);
    }
}
