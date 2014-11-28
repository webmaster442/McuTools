using System;
using System.Windows;
using System.Windows.Controls;

namespace MTools.ToolsDigital
{
    /// <summary>
    /// Interaction logic for ColorCalc.xaml
    /// </summary>
    public partial class ColorCalc : UserControl
    {
        private bool _loaded;

        public ColorCalc()
        {
            _loaded = false;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }

        private void Colpicker_ColorSelected(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            Calc();
        }

        private void PrefixInput_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            Calc();
        }

        private double Map(double Value, double InMinimum, double InMaximum, double OutMinimum, double OutMaximum) 
        { 
            return Math.Round((Value - InMinimum) * (OutMaximum - OutMinimum) /  (InMaximum - InMinimum) + OutMinimum, 0);
        }

        private string Conv(int value)
        {
            string temp = Convert.ToString(value, 16);
            if (temp.Length == 1) temp = "0" + temp;
            return temp.ToUpper();
        }

        private void Calc()
        {
            double maxpwmval = ((1 << (int)PWMBits.Value) - 1) * ((double)Colorpicker.Color.A / 255.0d);
            TbRed.Text = Map(Colorpicker.Color.R, 0, 255, 0, maxpwmval).ToString();
            TbGreen.Text = Map(Colorpicker.Color.G, 0, 255, 0, maxpwmval).ToString();
            TbBlue.Text = Map(Colorpicker.Color.B, 0, 255, 0, maxpwmval).ToString();

            int r, g, b;
            r = (int)Map(Colorpicker.Color.R, 0, 255, 0, 31);
            b = (int)Map(Colorpicker.Color.B, 0, 255, 0, 31);
            g = (int)Map(Colorpicker.Color.G, 0, 255, 0, 63);
            TbRGB565.Text = String.Format("{0}{1}{2}", Conv(r), Conv(g), Conv(b));
        }

        private void ColorTable_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            Colorpicker.Color = ColorTable.SelectedColor;
        }
    }
}
