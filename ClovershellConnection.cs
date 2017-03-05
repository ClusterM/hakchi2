using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace com.clusterrr.cloverhack
{
    public class ClovershellConnection : IDisposable
    {
        class ShellConnection
        {
            public Socket socket;
            public int id;
            public ShellConnection()
            {
                id = -1;
            }
        }
        class ExecConnection
        {
            public readonly string command;
            public Stream stdin;
            public Stream stdout;
            public Stream stderr;
            public int id;
            public bool finished;
            public bool stdoutFinished;
            public bool stderrFinished;
            public int result;
            public ExecConnection(string command, Stream stdin, Stream stdout, Stream stderr)
            {
                this.command = command;
                id = -1;
                this.stdin = stdin;
                this.stdout = stdout;
                this.stderr = stderr;
                finished = false;
                stdoutFinished = false;
                stderrFinished = false;
            }
        }

        const UInt16 vid = 0x1F3A;
        const UInt16 pid = 0xEFE8;
        UsbDevice device = null;
        UsbEndpointReader epReader = null;
        UsbEndpointWriter epWriter = null;
        Thread mainThread = null;
        Thread shellThread = null;
        bool online = false;
        ushort shellPort = 1023;
        Queue<ShellConnection> pendingShellConnections = new Queue<ShellConnection>();
        List<ExecConnection> pendingExecConnections = new List<ExecConnection>();
        ShellConnection[] shellConnections = new ShellConnection[256];
        ExecConnection[] execConnections = new ExecConnection[256];
        List<Thread> shellConnecionThreads = new List<Thread>();
        bool enabled = false;
        bool autoreconnect = false;
        byte[] lastPingResponse = null;
        DateTime lastAliveTime;

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) return;
                enabled = value;
                if (value)
                {
                    mainThread = new Thread(mainThreadLoop);
                    mainThread.Start();
                }
                else
                {
                    if (mainThread != null)
                        mainThread.Abort();
                    mainThread = null;
                    online = false;
                    if (device != null)
                        device.Close();
                    device = null;
                    if (epReader != null)
                        epReader.Dispose();
                    epReader = null;
                    if (epWriter != null)
                        epWriter.Dispose();
                    epWriter = null;
                }
            }
        }

        public bool AutoReconnect
        {
            get { return autoreconnect; }
            set { autoreconnect = value; }
        }

        public ushort ShellPort
        {
            get { return shellPort; }
            set
            {
                shellPort = value;
                if (ShellEnabled)
                {
                    ShellEnabled = false;
                    ShellEnabled = true;
                }
            }
        }
        bool shellEnabled = false;
        public bool ShellEnabled
        {
            get { return shellEnabled; }
            set
            {
                if (shellEnabled == value) return;
                if (value)
                {
                    var server = new TcpListener(IPAddress.Any, shellPort);
                    Debug.WriteLine(string.Format("Listening port {0}", shellPort));
                    server.Start();
                    shellThread = new Thread(shellThreadLoop);
                    shellThread.Start(server);
                }
                else
                {
                    shellThread.Abort();
                    shellThread = null;
                }
                foreach (var pending in pendingShellConnections)
                    pending.socket.Close();
                pendingShellConnections.Clear();
                foreach (var connection in shellConnecionThreads)
                    connection.Abort();
                shellConnecionThreads.Clear();
                shellEnabled = value;
            }
        }
        public bool Online
        {
            get { return online; }
        }

        enum ClovershellCommand
        {
            CMD_PING = 0,
            CMD_PONG = 1,
            CMD_SHELL_NEW_REQ = 2,
            CMD_SHELL_NEW_RESP = 3,
            CMD_SHELL_IN = 4,
            CMD_SHELL_OUT = 5,
            CMD_SHELL_CLOSED = 6,
            CMD_SHELL_KILL = 7,
            CMD_SHELL_KILL_ALL = 8,
            CMD_EXEC_NEW_REQ = 9,
            CMD_EXEC_NEW_RESP = 10,
            CMD_EXEC_PID = 11,
            CMD_EXEC_STDIN = 12,
            CMD_EXEC_STDOUT = 13,
            CMD_EXEC_STDERR = 14,
            CMD_EXEC_RESULT = 15,
            CMD_EXEC_KILL = 16,
            CMD_EXEC_KILL_ALL = 17
        }

        void dropAll()
        {
            writeUsb(ClovershellCommand.CMD_SHELL_KILL_ALL, 0);
            writeUsb(ClovershellCommand.CMD_EXEC_KILL_ALL, 0);
            foreach (var pending in pendingShellConnections)
                pending.socket.Close();
            pendingShellConnections.Clear();
            for (int i = 0; i < shellConnections.Length; i++)
            {
                var connection = shellConnections[i];
                if (connection == null)
                    continue;
                connection.socket.Close();
                shellConnections[i] = null;
            }
            for (int i = 0; i < execConnections.Length; i++)
                execConnections[i] = null;
            foreach (var connection in shellConnecionThreads)
                connection.Abort();
            shellConnecionThreads.Clear();
            pendingExecConnections.Clear();
        }

        void mainThreadLoop()
        {
            try
            {
                while (enabled)
                {
                    online = false;
                    Debug.WriteLine("Waiting for clovershell");
                    while (enabled)
                    {
                        try
                        {
                            var devices = UsbDevice.AllDevices;
                            device = null;
                            foreach (UsbRegistry regDevice in devices)
                            {
                                if (regDevice.Vid == vid && regDevice.Pid == pid)
                                {
                                    regDevice.Open(out device);
                                    break;
                                }
                            }
                            //device = USBDevice.GetSingleDevice(vid, pid);
                            if (device == null) break;
                            IUsbDevice wholeUsbDevice = device as IUsbDevice;
                            if (!ReferenceEquals(wholeUsbDevice, null))
                            {
                                // This is a "whole" USB device. Before it can be used, 
                                // the desired configuration and interface must be selected.

                                // Select config #1
                                wholeUsbDevice.SetConfiguration(1);

                                // Claim interface #0.
                                wholeUsbDevice.ClaimInterface(0);
                            }

                            int inEndp = -1;
                            int outEndp = -1;
                            int inMax = 0;
                            int outMax = 0;
                            foreach (var config in device.Configs)
                                foreach (var @interface in config.InterfaceInfoList)
                                    foreach (var endp in @interface.EndpointInfoList)
                                    {

                                        if ((endp.Descriptor.EndpointID & 0x80) != 0)
                                        {
                                            inEndp = endp.Descriptor.EndpointID;
                                            inMax = endp.Descriptor.MaxPacketSize;
                                            Debug.WriteLine("IN endpoint found: " + inEndp);
                                            Debug.WriteLine("IN endpoint maxsize: " + inMax);
                                        }
                                        else
                                        {
                                            outEndp = endp.Descriptor.EndpointID;
                                            outMax = endp.Descriptor.MaxPacketSize;
                                            Debug.WriteLine("OUT endpoint found: " + outEndp);
                                            Debug.WriteLine("OUT endpoint maxsize: " + outMax);
                                        }
                                    }
                            if (inEndp != 0x81 || inMax != 512 || outEndp != 0x01 || outMax != 512)
                                break;
                            epReader = device.OpenEndpointReader((ReadEndpointID)inEndp, 65536);
                            epWriter = device.OpenEndpointWriter((WriteEndpointID)outEndp);
                            Debug.WriteLine("clovershell connected");
                            // Kill all other serrions and drop all output
                            killAll();
                            var body = new byte[65536];
                            int len;
                            while (epReader.Read(body, 50, out len) == ErrorCode.Ok) ;
                            epReader.ReadBufferSize = 65536;
                            epReader.DataReceived += epReader_DataReceived;
                            epReader.DataReceivedEnabled = true;
                            online = true;
                            lastAliveTime = DateTime.Now;
                            while (device.mUsbRegistry.IsAlive)
                            {
                                Thread.Sleep(100);
                            }
                            break;
                        }
                        catch (ThreadAbortException)
                        {
                            return;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            break;
                        }
                    }
                    if (online) Debug.WriteLine("clovershell disconnected");
                    online = false;
                    foreach (var connection in shellConnecionThreads)
                        connection.Abort();
                    shellConnecionThreads.Clear(); 
                    if (device != null)
                        device.Close();
                    device = null;
                    if (epReader != null)
                        epReader.Dispose();
                    epReader = null;
                    if (epWriter != null)
                        epWriter.Dispose();
                    epWriter = null;
                    if (!autoreconnect) Enabled = false;
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

        public void Connect()
        {
            if (Enabled) return;
            Enabled = true;
            while (Enabled && !online)
            {
                Thread.Sleep(50);
            }
            if (!online) throw new Exception("no clovershell connection, make sure your NES Mini connected, turned on and clovershell mod installed");
        }

        void epReader_DataReceived(object sender, EndpointDataEventArgs e)
        {
            var cmd = (ClovershellCommand)e.Buffer[0];
            var arg = e.Buffer[1];
            var len = e.Buffer[2] | (e.Buffer[3] * 0x100);
            proceedPacket(cmd, arg, e.Buffer, 4, len);
        }

        void proceedPacket(ClovershellCommand cmd, byte arg, byte[] data, int pos, int len)
        {
            if (len < 0)
                len = data.Length;
            Debug.WriteLine(string.Format("cmd={0}, arg={1:X2}, len={2}", cmd, arg, len));
            lastAliveTime = DateTime.Now;
            switch (cmd)
            {
                case ClovershellCommand.CMD_PONG:
                    lastPingResponse = new byte[len];
                    Array.Copy(data, pos, lastPingResponse, 0, len);
                    break;
                case ClovershellCommand.CMD_SHELL_NEW_RESP:
                    acceptShellConnection(arg);
                    break;
                case ClovershellCommand.CMD_SHELL_OUT:
                    shellOut(arg, data, pos, len);
                    break;
                case ClovershellCommand.CMD_SHELL_CLOSED:
                    shellClosed(arg);
                    break;
                case ClovershellCommand.CMD_EXEC_NEW_RESP:
                    newExecConnection(arg, Encoding.UTF8.GetString(data, pos, len));
                    break;
                case ClovershellCommand.CMD_EXEC_STDOUT:
                    execOut(arg, data, pos, len);
                    break;
                case ClovershellCommand.CMD_EXEC_STDERR:
                    execErr(arg, data, pos, len);
                    break;
                case ClovershellCommand.CMD_EXEC_RESULT:
                    execResult(arg, data, pos, len);
                    break;
            }
        }

        void killAll()
        {
            int tLen;
            var buff = new byte[4];
            buff[0] = (byte)ClovershellCommand.CMD_SHELL_KILL_ALL;
            buff[1] = 0;
            buff[2] = 0;
            buff[3] = 0;
            epWriter.Write(buff, 0, buff.Length, 1000, out tLen);
            if (tLen != buff.Length)
                throw new Exception("kill all shell: write error");
            buff[0] = (byte)ClovershellCommand.CMD_EXEC_KILL_ALL;
            buff[1] = 0;
            buff[2] = 0;
            buff[3] = 0;
            epWriter.Write(buff, 0, buff.Length, 1000, out tLen);
            if (tLen != buff.Length)
                throw new Exception("kill all exec: write error");
        }

        void writeUsb(ClovershellCommand cmd, byte arg, byte[] data = null, int l = -1)
        {
            if (!online) throw new Exception("NES Mini is offline");
            var len = (l >= 0) ? l : ((data != null) ? data.Length : 0);
            var buff = new byte[len + 4];
            buff[0] = (byte)cmd;
            buff[1] = arg;
            buff[2] = (byte)(len & 0xFF);
            buff[3] = (byte)((len >> 8) & 0xFF);
            if (data != null)
                Array.Copy(data, 0, buff, 4, len);
            int tLen = 0;
            epWriter.Write(buff, 0, 4 + len, 1000, out tLen);
            if (tLen != 4 + len)
                throw new Exception("write error");
        }

        void shellThreadLoop(object o)
        {
            try
            {
                var server = o as TcpListener;
                while (true)
                {
                    var connection = new ShellConnection();
                    while (!server.Pending()) Thread.Sleep(100);
                    connection.socket = server.AcceptSocket();
                    Debug.WriteLine("Shell client connected");
                    try
                    {
                        if (!online) throw new Exception("NES Mini is offline");
                        pendingShellConnections.Enqueue(connection);
                        writeUsb(ClovershellCommand.CMD_SHELL_NEW_REQ, 0);
                        int t = 0;
                        while (connection.id < 0)
                        {
                            Thread.Sleep(50);
                            t++;
                            if (t >= 20) throw new Exception("shell request timeout");
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        connection.socket.Send(Encoding.ASCII.GetBytes("Error: " + ex.Message));
                        Thread.Sleep(3000);
                        connection.socket.Close();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            shellEnabled = false;
        }

        void acceptShellConnection(byte arg)
        {
            try
            {
                var connection = pendingShellConnections.Dequeue();
                if (connection == null) return;
                connection.id = arg;
                shellConnections[connection.id] = connection;
                Debug.WriteLine(string.Format("Shell started, id={0}", connection.id));
                var thread = new Thread(listenShellConnection);
                shellConnecionThreads.Add(thread);
                thread.Start(connection);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("shell error: " + ex.Message);
            }
        }

        void newExecConnection(byte arg, string command)
        {
            try
            {
                var connection = (from c in pendingExecConnections where c.command == command select c).First();
                pendingExecConnections.Remove(connection);
                connection.id = arg;
                execConnections[arg] = connection;
                if (connection.stdin != null)
                {
                    if (connection.stdin.CanSeek)
                        connection.stdin.Seek(0, SeekOrigin.Begin);
                    var buffer = new byte[8 * 1024];
                    int l;
                    do
                    {
                        l = connection.stdin.Read(buffer, 0, buffer.Length);
                        if (l > 0)
                            writeUsb(ClovershellCommand.CMD_EXEC_STDIN, (byte)connection.id, buffer, l);
                    } while (l > 0);
                    writeUsb(ClovershellCommand.CMD_EXEC_STDIN, (byte)connection.id); // eof
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("exec error: " + ex.Message);
            }
        }

        void execOut(byte arg, byte[] data, int pos, int len)
        {
            var c = execConnections[arg];
            if (c == null) return;
            if (c.stdout != null)
                c.stdout.Write(data, pos, len);
            if (len == 0)
                c.stdoutFinished = true;
        }

        void execErr(byte arg, byte[] data, int pos, int len)
        {
            var c = execConnections[arg];
            if (c == null) return;
            if (c.stderr != null)
                c.stderr.Write(data, pos, len);
            if (len == 0)
                c.stderrFinished = true;
        }

        void execResult(byte arg, byte[] data, int pos, int len)
        {
            var c = execConnections[arg];
            if (c == null) return;
            c.result = data[pos];
            c.finished = true;
        }

        void listenShellConnection(object o)
        {
            var connection = o as ShellConnection;
            var socket = connection.socket;
            var id = connection.id;
            try
            {
                var buff = new byte[1024];
                while (socket.Connected)
                {
                    var l = socket.Receive(buff);
                    if (l > 0)
                        writeUsb(ClovershellCommand.CMD_SHELL_IN, (byte)id, buff, l);
                    else
                        break;
                }
            }
            catch (Exception)
            {
            }
            Debug.WriteLine(string.Format("Client {0} disconnected", id));
            socket.Close();
            shellConnections[id] = null;
            shellConnecionThreads.Remove(Thread.CurrentThread);
        }

        void shellOut(byte id, byte[] data, int pos, int len)
        {
            try
            {
                if (shellConnections[id] == null) return;
                shellConnections[id].socket.Send(data, pos, len, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("socket write error: " + ex.Message);
            }
        }

        void shellClosed(byte id)
        {
            if (shellConnections[id] == null) return;
            shellConnections[id].socket.Close();
            shellConnections[id] = null;
        }

        public void Dispose()
        {
            Enabled = false;
            ShellEnabled = false;
        }

        public TimeSpan IdleTime
        {
            get { return DateTime.Now - lastAliveTime; }
        }
        public int Ping()
        {
            var rnd = new Random();
            var data = new byte[4];
            rnd.NextBytes(data);
            lastPingResponse = null;
            var start = DateTime.Now;
            writeUsb(ClovershellCommand.CMD_PING, 0, data);
            int t = 100;
            while ((lastPingResponse == null || !lastPingResponse.SequenceEqual(data)) && (t > 0))
            {
                Thread.Sleep(10);
                t--;
            }
            if (t <= 0) return -1;
            return (int)(DateTime.Now - start).TotalMilliseconds;
        }

        public int Execute(string command, Stream stdin, Stream stdout, Stream stderr, int timeout = 0)
        {
            var c = new ExecConnection(command, stdin, stdout, stderr);
            pendingExecConnections.Add(c);
            writeUsb(ClovershellCommand.CMD_EXEC_NEW_REQ, 0, Encoding.UTF8.GetBytes(command));
            while (!c.finished || !c.stdoutFinished || !c.stderrFinished)
            {
                Thread.Sleep(50);
                if (!Online)
                    throw new Exception("device goes offline");
                if (timeout > 0 && IdleTime.TotalMilliseconds > timeout)
                    throw new Exception("clovershell read timeout");
            }
            execConnections[c.id] = null;
            return c.result;
        }
    }
}

