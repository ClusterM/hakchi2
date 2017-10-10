using System;

namespace com.clusterrr.clovershell
{
    public class ClovershellException : Exception
    {
        public ClovershellException(string message) : base(message) { }
    }
}
