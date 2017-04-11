using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace mooftpserv
{
    /// <summary>
    /// Default log handler.
    ///
    public class DebugLogHandler : ILogHandler
    {
        private IPEndPoint peer;

        public DebugLogHandler()
        {
        }

        private DebugLogHandler(IPEndPoint peer)
        {
            this.peer = peer;
        }

        public ILogHandler Clone(IPEndPoint peer)
        {
            return new DebugLogHandler(peer);
        }

        private void Write(string format, params object[] args)
        {
            Debug.WriteLine(String.Format("{0}: {1}", peer, String.Format(format, args)));
        }

        public void NewControlConnection()
        {
            Write("new control connection");
        }

        public void ClosedControlConnection()
        {
            Write("closed control connection");
        }

        public void ReceivedCommand(string verb, string arguments)
        {
#if VERY_DEBUG
            string argtext = (arguments == null || arguments == "" ? "" : ' ' + arguments);
            Write("received command: {0}{1}", verb, argtext);
#endif
        }

        public void SentResponse(uint code, string description)
        {
#if VERY_DEBUG
            Write("sent response: {0} {1}", code, description);
#endif
        }

        public void NewDataConnection(IPEndPoint remote, IPEndPoint local, bool passive)
        {
#if VERY_DEBUG
            Write("new data connection: {0} <-> {1} ({2})", remote, local, (passive ? "passive" : "active"));
#endif
        }

        public void ClosedDataConnection(IPEndPoint remote, IPEndPoint local, bool passive)
        {
#if VERY_DEBUG
            Write("closed data connection: {0} <-> {1} ({2})", remote, local, (passive ? "passive" : "active"));
#endif
        }
    }
}

