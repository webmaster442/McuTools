using MTools.Controls;
using System.Windows;
using System.Windows.Controls;

namespace MTools.ToolsAnalog
{
    /// <summary>
    /// Interaction logic for TotalResistance.xaml
    /// </summary>
    public partial class TotalResistance : UserControl
    {
        bool _loaded;
        int _serialc, _paralellc;
 
        public TotalResistance()
        {
            _loaded = false;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _serialc = 0;
            _paralellc = 0;
            _loaded = true;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (Tabs.SelectedIndex == 0)
            {
                Serial.Children.Clear();
                _serialc = 0;
            }
            else
            {
                Paralell.Children.Clear();
                _paralellc = 0;
            }
            pi_ValueChanged(null, null);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            PrefixInput pi = new PrefixInput();
            pi.LabelText = "R";
            pi.UnitText = "Ω";
            pi.Value = 1;
            pi.ValueChanged += pi_ValueChanged;

            if (Tabs.SelectedIndex == 0)
            {
                pi.UnitText += _serialc.ToString();
                Serial.Children.Add(pi);
                ++_serialc;
            }
            else
            {
                pi.UnitText += _paralellc.ToString();
                Paralell.Children.Add(pi);
                ++_paralellc;
            }
            pi_ValueChanged(null, null);
        }

        private void pi_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double total = 0;
            if (Tabs.SelectedIndex == 0)
            {
                foreach (PrefixInput r in Serial.Children) total += r.Value;
            }
            else
            {
                foreach (PrefixInput r in Paralell.Children) total += (1.00d / r.Value);
                total = 1.00d / total;
            }
            TbDisplay.Text = total.ToString();
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pi_ValueChanged(null, null);
        }
    }
}
