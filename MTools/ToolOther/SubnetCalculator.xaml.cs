using MTools.classes;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MTools.ToolOther
{
    /// <summary>
    /// Interaction logic for SubnetCalculator.xaml
    /// </summary>
    public partial class SubnetCalculator : UserControl
    {
        public SubnetCalculator()
        {
            InitializeComponent();
            Mask.IP = SubnetMask.ClassC;
            Network.IP = IPAddress.Parse("192.168.1.0");
        }

        private int GetBits(int networks)
        {
            if (networks > 128) return 8;
            else if (networks > 64) return 7;
            else if (networks > 32) return 6;
            else if (networks > 16) return 5;
            else if (networks > 8) return 4;
            else if (networks > 4) return 3;
            else if (networks > 2) return 2;
            else return 1;
        }

        private int GetByteIndex(int netmasklength)
        {
            if (netmasklength > 23) return 3;
            else if (netmasklength > 15) return 2;
            else if (netmasklength > 7) return 1;
            else return 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Output.Clear();

            if (Network.IP == null || Mask.IP == null) return;
            int numnet = (int)ReqNetworks.Value;

            StringBuilder buffer = new StringBuilder();
            int requiredbits = GetBits(numnet);
            int maskbits = SubnetMask.GetBitLength(Mask.IP);
            int outmaskbits = requiredbits + maskbits;

            if (outmaskbits > 32)
            {
                buffer.AppendLine("IPv4 is too small for this");
                Output.Text = buffer.ToString();
                return;
            }

            buffer.AppendFormat("Required additional subnet bits for {0} networks: {1}\r\n", numnet, requiredbits);
            buffer.AppendFormat("New netmask: {0}/{1}\r\n", SubnetMask.CreateByNetBitLength(outmaskbits), outmaskbits);
            buffer.AppendLine("--------------------------------------------------------------------------------------");
            buffer.AppendLine();

            byte[] startnetwork = Network.IP.GetAddressBytes();
            List<int> indexes = new List<int>();
            int index = GetByteIndex(outmaskbits);
            int usedbitsinbyte =  maskbits - (index * 8);
            if (usedbitsinbyte < 0)
            {
                
            }
            else
            {
                int shiftvalue = 8 - usedbitsinbyte - requiredbits;
                int b = startnetwork[index];
                IPAddress tmp;

                for (int i = 0; i < numnet; i++)
                {
                    startnetwork[index] = (byte)(b | (i << shiftvalue));
                    tmp = new IPAddress(startnetwork);
                    buffer.AppendFormat("Subnet {0,-4} Network adress: {1,-15} Brodecast Adress: {2,-15}\r\n", i, tmp, tmp.GetBroadcastAddress(SubnetMask.CreateByNetBitLength(outmaskbits)));
                }
            }

            Output.Text = buffer.ToString();

        }
    }
}
