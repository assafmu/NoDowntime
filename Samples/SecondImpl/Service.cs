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
            return "Second";
        }
        private void HandleTick()
        {
            counter++;
            Console.WriteLine("Second tick...");
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
        public override void SetState(State state)
        {
            counter = int.Parse(Encoding.UTF8.GetString(state.Data));
        }
    }
}
