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
    public class Service : RecycableServiceBase
    {
        public State _state;
        Timer timer;

        public Service()
        {
            _state = new IntState();
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
            //_state.Number += 1;
            _state.Bar += 1;
            Console.WriteLine("First tick...");
        }

        public override void Start()
        {
            timer.Start();
        }

        public override void Stop()
        {
            Console.WriteLine("Total of {0} ticks",_state.Bar);
            timer.Stop();
        }
        public override State GetState()
        {
            return _state;
        }
    }
}
