using com.clusterrr.hakchi_gui;
using Renci.SshNet;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;

namespace com.clusterrr.ssh
{
    public class SshClientWrapper : ISystemShell, INetworkShell
    {
        public event OnConnectedEventHandler OnConnected = delegate { };
        public event OnDisconnectedEventHandler OnDisconnected = delegate { };

        private SshClient sshClient;
        Thread connectThread;
        private bool enabled;
        private bool hasConnected;
        //private int retries;
        private string service;
        private string ip;
        private ushort port;
        private string username;
        private string password;

        public bool AutoReconnect { set; get; }
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) return;
                enabled = value;
                if (value)
                {
                    // start connection watching thread
                    if (connectThread == null)
                    {
                        connectThread = new Thread(connectThreadLoop);
                        connectThread.Start();
                    }
                }
                else
                {
                    if (connectThread != null)
                    {
                        connectThread.Abort();
                        connectThread = null;
                    }
                    if (sshClient != null)
                    {
                        if (sshClient.IsConnected)
                        {
                            sshClient.Disconnect();
                        }
                        sshClient.Dispose();
                        sshClient = null;
                    }
                }
            }
        }

        private void connectThreadLoop()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        if (!IsOnline)
                        {
                            if (hasConnected)
                            {
                                Debug.WriteLine("SSH shell disconnected");
                                hasConnected = false;
                                OnDisconnected();
                                Thread.Sleep(1000); // give it additional time to disconnect
                            }
                            else if (AutoReconnect)
                            {
                                if (Ping() != -1)
                                {
                                    Connect();
                                }
                            }
                        }
                        Thread.Sleep(1000);
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error during connect loop: " + ex.Message + ex.StackTrace);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Critical error: " + ex.Message + ex.StackTrace);
            }
        }

        public bool IsOnline
        {
            get
            {
                return sshClient != null && sshClient.IsConnected;
            }
        }

        public ushort ShellPort // this is telnet port
        {
            get { return 23; }
        }

        public bool ShellEnabled
        {
            get { return IsOnline; }
            set { }
        }

        public string IPAddress
        {
            get { return ip; }
            private set { ip = value; }
        }

        public SshClientWrapper(string serviceName, string ip, ushort port, string username, string password)
        {
            sshClient = null;
            connectThread = null;
            enabled = false;
            hasConnected = false;
            //retries = 0;
            this.service = serviceName;
            this.ip = ip;
            this.port = port;
            this.username = username;
            this.password = password;
        }

        public void Dispose()
        {
            // this will shutdown everything
            Enabled = false;
        }

        public void Connect()
        {
            if (IsOnline)
                return;
            if (string.IsNullOrEmpty(ip) || ip == "0.0.0.0")
                return;

            if (sshClient == null)
            {
                sshClient = new SshClient(ip, port, username, password);
                sshClient.ErrorOccurred += SshClient_OnError;
            }
            if (!sshClient.IsConnected)
            {
                sshClient.Connect();
            }
            if (!sshClient.IsConnected)
            {
                throw new SshClientException(string.Format("Unable to connect to SSH server at {0}:{1}", 
                    sshClient.ConnectionInfo.Host, sshClient.ConnectionInfo.Port));
            }

            Debug.WriteLine("SSH shell connected");
#if DEBUG
            Debug.WriteLine($"Encryption: {sshClient.ConnectionInfo.CurrentServerEncryption}");
#endif

            hasConnected = true;
            OnConnected(this);
        }

        public void Disconnect()
        {
            if (sshClient == null) return;
            if (sshClient.IsConnected)
            {
                // this will disconnect the shell and thread loop will detect it and call OnDisconnected and clean up
                sshClient.Disconnect();
            }
        }

        public int Ping()
        {
            if (string.IsNullOrEmpty(ip) || ip == "0.0.0.0")
                return -1;

            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(ip, 100);
                if (reply != null && reply.Status.Equals(IPStatus.Success))
                {
#if DEBUG
                    Debug.WriteLine($"Pinged {reply.Address}, {reply.RoundtripTime}ms");
#endif
                    return (int)reply.RoundtripTime;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is ThreadAbortException)
                    {
                        Debug.WriteLine("Ping abort (usually happens when running clovershell");
                        return -1;
                    }
                }
                Debug.WriteLine("Error performing ping: " + ex.Message + "\r\n" + ex.StackTrace);
            }
            return -1;
        }

        public string ExecuteSimple(string command, int timeout = 2000, bool throwOnNonZero = false)
        {
            SshCommand sshCommand = sshClient.CreateCommand(command);
            if (timeout > 0)
                sshCommand.CommandTimeout = new TimeSpan(0, 0, 0, 0, timeout);

            string result = sshCommand.Execute();

            if (sshCommand.ExitStatus != 0 && throwOnNonZero)
            {
                throw new SshClientException(string.Format("Shell command \"{0}\" returned exit code {1} {2}", command, sshCommand.ExitStatus, sshCommand.Error));
            }

#if DEBUG
            Debug.WriteLine(string.Format("{0} # exit code {1}", command, sshCommand.ExitStatus));
#endif

            return result.Trim();
        }

        public int Execute(string command, Stream stdin = null, Stream stdout = null, Stream stderr = null, int timeout = 0, bool throwOnNonZero = false)
        {
            SshCommand sshCommand = sshClient.CreateCommand(command);
            if (timeout > 0)
                sshCommand.CommandTimeout = new TimeSpan(0, 0, 0, 0, timeout);

            IAsyncResult execResult = sshCommand.BeginExecute(null, null, stdout, stderr);

            if (stdin != null)
            {
                try
                {
                    stdin.Seek(0, SeekOrigin.Begin);
                }
                catch
                {
                    // no-op
                }

                sshCommand.SendData(stdin);
            }

            sshCommand.EndExecute(execResult);

            if (sshCommand.ExitStatus != 0 && throwOnNonZero)
            {
                throw new SshClientException(string.Format("Shell command \"{0}\" returned exit code {1} {2}", command, sshCommand.ExitStatus, sshCommand.Error));
            }

#if DEBUG
            Debug.WriteLine(string.Format("{0} # exit code {1}", command, sshCommand.ExitStatus));
#endif

            return sshCommand.ExitStatus;
        }

        private void SshClient_OnError(object src, Renci.SshNet.Common.ExceptionEventArgs args)
        {
#if VERY_DEBUG
            Debug.WriteLine(string.Format("Error occurred on SSH client: {0}\n{1}\n{2}",
                args.Exception.Message, args.Exception.InnerException, args.Exception.StackTrace));
#endif
            Disconnect();
        }
    }
}
