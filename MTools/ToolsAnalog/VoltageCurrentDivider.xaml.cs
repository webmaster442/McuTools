using MTools.Controls;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MTools.ToolsAnalog
{
    /// <summary>
    /// Interaction logic for VoltageCurrentDivider.xaml
    /// </summary>
    public partial class VoltageCurrentDivider : UserControl
    {
        private bool _loaded;
        private int _counter;
        private int _ccounter;

        public VoltageCurrentDivider()
        {
            InitializeComponent();
            _loaded = false;
            _counter = 1;
            _ccounter = 1;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;

            PrefixInput pi = new PrefixInput();
            pi.UnitText = "Ω";
            pi.LabelText = "R" + _counter.ToString();
            pi.ValueChanged += PrefixInput_ValueChanged;
            pi.Value = 1;
            SpItems.Children.Add(pi);
            _counter++;
            PrefixInput_ValueChanged(null, null);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            SpItems.Children.Clear();
            _counter = 1;
            TbResults.Clear();
        }

        private void PrefixInput_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double re = 0;
            foreach (PrefixInput r in SpItems.Children) re += r.Value;
            double I = Uin.Value / re;
            TbResults.Clear();
            StringBuilder sb = new StringBuilder();
            foreach (PrefixInput r in SpItems.Children) sb.AppendFormat("{0} voltage: {1} V\r\n", r.LabelText, Math.Round(r.Value * I, 4));
            TbResults.Text = sb.ToString();
        }

        private void BtnCurrentAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;

            PrefixInput pi = new PrefixInput();
            pi.UnitText = "Ω";
            pi.LabelText = "R" + _ccounter.ToString();
            pi.ValueChanged += PrefixInput_ValueChanged2;
            pi.Value = 1;
            SpCurrentItems.Children.Add(pi);
            _ccounter++;
            PrefixInput_ValueChanged2(null, null);
        }

        private void BtnCurrentClear_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            SpCurrentItems.Children.Clear();
            _ccounter = 1;
            TbCurrentResults.Clear();
        }

        private void PrefixInput_ValueChanged2(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            double re = 0;
            foreach (PrefixInput r in SpCurrentItems.Children) re += 1 / r.Value;
            re = 1 / re;

            double U = re * Iin.Value;
            TbCurrentResults.Clear();
            StringBuilder sb = new StringBuilder();
            foreach (PrefixInput r in SpCurrentItems.Children) sb.AppendFormat("{0} current: {1} A\r\n", r.LabelText, Math.Round(U / r.Value, 4));
            TbCurrentResults.Text = sb.ToString();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }
    }
}
