using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector
{
    public interface IRecycableService
    {
        string GetName();
        void Start();
        void Stop();
        State GetState();
        void SetState(State state);
    }
}
