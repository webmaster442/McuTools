using IronPython;
using IronPython.Hosting;
using McuTools.Interfaces.WPF;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MCalculator
{
    /// <summary>
    /// Interaction logic for FunctionTester.xaml
    /// </summary>
    internal partial class FunctionTester : UserControl
    {
        private char[] letters = "ABCDEFGH".ToArray();
        private bool _loaded;
        private ObservableCollection<Formul> _collection;
        private ScriptEngine _engine;
        private ScriptScope _scope;

        public FunctionTester()
        {
            InitializeComponent();
            _loaded = false;
            _collection = new ObservableCollection<Formul>();
            Dictionary<String, Object> options = new Dictionary<string, object>();
            options["DivisionOptions"] = PythonDivisionOptions.New;
            _engine = Python.CreateEngine(options);
            _scope = _engine.CreateScope();
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            Formulas.ItemsSource = _collection;
            GenerateToggles();
            _loaded = true;
        }

        private void GenerateToggles()
        {
            if (!_loaded) return;
            TogleButtons.Children.Clear();
            int count = Convert.ToInt32((ToggleNumber.SelectedItem as ComboBoxItem).Content);
            for (int i = 0; i < count; i++)
            {
                ToggleButton togle = new ToggleButton();
                togle.Content = letters[i];
                togle.IsChecked = false;
                togle.Click += togle_Click;
                togle.Margin = new Thickness(5);
                togle.Width = 60;
                TogleButtons.Children.Add(togle);
            }
        }

        private void UpdateVariableScope()
        {
            try
            {
                if (!(bool)BtnEnable.IsChecked) return;

                StringBuilder sb = new StringBuilder();
                int count = Convert.ToInt32((ToggleNumber.SelectedItem as ComboBoxItem).Content);
                ToggleButton btn;
                for (int i = 0; i < count; i++)
                {
                    btn = TogleButtons.Children[i] as ToggleButton;
                    sb.Append(letters[i]);
                    sb.Append(" = ");
                    if (btn.IsChecked == true) sb.Append("1");
                    else sb.Append("0");
                    sb.Append("\r\n");
                }
                ScriptSource source = _engine.CreateScriptSourceFromString(sb.ToString(), SourceCodeKind.AutoDetect);
                source.Execute(_scope);

                foreach (var f in _collection)
                {
                    source = _engine.CreateScriptSourceFromString(f.Formula, SourceCodeKind.AutoDetect);
                    object result = source.Execute(_scope);
                    f.Res = result.ToString();
                }
                Formulas.ItemsSource = null;
                Formulas.ItemsSource = _collection;
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }

        void togle_Click(object sender, RoutedEventArgs e)
        {
            UpdateVariableScope();
        }

        private void ToggleNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GenerateToggles();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _collection.Add(new Formul());
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (Formulas.SelectedIndex < 0) return;
            _collection.RemoveAt(Formulas.SelectedIndex);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _collection.Clear();
        }

        private void BtnEnable_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            if (!(bool)BtnEnable.IsChecked) return;
            foreach (var form in _collection)
            {
                form.Formula = form.Formula.ToUpper();
            }
            UpdateVariableScope();
        }
    }

    internal class Formul
    {
        public string Formula { get; set; }
        public string Res { get; set; }
    }
}
