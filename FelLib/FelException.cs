using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.FelLib
{
    public class FelException : Exception
    {
        public FelException(string message) : base(message)
        {
        }
    }
}
