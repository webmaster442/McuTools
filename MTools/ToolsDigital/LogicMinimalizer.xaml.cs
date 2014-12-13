using McuTools.Interfaces.WPF;
using MTools.classes;
using MTools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MTools
{
    /// <summary>
    /// Interaction logic for LogicMinimalizer.xaml
    /// </summary>
    public partial class LogicMinimalizer : UserControl
    {
        private bool _loaded;
        private string ABC;
        private int _variables;
        private ObservableCollection<LogicItem> _items;
        private bool reversed = false;

        public LogicMinimalizer()
        {
            InitializeComponent();
            _loaded = false;
            ABC = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        }

        private void GenerateTable(int variables, bool reverse, IEnumerable<LogicItem> values = null)
        {
            _items.Clear();
            if (reverse)
            {
                char[] arr = ABC.Substring(0, variables).ToCharArray();
                Array.Reverse(arr);
                Variables.Header = new string(arr);
            }
            else Variables.Header = ABC.Substring(0, variables);
            int limit = 1 << variables;
            if (values != null)
            {
                foreach (var item in values) _items.Add(item);
            }
            else
            {
                for (int i = 0; i < limit; i++)
                {
                    _items.Add(new LogicItem() { Index = i, BinaryValue = LogicItem.GetBinaryValue(i, variables), Checked = false });
                }
            }
        }

        private void InputByNumbers()
        {
            _variables = (int)MintermNumbers.Value;
            try
            {
                if (string.IsNullOrEmpty(MintermInput.Text)) throw new Exception("No minterm numbers entered");
                string[] items = MintermInput.Text.Split(',');
                string[] dontcare = null;
                if (!string.IsNullOrEmpty(DontcarInput.Text))
                {
                    dontcare = DontcarInput.Text.Split(',');
                    if (dontcare.Length < 1) items = DontcarInput.Text.Split(' ');
                }
                if (items.Length < 1) items = MintermInput.Text.Split(' ');
                if (items.Length < 1) throw new Exception("Incorrect input");
                List<LogicItem> litems = new List<LogicItem>();
                foreach (var item in items)
                {
                    litems.Add(new LogicItem()
                    {
                        Index = Convert.ToInt32(item),
                        BinaryValue = LogicItem.GetBinaryValue(Convert.ToInt32(item), _variables),
                        Checked = true
                    });
                }
                if (dontcare != null)
                {
                    foreach (var item in dontcare)
                    {
                        litems.Add(new LogicItem()
                        {
                            Index = Convert.ToInt32(item),
                            BinaryValue = LogicItem.GetBinaryValue(Convert.ToInt32(item), _variables),
                            Checked = null
                        });
                    }
                }
                SimpleMinterm.Text = QuineMcclusky.GetSimplified(litems, _variables, (bool)HazardSafe.IsChecked, (bool)LsbBit.IsChecked, false);
                SimpleMaxterm.Text = QuineMcclusky.GetSimplified(litems, _variables, (bool)HazardSafe.IsChecked, (bool)LsbBit.IsChecked, true);
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }

        private void LogicMinim_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _items = new ObservableCollection<LogicItem>();
            _variables = 2;
            TrouthTable.DataContext = _items;
            GenerateTable(2, (bool)LsbBit.IsChecked);
            _loaded = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            _variables = Convert.ToInt32(((RadioButton)sender).Content.ToString());
            if (_variables < 6) SwitchMinterm.IsEnabled = true;
            else SwitchMinterm.IsEnabled = false;
            GenerateTable(_variables, (bool)LsbBit.IsChecked);
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            //string simple = null;
            IMintermTable mintermatble = null;
            switch (InputMode.SelectedIndex)
            {
                case 0:
                    SimpleMinterm.Text = QuineMcclusky.GetSimplified(_items, _variables, (bool)HazardSafe.IsChecked, (bool)LsbBit.IsChecked, false);
                    SimpleMaxterm.Text = QuineMcclusky.GetSimplified(_items, _variables, (bool)HazardSafe.IsChecked, (bool)LsbBit.IsChecked, true);
                    break;
                case 1:
                    mintermatble = Minterm2x;
                    _variables = 2;
                    break;
                case 2:
                    mintermatble = Minterm3x;
                    _variables = 3;
                    break;
                case 3:
                    mintermatble = Minterm4x;
                    _variables = 4;
                    break;
                case 4:
                    mintermatble = Minterm5x;
                    _variables = 5;
                    break;
                case 5:
                    InputByNumbers();
                    break;

            }
            if (mintermatble != null)
            {
                SimpleMinterm.Text = QuineMcclusky.GetSimplified(mintermatble.Selected, _variables, (bool)HazardSafe.IsChecked, (bool)LsbBit.IsChecked, false);
                SimpleMaxterm.Text = QuineMcclusky.GetSimplified(mintermatble.Selected, _variables, (bool)HazardSafe.IsChecked, (bool)LsbBit.IsChecked, true);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            SimpleMinterm.Text = "";
            IMintermTable mintermatble = null;
            switch (InputMode.SelectedIndex)
            {
                case 0:
                    foreach (var i in McuTools.Interfaces.WPF.WpfHelpers.FindChildren<RadioButton>(VarButtons))
                    {
                        if (i.IsChecked == true)
                        {
                            RadioButton_Checked(i, null);
                            break;
                        }
                    }
                    break;
                case 1:
                    mintermatble = Minterm2x;
                    break;
                case 2:
                    mintermatble = Minterm3x;
                    break;
                case 3:
                    mintermatble = Minterm4x;
                    break;
                case 4:
                    mintermatble = Minterm5x;
                    break;
            }
            if (mintermatble != null)
            {
                mintermatble.ClearInput();
                if (LsbBit.IsChecked == true || reversed)
                {
                    mintermatble.SwapVarnames();
                    reversed = !reversed;
                }
            }
        }

        private void TrouthTableView(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;
            switch (s.Name)
            {
                case "Minterm5":
                    GenerateTable(5, false, Minterm5x.Selected);
                    break;
                case "Minterm4":
                    GenerateTable(4, false, Minterm4x.Selected);
                    break;
                case "Minterm3":
                    GenerateTable(3, false, Minterm3x.Selected);
                    break;
                case "Minterm2":
                    GenerateTable(2, false, Minterm2x.Selected);
                    break;
            }
            InputMode.SelectedIndex = 0;
        }

        private void SwitchMinterm_Click(object sender, RoutedEventArgs e)
        {
            switch (_variables)
            {
                case 2:
                    Minterm2x.Selected = _items.ToList();
                    break;
                case 3:
                    Minterm3x.Selected = _items.ToList();
                    break;
                case 4:
                    Minterm4x.Selected = _items.ToList();
                    break;
                case 5:
                    Minterm5x.Selected = _items.ToList();
                    break;
            }
            InputMode.SelectedIndex = _variables - 1;
        }
    }
}
