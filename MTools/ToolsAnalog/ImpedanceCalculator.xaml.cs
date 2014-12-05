using McuTools.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
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

        private Complex Replus (Complex a, Complex b)
        {
            return (a * b) / (a + b);
        }

        private bool IsNull(Complex a)
        {
            return (a.Imaginary == 0) && (a.Real == 0);
        }

        private string ComplexString(Complex input)
        {
            if (input.Imaginary >= 0) return string.Format("{0:0.0000} +{1:0.0000}j | ABS: {2:0.0000}", input.Real, input.Imaginary, input.Magnitude);
            else return string.Format("{0:0.0000} {1:0.0000}j | ABS: {2:0.0000}", input.Real, input.Imaginary, input.Magnitude);
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            
            Stack<Complex> parts = new Stack<Complex>();
            Complex Zt = new Complex();
            double Omega = Math.PI * 2 * Freq.Value;
            double resonance = 0;

            if (L1.Value != 0) parts.Push(new Complex(0, Omega * L1.Value));
            if (R1.Value != 0) parts.Push(new Complex(R1.Value, 0));
            if (C1.Value != 0) parts.Push(new Complex(0, -1 * (1 / (Omega * C1.Value))));

            if (Selector.SelectedIndex == 0)
            {
                while (parts.Count > 0)
                {
                    Zt += parts.Pop();
                }
            }
            else
            {
                if (parts.Count >= 2)
                {
                    Zt = Replus(parts.Pop(), parts.Pop());
                    if (parts.Count > 0) Zt = Replus(Zt, parts.Pop());
                }
                else if (parts.Count > 0) Zt = parts.Pop();
            }
            if (L1.Value != 0 && C1.Value != 0)
            {
                resonance = 1 / (Math.PI * Math.Sqrt(L1.Value * C1.Value));
                Resonance.Text = string.Format("{0:0.0000}", resonance);
            }
            Z.Text = ComplexString(Zt);
            Cosfi.Text = string.Format("{0:0.0000}, ({1:0.0000} °)", Cosinus(Zt.Phase), Rad2Deg(Zt.Phase));
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
