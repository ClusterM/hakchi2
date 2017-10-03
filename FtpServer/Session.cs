using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace mooftpserv
{
    /// <summary>
    /// FTP session/connection. Does all the heavy lifting of the FTP protocol.
    /// Reads commands, sends replies, manages data connections, and so on.
    /// Each session creates its own thread.
    /// </summary>
    class Session
    {
        // transfer data type, ascii or binary
        enum DataType { ASCII, IMAGE };

        // buffer size to use for reading commands from the control connection
        private static int CMD_BUFFER_SIZE = 4096;
        // version from AssemblyInfo
        private static string LIB_VERSION = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
        // monthnames for LIST command, since DateTime returns localized names
        private static string[] MONTHS = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        // response text for initial response. preceeded by application name and version number.
        private static string[] HELLO_TEXT = { "hakchi2 FTP server" };
        // response text for general ok messages
        private static string[] OK_TEXT = { "Sounds good.", "Success!", "Alright, I'll do it...", "Consider it done." };
        // Result for FEAT command
        private static string[] FEATURES = { "MDTM", "MLST modify*;perm*;size*;type*;unique*;UNIX.mode;", "PASV", "MFMT", "SIZE", "TVFS", "UTF8" };

        // local EOL flavor
        private static byte[] localEolBytes = Encoding.ASCII.GetBytes(Environment.NewLine);
        // FTP-mandated EOL flavor (= CRLF)
        private static byte[] remoteEolBytes = Encoding.ASCII.GetBytes("\r\n");
        // on Windows, no ASCII conversion is necessary (CRLF == CRLF)
        private static bool noAsciiConv = (localEolBytes == remoteEolBytes);

        // socket for the control connection
        private Socket controlSocket;
        // buffer size to use for sending/receiving with data connections
        private int dataBufferSize;
        // auth handler, checks user credentials
        private IAuthHandler authHandler;
        // file system handler, implements file system access for the FTP commands
        private IFileSystemHandler fsHandler;
        // log handler, used for diagnostic logging output. can be null.
        private ILogHandler logHandler;
        // Session thread, the control and data connections are processed in this thread
        private Thread thread;

        // .NET CF does not have Thread.IsAlive, so this flag replaces it
        private bool threadAlive = false;
        // Random Number Generator for OK and HELLO texts
        private Random randomTextIndex;
        // flag for whether the user has successfully logged in
        private bool loggedIn = false;
        // name of the logged in user, also used to remember the username when waiting for the PASS command
        private string loggedInUser = null;
        // argument of pending RNFR command, when waiting for an RNTO command
        private string renameFromPath = null;

        // remote data port. null when PASV is used.
        private IPEndPoint dataPort = null;
        // socket for data connections
        private Socket dataSocket = null;
        // .NET CF does not have Socket.Bound, so this flag replaces it
        private bool dataSocketBound = false;
        // buffer for reading from the control connection
        private byte[] cmdRcvBuffer;
        // number of bytes in the cmdRcvBuffer
        private int cmdRcvBytes;
        // buffer for sending/receiving with data connections
        private byte[] dataBuffer;
        // data type of the session, can be changed by the client
        private DataType transferDataType = DataType.ASCII;

        /// <summary>
        /// Creates a new session, which can afterwards be started with Start().
        /// </summary>
        public Session(Socket socket, int bufferSize, IAuthHandler authHandler, IFileSystemHandler fileSystemHandler, ILogHandler logHandler)
        {
            this.controlSocket = socket;
            this.dataBufferSize = bufferSize;
            this.authHandler = authHandler;
            this.fsHandler = fileSystemHandler;
            this.logHandler = logHandler;

            this.cmdRcvBuffer = new byte[CMD_BUFFER_SIZE];
            this.cmdRcvBytes = 0;
            this.dataBuffer = new byte[dataBufferSize + 1]; // +1 for partial EOL
            this.randomTextIndex = new Random();

            this.thread = new Thread(new ThreadStart(this.Work));
        }

        /// <summary>
        /// Indicates whether the session is still open
        /// </summary>
        public bool IsOpen
        {
            get { return threadAlive; }
        }

        /// <summary>
        /// Start the session in a new thread
        /// </summary>
        public void Start()
        {
            if (!threadAlive)
            {
                this.thread.Start();
                threadAlive = true;
            }
        }

        /// <summary>
        /// Stop the session
        /// </summary>
        public void Stop()
        {
            if (threadAlive)
            {
                threadAlive = false;
                thread.Abort();
            }

            if (controlSocket.Connected)
                controlSocket.Close();

            if (dataSocket != null && dataSocket.Connected)
                dataSocket.Close();
        }

        /// <summary>
        /// Main method of the session thread.
        /// Reads commands and executes them.
        /// </summary>
        private void Work()
        {
            if (logHandler != null)
                logHandler.NewControlConnection();

            try
            {
                if (!authHandler.AllowControlConnection())
                {
                    Respond(421, "Control connection refused.");
                    // first flush, then close
                    controlSocket.Shutdown(SocketShutdown.Both);
                    controlSocket.Close();
                    return;
                }

                Respond(220, String.Format("This is mooftpserv v{0}. {1}", LIB_VERSION, GetRandomText(HELLO_TEXT)));

                // allow anonymous login?
                if (authHandler.AllowLogin(null, null))
                {
                    loggedIn = true;
                }

                while (controlSocket.Connected)
                {
                    string verb;
                    string args;
                    if (!ReadCommand(out verb, out args))
                    {
                        if (controlSocket.Connected)
                        {
                            // assume clean disconnect if there are no buffered bytes
                            if (cmdRcvBytes != 0)
                                Respond(500, "Failed to read command, closing connection.");
                            controlSocket.Close();
                        }
                        break;
                    }
                    else if (verb.Trim() == "")
                    {
                        // ignore empty lines
                        continue;
                    }

                    try
                    {
                        if (loggedIn)
                            ProcessCommand(verb, args);
                        else if (verb == "QUIT")
                        { // QUIT should always be allowed
                            Respond(221, "Bye.");
                            // first flush, then close
                            controlSocket.Shutdown(SocketShutdown.Both);
                            controlSocket.Close();
                        }
                        else
                        {
                            HandleAuth(verb, args);
                        }
                    }
                    catch (Exception ex)
                    {
                        Respond(500, ex);
                    }
                }
            }
            catch (Exception)
            {
                // catch any uncaught stuff, the server should not throw anything
            }
            finally
            {
                if (controlSocket.Connected)
                    controlSocket.Close();

                if (logHandler != null)
                    logHandler.ClosedControlConnection();

                threadAlive = false;
            }
        }

        /// <summary>
        /// Process an FTP command.
        /// </summary>
        private void ProcessCommand(string verb, string arguments)
        {
            switch (verb)
            {
                case "SYST":
                    {
                        Respond(215, "UNIX emulated by mooftpserv");
                        break;
                    }
                case "QUIT":
                    {
                        Respond(221, "Bye.");
                        // first flush, then close
                        controlSocket.Shutdown(SocketShutdown.Both);
                        controlSocket.Close();
                        break;
                    }
                case "USER":
                    {
                        Respond(230, "You are already logged in.");
                        break;
                    }
                case "PASS":
                    {
                        Respond(230, "You are already logged in.");
                        break;
                    }
                case "FEAT":
                    {
                        Respond(211, "Features:\r\n " + String.Join("\r\n ", FEATURES), true);
                        Respond(211, "Features done.");
                        break;
                    }
                case "OPTS":
                    {
                        // Windows Explorer uses lowercase args
                        if (arguments != null && arguments.ToUpper() == "UTF8 ON")
                            Respond(200, "Always in UTF8 mode.");
                        else
                            Respond(504, "Unknown option.");
                        break;
                    }
                case "TYPE":
                    {
                        if (arguments == "A" || arguments == "A N")
                        {
                            transferDataType = DataType.ASCII;
                            Respond(200, "Switching to ASCII mode.");
                        }
                        else if (arguments == "I")
                        {
                            transferDataType = DataType.IMAGE;
                            Respond(200, "Switching to BINARY mode.");
                        }
                        else
                        {
                            Respond(500, "Unknown TYPE arguments.");
                        }
                        break;
                    }
                case "PORT":
                    {
                        IPEndPoint port = ParseAddress(arguments);
                        if (port == null)
                        {
                            Respond(500, "Invalid host-port format.");
                            break;
                        }

                        if (!authHandler.AllowActiveDataConnection(port))
                        {
                            Respond(500, "PORT arguments refused.");
                            break;
                        }

                        dataPort = port;
                        CreateDataSocket(false);
                        Respond(200, GetRandomText(OK_TEXT));
                        break;
                    }
                case "PASV":
                    {
                        dataPort = null;

                        try
                        {
                            CreateDataSocket(true);
                        }
                        catch (Exception ex)
                        {
                            Respond(500, ex);
                            break;
                        }

                        string port = FormatAddress((IPEndPoint)dataSocket.LocalEndPoint);
                        Respond(227, String.Format("Switched to passive mode ({0})", port));
                        break;
                    }
                case "XPWD":
                case "PWD":
                    {
                        ResultOrError<string> ret = fsHandler.GetCurrentDirectory();
                        if (ret.HasError)
                            Respond(500, ret.Error);
                        else
                            Respond(257, EscapePath(ret.Result));
                        break;
                    }
                case "XCWD":
                case "CWD":
                    {
                        ResultOrError<string> ret = fsHandler.ChangeDirectory(arguments);
                        if (ret.HasError)
                            Respond(550, ret.Error);
                        else
                            Respond(200, GetRandomText(OK_TEXT));
                        break;
                    }
                case "XCUP":
                case "CDUP":
                    {
                        ResultOrError<string> ret = fsHandler.ChangeDirectory("..");
                        if (ret.HasError)
                            Respond(550, ret.Error);
                        else
                            Respond(200, GetRandomText(OK_TEXT));
                        break;
                    }
                case "XMKD":
                case "MKD":
                    {
                        ResultOrError<string> ret = fsHandler.CreateDirectory(arguments);
                        if (ret.HasError)
                            Respond(550, ret.Error);
                        else
                            Respond(257, EscapePath(ret.Result));
                        break;
                    }
                case "XRMD":
                case "RMD":
                    {
                        ResultOrError<bool> ret = fsHandler.RemoveDirectory(arguments);
                        if (ret.HasError)
                            Respond(550, ret.Error);
                        else
                            Respond(250, GetRandomText(OK_TEXT));
                        break;
                    }
                case "RETR":
                    {
                        ResultOrError<Stream> ret = fsHandler.ReadFile(arguments);
                        if (ret.HasError)
                        {
                            Respond(550, ret.Error);
                            break;
                        }

                        SendData(ret.Result);
                        break;
                    }
                case "STOR":
                    {
                        ResultOrError<Stream> ret = fsHandler.WriteFile(arguments);
                        if (ret.HasError)
                        {
                            Respond(550, ret.Error);
                            break;
                        }
                        ReceiveData(ret.Result);
                        var ret2 = fsHandler.WriteFileFinalize(arguments, ret.Result);
                        if (ret2.HasError)
                        {
                            Respond(550, ret2.Error);
                            break;
                        }
                        break;
                    }
                case "DELE":
                    {
                        ResultOrError<bool> ret = fsHandler.RemoveFile(arguments);
                        if (ret.HasError)
                            Respond(550, ret.Error);
                        else
                            Respond(250, GetRandomText(OK_TEXT));
                        break;
                    }
                case "RNFR":
                    {
                        if (arguments == null || arguments.Trim() == "")
                        {
                            Respond(500, "Empty path is invalid.");
                            break;
                        }

                        renameFromPath = arguments;
                        Respond(350, "Waiting for target path.");
                        break;
                    }
                case "RNTO":
                    {
                        if (renameFromPath == null)
                        {
                            Respond(503, "Use RNFR before RNTO.");
                            break;
                        }

                        ResultOrError<bool> ret = fsHandler.RenameFile(renameFromPath, arguments);
                        renameFromPath = null;
                        if (ret.HasError)
                            Respond(550, ret.Error);
                        else
                            Respond(250, GetRandomText(OK_TEXT));
                        break;
                    }
                case "MDTM":
                    {
                        ResultOrError<DateTime> ret = fsHandler.GetLastModifiedTimeUtc(arguments);
                        if (ret.HasError)
                            Respond(550, ret.Error);
                        else
                            Respond(213, FormatTime(EnsureUnixTime(ret.Result)));
                        break;
                    }
                case "SIZE":
                    {
                        ResultOrError<long> ret = fsHandler.GetFileSize(arguments);
                        if (ret.HasError)
                            Respond(550, ret.Error);
                        else
                            Respond(213, ret.Result.ToString());
                        break;
                    }
                case "LIST":
                    {
                        // apparently browsers like to pass arguments to LIST
                        // assuming they are passed through to the UNIX ls command
                        /*
                        arguments = RemoveLsArgs(arguments);
                        
                        ResultOrError<FileSystemEntry[]> ret = fsHandler.ListEntries(arguments);
                        if (ret.HasError)
                        {
                            Respond(500, ret.Error);
                            break;
                        }

                        SendData(MakeStream(FormatDirList(ret.Result)));
                         */
                        ResultOrError<string> ret = fsHandler.ListEntriesRaw(arguments);
                        if (ret.HasError)
                        {
                            Respond(500, ret.Error);
                            break;
                        }

                        SendData(MakeStream(ret.Result));

                        break;
                    }
                case "STAT":
                    {
                        if (arguments == null || arguments.Trim() == "")
                        {
                            Respond(504, "Not implemented for these arguments.");
                            break;
                        }

                        arguments = RemoveLsArgs(arguments);

                        ResultOrError<FileSystemEntry[]> ret = fsHandler.ListEntries(arguments);
                        if (ret.HasError)
                        {
                            Respond(500, ret.Error);
                            break;
                        }

                        Respond(213, "Status:\r\n" + FormatDirList(ret.Result), true);
                        Respond(213, "Status done.");
                        break;
                    }
                case "NLST":
                    {
                        // remove common arguments, we do not support any of them
                        arguments = RemoveLsArgs(arguments);

                        ResultOrError<FileSystemEntry[]> ret = fsHandler.ListEntries(arguments);
                        if (ret.HasError)
                        {
                            Respond(500, ret.Error);
                            break;
                        }

                        SendData(MakeStream(FormatNLST(ret.Result)));
                        break;
                    }
                case "MLSD":
                case "MLST":
                    {
                        ResultOrError<FileSystemEntry[]> ret = fsHandler.ListEntries(arguments);
                        if (ret.HasError)
                        {
                            Respond(500, ret.Error);
                            break;
                        }

                        SendData(MakeStream(FormatMLST(ret.Result)));
                        break;
                    }
                case "MFMT":
                    {
                        string[] tokens = arguments.Split(' ');
                        var time = DateTime.ParseExact(tokens[0], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                        var file = (tokens.Length > 1 ? String.Join(" ", tokens, 1, tokens.Length - 1) : null);
                        fsHandler.SetLastModifiedTimeUtc(file, time);
                        Respond(213, string.Format("213 Modify={0}; {1}", tokens[0], file));
                        break;
                    }
                case "NOOP":
                    {
                        Respond(200, GetRandomText(OK_TEXT));
                        break;
                    }
                case "SITE":
                    {
                        string[] tokens = arguments.Split(' ');
                        var newverb = tokens[0].ToUpper(); // commands are case insensitive
                        var newargs = (tokens.Length > 1 ? String.Join(" ", tokens, 1, tokens.Length - 1) : null);
                        ProcessCommand(newverb, newargs);
                        break;
                    }
                case "CHMOD":
                    {
                        string[] tokens = arguments.Split(' ');
                        var mode = tokens[0].ToUpper(); // commands are case insensitive
                        var file = (tokens.Length > 1 ? String.Join(" ", tokens, 1, tokens.Length - 1) : "");
                        ResultOrError<bool> ret = fsHandler.ChmodFile(mode, file);
                        if (ret.HasError)
                            Respond(550, ret.Error);
                        else
                            Respond(250, GetRandomText(OK_TEXT));
                        break;
                    }
                default:
                    {
                        Respond(500, "Unknown command.");
                        break;
                    }
            }
        }

        /// <summary>
        /// Read a command from the control connection.
        /// </summary>
        /// <returns>
        /// True if a command was read.
        /// </returns>
        /// <param name='verb'>
        /// Will receive the verb of the command.
        /// </param>
        /// <param name='args'>
        /// Will receive the arguments of the command, or null.
        /// </param>
        private bool ReadCommand(out string verb, out string args)
        {
            verb = null;
            args = null;

            int endPos = -1;
            // can there already be a command in the buffer?
            if (cmdRcvBytes > 0)
                Array.IndexOf(cmdRcvBuffer, (byte)'\n', 0, cmdRcvBytes);

            try
            {
                // read data until a newline is found
                do
                {
                    int freeBytes = cmdRcvBuffer.Length - cmdRcvBytes;
                    int bytes = controlSocket.Receive(cmdRcvBuffer, cmdRcvBytes, freeBytes, SocketFlags.None);
                    if (bytes <= 0)
                        break;

                    cmdRcvBytes += bytes;

                    // search \r\n
                    endPos = Array.IndexOf(cmdRcvBuffer, (byte)'\r', 0, cmdRcvBytes);
                    if (endPos != -1 && (cmdRcvBytes <= endPos + 1 || cmdRcvBuffer[endPos + 1] != (byte)'\n'))
                        endPos = -1;
                } while (endPos == -1 && cmdRcvBytes < cmdRcvBuffer.Length);
            }
            catch (SocketException)
            {
                // in case the socket is closed or has some other error while reading
                return false;
            }

            if (endPos == -1)
                return false;

            string command = DecodeString(cmdRcvBuffer, endPos);

            // remove the command from the buffer
            cmdRcvBytes -= (endPos + 2);
            Array.Copy(cmdRcvBuffer, endPos + 2, cmdRcvBuffer, 0, cmdRcvBytes);

            // CF is missing a limited String.Split
            string[] tokens = command.Split(' ');
            verb = tokens[0].ToUpper(); // commands are case insensitive
            args = (tokens.Length > 1 ? String.Join(" ", tokens, 1, tokens.Length - 1) : null);

            if (logHandler != null)
                logHandler.ReceivedCommand(verb, args);

            return true;
        }

        /// <summary>
        /// Send a response on the control connection
        /// </summary>
        private void Respond(uint code, string desc, bool moreFollows)
        {
            string response = code.ToString();
            if (desc != null)
                response += (moreFollows ? '-' : ' ') + desc;

            if (!response.EndsWith("\r\n"))
                response += "\r\n";

            byte[] sendBuffer = EncodeString(response);
            controlSocket.Send(sendBuffer);

            if (logHandler != null)
                logHandler.SentResponse(code, desc);
        }

        /// <summary>
        /// Send a response on the control connection
        /// </summary>
        private void Respond(uint code, string desc)
        {
            Respond(code, desc, false);
        }

        /// <summary>
        /// Send a response on the control connection, with an exception as text
        /// </summary>
        private void Respond(uint code, Exception ex)
        {
            Respond(code, ex.Message.Replace(Environment.NewLine, " "));
        }

        /// <summary>
        /// Process FTP commands when the user is not yet logged in.
        /// Mostly handles the login commands USER and PASS.
        /// </summary>
        private void HandleAuth(string verb, string args)
        {
            if (verb == "USER" && args != null)
            {
                if (authHandler.AllowLogin(args, null))
                {
                    Respond(230, "Login successful.");
                    loggedIn = true;
                }
                else
                {
                    loggedInUser = args;
                    Respond(331, "Password please.");
                }
            }
            else if (verb == "PASS")
            {
                if (loggedInUser != null)
                {
                    if (authHandler.AllowLogin(loggedInUser, args))
                    {
                        Respond(230, "Login successful.");
                        loggedIn = true;
                    }
                    else
                    {
                        loggedInUser = null;
                        Respond(530, "Login failed, please try again.");
                    }
                }
                else
                {
                    Respond(530, "No USER specified.");
                }
            }
            else
            {
                Respond(530, "Please login first.");
            }
        }

        /// <summary>
        /// Read from the given stream and send the data over a data connection
        /// </summary>
        private void SendData(Stream stream)
        {
            try
            {
                bool passive = (dataPort == null);
                using (Socket socket = OpenDataConnection())
                {
                    if (socket == null)
                        return;

                    IPEndPoint remote = (IPEndPoint)socket.RemoteEndPoint;
                    IPEndPoint local = (IPEndPoint)socket.LocalEndPoint;

                    if (logHandler != null)
                        logHandler.NewDataConnection(remote, local, passive);

                    try
                    {
                        while (true)
                        {
                            int bytes = stream.Read(dataBuffer, 0, dataBufferSize);
                            if (bytes <= 0)
                            {
                                break;
                            }

                            if (transferDataType == DataType.IMAGE || noAsciiConv)
                            {
                                // TYPE I -> just pass through
                                socket.Send(dataBuffer, bytes, SocketFlags.None);
                            }
                            else
                            {
                                // TYPE A -> convert local EOL style to CRLF

                                // if the buffer ends with a potential partial EOL,
                                // try to read the rest of the EOL
                                // (i assume that the EOL has max. two bytes)
                                if (localEolBytes.Length == 2 &&
                                    dataBuffer[bytes - 1] == localEolBytes[0])
                                {
                                    if (stream.Read(dataBuffer, bytes, 1) == 1)
                                        ++bytes;
                                }

                                byte[] convBuffer = null;
                                int convBytes = ConvertAsciiBytes(dataBuffer, bytes, true, out convBuffer);
                                socket.Send(convBuffer, convBytes, SocketFlags.None);
                            }
                        }

                        // flush socket before closing (done by using-statement)
                        socket.Shutdown(SocketShutdown.Send);
                        Respond(226, "Transfer complete.");
                    }
                    catch (Exception ex)
                    {
                        Respond(500, ex);
                        return;
                    }
                    finally
                    {
                        if (logHandler != null)
                            logHandler.ClosedDataConnection(remote, local, passive);
                    }
                }
            }
            finally
            {
                stream.Close();
            }
        }

        /// <summary>
        /// Read from a data connection and write to the given stream
        /// </summary>
        private void ReceiveData(Stream stream)
        {
            try
            {
                bool passive = (dataPort == null);
                using (Socket socket = OpenDataConnection())
                {
                    if (socket == null)
                        return;

                    IPEndPoint remote = (IPEndPoint)socket.RemoteEndPoint;
                    IPEndPoint local = (IPEndPoint)socket.LocalEndPoint;

                    if (logHandler != null)
                        logHandler.NewDataConnection(remote, local, passive);

                    try
                    {
                        while (true)
                        {
                            // fill up the in-memory buffer before writing to disk
                            int totalBytes = 0;
                            while (totalBytes < dataBufferSize)
                            {
                                int freeBytes = dataBufferSize - totalBytes;
                                int newBytes = socket.Receive(dataBuffer, totalBytes, freeBytes, SocketFlags.None);

                                if (newBytes > 0)
                                {
                                    totalBytes += newBytes;
                                }
                                else if (newBytes < 0)
                                {
                                    Respond(500, String.Format("Transfer failed: Receive() returned {0}", newBytes));
                                    return;
                                }
                                else
                                {
                                    // end of data
                                    break;
                                }
                            }

                            // end of data
                            if (totalBytes == 0)
                                break;

                            if (transferDataType == DataType.IMAGE || noAsciiConv)
                            {
                                // TYPE I -> just pass through
                                stream.Write(dataBuffer, 0, totalBytes);
                            }
                            else
                            {
                                // TYPE A -> convert CRLF to local EOL style

                                // if the buffer ends with a potential partial CRLF,
                                // try to read the LF
                                if (dataBuffer[totalBytes - 1] == remoteEolBytes[0])
                                {
                                    if (socket.Receive(dataBuffer, totalBytes, 1, SocketFlags.None) == 1)
                                        ++totalBytes;
                                }

                                byte[] convBuffer = null;
                                int convBytes = ConvertAsciiBytes(dataBuffer, totalBytes, false, out convBuffer);
                                stream.Write(convBuffer, 0, convBytes);
                            }
                        }

                        socket.Shutdown(SocketShutdown.Receive);
                        Respond(226, "Transfer complete.");
                    }
                    catch (Exception ex)
                    {
                        Respond(500, ex);
                        return;
                    }
                    finally
                    {
                        if (logHandler != null)
                            logHandler.ClosedDataConnection(remote, local, passive);
                    }
                }
            }
            finally
            {
                //stream.Close();
            }
        }

        /// <summary>
        /// Create a socket for a data connection.
        /// </summary>
        /// <param name='listen'>
        /// If true, the socket will be bound to a local port for the PASV command.
        /// Otherwise the socket can be used for connecting to the address given in a PORT command.
        /// </param>
        private void CreateDataSocket(bool listen)
        {
            if (dataSocket != null)
                dataSocket.Close();

            dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            if (listen)
            {
                IPAddress serverIP = ((IPEndPoint)controlSocket.LocalEndPoint).Address;
                dataSocket.Bind(new IPEndPoint(serverIP, 0));
                dataSocketBound = true; // CF is missing Socket.IsBound
                dataSocket.Listen(1);
            }
        }

        /// <summary>
        /// Opens an active or passive data connection and returns the socket
        /// or null if there was no preceding PORT or PASV command or in case or error.
        /// </summary>
        private Socket OpenDataConnection()
        {
            if (dataPort == null && !dataSocketBound)
            {
                Respond(425, "No data port configured, use PORT or PASV.");
                return null;
            }

            Respond(150, "Opening data connection.");

            try
            {
                if (dataPort != null)
                {
                    // active mode
                    dataSocket.Connect(dataPort);
                    dataPort = null;
                    return dataSocket;
                }
                else
                {
                    // passive mode
                    Socket socket = dataSocket.Accept();
                    dataSocket.Close();
                    dataSocketBound = false;
                    return socket;
                }
            }
            catch (Exception ex)
            {
                Respond(500, String.Format("Failed to open data connection: {0}", ex.Message.Replace(Environment.NewLine, " ")));
                return null;
            }
        }

        /// <summary>
        /// Convert between different EOL flavors.
        /// </summary>
        /// <returns>
        /// The number of bytes in the resultBuffer.
        /// </returns>
        /// <param name='buffer'>
        /// The input buffer whose data will be converted.
        /// </param>
        /// <param name='len'>
        /// The number of bytes in the input buffer.
        /// </param>
        /// <param name='localToRemote'>
        /// If true, the conversion will be made from local to FTP flavor,
        /// otherwise from FTP to local flavor.
        /// </param>
        /// <param name='resultBuffer'>
        /// The resulting buffer with the converted text.
        /// Can be the same reference as the input buffer if there is nothing to convert.
        /// </param>
        private int ConvertAsciiBytes(byte[] buffer, int len, bool localToRemote, out byte[] resultBuffer)
        {
            byte[] fromBytes = (localToRemote ? localEolBytes : remoteEolBytes);
            byte[] toBytes = (localToRemote ? remoteEolBytes : localEolBytes);
            resultBuffer = null;

            int startIndex = 0;
            int resultLen = 0;
            int searchLen;
            while ((searchLen = len - startIndex) > 0)
            {
                // search for the first byte of the EOL sequence
                int eolIndex = Array.IndexOf(buffer, fromBytes[0], startIndex, searchLen);

                // shortcut if there is no EOL in the whole buffer
                if (eolIndex == -1 && startIndex == 0)
                {
                    resultBuffer = buffer;
                    return len;
                }

                // allocate to worst-case size
                if (resultBuffer == null)
                    resultBuffer = new byte[len * 2];

                if (eolIndex == -1)
                {
                    Array.Copy(buffer, startIndex, resultBuffer, resultLen, searchLen);
                    resultLen += searchLen;
                    break;
                }
                else
                {
                    // compare the rest of the EOL
                    int matchBytes = 1;
                    for (int i = 1; i < fromBytes.Length && eolIndex + i < len; ++i)
                    {
                        if (buffer[eolIndex + i] == fromBytes[i])
                            ++matchBytes;
                    }

                    if (matchBytes == fromBytes.Length)
                    {
                        // found an EOL to convert
                        int copyLen = eolIndex - startIndex;
                        if (copyLen > 0)
                        {
                            Array.Copy(buffer, startIndex, resultBuffer, resultLen, copyLen);
                            resultLen += copyLen;
                        }
                        Array.Copy(toBytes, 0, resultBuffer, resultLen, toBytes.Length);
                        resultLen += toBytes.Length;
                        startIndex += copyLen + fromBytes.Length;
                    }
                    else
                    {
                        int copyLen = (eolIndex - startIndex) + 1;
                        Array.Copy(buffer, startIndex, resultBuffer, resultLen, copyLen);
                        resultLen += copyLen;
                        startIndex += copyLen;
                    }
                }
            }

            return resultLen;
        }

        /// <summary>
        /// Parse the argument of a PORT command into an IPEndPoint
        /// </summary>
        private IPEndPoint ParseAddress(string address)
        {
            string[] tokens = address.Split(',');
            byte[] bytes = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; ++i)
            {
                try
                {
                    // CF is missing TryParse
                    bytes[i] = byte.Parse(tokens[i]);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            long ip = bytes[0] | bytes[1] << 8 | bytes[2] << 16 | bytes[3] << 24;
            int port = bytes[4] << 8 | bytes[5];
            return new IPEndPoint(ip, port);
        }

        /// <summary>
        /// Format an IPEndPoint so that it can be used in a response for a PASV command
        /// </summary>
        private string FormatAddress(IPEndPoint address)
        {
            byte[] ip = address.Address.GetAddressBytes();
            int port = address.Port;

            return String.Format("{0},{1},{2},{3},{4},{5}",
                                 ip[0], ip[1], ip[2], ip[3],
                                 (port & 0xFF00) >> 8, port & 0x00FF);
        }

        /// <summary>
        /// Formats a list of file system entries for a response to a LIST or STAT command
        /// </summary>
        private string FormatDirList(FileSystemEntry[] list)
        {
            int maxSizeChars = 0;
            foreach (FileSystemEntry entry in list)
            {
                maxSizeChars = Math.Max(maxSizeChars, entry.Size.ToString().Length);
            }

            DateTime sixMonthsAgo = EnsureUnixTime(DateTime.Now.ToUniversalTime().AddMonths(-6));

            StringBuilder result = new StringBuilder();
            foreach (FileSystemEntry entry in list)
            {
                char dirflag = (entry.IsDirectory ? 'd' : '-');
                string size = entry.Size.ToString().PadLeft(maxSizeChars);
                DateTime time = EnsureUnixTime(entry.LastModifiedTimeUtc);
                string timestr = MONTHS[time.Month - 1];
                if (time < sixMonthsAgo)
                    timestr += time.ToString(" dd  yyyy");
                else
                    timestr += time.ToString(" dd hh:mm");
                string mode = entry.Mode;

                if (string.IsNullOrEmpty(mode))
                    mode = dirflag + "rwxr--r--";

                result.AppendFormat("{0} 1 owner group {1} {2} {3}\r\n",
                                    mode, size, timestr, entry.Name);
            }

            return result.ToString();
        }

        /// <summary>
        /// Formats a list of file system entries for a response to an NLST command
        /// </summary>
        private string FormatNLST(FileSystemEntry[] list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (FileSystemEntry entry in list)
            {
                sb.Append(entry.Name);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Formats a list of file system entries for a response to an MLST command
        /// </summary>
        private string FormatMLST(FileSystemEntry[] list)
        {
            StringBuilder sb = new StringBuilder();
            var cd = fsHandler.GetCurrentDirectory();
            foreach (FileSystemEntry entry in list)
            {
                int p = entry.Name.IndexOf(" -> ");
                string l, f;
                if (p >= 0)
                {
                    l = entry.Name.Substring(0, p);
                    f = entry.Name.Substring(p + 4);
                }
                else
                {
                    l = f = entry.Name;
                }
                sb.AppendFormat("modify={0:yyyyMMddHHmmss};perm={1};size={2};type={3};unique={4:X};unix.mode={5:D4}; {6}\r\n",
                    entry.LastModifiedTimeUtc,
                    "rw" + (entry.IsDirectory ? "l" : "") + (entry.Mode != null && entry.Mode.Contains("x") ? "x" : ""),
                    entry.Size,
                    (l != f) ? "symlink" : (entry.IsDirectory ? "dir" : "file"),
                    (cd.Result + f).GetHashCode(),
                    (string.IsNullOrEmpty(entry.Mode) || entry.Mode.Length < 10) ? 0 : (
                    ((entry.Mode[3] == 'S') ? 4000 : 0) +
                    ((entry.Mode[6] == 'S') ? 2000 : 0) +
                    ((entry.Mode[9] == 'T') ? 1000 : 0) +
                    ((entry.Mode[1] == 'r') ? 400 : 0) +
                    ((entry.Mode[2] == 'w') ? 200 : 0) +
                    ((entry.Mode[3] != '-') ? 100 : 0) +
                    ((entry.Mode[4] == 'r') ? 040 : 0) +
                    ((entry.Mode[5] == 'w') ? 020 : 0) +
                    ((entry.Mode[6] != '-') ? 010 : 0) +
                    ((entry.Mode[7] == 'r') ? 004 : 0) +
                    ((entry.Mode[8] == 'w') ? 002 : 0) +
                    ((entry.Mode[9] != '-') ? 001 : 0)
                    ),
                    l);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Format a timestamp for a reponse to a MDTM command
        /// </summary>
        private string FormatTime(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// Restrict the year in a timestamp to >= 1970
        /// </summary>
        private DateTime EnsureUnixTime(DateTime time)
        {
            // the server claims to be UNIX, so there should be
            // no timestamps before 1970.
            // e.g. FileZilla does not handle them correctly.

            int yearDiff = time.Year - 1970;
            if (yearDiff < 0)
                return time.AddYears(-yearDiff);
            else
                return time;
        }

        /// <summary>
        /// Escape a path for a response to a PWD command
        /// </summary>
        private string EscapePath(string path)
        {
            // double-quotes in paths are escaped by doubling them
            return '"' + path.Replace("\"", "\"\"") + '"';
        }

        /// <summary>
        /// Remove "-a" or "-l" from the arguments for a LIST or STAT command
        /// </summary>
        private string RemoveLsArgs(string args)
        {
            if (args != null && (args.StartsWith("-a") || args.StartsWith("-l")))
            {
                if (args.Length == 2)
                    return null;
                else if (args.Length > 3 && args[2] == ' ')
                    return args.Substring(3);
            }

            return args;
        }

        /// <summary>
        /// Convert a string to a list of UTF8 bytes
        /// </summary>
        private byte[] EncodeString(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        /// <summary>
        /// Convert a list of UTF8 bytes to a string
        /// </summary>
        private string DecodeString(byte[] data, int len)
        {
            return Encoding.UTF8.GetString(data, 0, len);
        }

        /// <summary>
        /// Convert a list of UTF8 bytes to a string
        /// </summary>
        private string DecodeString(byte[] data)
        {
            return DecodeString(data, data.Length);
        }

        /// <summary>
        /// Fill a stream with the given string as UTF8 bytes
        /// </summary>
        private Stream MakeStream(string data)
        {
            return new MemoryStream(EncodeString(data));
        }

        /// <summary>
        /// Return a randomly selected text from the given list
        /// </summary>
        private string GetRandomText(string[] texts)
        {
            int index = randomTextIndex.Next(0, texts.Length);
            return texts[index];
        }
    }
}
