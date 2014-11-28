using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace MTools.ToolsDigital
{
    /// <summary>
    /// Interaction logic for SerialTerminal.xaml
    /// </summary>
    public partial class SerialTerminal : UserControl
    {
        private SerialPort _port;
        private bool _portopen;
        private bool _asciimode;
        private bool _loaded;

        public SerialTerminal()
        {
            InitializeComponent();
            _portopen = false;
            _asciimode = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            PortConfig();
            HexInput.CreateDefault();
            _loaded = true;
        }

        private void PortConfig()
        {
            ClosePort();
            var dialog = new SerialSettings();
            if (dialog.ShowDialog() == true)
            {
                _port = dialog.Port;
                _port.DataReceived += _port_DataReceived;
                _port.Open();
                _portopen = true;
                PortStatus.Text = _port.PortName;
            }
        }

        private void ClosePort()
        {
            if (_port != null)
            {
                if (_port.IsOpen) _port.Close();
                _port = null;
                PortStatus.Text = "No Port opened";
            }
        }

        private string GetTimeStamp()
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2},{3:D4}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
        }

        void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            if (_asciimode)
            {
                for (int i = 0; i < _port.BytesToRead; i++) sb.Append((char)_port.ReadChar());
            }
            else
            {
                for (int i = 0; i < _port.BytesToRead; i++)
                {

                    sb.Append(_port.ReadByte());
                    sb.Append(" ");
                }
            }

            Dispatcher.BeginInvoke(new Action(delegate
                {
                    TbRecieve.AppendText(sb.ToString());
                    TbRecieve.ScrollToEnd();
                }));
        }

        private byte[] StringToBytes(string input)
        {
            List<byte> text = new List<byte>();
            text.AddRange(Encoding.ASCII.GetBytes(input));
            string linebreak = CBLinebreak.SelectedItem.ToString();
            switch (linebreak)
            {
                case "CR":
                    text.AddRange(Encoding.ASCII.GetBytes("\r"));
                    break;
                case "LF":
                    text.AddRange(Encoding.ASCII.GetBytes("\n"));
                    break;
                case "CR+LF":
                    text.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                    break;
            }
            return text.ToArray();
        }

        private void SendData()
        {
            if (!_portopen) return;

            if (InputMode.SelectedIndex == 0)
            {
                if (RbASCII.IsChecked == true)
                {
                    byte[] b = StringToBytes(TbInput.Text);
                    _port.Write(b, 0, b.Length);
                    LbSend.Items.Add("Send text: " + TbInput.Text + "\r\n");
                }
                else
                {
                    string[] values = TbInput.Text.Split(' ');
                    if (RbByte.IsChecked == true)
                    {
                        List<byte> bval = new List<byte>();
                        try { foreach (var v in values) bval.Add(Convert.ToByte(v)); }
                        catch (Exception) { MessageBox.Show("Error parsing input", "Error", MessageBoxButton.OK); }
                        _port.Write(bval.ToArray(), 0, bval.Count);
                        LbSend.Items.Add("Send bytes: " + TbInput.Text);
                    }
                }
                if (CbClear.IsChecked == true) TbInput.Clear();
            }
            else
            {
                byte[] data = HexInput.GetBytes();
                _port.Write(data, 0, data.Length);
                LbSend.Items.Add("Send bytes: " + ByteArrayToString(data));
                HexInput.CreateDefault();
            }
        }

        private string ByteArrayToString(byte[] input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in input)
            {
                sb.Append(Convert.ToString(b, 16));
                sb.Append(" ");
            }
            return sb.ToString();
        }

        private void TbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && CbEnterSend.IsChecked == true) SendData();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendData();
        }

        private void RbASCII_Checked(object sender, RoutedEventArgs e)
        {
            _asciimode = (bool)RbASCII.IsChecked;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ClosePort();
        }

        private void BtnOpenPort_Click(object sender, RoutedEventArgs e)
        {
            PortConfig();
        }

        private void LbSend_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LbSend.SelectedItem == null) return;
            TbInput.Text = LbSend.SelectedItem.ToString();
        }

        private void BtnClearRec_Click(object sender, RoutedEventArgs e)
        {
            TbRecieve.Clear();
        }

        private void BtnClearSend_Click(object sender, RoutedEventArgs e)
        {
            LbSend.Items.Clear();
        }

    }
}
