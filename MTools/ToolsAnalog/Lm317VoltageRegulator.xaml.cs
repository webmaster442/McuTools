using McuTools.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTools
{
    /// <summary>
    /// Interaction logic for Lm317VoltageRegulator.xaml
    /// </summary>
    public partial class Lm317VoltageRegulator : UserControl
    {
        private bool _loaded;

        public Lm317VoltageRegulator()
        {
            InitializeComponent();
            _loaded = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double vout = 1.25 * (1 + (R2.Value / R1.Value)) + (0.00005 * R2.Value);
            if (Vin.Value < vout) vout = Vin.Value - 1.25;
            if (vout < 0) vout = 0;
            Vout.Text = vout.ToString();
        }

        private void CurrentCalculate(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double amp = 1.25 / CR1.Value;
            double pwr = amp * amp * CR1.Value;
            AmperOut.Text = amp.ToString();
            Power.Text = pwr.ToString();
        }
    }
}
