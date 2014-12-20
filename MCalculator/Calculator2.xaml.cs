using MCalculator.Classes;
using MCalculator.Classes.Syntax;
using McuTools.Interfaces.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MCalculator
{
    /// <summary>
    /// Interaction logic for Calculator2.xaml
    /// </summary>
    internal partial class Calculator2 : UserControl, IDisposable
    {
        private bool _canrun;
        private PythonCore _core;
        private string _command;
        private StringBuilder _buffer;
        private Thread _calc;
        private HelpGenerator _help;
        private string[] _defaultvars;
        private Dictionary<string, string> vars;

        class HistoryItem
        {
            public int index { get; set; }
            public string value { get; set; }
        }

        private double col0, col1, col2;
        private ObservableCollection<HistoryItem> _history;

        public Calculator2()
        {
            InitializeComponent();
            col0 = InputGrid.ColumnDefinitions[0].Width.Value;
            col1 = InputGrid.ColumnDefinitions[1].Width.Value;
            col2 = InputGrid.ColumnDefinitions[2].Width.Value;

            _core = new PythonCore(this.Output);
            vars = new Dictionary<string, string>();
            Calc.Core = _core;
            _buffer = new StringBuilder();
            _calc = new Thread(TFunc);
            _help = new HelpGenerator();
            _history = new ObservableCollection<HistoryItem>();
            LbHistory.ItemsSource = _history;
            _canrun = true;
            _core.AttachTypeToRuntime(typeof(Calc));
            _core.AttachTypeToRuntime(typeof(Maths.Fraction));
            _core.AttachTypeToRuntime(typeof(UserInterface.InputForm));

            CreateButtons(typeof(Maths.Functions), Functions);
            CreateButtons(typeof(Maths.Trigonometry), Trig);
            CreateButtons(typeof(Maths.Rnd), Rand);
            CreateButtons(typeof(Maths.Binary), Bin);
            CreateButtons(typeof(Maths.Complex), Cplx);
            CreateButtons(typeof(Maths.Sets), SetStat);
            CreateButtons(typeof(Maths.Stat), SetStat);
            CreateButtons(typeof(Maths.Matrices), Matrix);
            CreateButtons(typeof(Maths.Specials), Specials);
            CreateButtons(typeof(Maths.NumberInfo), Specials);
            CreateButtons(typeof(Maths.Const), ConstantsPanel);
            CreateButtons(typeof(UserInterface.UserInterface), Dialogs);

            _defaultvars = _core.VariablesNames;
            SynBox.SyntaxLexer = _core.SyntaxProvider;

            Output.WriteImage(new Uri("pack://application:,,,/MCalculator.Tool;component/icon/calcintro.png", UriKind.Absolute));
        }

        private void TFunc()
        {
            _core.Run(_command);
            this.Dispatcher.Invoke((Action)delegate
            {
                Output.WriteBuffer();
                ProgrssPanel.Visibility = System.Windows.Visibility.Collapsed;
                Input.Text = "";
                SynBox.Text = "";
                BtnVarRefres_Click(null, null);
            }, null);
        }

        public void RunCommand()
        {
            if (MultiSelector.IsChecked == true) _command = SynBox.Text;
            else _command = Input.Text;
            ProgrssPanel.Visibility = System.Windows.Visibility.Visible;
            ProgrssPanel.TimerEnabled = true;

            _history.Add(new HistoryItem { index = _history.Count == 0 ? 0 : _history.Count, value = _command });

            _calc = new Thread(TFunc);
            _calc.SetApartmentState(ApartmentState.STA);
            Output.WriteLine(_command+"\r\n");
            _calc.Start();
        }

        private void CreateButtons(Type Class, WrapPanel target)
        {
            _core.AttachTypeToRuntime(Class);
            var items = PythonCore.GetClassMembers(Class);
            CompleteButton cb;
            foreach (var item in items)
            {
                cb = new CompleteButton();
                cb.Content = item.Key;
                cb.CompleteText = item.Value;
                cb.Click += InstertButton_Click;
                cb.Margin = new Thickness(2);
                cb.ToolTip += _help.LookupDescription(item.Value);
                cb.Height = 54;
                cb.Width = 100;
                target.Children.Add(cb);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void InstertText(string inserttext)
        {
            TextBox control = Input;

            if (Input.IsEnabled == false) control = SynBox;

            if (string.IsNullOrEmpty(control.Text))
            {
                control.Text = inserttext;
                control.CaretIndex = control.Text.Length;
            }
            else
            {
                int index = control.CaretIndex;
                control.Text = control.Text.Insert(control.CaretIndex, inserttext);
                control.CaretIndex = index + 1;
            }
            control.Focus();
        }

        private void InstertButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_canrun) return;
            string inserttext = null;
            if (sender is CompleteButton) inserttext = (sender as CompleteButton).CompleteText;
            else if (sender is Button) inserttext = (sender as Button).Content.ToString();
            InstertText(inserttext);
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            if (!_canrun) return;
            RunCommand();
        }

        private void TerminateClick(object sender, RoutedEventArgs e)
        {
            if (_calc != null)
            {
                ProgrssPanel.Visibility = System.Windows.Visibility.Collapsed;
                ProgrssPanel.TimerEnabled = false;
                _calc.Abort();
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Input.Clear();
        }

        private void ModeChange(object sender, RoutedEventArgs e)
        {
            Input.Text = "Calc.SetMode(\"" + (sender as RadioButton).Content.ToString() + "\")";
            Execute_Click(sender, null);
        }

        private void BtnClearLog_Click(object sender, RoutedEventArgs e)
        {
            Output.Clear();
        }

        private void Input_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (sender is SyntaxHighlightTextBox)
            {
                if (e.Key == System.Windows.Input.Key.Enter && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Control) Execute_Click(sender, null);
            }
            else
            {
                if (e.Key == System.Windows.Input.Key.Enter) Execute_Click(sender, null);
            }
        }

        private void BtnExpand_Click(object sender, RoutedEventArgs e)
        {
            if (BtnExpand.IsChecked == true) BtnExpand.Content = "Colapse All";
            else BtnExpand.Content = "Expand All";

            foreach (var expander in WpfHelpers.FindChildren<Expander>(FunctionL))
            {
                expander.IsExpanded = (bool)BtnExpand.IsChecked;
            }
        }

        private void BtnVarRefres_Click(object sender, RoutedEventArgs e)
        {
            VarList.ItemsSource = null;
            vars.Clear();
            foreach (var i in _core.VariablesNames)
            {
                if (_defaultvars.Contains(i)) continue;
                if (i.StartsWith("_")) continue;
                try
                {
                    vars.Add(i, _core.GetVariable(i).GetType().ToString());
                }
                catch (Exception) { }
            }
            VarList.ItemsSource = vars;
        }

        private void BtnVarDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in vars.Keys)
            {
                _core.DeleteVariable(item);
            }
            BtnVarRefres_Click(sender, e);
        }

        private void BtnVarDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (VarList.SelectedIndex < 0) return;
            _core.DeleteVariable(vars.Keys.ToArray()[VarList.SelectedIndex]);
            BtnVarRefres_Click(sender, e);
        }

        private void BtnVarInstertSelected_Click(object sender, RoutedEventArgs e)
        {
            if (VarList.SelectedIndex < 0) return;
            string text = vars.Keys.ToArray()[VarList.SelectedIndex];
            InstertText(text);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight > 450)
            {
                double displaysize = this.ActualHeight - 320;
                MainGrid.RowDefinitions[0].Height = new GridLength(displaysize);

                if ((bool)MultiSelector.IsChecked)
                {
                    InputGrid.ColumnDefinitions[0].Width = new GridLength(0.3 * InputGrid.ActualWidth);
                    InputGrid.ColumnDefinitions[1].Width = new GridLength(0.4 * InputGrid.ActualWidth);
                    InputGrid.ColumnDefinitions[2].Width = new GridLength(0.3 * InputGrid.ActualWidth); 
                }
                else
                {
                    InputGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    InputGrid.ColumnDefinitions[1].Width = new GridLength(col1);
                    InputGrid.ColumnDefinitions[2].Width = new GridLength(col2);
                }
            }
            else MainGrid.RowDefinitions[0].Height = new GridLength(125);
        }

        private void BtnSaveLog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Text File (*.txt)|*.txt|Rich text (*.rtf)|*.rtf";
            sfd.FilterIndex = 1;
            sfd.AddExtension = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextRange t = new TextRange(Output.Document.ContentStart, Output.Document.ContentEnd);
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    if (sfd.FileName.ToLower().EndsWith(".txt")) t.Save(fs, System.Windows.DataFormats.Text);
                    else if (sfd.FileName.ToLower().EndsWith(".rtf")) t.Save(fs, System.Windows.DataFormats.Rtf);
                }
            }
        }

        private void MultiSelector_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)MultiSelector.IsChecked) MultiSelector.Content = "Singleline";
            else MultiSelector.Content = "Multiline";
            UserControl_SizeChanged(this, null);
        }

        private void LbHistory_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LbHistory.SelectedIndex < 0) return;
            string data = _history[LbHistory.SelectedIndex].value;
            InstertText(data);
        }

        private void BtnClearHistory_Click(object sender, RoutedEventArgs e)
        {
            _history.Clear();
        }

        protected virtual void Dispose(bool native)
        {
            if (_core != null)
            {
                _core.Dispose();
                _core = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
