using McuTools.Interfaces.WPF;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MTools.ToolOther
{
    /// <summary>
    /// Interaction logic for File2Array.xaml
    /// </summary>
    public partial class File2Array : UserControl
    {
        public File2Array()
        {
            InitializeComponent();
        }

        private static string ArrayToText(byte[] array, int len, int byteinrow = 8)
        {
            StringBuilder sb = new StringBuilder();
            string hex;
            for (int i=0; i<len; i++)
            {
                hex = Convert.ToString(array[i], 16).ToUpper();
                if (hex.Length < 2) hex = "0" + hex;
                sb.AppendFormat("0x{0}, ", hex);
                if (((i+1) % byteinrow) == 0) sb.AppendFormat("\r\n");
            }
            return sb.ToString();
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder result = new StringBuilder();
                result.Append("static byte file[] =\r\n{\r\n");
                using (var file = File.Open(FFSInput.SelectedPath, FileMode.Open))
                {
                    int count;
                    byte[] buffer = new byte[4096];
                    do
                    {
                        count = file.Read(buffer, 0, buffer.Length);
                        result.Append(ArrayToText(buffer, count, (int)EsBytesRow.Value));
                    }
                    while (count > 0);
                    result.Remove(result.Length - 2, 2);
                    result.AppendLine("};");
                    Output.Text = result.ToString();
                }
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }
    }
}
