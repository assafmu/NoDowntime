using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector
{
    /// <summary>
    /// An abstract class implementing the IRecycableService interface, ignoring state hand-over. 
    /// 
    /// </summary>
    public abstract class StatelessRecycableService : MarshalByRefObject, IRecycableService
    {
        public abstract string GetName();

        public virtual State GetState()
        {
            return null;
        }

        public virtual void SetState(State state)
        {
            
        }

        public abstract void Start();
        public abstract void Stop();
    }
}
