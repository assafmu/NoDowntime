using Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime
{
    public class HostService
    {
        #region fields
        AppDomainSetup info;
        AppDomain ad2;
        IRecycableService currentService;
        bool area1Loaded = false;
        #endregion
        #region Constructor
        public HostService()
        {

            info = new AppDomainSetup();
        }
        #endregion
        
        public void Initialize()
        {
            Load();
        }
        
        public void Recycle()
        {
            Unload();
            Load();
        }

        public void DisplayName()
        {
            Console.WriteLine(currentService.GetName());
        }

        internal void Unload()
        {
            currentService.Stop();
            //Save the state
            currentService = null;
            AppDomain.Unload(ad2);
        }

        internal void Load()
        {
            ConfigurationManager.RefreshSection("appSettings");
            string dllName = ConfigurationManager.AppSettings["RootDll"];
            string className = ConfigurationManager.AppSettings["RootClass"];
            ad2 = AppDomain.CreateDomain("Ad2", null, info);
            RemoteFactory factory = ad2.CreateInstanceAndUnwrap("Connector", "Connector.RemoteFactory") as RemoteFactory;
            string areaToLoad = area1Loaded ? "Area2" : "Area1";
            currentService = factory.Create(areaToLoad, dllName, className);
            area1Loaded = !area1Loaded;
        }


    }
}
