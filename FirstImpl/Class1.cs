using Connector;
using FirstDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impl
{
    public class Class1 : MarshalByRefObject,Thing
    {
        public ADependency o;

        public Class1()
        {
            o = new ADependency();
        }
        public  string GetName()
        {
            return "First" + o.Something();
        }
    }
}
