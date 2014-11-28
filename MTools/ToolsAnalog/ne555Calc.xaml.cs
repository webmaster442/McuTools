using McuTools.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            AstableF.Text = f.ToString();
            AstableTOn.Text = ton.ToString();
            AstableTOff.Text = toff.ToString();
        }

        private void Monostable_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double t = MonostableR.Value * MonostableC.Value * Math.Log(3, Math.E);
            MonostableT.Text = t.ToString();
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
