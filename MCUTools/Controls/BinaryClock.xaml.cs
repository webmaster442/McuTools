using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace McuTools.Controls
{
    /// <summary>
    /// Interaction logic for BinaryClock.xaml
    /// </summary>
    public partial class BinaryClock : UserControl
    {
        private DispatcherTimer t;

        public static DependencyProperty HumanTimeProperty = DependencyProperty.Register("HumanTime", typeof(string), typeof(BinaryClock));

        public string HumanTime
        {
            get { return (string)GetValue(HumanTimeProperty); }
            set { SetValue(HumanTimeProperty, value); }
        }

        public BinaryClock()
        {
            InitializeComponent();
        }

        private static void SetRectangle(int column, Grid grid, Color c)
        {
            var q = from Rectangle i in grid.Children where Grid.GetColumn(i) == column select i;
            Rectangle r = q.FirstOrDefault();
            if (r == null) return;
            r.Fill = new SolidColorBrush(c);
        }

        private static void SetDigitValue(Grid Display, int value)
        {
            switch (value)
            {
                case 0:
                    SetRectangle(0, Display, Colors.Transparent);
                    SetRectangle(1, Display, Colors.Transparent);
                    SetRectangle(2, Display, Colors.Transparent);
                    SetRectangle(3, Display, Colors.Transparent);
                    break;
                case 1:
                    SetRectangle(0, Display, Colors.Transparent);
                    SetRectangle(1, Display, Colors.Transparent);
                    SetRectangle(2, Display, Colors.Transparent);
                    SetRectangle(3, Display, Colors.Black);
                    break;
                case 2:
                    SetRectangle(0, Display, Colors.Transparent);
                    SetRectangle(1, Display, Colors.Transparent);
                    SetRectangle(2, Display, Colors.Black);
                    SetRectangle(3, Display, Colors.Transparent);
                    break;
                case 3:
                    SetRectangle(0, Display, Colors.Transparent);
                    SetRectangle(1, Display, Colors.Transparent);
                    SetRectangle(2, Display, Colors.Black);
                    SetRectangle(3, Display, Colors.Black);
                    break;
                case 4:
                    SetRectangle(0, Display, Colors.Transparent);
                    SetRectangle(1, Display, Colors.Black);
                    SetRectangle(2, Display, Colors.Transparent);
                    SetRectangle(3, Display, Colors.Transparent);
                    break;
                case 5:
                    SetRectangle(0, Display, Colors.Transparent);
                    SetRectangle(1, Display, Colors.Black);
                    SetRectangle(2, Display, Colors.Transparent);
                    SetRectangle(3, Display, Colors.Black);
                    break;
                case 6:
                    SetRectangle(0, Display, Colors.Transparent);
                    SetRectangle(1, Display, Colors.Black);
                    SetRectangle(2, Display, Colors.Black);
                    SetRectangle(3, Display, Colors.Transparent);
                    break;
                case 7:
                    SetRectangle(0, Display, Colors.Transparent);
                    SetRectangle(1, Display, Colors.Black);
                    SetRectangle(2, Display, Colors.Black);
                    SetRectangle(3, Display, Colors.Black);
                    break;
                case 8:
                    SetRectangle(0, Display, Colors.Black);
                    SetRectangle(1, Display, Colors.Transparent);
                    SetRectangle(2, Display, Colors.Transparent);
                    SetRectangle(3, Display, Colors.Transparent);
                    break;
                case 9:
                    SetRectangle(0, Display, Colors.Black);
                    SetRectangle(1, Display, Colors.Transparent);
                    SetRectangle(2, Display, Colors.Transparent);
                    SetRectangle(3, Display, Colors.Black);
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(1);
            t.Tick += t_Tick;
            t.Start();
        }

        private void t_Tick(object sender, EventArgs e)
        {
            HumanTime = DateTime.Now.ToString();
            SetDigitValue(H1, DateTime.Now.Hour / 10);
            SetDigitValue(H2, DateTime.Now.Hour % 10);
            SetDigitValue(M1, DateTime.Now.Minute / 10);
            SetDigitValue(M2, DateTime.Now.Minute % 10);
            SetDigitValue(S1, DateTime.Now.Second / 10);
            SetDigitValue(S2, DateTime.Now.Second % 10);
        }
    }
}
