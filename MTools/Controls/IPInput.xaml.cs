using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class IPInput : UserControl
    {
        public IPInput()
        {
            InitializeComponent();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox s = (TextBox)sender;
            try
            {
                int szam = Convert.ToInt32(s.Text);
                if (szam < 0 || szam > 255) s.Text = "0";
            }
            catch (Exception)
            {
                s.Text = "0";
            }
        }

        public bool HasValidAdress
        {
            get
            {
                return !(string.IsNullOrEmpty(P1.Text) || string.IsNullOrEmpty(P2.Text) || string.IsNullOrEmpty(P3.Text) || string.IsNullOrEmpty(P4.Text));
            }
        }

        public IPAddress IP
        {
            get
            {
                IPAddress addr = new IPAddress(new byte[] { Convert.ToByte(P1.Text), Convert.ToByte(P2.Text), Convert.ToByte(P3.Text), Convert.ToByte(P4.Text) });
                return addr;
            }
            set
            {
                if (value.AddressFamily != AddressFamily.InterNetwork) return;
                byte[] bytes = value.GetAddressBytes();
                P1.Text = bytes[0].ToString();
                P2.Text = bytes[1].ToString();
                P3.Text = bytes[2].ToString();
                P4.Text = bytes[3].ToString();
            }
        }
    }
}
