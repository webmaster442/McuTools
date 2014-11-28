using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Linq;
using System.Windows.Controls;

namespace MTools
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AdapterList : Window
    {
        NetworkInterface[] _adapters;
        private bool _loaded;

        public AdapterList()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _adapters = (from i in NetworkInterface.GetAllNetworkInterfaces() orderby i.Name select i).ToArray();
            foreach (var adapter in _adapters)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up && adapter.GetIPProperties().UnicastAddresses.Count > 0)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = adapter.Name;
                    AdaptersList.Items.Add(item);
                }
            }
            _loaded = true;
        }

        private IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }

        private int GetMaskBits(IPAddress mask)
        {
            int count = 0;
            byte[] bytes = mask.GetAddressBytes();
            foreach (var i in bytes)
            {
                string result = Convert.ToString(i, 2);
                for (int j = 0; j < result.Length; j++)
                {
                    if (result[j] == '1') ++count;
                }
            }
            return count;
        }

        private void AdapterSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string search = (AdaptersList.SelectedItem as ListBoxItem).Content.ToString();
                var adapter = (from i in _adapters where i.Name == search select i).FirstOrDefault();
                var addr = from i in adapter.GetIPProperties().UnicastAddresses where i.Address.AddressFamily == AddressFamily.InterNetwork select i.Address;
                IPAddress adress = addr.FirstOrDefault();
                IPAddress mask = GetSubnetMask(adress);

                int maskbits = GetMaskBits(mask);


                uint m = ~(uint.MaxValue >> maskbits);

                byte[] ipBytes = adress.GetAddressBytes();

                byte[] maskBytes = BitConverter.GetBytes(m).Reverse().ToArray();

                byte[] startIPBytes = new byte[ipBytes.Length];
                byte[] endIPBytes = new byte[ipBytes.Length];

                // Calculate the bytes of the start and end IP addresses.
                for (int i = 0; i < ipBytes.Length; i++)
                {
                    startIPBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
                    endIPBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
                }

                // Convert the bytes to IP addresses.
                ++startIPBytes[3];
                --endIPBytes[3];
                Start = new IPAddress(startIPBytes);
                End = new IPAddress(endIPBytes);

                this.DialogResult = true;
            }
            catch (Exception)
            {
                this.DialogResult = false;
            }
            this.Close();
        }

        public IPAddress Start
        {
            private set;
            get;
        }

        public IPAddress End
        {
            private set;
            get;
        }

        private void AdaptersList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AdapterSelected_Click(sender, null);
        }
    }
}
