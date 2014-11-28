using McuTools.Interfaces;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTools
{
    /// <summary>
    /// Interaction logic for AsciiTable.xaml
    /// </summary>
    public partial class AsciiTable : UserControl
    {
        private bool _loaded;

        public AsciiTable()
        {
            InitializeComponent();
        }

        string[] NonPrint = { "NULL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS", "TAB", "LF",
                              "VT", "FF", "CR", "SO", "SI", "DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN",
                              "ETB", "CAN", "EM", "SUB", "ESC", "FS", "GS", "RS", "US", "Space"};


        private void Generate()
        {
            StringBuilder target = new StringBuilder();
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int index = (j * 32) + i;
                    if (index < 33) target.AppendFormat("Dec: {0,3} | Hex: {1,2} |  {2}\t", index, Convert.ToString(index, 16).ToUpper(), NonPrint[index]);
                    else target.AppendFormat("Dec: {0,3} | Hex: {1,2} |  {2}\t", index, Convert.ToString(index, 16).ToUpper(), (char)index);
                }
                target.Append("\r\n");
            }
            
            Ascii.Text = target.ToString();
            target.Clear();

            for (int i = 128; i < 160; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int index = (j * 32) + i;
                    target.AppendFormat("Dec: {0,3} | Hex: {1,2} |  {2}\t", index, Convert.ToString(index, 16).ToUpper(), Encoding.GetEncoding(850).GetString(new byte[] { (byte)index }));
                }
                target.Append("\r\n");
            }

            
            
            
            Extended.Text = target.ToString();
            target.Clear();
            target = null;
        }

        private void AsciiTables_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            Ascii.Clear();
            Generate();
            _loaded = true;
        }
    }
}
