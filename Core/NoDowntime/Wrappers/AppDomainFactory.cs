using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime.Wrappers
{
    internal class AppDomainFactory : IAppDomainFactory
    {
        public IApplicationDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup info)
        {
            return new ApplicationDomain(AppDomain.CreateDomain(friendlyName, securityInfo, info));
        }
    }
    internal interface IAppDomainFactory
    {
        IApplicationDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup info);
    }
}
