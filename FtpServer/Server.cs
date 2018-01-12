using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace mooftpserv
{
    /// <summary>
    /// Main FTP server class. Manages the server socket, creates sessions.
    /// Can be used to configure the server.
    /// </summary>
    public class Server
    {
        // default buffer size for send/receive buffers
        private const int DEFAULT_BUFFER_SIZE = 64 * 1024;
        // default port for the server socket
        private const int DEFAULT_PORT = 21;

        private IPEndPoint endpoint;
        private int bufferSize = DEFAULT_BUFFER_SIZE;
        private IAuthHandler authHandler = null;
        private IFileSystemHandler fsHandler = null;
        private ILogHandler logHandler = null;
        private TcpListener socket = null;
        private List<Session> sessions;

        public Server()
        {
            this.endpoint = new IPEndPoint(GetDefaultAddress(), DEFAULT_PORT);
            this.sessions = new List<Session>();
        }

        /// <summary>
        /// Gets or sets the local end point on which the server will listen.
        /// Has to be an IPv4 endpoint.
        /// The default value is IPAdress.Any and port 21, except on WinCE,
        /// where the first non-loopback IPv4 address will be used.
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get { return endpoint; }
            set { endpoint = value; }
        }

        /// <summary>
        /// Gets or sets the local IP address on which the server will listen.
        /// Has to be an IPv4 address.
        /// If none is set, IPAddress.Any will be used, except on WinCE,
        /// where the first non-loopback IPv4 address will be used.
        /// </summary>
        public IPAddress LocalAddress
        {
            get { return endpoint.Address; }
            set { endpoint.Address = value; }
        }

        /// <summary>
        /// Gets or sets the local port on which the server will listen.
        /// The default value is 21. Note that on Linux, only root can open ports < 1024.
        /// </summary>
        public int LocalPort
        {
            get { return endpoint.Port; }
            set { endpoint.Port = value; }
        }

        /// <summary>
        /// Gets or sets the size of the send/receive buffer to be used by each session/connection.
        /// The default value is 64k.
        /// </summary>
        public int BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the auth handler that is used to check user credentials.
        /// If none is set, a DefaultAuthHandler will be created when the server starts.
        /// </summary>
        public IAuthHandler AuthHandler
        {
            get { return authHandler; }
            set { authHandler = value; }
        }

        /// <summary>
        /// Gets or sets the file system handler that implements file system access for FTP commands.
        /// If none is set, a DefaultFileSystemHandler is created when the server starts.
        /// </summary>
        public IFileSystemHandler FileSystemHandler
        {
            get { return fsHandler; }
            set { fsHandler = value; }
        }

        /// <summary>
        /// Gets or sets the log handler. Can be null to disable logging.
        /// The default value is null.
        /// </summary>
        public ILogHandler LogHandler
        {
            get { return logHandler; }
            set { logHandler = value; }
        }

        /// <summary>
        /// Run the server. The method will not return until Stop() is called.
        /// </summary>
        public void Run()
        {
            //if (authHandler == null)
            //    authHandler = new DefaultAuthHandler();

            //if (fsHandler == null)
            //    fsHandler = new DefaultFileSystemHandler();

            if (socket == null)
                socket = new TcpListener(endpoint);

            socket.Start();

            // listen for new connections
            try {
                while (true)
                {
                    Socket peer = socket.AcceptSocket();

                    IPEndPoint peerPort = (IPEndPoint) peer.RemoteEndPoint;
                    Session session = new Session(peer, bufferSize,
                                                  authHandler.Clone(peerPort),
                                                  fsHandler.Clone(peerPort),
                                                  logHandler.Clone(peerPort));

                    session.Start();
                    sessions.Add(session);

                    // purge old sessions
                    for (int i = sessions.Count - 1; i >= 0; --i)
                    {
                        if (!sessions[i].IsOpen) {
                            sessions.RemoveAt(i);
                            --i;
                        }
                    }
                }
            } catch (SocketException) {
                // ignore, Stop() will probably cause this exception
            } finally {
                // close all running connections
                foreach (Session s in sessions) {
                    s.Stop();
                }
            }
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            if (socket == null) return;
            socket.Stop();
        }

        /// <summary>
        /// Get the default address, which is IPAddress.Any everywhere except on WinCE,
        /// where all local addresses are enumerated and the first non-loopback IP is used.
        /// </summary>
        private IPAddress GetDefaultAddress()
        {
            // on WinCE, 0.0.0.0 does not work because for accepted sockets,
            // LocalEndPoint would also say 0.0.0.0 instead of the real IP
#if WindowsCE
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress bindIp = IPAddress.Loopback;
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip)) {
                    return ip;
                }
            }

            return IPAddress.Loopback;
#else
            return IPAddress.Any;
#endif
        }
    }
}
