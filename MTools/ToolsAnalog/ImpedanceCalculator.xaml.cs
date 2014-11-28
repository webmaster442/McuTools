using McuTools.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTools
{
    /// <summary>
    /// Interaction logic for ImpedanceCalculator.xaml
    /// </summary>
    public partial class ImpedanceCalculator : UserControl
    {
        private bool _loaded;

        public ImpedanceCalculator()
        {
            InitializeComponent();
            _loaded = false;
        }

        private double Deg2Rad(double deg)
        {
            return (Math.PI / 180) * deg;
        }

        private double Rad2Deg(double rad)
        {
            return (rad * 180) / Math.PI;
        }

        private double Cosinus(double value1)
        {
            if ((((value1 - (Math.PI / 2)) % Math.PI) == 0) || value1 == (Math.PI / 2)) return 0;
            else return Math.Cos(value1);
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double xl, xc, z, angle, cosfi, resonance;

            if (Selector.SelectedIndex == 0)
            {
                if (L1.Value == 0) xl = 0;
                else xl = (Math.PI * 2) * Freq.Value * L1.Value;

                if (C1.Value == 0) xc = 0;
                else xc = 1 / ((Math.PI * 2) * Freq.Value * C1.Value);

                z = Math.Sqrt(Math.Pow(R1.Value, 2) + Math.Pow((xl - xc), 2));
                angle = Math.Round(Rad2Deg(Math.Atan((xl - xc) / R1.Value)), 5);
            }
            else
            {
                if (L1.Value == 0) xl = 0;
                else xl = 1 / ((Math.PI * 2) * Freq.Value * L1.Value);

                if (C1.Value == 0) xc = 0;
                else xc = (Math.PI * 2) * Freq.Value * C1.Value;

                z = Math.Sqrt(Math.Pow(R1.Value, 2) + Math.Pow((xc - xl), 2));
                angle = Math.Round(Rad2Deg(Math.Atan((xc * R1.Value) - (R1.Value / xl))));
            }
            
            resonance = 1 / ((Math.PI * 2) * Math.Sqrt(L1.Value * C1.Value));
            cosfi = Math.Round(Cosinus(Deg2Rad(angle)), 5);

            Z.Text = z.ToString();
            Resonance.Text = resonance.ToString();
            Cosfi.Text = String.Format("{0} ({1}°)", cosfi.ToString(), angle.ToString());
        }

        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Calculate(sender, null);
        }

        private void RLC_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }
    }
}
