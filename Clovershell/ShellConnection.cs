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
        internal Socket socket;
        internal int id;
        internal Thread shellConnectionThread;

        public ShellConnection(ClovershellConnection connection, Socket socket)
        {
            this.connection = connection;
            this.socket = socket;
            id = -1;
            socket.Send(new byte[] { 0xFF, 0xFD, 0x03 }); // Do Suppress Go Ahead
            socket.Send(new byte[] { 0xFF, 0xFB, 0x03 }); // Will Suppress Go Ahead
            socket.Send(new byte[] { 0xFF, 0xFB, 0x01 }); // Will Echo
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
                    {
                        int start = 0;
                        int pos = 0;
                        do
                        {
                            if ((pos + 1 < l) && (buff[pos] == '\r') && (buff[pos + 1] == '\n')) // New line?
                            {
                                // Hey, dot not send \r\n! I'll cut it to \n
                                buff[pos] = (byte)'\n';
                                connection.writeUsb(ClovershellConnection.ClovershellCommand.CMD_SHELL_IN, (byte)id, buff, start, pos - start + 1);
                                pos += 2;
                                start = pos;
                            }
                            else if ((pos + 1 < l) && (buff[pos] == 0xFF)) // Telnet command?
                            {
                                if (buff[pos + 1] == 0xFF) // Or just 0xFF...
                                {
                                    connection.writeUsb(ClovershellConnection.ClovershellCommand.CMD_SHELL_IN, (byte)id, buff, start, pos - start + 1);
                                    pos += 2;
                                    start = pos;
                                }
                                else if (pos + 2 < l)
                                {
                                    if (pos - start > 0)
                                        connection.writeUsb(ClovershellConnection.ClovershellCommand.CMD_SHELL_IN, (byte)id, buff, start, pos - start);
                                    var cmd = buff[pos + 1]; // Telnet command code
                                    var opt = buff[pos + 2]; // Telnet option code
#if VERY_DEBUG
                                    Debug.WriteLine(string.Format("Telnet command: CMD={0:X2} ARG={1:X2}", cmd, opt));
#endif
                                    pos += 3;
                                    start = pos;
                                }
                            }
                            else pos++; // No, moving to next character
                            if ((pos == l) && (l - start > 0)) // End of packet
                            {
                                connection.writeUsb(ClovershellConnection.ClovershellCommand.CMD_SHELL_IN, (byte)id, buff, start, l - start);
                            }
                        } while (pos < l);
                    }
                    else
                        break;
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (ClovershellException ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (socket.Connected)
                    socket.Send(Encoding.ASCII.GetBytes("Error: " + ex.Message));
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
