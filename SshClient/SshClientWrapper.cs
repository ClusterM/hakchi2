using com.clusterrr.hakchi_gui;
using Renci.SshNet;
using Tmds.MDns;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private Thread connectThread;
        private Devices devices;

        private bool enabled;
        private bool hasConnected;
        DateTime lastDisconnected;

        private string serviceName;
        private string serviceType;
        private int? port;
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
                    // start devices listener
                    if (devices == null)
                    {
                        devices = new Devices(serviceName, serviceType);
                    }

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
                    if (devices != null)
                    {
                        devices.Abort();
                        devices = null;
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

        public SshClientWrapper(string serviceName, string serviceType, string IPAddress, int? port, string username, string password)
        {
            sshClient = null;
            connectThread = null;
            devices = null;
            enabled = false;
            hasConnected = false;
            lastDisconnected = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(3000));

            this.serviceName = serviceName;
            this.serviceType = serviceType;
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
            if (IsOnline || string.IsNullOrEmpty(IPAddress) || IPAddress == "0.0.0.0")
                return;

            try
            {
                if (sshClient == null)
                {
                    sshClient = new SshClient(IPAddress, port.Value, username, password);
                    sshClient.ErrorOccurred += SshClient_OnError;
                }
                if (!sshClient.IsConnected)
                {
                    sshClient.Connect();
                }
                Trace.WriteLine("SSH shell connected");
                Trace.WriteLine($"IP Address: {IPAddress}");
                Trace.WriteLine($"Encryption: {sshClient.ConnectionInfo.CurrentServerEncryption}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Unable to connect to SSH server at {0}:{1} ({2})", sshClient.ConnectionInfo.Host, sshClient.ConnectionInfo.Port, ex.Message));
                sshClient = null;
                IPAddress = null;
                port = null;
                return;
            }

            hasConnected = true;
            OnConnected(this);
        }

        public void Disconnect()
        {
            if (sshClient == null) return;
            if (sshClient.IsConnected)
            {
                sshClient.Disconnect(); // this will disconnect the shell and thread loop will detect it and call OnDisconnected and clean up
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
                                lastDisconnected = DateTime.Now;
                                if (sshClient != null)
                                {
                                    sshClient.Dispose();
                                    sshClient = null;
                                }
                                IPAddress = null;
                                hasConnected = false;
                                OnDisconnected();
                            }
                            else if (AutoReconnect)
                            {
                                if (DateTime.Now.Subtract(lastDisconnected).TotalMilliseconds > 3000)
                                {
                                    attemptConnect();
                                }
                            }
                        }
                        Thread.Sleep(500);
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

        private void attemptConnect()
        {
            if (devices.Available.Any())
            {
                foreach (var dev in devices.Available)
                {
                    IPAddress = dev.Addresses.First().ToString();
                    port = dev.Port;
                    if (ping(IPAddress) != -1)
                    {
                        Connect();
                        if (IsOnline)
                        {
                            Debug.WriteLine("Success!");
                            return;
                        }
                    }
                }
            }
        }

        private int ping(string ip, bool verbose = false)
        {
            try
            {
                using (Ping pingSender = new Ping())
                {
                    PingReply reply = pingSender.Send(ip, 500);
                    if (reply != null && reply.Status.Equals(IPStatus.Success))
                    {
                        if (verbose)
                            Trace.WriteLine($"Pinged {reply.Address}, {reply.RoundtripTime}ms");
                        IPAddress = reply.Address.ToString();
                        return (int)reply.RoundtripTime;
                    }
                }
            }
#pragma warning disable CS0168
            catch (Exception ex)
            {
#if VERY_DEBUG
                Debug.WriteLine($"Error during ping \"{IPAddress ?? service}\": {(ex.InnerException ?? ex).Message}");
#endif
            }
#pragma warning restore CS0168
            return -1;
        }

        public int Ping()
        {
            if (IPAddress == "0.0.0.0")
                IPAddress = null;
            if (string.IsNullOrEmpty(IPAddress))
                return -1;
            return ping(this.IPAddress, true);
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
