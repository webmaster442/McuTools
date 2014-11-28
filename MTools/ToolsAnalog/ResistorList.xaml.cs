using McuTools.Interfaces;
using MTools.classes;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTools
{
    /// <summary>
    /// Interaction logic for ResistorList.xaml
    /// </summary>
    public partial class ResistorList : UserControl
    {
        bool _loaded;

        public ResistorList()
        {
            InitializeComponent();
            _loaded = false;
        }

        private void GenerateResitorSeries(int decade, ResitorListGenerator.Series serie)
        {
            if (!_loaded) return;
            ValuesList.Clear();

            double multiply = 1;
            switch (decade)
            {
                case 1:
                    multiply = 0.01;
                    break;
                case 10:
                    multiply = 0.1;
                    break;
                case 1000:
                    multiply = 10;
                    break;
                case 10000:
                    multiply = 100;
                    break;
                case 100000:
                    multiply = 1000;
                    break;
                case 1000000:
                    multiply = 10000;
                    break;
                case 100:
                default:
                    multiply = 1;
                    break;
            }

            StringBuilder sb = new StringBuilder();
            var list = ResitorListGenerator.GenerateList(serie, multiply);
            foreach (var i in list)
            {
                sb.AppendFormat("{0}\r\n", i);
            }
            ValuesList.Text = sb.ToString();
            sb.Clear();
            sb = null;
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            GenerateResitorSeries(0, ResitorListGenerator.Series.e12);
            _loaded = true;
        }

        private void SeriesSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_loaded) return;
            string val = (Decade.SelectedItem as ComboBoxItem).Content.ToString();
            int decadevalue = Convert.ToInt32(val);
            string series = (SeriesSelector.SelectedItem as TabItem).Header.ToString();
            string ecode = series.Split(' ')[0];
            switch (ecode)
            {
                case "E12":
                    GenerateResitorSeries(decadevalue, ResitorListGenerator.Series.e12);
                    break;
                case "E24":
                    GenerateResitorSeries(decadevalue, ResitorListGenerator.Series.e24);
                    break;
                case "E48":
                    GenerateResitorSeries(decadevalue, ResitorListGenerator.Series.e48);
                    break;
                case "E96":
                    GenerateResitorSeries(decadevalue, ResitorListGenerator.Series.e96);
                    break;
                case "E192":
                    GenerateResitorSeries(decadevalue, ResitorListGenerator.Series.e192);
                    break;
            }
        }
    }
}
