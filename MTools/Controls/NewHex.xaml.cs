using McuTools.Interfaces.WPF;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for NewHex.xaml
    /// </summary>
    public partial class NewHex : Window
    {
        public NewHex()
        {
            InitializeComponent();
        }

        private int CalculateSize(int value)
        {
            int multiply = 1;
            foreach (var radio in WpfHelpers.FindChildren<RadioButton>(MultiplySelect))
            {
                if (radio.IsChecked == true)
                {
                    multiply = Convert.ToInt32(radio.Content);
                    break;
                }
            }
            return multiply * value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;
            int val = Convert.ToInt32(s.Content);
            SizeSlider.Value = CalculateSize(val);
        }

        public int FileSize
        {
            get { return (int)SizeSlider.Value; }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
