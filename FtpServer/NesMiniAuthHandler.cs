using System;
using System.Net;

namespace mooftpserv
{
    public class NesMiniAuthHandler : IAuthHandler
    {
        private IPEndPoint peer;

        public NesMiniAuthHandler()
        {
        }

        private NesMiniAuthHandler(IPEndPoint peer)
        {
          this.peer = peer;
        }

        public IAuthHandler Clone(IPEndPoint peer)
        {
            return new NesMiniAuthHandler(peer);
        }

        public bool AllowLogin(string user, string pass)
        {
            return (user == "root" && pass == "clover");
        }

        public bool AllowControlConnection()
        {
            return true;
        }

        public bool AllowActiveDataConnection(IPEndPoint port)
        {
            // only allow active connections to the same peer as the control connection
            return peer.Address.Equals(port.Address);
        }
    }
}

