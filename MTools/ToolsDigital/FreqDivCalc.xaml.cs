using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MTools.ToolsDigital
{
    /// <summary>
    /// Interaction logic for FreqDivCalc.xaml
    /// </summary>
    public partial class FreqDivCalc : UserControl
    {
        private bool _loaded;

        public FreqDivCalc()
        {
            InitializeComponent();
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Bits.Value; i++)
            {
                ulong value = 1UL << i;
                double result = Clock.Value / value;
                if (double.IsNaN(result) || double.IsNegativeInfinity(result) || double.IsPositiveInfinity(value)) break;
                sb.AppendFormat("counter pin {0} clock: {1} Hz\r\n", i, result);
            }
            results.Text = sb.ToString();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
        }
    }
}
