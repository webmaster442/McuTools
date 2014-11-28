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
    /// Interaction logic for FourteenSegmentCalculator.xaml
    /// </summary>
    public partial class FourteenSegmentCalculator : UserControl
    {
        private bool _loaded;

        public FourteenSegmentCalculator()
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
            if (Functions.isSegmentOn(SegmentA)) number += LSBBitorder.IsChecked == true ? 1 : 8192;
            if (Functions.isSegmentOn(SegmentB)) number += LSBBitorder.IsChecked == true ? 2 : 4096;
            if (Functions.isSegmentOn(SegmentC)) number += LSBBitorder.IsChecked == true ? 4 : 2048;
            if (Functions.isSegmentOn(SegmentD)) number += LSBBitorder.IsChecked == true ? 8 : 1024;
            if (Functions.isSegmentOn(SegmentE)) number += LSBBitorder.IsChecked == true ? 16 : 512;
            if (Functions.isSegmentOn(SegmentF)) number += LSBBitorder.IsChecked == true ? 32 : 256;
            if (Functions.isSegmentOn(SegmentG1)) number += LSBBitorder.IsChecked == true ? 64 : 128;
            if (Functions.isSegmentOn(SegmentG2)) number += LSBBitorder.IsChecked == true ? 128 : 64;
            if (Functions.isSegmentOn(SegmentH)) number += LSBBitorder.IsChecked == true ? 256 : 32;
            if (Functions.isSegmentOn(SegmentJ)) number += LSBBitorder.IsChecked == true ? 512 : 16;
            if (Functions.isSegmentOn(SegmentK)) number += LSBBitorder.IsChecked == true ? 1024 : 8;
            if (Functions.isSegmentOn(SegmentL)) number += LSBBitorder.IsChecked == true ? 2048 : 4;
            if (Functions.isSegmentOn(SegmentM)) number += LSBBitorder.IsChecked == true ? 4096 : 2;
            if (Functions.isSegmentOn(SegmentDP)) number += LSBBitorder.IsChecked == true ? 8192 : 1;

            TbComCathode.Text = Convert.ToString(number, 16);
            TbComAnode.Text = Convert.ToString(0x3fff - number, 16);
        }

        private void LSBBitorder_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            CalculateSegments();
        }

        private void FourteenSegmentCalc_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }
    }
}
