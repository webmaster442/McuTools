using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for ToleranceSelector.xaml
    /// </summary>
    public partial class ToleranceSelector : UserControl
    {
        public ToleranceSelector()
        {
            InitializeComponent();
            _value = "10%";
        }

        string _value;

        public event RoutedEventHandler ValueChanged;

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton s = (ToggleButton)sender;
            foreach (ToggleButton btn in ToleranceGrid.Children)
            {
                if (btn != s) btn.IsChecked = false;
            }
            _value = s.Content.ToString();
            if (ValueChanged != null) ValueChanged(sender, e);
        }

        public string Value
        {
            get { return _value; }
        }
    }
}
