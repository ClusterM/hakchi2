using com.clusterrr.hakchi_gui;
using Renci.SshNet;
using Zeroconf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Threading;

namespace com.clusterrr.ssh
{
    public class SshClientWrapper : ISystemShell
    {
        public event OnConnectedEventHandler OnConnected = delegate { };
        public event OnDisconnectedEventHandler OnDisconnected = delegate { };

        private ZeroconfResolver.ResolverListener avahiListener;
        private IZeroconfHost avahiHost;
        private SshClient sshClient;
        Thread connectThread;
        private bool enabled;
        private bool hasConnected;
        private int retries;
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
                    // start listening for proper network connections
                    avahiListener = ZeroconfResolver.CreateListener(service);
                    avahiListener.ServiceFound += ZeroconfHost_OnServiceFound;
                    avahiListener.ServiceLost += ZeroconfHost_OnServiceLost;

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
                    if (avahiListener != null)
                    {
                        avahiListener.Dispose();
                        avahiListener = null;
                        avahiHost = null;
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
                    if (!IsOnline)
                    {
                        if (hasConnected)
                        {
                            Debug.WriteLine("SSH shell disconnected");
                            hasConnected = false;
                            OnDisconnected();
                            continue;
                        }
                        else if (AutoReconnect)
                        {
                            if (avahiHost != null)
                            {
                                if (Ping() != -1)
                                {
                                    Connect();
                                }
                                else
                                {
                                    if (retries++ > 30)
                                    {
                                        retries = 0;
                                        avahiHost = null;
                                    }
                                }
                            }
                            else
                            {
                                Thread.Sleep(250);
                                continue;
                            }
                        }
                    }
                    Thread.Sleep(1000);
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
                return (avahiHost != null && sshClient != null) ? sshClient.IsConnected : false;
            }
        }

        public ushort ShellPort
        {
            get { return 23; }
        }

        public bool ShellEnabled
        {
            get { return true; }
            set { }
        }

        public string IPAddress
        {
            get
            {
                return avahiHost == null ? null : avahiHost.IPAddress;
            }
        }

        private void ZeroconfHost_OnServiceFound(object src, IZeroconfHost host)
        {
            if (avahiHost == null && host != null)
            {
                avahiHost = host;
            }
#if DEBUG
            Debug.WriteLine($"Detected host: \"{host.DisplayName ?? "Unknown"}\", IP: {host.IPAddress ?? "Unknown"}");
#endif
        }

        private void ZeroconfHost_OnServiceLost(object src, IZeroconfHost host)
        {
#if VERY_DEBUG
            Debug.WriteLine($"Lost host: \"{host.DisplayName ?? "Unknown"}\", IP: {host.IPAddress ?? "Unknown"}");
#endif
        }

        public SshClientWrapper(string serviceName, ushort port, string username, string password)
        {
            avahiListener = null;
            avahiHost = null;
            sshClient = null;
            connectThread = null;
            enabled = false;
            hasConnected = false;
            retries = 0;
            this.service = serviceName;
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
            if (avahiHost == null)
                return;

            if (sshClient == null)
            {
                sshClient = new SshClient(avahiHost.IPAddress, port, username, password);
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
#if VERU_DEBUG
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
            if (avahiHost != null)
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(avahiHost.IPAddress, 100);
                if (reply != null && reply.Status.Equals(IPStatus.Success))
                {
#if DEBUG
                    Debug.WriteLine($"Pinged {reply.Address}, {reply.RoundtripTime}ms");
#endif
                    return (int)reply.RoundtripTime;
                }
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
