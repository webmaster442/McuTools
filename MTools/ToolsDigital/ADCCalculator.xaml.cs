using McuTools.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTools
{
    /// <summary>
    /// Interaction logic for ADCCalculator.xaml
    /// </summary>
    public partial class ADCCalculator : UserControl
    {
        private bool _loaded;

        public ADCCalculator()
        {
            InitializeComponent();
            _loaded = false;
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            ulong maxval = (ulong)1 << (int)ADCResolution.Value;
            double volts = ADCRefVolt.Value /  maxval;

            VoltsperVal.Text = (volts * 1000).ToString();

            if (DigitalIput.Value > maxval) DigitalIput.Value = maxval;
            if (DigitalIput.Value < 0) DigitalIput.Value = 0; 

            DigitaltoVolts.Text = (volts * DigitalIput.Value * 1000).ToString();

            if (VoltInput.Value > ADCRefVolt.Value) VoltInput.Value = ADCRefVolt.Value;
            if (VoltInput.Value < 0) VoltInput.Value = 0;

            OutNum.Text = Math.Round((VoltInput.Value / volts), 0).ToString();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }
    }
}
