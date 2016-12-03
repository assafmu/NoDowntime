using Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondImpl
{
    public class Service : MarshalByRefObject, IRecycableService
    {
        public string GetName()
        {
            return "Second";
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
