using com.clusterrr.hakchi_gui;
using Tmds.MDns;
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
    public class Devices
    {
        public class Entry
        {
            public IList<IPAddress> Addresses;
            public ushort Port;
            public string UniqueID;
            public string ConsoleType;
            public string ConsoleRegion;
        }

        public IList<Entry> Available
        {
            get; private set;
        }

        ServiceBrowser serviceBrowser;
        string serviceName;
        string serviceType;

        public Devices(string name, string type)
        {
            Available = new List<Entry>();
            serviceName = name;
            serviceType = type;

            // enable service browser
            serviceBrowser = new ServiceBrowser();
            serviceBrowser.ServiceAdded += onServiceAdded;
            serviceBrowser.ServiceChanged += onServiceChanged;
            serviceBrowser.ServiceRemoved += onServiceRemoved;
            serviceBrowser.StartBrowse(type);
        }

        public void Abort()
        {
            serviceBrowser.StopBrowse();
            Available.Clear();
        }

        private void debugAnnouncement(string header, ServiceAnnouncement a)
        {
#if DEBUG
            Trace.WriteLine(header);
            Trace.Indent();
            Trace.WriteLine("Instance: " + a.Instance);
            Trace.WriteLine("Type: " + a.Type);
            Trace.WriteLine("IP: " + string.Join(", ", a.Addresses));
            Trace.WriteLine("Port: " + a.Port);
            Trace.WriteLine("Txt: " + string.Join(", ", a.Txt));
            Trace.Unindent();
#endif
        }

        private void onServiceAdded(object sender, ServiceAnnouncementEventArgs e)
        {
            // ignore other services
            if (e.Announcement.Instance != this.serviceName)
            {
                Trace.WriteLine("Ignoring service: " + e.Announcement.Instance);
                return;
            }

            // debug
            debugAnnouncement("Service added:", e.Announcement);

            // create entry
            var dev = new Entry()
            {
                Addresses = e.Announcement.Addresses,
                Port = e.Announcement.Port,
            };

            // build device info
            foreach (var txt in e.Announcement.Txt)
            {
                var tokens = txt.Split('=');
                if (tokens.Length == 2)
                {
                    switch (tokens[0])
                    {
                        case "hwid":
                            dev.UniqueID = tokens[1].Replace(" ", "").ToUpper();
                            break;

                        case "type":
                            dev.ConsoleType = tokens[1];
                            break;

                        case "region":
                            dev.ConsoleRegion = tokens[1];
                            break;
                    }
                }
            }

            // check to avoid adding duplicate devices
            foreach (var a in Available)
            {
                if (dev.Addresses.SequenceEqual(e.Announcement.Addresses))
                {
                    Trace.WriteLine("Duplicate announce for addresses: " + string.Join(", ", e.Announcement.Addresses));
                    return;
                }
                if (dev.UniqueID == a.UniqueID)
                {
                    Trace.WriteLine("Duplicate announce for same device: " + a.UniqueID);
                    return;
                }
            }

            Available.Add(dev);
        }

        private void onServiceChanged(object sender, ServiceAnnouncementEventArgs e)
        {
            // silently ignore other services
            if (e.Announcement.Instance != this.serviceName)
            {
                return;
            }
            debugAnnouncement("A service changed:", e.Announcement);
        }

        private void onServiceRemoved(object sender, ServiceAnnouncementEventArgs e)
        {
            // silently ignore other services
            if (e.Announcement.Instance != this.serviceName)
            {
                return;
            }
            debugAnnouncement("A service was removed:", e.Announcement);

            foreach (var a in Available)
            {
                if (a.Addresses.SequenceEqual(e.Announcement.Addresses))
                {
                    Available.Remove(a);
                    return;
                }
            }
            Trace.WriteLine("Service had not been detected before. Hmmm...");
        }

    }
}
