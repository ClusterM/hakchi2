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
        public IList<Device> Available
        {
            get
            {
                Trace.WriteLine("Querying dns...");
                IList<Device> devices = new List<Device>();
                try
                {
                    IPHostEntry ihe = Dns.GetHostEntry(serviceName);
                    if (ihe.AddressList != null && ihe.AddressList.Length > 0)
                    {
                        Trace.WriteLine($"Found {ihe.AddressList.Length} addresses...");
                        foreach (var address in ihe.AddressList)
                        {
                            devices.Add(new Device() { Addresses = new List<IPAddress>() { address }, Port = SshClientWrapper.DEFAULT_SSH_PORT });
                        }
                    }
                }
                catch
                {
                    Trace.WriteLine("No response!");
                }
                return devices;
            }
        }

        private string serviceName;

        public DnsListener(string name)
        {
            serviceName = name;
        }

        public void Dispose()
        {

        }
    }
}
