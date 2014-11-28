using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for PrefixInput.xaml
    /// </summary>
    public partial class PrefixInput : UserControl
    {
        private Dictionary<string, int> _prefixcodes;
        private bool _hadprefix;

        public PrefixInput()
        {
            InitializeComponent();
            _hadprefix = false;
            _prefixcodes = new Dictionary<string, int>();
            _prefixcodes.Add("p", 0);
            _prefixcodes.Add("n", 1);
            _prefixcodes.Add("u", 2);
            _prefixcodes.Add("m", 3);
            _prefixcodes.Add("k", 5);
            _prefixcodes.Add("M", 6);
            _prefixcodes.Add("G", 7);
        }

        public static DependencyProperty LabelTextPropery = DependencyProperty.Register("LabelText", typeof(string), typeof(PrefixInput));
        public static DependencyProperty UnitTextPropery = DependencyProperty.Register("UnitText", typeof(string), typeof(PrefixInput), new PropertyMetadata("-"));

        public string LabelText
        {
            get { return (string)GetValue(LabelTextPropery); }
            set { SetValue(LabelTextPropery, value); }
        }

        public string UnitText
        {
            get { return (string)GetValue(UnitTextPropery); }
            set { SetValue(UnitTextPropery, value); }
        }

        public double Value
        {
            get
            {
                try
                {
                    string valtext = TbValue.Text;
                    foreach (var prefix in _prefixcodes.Keys)
                    {
                        if (valtext.Contains(prefix))
                        {
                            valtext = valtext.Replace(prefix, "");
                            break;
                        }
                    }
                    double conv = Convert.ToDouble(valtext);
                    switch (PrefixSelect.SelectedIndex)
                    {
                        case 0:
                            conv *= Math.Pow(10, -12);
                            break;
                        case 1:
                            conv *= Math.Pow(10, -9);
                            break;
                        case 2:
                            conv *= Math.Pow(10, -6);
                            break;
                        case 3:
                            conv *= Math.Pow(10, -3);
                            break;
                        case 4:
                            break;
                        case 5:
                            conv *= Math.Pow(10, 3);
                            break;
                        case 6:
                            conv *= Math.Pow(10, 6);
                            break;
                        case 7:
                            conv *= Math.Pow(10, 9);
                            break;
                    }
                    return conv;
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                TbValue.Text = value.ToString();
            }
        }

        public event RoutedEventHandler ValueChanged;

        private void TbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool prefixstate = _hadprefix;
            bool find = false;
            foreach (var prefix in _prefixcodes.Keys)
            {
                if (TbValue.Text.Contains(prefix))
                {
                    PrefixSelect.SelectedIndex = _prefixcodes[prefix];
                    find = true;
                    break;
                }
            }
            _hadprefix = find;
            if (prefixstate && !_hadprefix) PrefixSelect.SelectedIndex = 4;
            if (ValueChanged != null) ValueChanged(this, new RoutedEventArgs());

        }

        private void PrefixSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ValueChanged != null) ValueChanged(this, new RoutedEventArgs());
        }
    }
}
