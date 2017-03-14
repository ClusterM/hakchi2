using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace com.clusterrr.clovershell
{
    internal class ShellConnection : IDisposable
    {
        public readonly ClovershellConnection connection;
        Socket socket;
        internal int id;
        internal Thread shellConnectionThread;

        public ShellConnection(ClovershellConnection connection, Socket socket)
        {
            this.connection = connection;
            this.socket = socket;
            id = -1;
        }

        internal void shellConnectionLoop()
        {
            try
            {
                var buff = new byte[1024];
                while (socket.Connected)
                {
                    var l = socket.Receive(buff);
                    if (l > 0)
                        connection.writeUsb(ClovershellConnection.ClovershellCommand.CMD_SHELL_IN, (byte)id, buff, l);
                    else
                        break;
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (ClovershellException)
            {
            }
            finally
            {
                shellConnectionThread = null;
            }
            Debug.WriteLine(string.Format("Shell client {0} disconnected", id));
            socket.Close();
            connection.shellConnections[id] = null;
        }
        
        public void Dispose()
        {
            if (shellConnectionThread != null)
                shellConnectionThread.Abort();
            if (socket != null)
                socket.Close();
            socket = null;
            if (id > 0)
                connection.shellConnections[id] = null;
        }

        internal void Send(byte[] data, int pos, int len)
        {
            socket.Send(data, pos, len, SocketFlags.None);
        }
    }

}
