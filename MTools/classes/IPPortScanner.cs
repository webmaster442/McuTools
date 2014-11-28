using McuTools.Interfaces.WPF;
using MTools.Controls;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MTools.classes
{
    internal class IPPortScanner
    {

        public IPPortScanner() { PingTimeOut = 2200; }

        public IPAddress IPStart { get; set; }
        public IPAddress IPEnd { get; set; }
        public StackPanel Results { get; set; }

        public ObservableCollection<PortDataItem> Ports { get; set; }

        public int PingTimeOut { get; set; }

        private string ScanPorts(IPAddress addr)
        {
            StringBuilder sb = new StringBuilder();
            if (Ports == null) return "";
            foreach (var portdata in Ports)
            //Parallel.ForEach(Ports, portdata =>
            {
                try
                {
                    Socket s;
                    if (portdata.IsUDP) s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
                    else s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    s.Connect(addr, portdata.Port);
                    if (s.Connected)
                    {
                        if (portdata.IsUDP) sb.AppendFormat("UDP Port open: {0}, Possible service: {1}\r\n", portdata.Port, portdata.Description);
                        else sb.AppendFormat("TCP Port open: {0}, Possible service: {1}\r\n", portdata.Port, portdata.Description);
                        s.Disconnect(true);
                    }
                }
                catch (Exception) { }
            }//);
            return sb.ToString();
        }

        public int GetCount()
        {
            byte[] start = IPStart.GetAddressBytes();
            byte[] end = IPEnd.GetAddressBytes();
            int[] count = new int[start.Length];
            int c = 1;
            for (int i = 0; i < start.Length; i++)
            {
                count[i] = (end[i] - start[i]) + 1;
                c *= count[i];
            }
            return c;
        }

        public void Scann(IProgress<int> progress, CancellationToken ct, bool portscan = true)
        {
            if (IPStart == null || IPEnd == null) throw new ArgumentException("Start or End IP not specified");
            if (Results == null) throw new ArgumentException("No control set for results");
            byte[] start = IPStart.GetAddressBytes();
            byte[] end = IPEnd.GetAddressBytes();
            Results.Dispatcher.Invoke(new Action(delegate
                {
                    Results.Children.Clear();
                }));
            int done = 1;

            for (int i = 0; i < start.Length; i++)
            {
                if (end[i] < start[i]) throw new ArgumentException("End IP is smaller than Start IP");
            }

            for (byte a = start[0]; a <= end[0]; a++)
            {
                for (byte b = start[1]; b <= end[1]; b++)
                {
                    for (byte c = start[2]; c <= end[2]; c++)
                    {
                        Parallel.For(start[3], end[3], (d, loopstate) =>
                        //for (byte d = start[3]; d <= end[3]; d++)
                        {

                            try
                            {
                                ct.ThrowIfCancellationRequested();
                                IPAddress current = new IPAddress(new byte[] { a, b, c, (byte)d });
                                Ping ping = new Ping();
                                if (ping.Send(current, PingTimeOut).Status == IPStatus.Success)
                                {
                                    string result = null;
                                    if (portscan) result = ScanPorts(current);
                                    Results.Dispatcher.Invoke(new Action(delegate
                                        {
                                            ResultsView rv = new ResultsView();
                                            string host = "";
                                            try { host = Dns.GetHostEntry(current).HostName; }
                                            catch (Exception) { }
                                            rv.ComputerAdress = string.Format("{0} - {1}", current.ToString(), host);
                                            rv.PingResult = result;
                                            Results.Children.Add(rv);
                                        }));
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                loopstate.Break();
                                if (progress != null) progress.Report(0);
                                return;
                            }
                            if (progress != null) progress.Report(done);
                            done++;
                        });
                    }
                }
            }
            Results.Dispatcher.Invoke(new Action(delegate
            {
                System.Windows.Forms.MessageBox.Show("Scann completed", "Information", System.Windows.Forms.MessageBoxButtons.OK);
            }));
        }
    }
}
