using com.clusterrr.hakchi_gui;
using DnsUtils.Services;
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
    public class LlmnrListener : IListener
    {
        public const ushort SSH_PORT = 22;

        public IList<Device> Available
        {
            get
            {
                Trace.WriteLine("Querying llmnr...");
                IPAddress[] addresses = NameResolving.ResolveAsync("hakchi", 2000, llmnr).Result;

                IList<Device> devices = new List<Device>();
                if (addresses != null && addresses.Length > 0)
                {
                    Trace.WriteLine($"Found {addresses.Length} addresses...");
                    foreach (var address in addresses)
                    {
                        devices.Add(new Device() { Addresses = new List<IPAddress>() { address }, Port = SSH_PORT });
                    }
                }

                return devices;
            }
        }

        private string serviceName;
        private Llmnr llmnr;

        public LlmnrListener(string name)
        {
            serviceName = name;
            llmnr = new Llmnr();
        }

        public void Abort()
        {
            llmnr = null;
        }
    }
}
