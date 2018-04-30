using com.clusterrr.hakchi_gui;
using Renci.SshNet;
using DnsUtils.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace com.clusterrr.ssh
{

    public class SshClientWrapper : ISystemShell, INetworkShell
    {
        public event OnConnectedEventHandler OnConnected = delegate { };
        public event OnDisconnectedEventHandler OnDisconnected = delegate { };

        private SshClient sshClient;
        private Llmnr llmnr;

        Thread connectThread;
        private bool enabled;
        private bool hasConnected;
        DateTime lastDisconnected;
        private string service;
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
                                Trace.WriteLine("SSH shell disconnected");
                                this.IPAddress = null;
                                hasConnected = false;
                                lastDisconnected = DateTime.Now;
                                OnDisconnected();
                            }
                            else if (AutoReconnect)
                            {
                                if (DateTime.Now.Subtract(lastDisconnected).TotalMilliseconds > 3000 && Resolve() && Ping() != -1)
                                {
                                    Connect();
                                }
                            }
                        }
                        Thread.Sleep(250);
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Error during connect loop: " + ex.Message + ex.StackTrace);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Critical error: " + ex.Message + ex.StackTrace);
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
            get; private set;
        }

        public SshClientWrapper(string service, string IPAddress, ushort port, string username, string password)
        {
            sshClient = null;
            llmnr = null;
            connectThread = null;
            enabled = false;
            hasConnected = false;
            lastDisconnected = DateTime.Now.Subtract(TimeSpan.FromSeconds(3));
            this.service = service;
            this.IPAddress = IPAddress;
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
            if (string.IsNullOrEmpty(IPAddress) || IPAddress == "0.0.0.0")
                return;

            try
            {
                if (sshClient == null)
                {
                    sshClient = new SshClient(IPAddress, port, username, password);
                    sshClient.ErrorOccurred += SshClient_OnError;
                }
                if (!sshClient.IsConnected)
                {
                    sshClient.Connect();
                }
            }
            catch (Exception)
            {
                throw new SshClientException(string.Format("Unable to connect to SSH server at {0}:{1}", sshClient.ConnectionInfo.Host, sshClient.ConnectionInfo.Port));
            }

            Trace.WriteLine("SSH shell connected");
            Trace.WriteLine($"IP Address: {IPAddress}");
            Trace.WriteLine($"Encryption: {sshClient.ConnectionInfo.CurrentServerEncryption}");

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

        public bool Resolve()
        {
            if (llmnr == null)
            {
                llmnr = new Llmnr();
            }

            IPAddress address = NameResolving.ResolveAsync(this.service, 1000, llmnr).Result;
            if (address != null)
            {
                this.IPAddress = address.ToString();
                return true;
            }
            return false;
        }

        public int Ping()
        {
            if ((string.IsNullOrEmpty(IPAddress) || IPAddress == "0.0.0.0") && string.IsNullOrEmpty(service))
                return -1;

            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(IPAddress ?? service, 1000);
                if (reply != null && reply.Status.Equals(IPStatus.Success))
                {
                    Trace.WriteLine($"Pinged {reply.Address}, {reply.RoundtripTime}ms");
                    IPAddress = reply.Address.ToString();
                    return (int)reply.RoundtripTime;
                }
            }
            catch (Exception ex)
            {
                string msg = $"Error during ping \"{IPAddress ?? service}\": {(ex.InnerException ?? ex).Message}";
#if VERY_DEBUG
                Debug.WriteLine(msg);
#endif
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

            Trace.WriteLine(string.Format("{0} # exit code {1}", command, sshCommand.ExitStatus));

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

            Trace.WriteLine(string.Format("{0} # exit code {1}", command, sshCommand.ExitStatus));

            return sshCommand.ExitStatus;
        }

        public Task<string> ExecuteSimpleAsync(string command, int timeout = 2000, bool throwOnNonZero = false)
        {
            return new Task<string>(() =>
            {
                return ExecuteSimple(command, timeout, throwOnNonZero);
            });
        }

        public Task<int> ExecuteAsync(string command, Stream stdin = null, Stream stdout = null, Stream stderr = null, int timeout = 0, bool throwOnNonZero = false)
        {
            return new Task<int>(() =>
            {
                return Execute(command, stdin, stdout, stderr, timeout, throwOnNonZero);
            });
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
