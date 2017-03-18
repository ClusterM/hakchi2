using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace com.clusterrr.clovershell
{
    internal class ExecConnection : IDisposable
    {
        internal readonly ClovershellConnection connection;
        internal readonly string command;
        internal Stream stdin;
        internal Int32 stdinPipeSize;
        internal Int32 stdinQueue;
        internal Stream stdout;
        internal Stream stderr;
        internal int id;
        internal bool finished;
        internal int result;
        internal bool stdinFinished;
        internal bool stdoutFinished;
        internal bool stderrFinished;
        internal Thread stdinThread;
        internal DateTime LastDataTime;

        public ExecConnection(ClovershellConnection connection, string command, Stream stdin, Stream stdout, Stream stderr)
        {
            this.connection = connection;
            this.command = command;
            id = -1;
            stdinPipeSize = 0;
            stdinQueue = 0;
            this.stdin = stdin;
            this.stdout = stdout;
            this.stderr = stderr;
            finished = false;
            stdinFinished = false;
            stdoutFinished = false;
            stderrFinished = false;
            LastDataTime = DateTime.Now;
        }

        public void stdinLoop()
        {
            try
            {
                if (stdin == null) return;
                if (stdin.CanSeek)
                    stdin.Seek(0, SeekOrigin.Begin);
                var buffer = new byte[8 * 1024];
                int l;
                while (connection.IsOnline)
                {
                    l = stdin.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        connection.writeUsb(ClovershellConnection.ClovershellCommand.CMD_EXEC_STDIN, (byte)id, buffer, l);
                    else
                        break;
                    LastDataTime = DateTime.Now;
                    if (stdinQueue > 32 * 1024 && connection.IsOnline)
                    {
                        Debug.WriteLine(string.Format("queue: {0} / {1}, {2}MB / {3}MB ({4}%)",
                            stdinQueue, stdinPipeSize, stdin.Position / 1024 / 1024, stdin.Length / 1024 / 1024, 100 * stdin.Position / stdin.Length));
                        while (stdinQueue > 16 * 1024)
                        {
                            Thread.Sleep(50);
                            connection.writeUsb(ClovershellConnection.ClovershellCommand.CMD_EXEC_STDIN_FLOW_STAT_REQ, (byte)id);
                        }
                    }
                }
                connection.writeUsb(ClovershellConnection.ClovershellCommand.CMD_EXEC_STDIN, (byte)id); // eof
                if (stdinQueue > 0 && connection.IsOnline)
                {
                    Thread.Sleep(50);
                    connection.writeUsb(ClovershellConnection.ClovershellCommand.CMD_EXEC_STDIN_FLOW_STAT_REQ, (byte)id);
                }
                stdinFinished = true;
            }
            catch (ThreadAbortException) { }
            catch (ClovershellException ex)
            {
                Debug.WriteLine("stdin error: " + ex.Message + ex.StackTrace);
            }
            finally
            {
                stdinThread = null;
            }
        }

        public void Dispose()
        {
            if (stdinThread != null)
                stdinThread.Abort();            
        }
    }

}
