using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector
{
    public abstract class State: MarshalByRefObject
    {
        public byte[] Data { get; set; }
    }
}
