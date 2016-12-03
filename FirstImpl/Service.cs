using Connector;
using FirstDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impl
{
    public class Service : MarshalByRefObject,IRecycableService
    {
        public ADependency o;

        public Service()
        {
            o = new ADependency();
        }
        public  string GetName()
        {
            return "First" + o.Something();
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
