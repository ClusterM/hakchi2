using System;
using System.Net;

namespace mooftpserv
{
    /// <summary>
    /// Interface for a logger. Methods should be self-explanatory.
    /// </summary>
    public interface ILogHandler
    {
        /// <summary>
        /// Make a new instance for a new session with the given peer.
        /// Each FTP session uses a separate, cloned instance.
        /// </summary>
        ILogHandler Clone(IPEndPoint peer);

        void NewControlConnection();
        void ClosedControlConnection();
        void ReceivedCommand(string verb, string arguments);
        void SentResponse(uint code, string description);
        void NewDataConnection(IPEndPoint remote, IPEndPoint local, bool passive);
        void ClosedDataConnection(IPEndPoint remote, IPEndPoint local, bool passive);
    }
}

