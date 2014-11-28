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
    /// Interaction logic for RColorSelect.xaml
    /// </summary>
    public partial class RColorSelect : UserControl
    {
        private bool _loaded;

        public RColorSelect()
        {
            InitializeComponent();
        }

        private double _value;

        public static DependencyProperty MultiplyerSelectorModeProperty = DependencyProperty.Register("MultiplyerSelectorMode", typeof(bool), typeof(RColorSelect), new PropertyMetadata(false));

        public bool MultiplyerSelectorMode
        {
            get { return (bool)GetValue(MultiplyerSelectorModeProperty); }
            set { SetValue(MultiplyerSelectorModeProperty, value); }
        }

        public event RoutedEventHandler ValueChanged;

        private void ValueSect_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton s = (ToggleButton)sender;
            foreach (ToggleButton btn in NumberSelector.Children)
            {
                if (btn != s) btn.IsChecked = false;
            }
            _value = DecodeValue(s.Content.ToString());
            if (ValueChanged != null) ValueChanged(sender, e);
        }

        private void MultiplyerSect_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton s = (ToggleButton)sender;
            foreach (ToggleButton btn in MultiplyerSelector.Children)
            {
                if (btn != s) btn.IsChecked = false;
            }
            _value = DecodeMultipy(s.Content.ToString());
            if (ValueChanged != null) ValueChanged(sender, e);
        }

        private double DecodeValue(string color)
        {
            switch (color)
            {
                case "Black":
                    return 0;
                case "Brown":
                    return 1;
                case "Red":
                    return 2;
                case "Orange":
                    return 3;
                case "Yellow":
                    return 4;
                case "Green":
                    return 5;
                case "Blue":
                    return 6;
                case "Purple":
                    return 7;
                case "Gray":
                    return 8;
                case "White":
                    return 9;
                default:
                    return double.NaN;
            }
        }

        private double DecodeMultipy(string color)
        {
            switch (color)
            {
                case "Black":
                    return 1;
                case "Brown":
                    return 10;
                case "Red":
                    return 100;
                case "Orange":
                    return 1000;
                case "Yellow":
                    return 10000;
                case "Green":
                    return 100000;
                case "Blue":
                    return 1000000;
                case "Purple":
                    return 10000000;
                case "Silver":
                    return 0.01;
                case "Gold":
                    return 0.1;
                default:
                    return double.NaN;
            }
        }


        public double Value
        {
            get
            {
                return _value;
            }
        }

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            if (MultiplyerSelectorMode) _value = 1;
            else _value = 0;
            _loaded = true;
        }
    }
}
