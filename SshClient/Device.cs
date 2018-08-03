using System.Collections.Generic;
using System.Net;

namespace com.clusterrr.ssh
{
    public class Device
    {
        public IList<IPAddress> Addresses;
        public ushort Port;
        public string UniqueID;
        public string ConsoleType;
        public string ConsoleRegion;
    }
}
