﻿using Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impl
{
    public class Class1 : MarshalByRefObject,Thing
    {
        public  string GetName()
        {
            return "First";
        }
    }
}
