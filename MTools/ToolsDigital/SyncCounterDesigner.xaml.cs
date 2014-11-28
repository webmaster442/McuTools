using MTools.classes;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MTools.ToolsDigital
{
    /// <summary>
    /// Interaction logic for SyncCounterDesigner.xaml
    /// </summary>
    public partial class SyncCounterDesigner : UserControl
    {
        private bool _loaded;
        private int _variables;
        private ObservableCollection<string> _items;
        private ObservableCollection<Counter> _counter;
        private string _flipflop;

        public SyncCounterDesigner()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _items = new ObservableCollection<string>();
            _counter = new ObservableCollection<Counter>();
            _flipflop = "SR";
            CounterDesigner.DataContext = _counter;
            _variables = 2;
            Generate();
            _loaded = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            _variables = Convert.ToInt32(((RadioButton)sender).Content.ToString());
            Generate();
        }

        private void Generate()
        {
            _counter.Clear();
            _items.Clear();
            int limit = 1 << _variables;
            for (int i = 0; i < limit; i++) _items.Add(LogicItem.GetBinaryValue(i, _variables));
            StageCurrent.ItemsSource = _items;
            StageNext.ItemsSource = _items;
        }

        private void FlipFlopCheck(object sender, RoutedEventArgs e)
        {
            _flipflop = ((RadioButton)sender).Content.ToString();
        }

        private LogicItem[] GenerateMinterms()
        {
            LogicItem[] ret = new LogicItem[1 << _variables];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = new LogicItem();
                ret[i].Checked = null;
                ret[i].Index = i;
                ret[i].BinaryValue = LogicItem.GetBinaryValue(i, _variables);
            }
            return ret;
        }

        private bool? DecodeFlipFlop(char c, char n, string flipfloptype)
        {
            switch (flipfloptype)
            {
                case "S":
                    if (c == '0' && n == '0') return false;
                    else if (c == '0' && n == '1') return true;
                    else if (c == '1' && n == '0') return false;
                    else return null;
                case "R":
                    if (c == '0' && n == '0') return null;
                    else if (c == '0' && n == '1') return false;
                    else if (c == '1' && n == '0') return true;
                    else return false;
                case "S'":
                    if (c == '0' && n == '0') return true;
                    else if (c == '0' && n == '1') return false;
                    else if (c == '1' && n == '0') return true;
                    else return null;
                case "R'":
                    if (c == '0' && n == '0') return null;
                    else if (c == '0' && n == '1') return true;
                    else if (c == '1' && n == '0') return false;
                    else return true;
                case "J":
                    if (c == '0' && n == '0') return false;
                    else if (c == '0' && n == '1') return true;
                    else if (c == '1' && n == '0') return null;
                    else return null;
                case "K":
                    if (c == '0' && n == '0') return null;
                    else if (c == '0' && n == '1') return null;
                    else if (c == '1' && n == '0') return true;
                    else return false;
                case "T":
                    if (c == '0' && n == '0') return false;
                    else if (c == '0' && n == '1') return true;
                    else if (c == '1' && n == '0') return true;
                    else return false;
                case "D":
                    if (c == '0' && n == '0') return false;
                    else if (c == '0' && n == '1') return true;
                    else if (c == '1' && n == '0') return false;
                    else return true;
                default:
                    return null;
            }
        }

        private string Design()
        {
            LogicItem[] minterms1, minterms2;
            StringBuilder sb = new StringBuilder();

            //i = betű
            //j = sorszm
            for (int i = 0; i < _variables; i++)
            {
                minterms1 = GenerateMinterms();
                minterms2 = GenerateMinterms();
                for (int j = 0; j < _counter.Count; j++)
                {
                    if (_counter[j].Current == null && _counter[j].Next == null) break;
                    int index = Convert.ToInt32(_counter[j].Current, 2);
                    switch (_flipflop)
                    {
                        case "SR":
                            minterms1[index].Checked = DecodeFlipFlop(_counter[j].Current[i], _counter[j].Next[i], "S");
                            minterms2[index].Checked = DecodeFlipFlop(_counter[j].Current[i], _counter[j].Next[i], "R");
                            break;
                        case "S'R'":
                            minterms1[index].Checked = DecodeFlipFlop(_counter[j].Current[i], _counter[j].Next[i], "S'");
                            minterms2[index].Checked = DecodeFlipFlop(_counter[j].Current[i], _counter[j].Next[i], "R'");
                            break;
                        case "JK":
                            minterms1[index].Checked = DecodeFlipFlop(_counter[j].Current[i], _counter[j].Next[i], "J");
                            minterms2[index].Checked = DecodeFlipFlop(_counter[j].Current[i], _counter[j].Next[i], "K");
                            break;
                        case "T":
                            minterms1[index].Checked = DecodeFlipFlop(_counter[j].Current[i], _counter[j].Next[i], "T");
                            break;
                        case "D":
                            minterms1[index].Checked = DecodeFlipFlop(_counter[j].Current[i], _counter[j].Next[i], "D");
                            break;
                    }
                }
                switch (_flipflop)
                {
                    case "SR":
                        sb.AppendFormat("S{0} = {1}\r\n", (char)(i+65), QuineMcclusky.GetSimplified(minterms1, _variables, true));
                        sb.AppendFormat("R{0} = {1}\r\n", (char)(i + 65), QuineMcclusky.GetSimplified(minterms2, _variables, true));
                        break;
                    case "S'R'":
                        sb.AppendFormat("S'{0} = {1}\r\n", (char)(i + 65), QuineMcclusky.GetSimplified(minterms1, _variables, true));
                        sb.AppendFormat("R'{0} = {1}\r\n", (char)(i + 65), QuineMcclusky.GetSimplified(minterms2, _variables, true));
                        break;
                    case "JK":
                        sb.AppendFormat("J{0} = {1}\r\n", (char)(i + 65), QuineMcclusky.GetSimplified(minterms1, _variables, true));
                        sb.AppendFormat("K{0} = {1}\r\n", (char)(i + 65), QuineMcclusky.GetSimplified(minterms2, _variables, true));
                        break;
                    case "T":
                        sb.AppendFormat("T{0} = {1}\r\n", (char)(i + 65), QuineMcclusky.GetSimplified(minterms1, _variables, true));
                        break;
                    case "D":
                        sb.AppendFormat("D{0} = {1}\r\n", (char)(i + 65), QuineMcclusky.GetSimplified(minterms1, _variables, true));
                        break;

                }
            }
            return sb.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DesignText.Text = Design();
            Tabs.SelectedIndex = 1;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _counter.Add(new Counter());
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CounterDesigner.SelectedCells.Count > 0)
            {
                var data = (CounterDesigner.SelectedCells[0].Item as Counter);
                _counter.Remove(data);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _counter.Clear();
        }
    }
}
