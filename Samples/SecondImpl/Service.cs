using Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Impl
{
    public class Service : MarshalByRefObject, IRecycableService
    {
        Timer timer;
        public Service()
        {
            timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += (o,e) => HandleTick();
        }

        private void HandleTick()
        {
            Console.WriteLine("Second tick...");
        }

        public string GetName()
        {
            return "Second";
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
