using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTools.ToolsAnalog
{
    /// <summary>
    /// Interaction logic for FilterDesigner.xaml
    /// </summary>
    public partial class FilterDesigner : UserControl
    {
        private bool _loaded;
        public FilterDesigner()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }

        private void ValueChanged(object sender, RoutedEventArgs e)
        {
            Calculate();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Calculate();
        }

        private void Calculate()
        {
            if (!_loaded) return;
            double f = 0, r = 0;
            switch (Tabs.SelectedIndex)
            {
                case 0:
                    f = 1 / (Math.PI * 2 * LowpassR.Value * LowpassC.Value);
                    r = 1 / (LowpassR.Value * LowpassC.Value);
                    break;
                case 1:
                    f = 1 / (Math.PI * 2 * HighpassR.Value * HighpassC.Value);
                    r = 1 / (HighpassR.Value * HighpassC.Value);
                    break;

            }
            TbHertz.Text = f.ToString();
            TbRad.Text = r.ToString();
        }
    }
}
