using McuTools.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTools
{
    /// <summary>
    /// Interaction logic for OpAmpCalculator.xaml
    /// </summary>
    public partial class OpAmpCalculator : UserControl
    {
        bool _loaded = false;

        public OpAmpCalculator()
        {
            InitializeComponent();
        }

        private void OpampCalc_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }

        private void NoninvertingCalc(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double vout = NoninvVin.Value * (1 + (NoninvR2.Value / NoninvR1.Value));
            NoninvVout.Text = vout.ToString();
        }

        private void InvertingCalc(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double vout = -1 * (InvRf.Value / InvRin.Value) * InvVin.Value;
            InvVout.Text = vout.ToString();
        }

        private void DifCalc(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double p1, p2;
            p1 = ((DifR1.Value + DifRf.Value) / DifR1.Value) * (DifRg.Value / (DifRg.Value + DifR2.Value)) * DifV2.Value;
            p2 = (DifRf.Value / DifR1.Value) * DifV1.Value;
            DifVout.Text = (p1 - p2).ToString();
        }

        private void SumCalc(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double vout = -1 * SumRf.Value * ((SumV1.Value / SumR1.Value) + (SumV2.Value / SumR2.Value));
            SumVout.Text = vout.ToString();
        }

        private void InstCalc(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double vdif = InstV2.Value - InstV1.Value;
            double r32 = InstR3.Value / InstR2.Value;
            double p1 =  1 + ((2 * InstR1.Value) / InstRgain.Value);
            InstVout.Text = (p1 * r32 * vdif).ToString();
        }

        private void ComparatorCalc(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            if (CompV1.Value > CompV2.Value) CompVout.Text = VsP.Value.ToString();
            else if (CompV1.Value < CompV2.Value) CompVout.Text = VsN.Value.ToString();
            else CompVout.Text = "0";
        }

    }
}
