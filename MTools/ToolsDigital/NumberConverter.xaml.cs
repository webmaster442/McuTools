using McuTools.Interfaces;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTools
{
    /// <summary>
    /// Interaction logic for NumberConverter.xaml
    /// </summary>
    public partial class NumberConverter : UserControl
    {
        public NumberConverter()
        {
            InitializeComponent();
        }

        private void ConvertSys()
        {
            try
            {
                long input = 0;
                var input_radio = InputSelector.Children.OfType<RadioButton>().FirstOrDefault(i => i.IsChecked.Value);
                var output_radio = OutputSelector.Children.OfType<RadioButton>().FirstOrDefault(i => i.IsChecked.Value);
                switch (input_radio.Content.ToString())
                {
                    case "Decimal":
                        input = Convert.ToInt64(InputNumber.Text, 10);
                        break;
                    case "Binary":
                        input = Convert.ToInt64(InputNumber.Text, 2);
                        break;
                    case "Octal":
                        input = Convert.ToInt64(InputNumber.Text, 8);
                        break;
                    case "Hexa":
                        input = Convert.ToInt64(InputNumber.Text, 16);
                        break;
                }
                switch (output_radio.Content.ToString())
                {
                    case "Decimal":
                        OutputNumber.Text = Convert.ToString(input, 10);
                        break;
                    case "Binary":
                        OutputNumber.Text = Convert.ToString(input, 2);
                        break;
                    case "Octal":
                        OutputNumber.Text = Convert.ToString(input, 8);
                        break;
                    case "Hexa":
                        OutputNumber.Text = Convert.ToString(input, 16);
                        break;
                }
            }
            catch (Exception)
            {
                if (OutputNumber != null) OutputNumber.Text = "Conversion Error";
            }

        }

        private void InputNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConvertSys();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ConvertSys();
        }

        public string Description
        {
            get { return "Number System conversion"; }
        }
    }
}
