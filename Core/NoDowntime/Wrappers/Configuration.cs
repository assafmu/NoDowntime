using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime.Wrappers
{
    internal class Configuration : IConfiguration
    {
        public NameValueCollection AppSettings
        {
            get
            {
                return ConfigurationManager.AppSettings;
            }
        }

        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                return ConfigurationManager.ConnectionStrings;
            }
        }

        public object GetSection(string sectionName)
        {
            return ConfigurationManager.GetSection(sectionName);
        }

        public void RefreshSection(string sectionName)
        {
            ConfigurationManager.RefreshSection(sectionName);
        }
    }
}
