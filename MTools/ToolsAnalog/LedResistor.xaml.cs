using McuTools.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTools
{
    /// <summary>
    /// Interaction logic for LedResistor.xaml
    /// </summary>
    public partial class LedResistor : UserControl
    {
        private bool _loaded = false;

        public LedResistor()
        {
            InitializeComponent();
        }

        private void LedCalculate(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double r = 0;
            double p = 0;
            switch (Mode.SelectedIndex)
            {
                case 0:
                    r = (SuplyVoltage.Value - (LedVoltage.Value * NumLeds.Value)) / LedCurrent.Value;
                    break;
                case 1:
                    r = (SuplyVoltage.Value - LedVoltage.Value) / (LedCurrent.Value * NumLeds.Value);
                    break;
            }
            p = (SuplyVoltage.Value * SuplyVoltage.Value) / r;
            ResistorValue.Text = r.ToString();
            ResistorPower.Text = p.ToString();
        }

        private void Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LedCalculate(sender, null);
        }

        private void LedResistiorCalc_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            LedCalculate(sender, e);
            _loaded = true;
        }
    }
}
