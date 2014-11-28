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
    /// Interaction logic for OhmCalculator.xaml
    /// </summary>
    public partial class OhmCalculator : UserControl
    {
        private bool _loaded;

        public OhmCalculator()
        {
            InitializeComponent();
        }

        private void R_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            O.Text = (RV.Value / RA.Value).ToString();
        }

        private void V_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            V.Text = (VA.Value * VO.Value).ToString();
        }

        private void C_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            A.Text = (CV.Value / CO.Value).ToString();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }
    }
}
