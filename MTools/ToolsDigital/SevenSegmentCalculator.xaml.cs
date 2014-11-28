using McuTools.Interfaces;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MTools
{
    /// <summary>
    /// Interaction logic for SevenSegmentCalculator.xaml
    /// </summary>
    public partial class SevenSegmentCalculator : UserControl
    {
        private bool _loaded;

        public SevenSegmentCalculator()
        {
            InitializeComponent();
        }

        private void Seg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle segment = (Rectangle)sender;
            SolidColorBrush segmentfill = (SolidColorBrush)segment.Fill;
            if (segmentfill.Color == Colors.Black) segment.Fill = new SolidColorBrush(Colors.Red);
            else segment.Fill = new SolidColorBrush(Colors.Black);
            CalculateSegments();
        }

        private void CalculateSegments()
        {
            if (!_loaded) return;
            int number = 0;
            if (Functions.isSegmentOn(SegA)) number += LSBBitorder.IsChecked == true ? 1 : 128;
            if (Functions.isSegmentOn(SegB)) number += LSBBitorder.IsChecked == true ? 2 : 64;
            if (Functions.isSegmentOn(SegC)) number += LSBBitorder.IsChecked == true ? 4 : 32;
            if (Functions.isSegmentOn(SegD)) number += LSBBitorder.IsChecked == true ? 8 : 16;
            if (Functions.isSegmentOn(SegE)) number += LSBBitorder.IsChecked == true ? 16 : 8;
            if (Functions.isSegmentOn(SegF)) number += LSBBitorder.IsChecked == true ? 32 : 4;
            if (Functions.isSegmentOn(SegG)) number += LSBBitorder.IsChecked == true ? 64 : 2;
            if (Functions.isSegmentOn(SegDP)) number += LSBBitorder.IsChecked == true ? 128 : 1;

            int ca = 255 - number;

            TbComAnode.Text = Convert.ToString(ca, 16);
            TbComCathode.Text = Convert.ToString(number, 16);
        }

        private void LSBBitorder_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            CalculateSegments();
        }

        private void SevenSegmentCalc_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }
    }
}
