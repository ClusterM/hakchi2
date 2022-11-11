using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace com.clusterrr.hakchi_gui.Wireless.Bluetooth
{
    class BluetoothControl : IDisposable
    {
        private delegate void CmdDataHandler(string line, string untrimmedLine);
        private event CmdDataHandler OnCmdData;
        public enum EventType { New, Delete, Change }
        public delegate void OnDataHandler(EventType eventType, BluetoothDeviceData data, BluetoothDeviceData.DataType dataType = BluetoothDeviceData.DataType.Undefined);
        public event OnDataHandler OnData;

        private Thread cmdThread;
        private TcpClient cmdConnection;
        private NetworkStream cmdStream;
        private StreamReader cmdReader;
        private StreamWriter cmdWriter;
        private List<BluetoothCommand> _cmdQueue = new List<BluetoothCommand>();
        public string Address { get; private set; } = null;
        public int Port { get; private set; } = 787;
        public bool CanRunCommand { get; private set; }
        private bool CanRunCommands = false;
        public bool Connected { get => cmdConnection?.Connected ?? false; }

        private readonly Regex ChangeRegex = new Regex(@"^\[(CHG)] (Device|Controller) ([A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}) ([^:]+): (.+)$", RegexOptions.Compiled);
        private readonly Regex NewDelRegex = new Regex(@"^\[(NEW|DEL)] (Device|Controller) ([A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}) *(.+)$", RegexOptions.Compiled);
        private readonly Regex DeviceRegex = new Regex(@"^\s*Device ([A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}) *(.*)\s*$", RegexOptions.Compiled);
        private readonly Regex ControllerRegex = new Regex(@"^Controller ([A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}) *(.*)\s*$", RegexOptions.Compiled);
        private readonly Regex InfoRegex = new Regex(@"^\s+([^:]+):\s+(.*)\s*$", RegexOptions.Compiled);

        private Dictionary<string, BluetoothDeviceData> _Devices = new Dictionary<string, BluetoothDeviceData>();
        public IReadOnlyCollection<BluetoothCommand> cmdQueue { get => _cmdQueue; }
        public IReadOnlyDictionary<string, BluetoothDeviceData> Devices { get => _Devices; }

        public enum BluetoothDeviceType { Controller, Device }

        public class BluetoothCommand
        {
            public string Command;
            public Action<string[]> Callback = null;
        }
        public class BluetoothDeviceData
        {
            public enum DataType { Undefined, Type, Address, Name, Alias, Class, Icon, Pairable, Powered, Discovering, Paired, Trusted, Blocked, Connected, LegacyPairing, Modalias, Uuid,
                PairableTimeout
            }
            public BluetoothDeviceType Type;
            public string Address;
            public string Name;
            public string Alias;
            public string Class;
            public string Icon;
            public bool Pairable = false;
            public bool Powered = false;
            public bool Discovering = false;
            public bool Pairing = false;
            public bool Paired = false;
            public bool Trusted = false;
            public bool Blocked = false;
            public bool Connected = false;
            public bool LegacyPairing = false;
            public string Modalias;
            public List<string> Uuid = new List<string>();
            public long PairableTimeout;

            public bool Trusting = false;
        }

        public BluetoothControl()
        {
            OnCmdData += BluetoothControl_OnCmdData;
            OnData += BluetoothControl_OnData;
        }

        private void BluetoothControl_OnData(EventType eventType, BluetoothDeviceData data, BluetoothDeviceData.DataType dataType = BluetoothDeviceData.DataType.Undefined)
        {
            if (eventType == EventType.Change && dataType == BluetoothDeviceData.DataType.Trusted && data.Trusting == true && data.Trusted == true)
            {
                data.Trusting = false;
                PairDevice(data.Address);
                data.Pairing = true;
            }

            if (eventType == EventType.Change && dataType == BluetoothDeviceData.DataType.Paired && data.Pairing == true)
            {
                data.Pairing = false;
                ConnectDevice(data.Address);
            }
        }

        public void StartListener(string address, int port = 787, bool probe = true)
        {
            _cmdQueue.Clear();
            _Devices.Clear();

            CanRunCommand = false;
            CanRunCommands = false;

            Address = address;
            Port = port;
            cmdThread = new Thread(ListenerThread);
            cmdThread.Start();

            new Thread(() =>
            {
                Thread.Sleep(500);
                if (probe)
                {
                    ProbeDevices();
                }
                CanRunCommands = true;
                cmdWriter?.WriteLine("");
                cmdWriter?.Flush();
            }).Start();
        }

        public void StopListener()
        {
            if (cmdThread?.IsAlive ?? false)
            {
                #warning Refactor this to get rid of Thread.Abort!
                cmdThread.Abort();
            }
            cmdThread = null;
            _cmdQueue.Clear();
            _Devices.Clear();
        }

        private BluetoothDeviceData GetDeviceData(string deviceAddress, bool add = true)
        {
            BluetoothDeviceData data;
            if (!_Devices.TryGetValue(deviceAddress, out data))
            {
                data = new BluetoothDeviceData()
                {
                    Address = deviceAddress
                };
                if (add)
                {
                    _Devices.Add(deviceAddress, data);
                }
            }
            return data;
        }

        private void ProcessDevices(string[] lines)
        {
            Match match;
            foreach (var line in lines)
            {
                if ((match = DeviceRegex.Match(line)).Success)
                {
                    string deviceAddress = match.Groups[1].Value;
                    RunCommand($"info {deviceAddress}", ProcessDevice);
                }
            }
        }

        private void RemoveAllDevices(string[] lines)
        {
            Match match;
            foreach (var line in lines)
            {
                if ((match = DeviceRegex.Match(line)).Success)
                {
                    string deviceAddress = match.Groups[1].Value;
                    UntrustDevice(deviceAddress);
                    RemoveDevice(deviceAddress);
                }
            }
        }

        private void ProcessDevice(string[] lines)
        {
            Match match = null;
            if (lines.Length > 0)
            {
                foreach (var line in lines)
                {
                    var newMatch = DeviceRegex.Match(line);

                    if (newMatch.Success)
                    {
                        match = newMatch;
                        continue;
                    }

                    if (match?.Success ?? false)
                    {
                        var chgLine = $"[CHG] Device {match.Groups[1].Value} {line.Substring(1)}";
                        BluetoothControl_OnCmdData(chgLine, chgLine);
                    }
                }
            }
        }
        private void BluetoothControl_OnCmdData(string line, string untrimmedLine)
        {
            Match match;

            if ((match = ControllerRegex.Match(line)).Success)
            {
                BluetoothControl_OnCmdData($"[NEW] {line}", $"[NEW] {line}");
                return;
            }

            if ((match = NewDelRegex.Match(line)).Success)
            {
                string matchType = match.Groups[1].Value;
                string deviceType = match.Groups[2].Value;
                string deviceAddress = match.Groups[3].Value;
                string deviceName = match.Groups[4].Value.Trim();

                BluetoothDeviceData data = GetDeviceData(deviceAddress);

                data.Type = deviceType == "Device" ? BluetoothDeviceType.Device : BluetoothDeviceType.Controller;
                data.Name = deviceName;

                if (matchType == "NEW")
                {
                    OnData?.Invoke(EventType.New, data);
                    if (data.Type == BluetoothDeviceType.Device)
                    {
                        RunCommand($"info {deviceAddress}", ProcessDevice);
                    }
                }
                else if (matchType == "DEL")
                {
                    if (_Devices.Keys.Contains(deviceAddress))
                    {
                        _Devices.Remove(deviceAddress);
                    }

                    OnData?.Invoke(EventType.Delete, data);
                }
            }

            if ((match = ChangeRegex.Match(line)).Success)
            {
                // @"\[(CHG)] (Device|Controller) ([A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}:[A-F0-9]{2}) ([^:]+): (.+)"
                string matchType = match.Groups[1].Value;
                string deviceType = match.Groups[2].Value;
                string deviceAddress = match.Groups[3].Value;
                string keyName = match.Groups[4].Value.Trim();
                string keyValue = match.Groups[5].Value.Trim();
                BluetoothDeviceData.DataType keyType = BluetoothDeviceData.DataType.Undefined;
                BluetoothDeviceData data = GetDeviceData(deviceAddress);

                data.Type = deviceType == "Device" ? BluetoothDeviceType.Device : BluetoothDeviceType.Controller;

                switch (keyName)
                {
                    case "Name":
                        keyType = BluetoothDeviceData.DataType.Name;
                        data.Name = keyValue;
                        break;

                    case "Alias":
                        keyType = BluetoothDeviceData.DataType.Alias;
                        data.Alias = keyValue;
                        break;

                    case "Class":
                        keyType = BluetoothDeviceData.DataType.Class;
                        data.Class = keyValue;
                        break;

                    case "Powered": // yes/no
                        keyType = BluetoothDeviceData.DataType.Powered;
                        data.Powered = keyValue == "yes";
                        break;

                    case "Icon":
                        keyType = BluetoothDeviceData.DataType.Icon;
                        data.Icon = keyValue;
                        break;

                    case "Paired": // yes/no
                        keyType = BluetoothDeviceData.DataType.Paired;
                        data.Paired = keyValue == "yes";
                        break;

                    case "Pairable": // yes/no
                        keyType = BluetoothDeviceData.DataType.Pairable;
                        data.Pairable = keyValue == "yes";
                        break;

                    case "PairableTimeout":
                        keyType = BluetoothDeviceData.DataType.PairableTimeout;
                        data.PairableTimeout = Convert.ToInt32(keyValue, 16);
                        break;

                    case "Trusted": // yes/no
                        keyType = BluetoothDeviceData.DataType.Trusted;
                        data.Trusted = keyValue == "yes";
                        break;

                    case "Blocked": // yes/no
                        keyType = BluetoothDeviceData.DataType.Blocked;
                        data.Blocked = keyValue == "yes";
                        break;

                    case "Connected": // yes/no
                        keyType = BluetoothDeviceData.DataType.Connected;
                        data.Connected = keyValue == "yes";
                        break;

                    case "LegacyPairing":
                        keyType = BluetoothDeviceData.DataType.LegacyPairing;
                        data.LegacyPairing = keyValue == "yes";
                        break;

                    case "Modalias":
                        keyType = BluetoothDeviceData.DataType.Modalias;
                        data.Modalias = keyValue;
                        break;

                    case "Discovering": // yes/no
                        keyType = BluetoothDeviceData.DataType.Discovering;
                        data.Discovering = keyValue == "yes";
                        break;
                }

                if (keyType != BluetoothDeviceData.DataType.Undefined)
                {
                    OnData?.Invoke(EventType.Change, data, keyType);
                }
            }

        }

        private void ListenerThread()
        {
            try
            {
                cmdConnection = new TcpClient();
                cmdConnection.Connect(Address, Port);

                cmdStream = cmdConnection.GetStream();
                cmdReader = new StreamReader(cmdStream);
                cmdWriter = new StreamWriter(cmdStream);

                string lastLine = "";
                string line;
                string cleanedLine;
                string trimmedLine;
                var escapeRegex = new Regex(@"(\x1b\[[0-9;]*[a-zA-Z]|\u0001|\u0002)", RegexOptions.Compiled);

                while (cmdConnection.Connected && (line = cmdReader?.ReadLine()) != null)
                {
                    cleanedLine = escapeRegex.Replace(line, "");
                    trimmedLine = cleanedLine.Trim();

                    if (trimmedLine == lastLine || trimmedLine.Length == 0) continue;
                    lastLine = trimmedLine;

                    if (trimmedLine.EndsWith("]#"))
                    {
                        CanRunCommand = true;
                        if (CanRunCommands)
                        {
                            if (_cmdQueue.Count > 0)
                            {
                                var firstCommand = _cmdQueue[0];
                                _cmdQueue.RemoveAt(0);
                                RunCommand(firstCommand);
                            }
                        }
                    }
                    else
                    {
                        Trace.WriteLine($"{(trimmedLine.StartsWith("[bluetooth]") ? "" : "[bluetooth] ")}{trimmedLine}");
                    }

                    OnCmdData?.Invoke(trimmedLine, cleanedLine);
                }

                if (cmdConnection?.Connected ?? false)
                {
                    cmdConnection?.Close();
                }
                cmdConnection = null;
                cmdReader = null;
                cmdWriter = null;
                cmdStream = null;
            }
            catch (ThreadAbortException)
            {
                if (cmdConnection?.Connected ?? false)
                {
                    cmdConnection?.Close();
                }
                cmdConnection = null;
                cmdReader = null;
                cmdWriter = null;
                cmdStream = null;
            }
            catch (Exception ex) { }
        }

        public void SetPower(bool state) => RunCommand($"power {(state ? "on" : "off")}");
        public void SetPairable(bool state) => RunCommand($"pairable {(state ? "on" : "off")}");
        public void SetDiscoverable(bool state) => RunCommand($"discoverable {(state ? "on" : "off")}");
        public void SetScan(bool state) => RunCommand($"scan {(state ? "on" : "off")}");
        public void TrustDevice(string deviceAddress) => RunCommand($"trust {deviceAddress}");
        public void UntrustDevice(string deviceAddress) => RunCommand($"untrust {deviceAddress}");
        public void BlockDevice(string deviceAddress) => RunCommand($"block {deviceAddress}");
        public void UnblockDevice(string deviceAddress) => RunCommand($"unblock {deviceAddress}");
        public void PairDevice(string deviceAddress) => RunCommand($"pair {deviceAddress}");
        public void RemoveDevice(string deviceAddress) => RunCommand($"remove {deviceAddress}");
        public void ConnectDevice(string deviceAddress) => RunCommand($"connect {deviceAddress}");
        public void DisconnectDevice(string deviceAddress) => RunCommand($"disconnect {deviceAddress}");
        public void RemoveAllDevices() {
            foreach(var device in Devices.Values.Where(e => e.Type == BluetoothDeviceType.Device))
            {
                UntrustDevice(device.Address);
                RemoveDevice(device.Address);
            }
        }
        public void ProbeDevices()
        {
            RunCommand("list");
            RunCommand("devices", ProcessDevices);
        }

        public void RunCommand(BluetoothCommand command)
        {
            if (CanRunCommand && CanRunCommands)
            {
                CanRunCommand = false;
                cmdWriter?.WriteLine(command.Command);
                cmdWriter?.Flush();

                if (command.Callback != null)
                {
                    var callbacks = new CmdDataHandler[1];
                    var lines = new List<string>();
                    var skipLines = 1;
                    callbacks[0] = (string trimmedLine, string untrimmedLine) =>
                    {
                        if (trimmedLine.Contains("]# ")) return;

                        if (trimmedLine.EndsWith("]#"))
                        {
                            if (skipLines > 0)
                            {
                                skipLines -= 1;
                                return;
                            }
                            OnCmdData -= callbacks[0];
                            command.Callback(lines.ToArray());
                            return;
                        }

                        lines.Add(untrimmedLine);
                    };

                    OnCmdData += callbacks[0];
                }
            }
            else
            {
                _cmdQueue.Add(command);
            }
        }

        public void RunCommand(string command, Action<string[]> action = null)
        {
            RunCommand(new BluetoothCommand()
            {
                Command = command,
                Callback = action
            });
        }

        public void Dispose()
        {
            cmdConnection?.Close();
            cmdConnection = null;
        }
    }
}
