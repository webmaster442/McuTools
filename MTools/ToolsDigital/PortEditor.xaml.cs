using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MTools.ToolsDigital
{
    /// <summary>
    /// Interaction logic for PortEditor.xaml
    /// </summary>
    public partial class PortEditor : UserControl
    {
        private bool _loaded;

        public PortEditor()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            Port8Value.Text = "0";
            Port16Value.Text = "0";
            _loaded = true;
        }

        private void Bit16Bit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_loaded) return;
            Rectangle segment = (Rectangle)sender;
            SolidColorBrush segmentfill = (SolidColorBrush)segment.Fill;
            if (segmentfill.Color == Colors.Black) segment.Fill = new SolidColorBrush(Colors.Red);
            else segment.Fill = new SolidColorBrush(Colors.Black);
            Calculate(true);
        }

        private void Bit8Bit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_loaded) return;
            Rectangle segment = (Rectangle)sender;
            SolidColorBrush segmentfill = (SolidColorBrush)segment.Fill;
            if (segmentfill.Color == Colors.Black) segment.Fill = new SolidColorBrush(Colors.Red);
            else segment.Fill = new SolidColorBrush(Colors.Black);
            Calculate(false);
        }

        private void Calculate(bool b16bit = false)
        {
            Rectangle item = null;
            int result = 0;
            if (!b16bit)
            {
                for (int i = 0; i < 8; i++)
                {
                    item = McuTools.Interfaces.WPF.WpfHelpers.FindChild<Rectangle>(Port8, "b8b" + i.ToString());
                    if (Functions.isSegmentOn(item)) result += (1 << i);
                }
                Port8Value.Text = Convert.ToString(result, 16);
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    item = McuTools.Interfaces.WPF.WpfHelpers.FindChild<Rectangle>(Port16, "b16b" + i.ToString());
                    if (Functions.isSegmentOn(item)) result += (1 << i);
                }
                Port16Value.Text = Convert.ToString(result, 16);
            }
        }
    }
}
