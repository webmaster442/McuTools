using McuTools.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTools
{
    /// <summary>
    /// Interaction logic for ResistorColorCode.xaml
    /// </summary>
    public partial class ResistorColorCode : UserControl
    {
        public ResistorColorCode()
        {

                InitializeComponent();
                B4_Result.Text = "0 Ω ±10%";
                B5_result.Text = "0 Ω ±10%";
        }


        public string Description
        {
            get { return "Resistor Color Code decoder"; }
        }

        private void Band4_ValueChanged(object sender, RoutedEventArgs e)
        {
            double val = ((B4_b1.Value * 10) + B4_b2.Value) * B4_b3.Value;
            B4_Result.Text = string.Format("{0} Ω ±{1}", val, B4_b4.Value);
        }

        private void Band5_ValueChanged(object sender, RoutedEventArgs e)
        {
            double val = ((B5_b1.Value * 100) + (B5_b2.Value *10) + (B5_b3.Value)) * B5_b4.Value;
            B5_result.Text = string.Format("{0} Ω ±{1}", val, B5_b5.Value);
        }
    }
}
