using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.clovershell
{
    public class ClovershellException : Exception
    {
        public ClovershellException(string message) : base(message) { }
    }
}
