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
    public class PingListener : IListener
    {
        public const ushort SSH_PORT = 22;

        public IList<Device> Available
        {
            get
            {
                IList<Device> devices = new List<Device>();
                try
                {
                    using (Ping pingSender = new Ping())
                    {
                        Trace.WriteLine("Pinging " + serviceName + "...");
                        PingReply reply = pingSender.Send(serviceName, 2000);
                        if (reply != null && reply.Status.Equals(IPStatus.Success))
                        {
                            Trace.WriteLine("Found device at address " + reply.Address + " !");
                            devices.Add(new Device() { Addresses = new List<IPAddress>() { reply.Address }, Port = SSH_PORT });
                        }
                        else
                        {
                            Trace.WriteLine("Ping no response.");
                        }
                    }
                }
                catch
                {
                    Trace.WriteLine("Ping failed.");
                }
                return devices;
            }
        }

        private string serviceName;

        public PingListener(string name)
        {
            serviceName = name;
        }

        public void Abort()
        {
        }
    }
}
