using com.clusterrr.hakchi_gui;
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
    public class DnsListener : IListener
    {
        public const int TTL = 60;
        public const int TTR = 15;

        public IList<Device> Available
        {
            get
            {
                if (DateTime.Now.Subtract(lastChecked) > TimeSpan.FromSeconds(TTR))
                {
                    lastChecked = DateTime.Now;
                    query();
                }
                return devices;
            }
        }

        private string serviceName;
        private List<Device> devices;
        private DateTime lastUpdated;
        private DateTime lastChecked;

        private void query()
        {
            if (DateTime.Now.Subtract(lastUpdated) > TimeSpan.FromSeconds(TTL))
            {
                devices.Clear();
                try
                {
                    IPHostEntry ihe = Dns.GetHostEntry(serviceName);
                    if (ihe.AddressList != null && ihe.AddressList.Length > 0)
                    {
                        Trace.WriteLine($"DNS Resolution returned IPs: " + string.Join(", ", (IEnumerable<IPAddress>)ihe.AddressList));
                        lastUpdated = DateTime.Now;
                        foreach (var address in ihe.AddressList)
                            devices.Add(new Device() { Addresses = new List<IPAddress>() { address }, Port = SshClientWrapper.DEFAULT_SSH_PORT });
                    }
                }
                catch
                {
                    // no-op (dns resolution fail causes exception)
                }
            }
        }

        public DnsListener(string name)
        {
            serviceName = name;
            devices = new List<Device>();
            lastUpdated = DateTime.Now.Subtract(TimeSpan.FromSeconds(TTL));
            lastChecked = DateTime.Now;
        }

        public void Dispose()
        {
            devices.Clear();
            devices = null;
        }
    }
}
