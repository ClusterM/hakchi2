using System;
using System.IO;
using System.Threading.Tasks;

namespace com.clusterrr.hakchi_gui
{
    public delegate void OnConnectedEventHandler(ISystemShell caller);
    public delegate void OnDisconnectedEventHandler();
    public interface ISystemShell : IDisposable
    {
        bool Enabled { get; set; }
        bool IsOnline { get; }
        bool ShellEnabled { get; set; }
        ushort ShellPort { get; }
        void Connect();
        void Disconnect();
        event OnConnectedEventHandler OnConnected;
        event OnDisconnectedEventHandler OnDisconnected;
        string ExecuteSimple(string command, int timeout = 2000, bool throwOnNonZero = false);
        int Execute(string command, Stream stdin = null, Stream stdout = null, Stream stderr = null, int timeout = 0, bool throwOnNonZero = false);
        Task<string> ExecuteSimpleAsync(string command, int timeout = 2000, bool throwOnNonZero = false);
        Task<int> ExecuteAsync(string command, Stream stdin = null, Stream stdout = null, Stream stderr = null, int timeout = 0, bool throwOnNonZero = false);
    }

    public class UnknownShell : ISystemShell
    {
        public bool Enabled { get { return false; } set { } }
        public bool IsOnline { get { return false; } }
        public bool ShellEnabled { get { return false; } set { } }
        public ushort ShellPort { get { return 0; } }
        public void Connect() { }
        public void Disconnect() { }
        public event OnConnectedEventHandler OnConnected = delegate { };
        public event OnDisconnectedEventHandler OnDisconnected = delegate { };
        public string ExecuteSimple(string command, int timeout = 2000, bool throwOnNonZero = false) { return string.Empty; }
        public int Execute(string command, Stream stdin = null, Stream stdout = null, Stream stderr = null, int timeout = 0, bool throwOnNonZero = false) { return 255; }
        public Task<string> ExecuteSimpleAsync(string command, int timeout = 2000, bool throwOnNonZero = false)
        {
            return new Task<string>(() => {
                return string.Empty;
            });
        }
        public Task<int> ExecuteAsync(string command, Stream stdin = null, Stream stdout = null, Stream stderr = null, int timeout = 0, bool throwOnNonZero = false)
        {
            return new Task<int>(() => {
                return 255;
            });
        }
        public void Dispose() { }
    }

}
