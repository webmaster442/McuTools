using System;
using System.Windows;
using System.Windows.Controls;

namespace MTools
{
    /// <summary>
    /// Interaction logic for ne555Calc.xaml
    /// </summary>
    public partial class ne555Calc : UserControl
    {
        private double ln2 = Math.Log(2, Math.E);
        private bool _loaded;

        public ne555Calc()
        {
             InitializeComponent();
        }

        private void Astable_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double f = 1 / ln2 * AstableC.Value * (AstableR1.Value + AstableR2.Value);
            double ton = ln2 * (AstableR1.Value + AstableR2.Value) * AstableC.Value;
            double toff = ln2 * AstableR2.Value * AstableC.Value;
            double total = ton + toff;

            double percent_on = (ton / total) * 100;
            double percent_off = (toff / total) * 100;

            AstableF.Text = string.Format("{0:0.00000}", f);
            AstableTOn.Text = string.Format("{0:0.00000} ({1:0.000} %)", ton, percent_on);
            AstableTOff.Text = string.Format("{0:0.00000} ({1:0.000} %)", toff, percent_off);
        }

        private void Monostable_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double t = MonostableR.Value * MonostableC.Value * Math.Log(3, Math.E);
            MonostableT.Text = string.Format("{0:0.00000}", t);
        }

        private void Ne555Calculator_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
            Astable_ValueChanged(sender, e);
            Monostable_ValueChanged(sender, e);
        }
    }
}
