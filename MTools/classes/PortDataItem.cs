using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTools.classes
{
    internal class PortDataItem : INotifyPropertyChanged
    {
        private string _Description;
        private int _Port;
        private bool _IsUDP;
        private bool _IsChecked;

        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                FirePropertyChangedEvent("Description");
            }
        }

        public int Port
        {
            get { return _Port; }
            set
            {
                _Port = value;
                FirePropertyChangedEvent("Port");
            }
        }

        public bool IsUDP
        {
            get { return _IsUDP; }
            set
            {
                _IsUDP = value;
                FirePropertyChangedEvent("Port");
            }
        }

        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                _IsChecked = value;
                FirePropertyChangedEvent("IsChecked");
            }
        }

        public PortDataItem() { }

        public PortDataItem(int port, string description, bool ischecked = false, bool isUDP = false)
        {
            this.Port = port;
            this.Description = description;
            this.IsChecked = ischecked;
            this.IsUDP = isUDP;
        }

        private void FirePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static void FillCollection(ref ObservableCollection<PortDataItem> target)
        {
            if (target == null) return;
            target.Add(new PortDataItem(9, "Wake on LAN", true, true));
            target.Add(new PortDataItem(21, "FTP Command", true));
            target.Add(new PortDataItem(22, "SSH", true));
            target.Add(new PortDataItem(23, "Telnet", true));
            target.Add(new PortDataItem(25, "SMTP"));
            target.Add(new PortDataItem(53, "DNS", true, true));
            target.Add(new PortDataItem(67, "DHCP", true, true));
            target.Add(new PortDataItem(68, "DHCP", true, true));
            target.Add(new PortDataItem(69, "TFTP", false, true));
            target.Add(new PortDataItem(80, "HTTP", true));
            target.Add(new PortDataItem(81, "TOR"));
            target.Add(new PortDataItem(82, "TOR", false, true));
            target.Add(new PortDataItem(110, "POP3"));
            target.Add(new PortDataItem(123, "NTP", false, true));
            target.Add(new PortDataItem(161, "SNMP", false, true));
            target.Add(new PortDataItem(194, "IRC"));
            target.Add(new PortDataItem(220, "IMAP"));
            target.Add(new PortDataItem(443, "HTTPS", true));
            target.Add(new PortDataItem(445, "Microsoft-DS SMB file sharing / Windows shares", true));
            target.Add(new PortDataItem(514, "Syslog", false, true));
            target.Add(new PortDataItem(520, "RIP", false, true));
            target.Add(new PortDataItem(521, "RIPng", false, true));
            target.Add(new PortDataItem(860, "iSCSI", true));
            target.Add(new PortDataItem(989, "FTPS"));
            target.Add(new PortDataItem(990, "FTPS"));
            target.Add(new PortDataItem(994, "IRCS"));
            target.Add(new PortDataItem(995, "POP3S"));
            target.Add(new PortDataItem(1080, "SOCKS Proxy"));
            target.Add(new PortDataItem(1194, "OpenVPN"));
            target.Add(new PortDataItem(1293, "IPSEC"));
            target.Add(new PortDataItem(1725, "STEAM Client"));
            target.Add(new PortDataItem(2049, "NFS"));
            target.Add(new PortDataItem(3389, "RDP, Microsoft Terminal Server", true));
            target.Add(new PortDataItem(5000, "UPNP/UnPNP", true));
        }
    }
}
