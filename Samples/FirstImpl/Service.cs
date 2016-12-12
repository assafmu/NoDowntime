using Connector;
using ImplCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Impl
{
    public class Service : StatelessRecycableService
    {
        private int counter = 0;
        Timer timer;

        public Service()
        {
            timer = new Timer();
            timer.Interval = 3000;
            timer.Elapsed += (o, e) => HandleTick();
        }
        public override string GetName()
        {
            return "First";
        }
        private void HandleTick()
        {
            counter++;
            Console.WriteLine("First tick...");
        }

        public override void Start()
        {
            timer.Start();
        }

        public override void Stop()
        {
            Console.WriteLine("Total of {0} ticks", counter);
            timer.Stop();
        }
        public override State GetState()
        {
            return new EmptyState()
            {
                Data = Encoding.UTF8.GetBytes(counter.ToString())
            };
        }
    }
}
