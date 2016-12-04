using Connector;
using FirstDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Impl
{
    public class Service : MarshalByRefObject,IRecycableService
    {
        public ADependency o;
        Timer timer;

        public Service()
        {
            o = new ADependency();
            timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += (o, e) => HandleTick();
        }
        public  string GetName()
        {
            return "First" + o.Something();
        }
        private void HandleTick()
        {
            Console.WriteLine("First tick...");
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
