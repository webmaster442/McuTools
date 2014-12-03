using System;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace McuTools.Controls
{
    /// <summary>
    /// Interaction logic for BatteryInfo.xaml
    /// </summary>
    public partial class BatteryInfo : UserControl
    {
        private DispatcherTimer _t;

        public static DependencyProperty PercentRemainProperty = DependencyProperty.Register("PercentRemain", typeof(double), typeof(BatteryInfo), new PropertyMetadata(0.0d));
        public static DependencyProperty TimeRemainProperty = DependencyProperty.Register("TimeRemain", typeof(string), typeof(BatteryInfo), new PropertyMetadata(""));
        public static DependencyProperty InfoVisibleProperty = DependencyProperty.Register("InfoVisible", typeof(Visibility), typeof(BatteryInfo), new PropertyMetadata(Visibility.Visible));

        public BatteryInfo()
        {
            InitializeComponent();
            DoQuery();
            _t = new DispatcherTimer();
            _t.Interval = TimeSpan.FromSeconds(1);
            _t.Tick += _t_Tick;
            _t.IsEnabled = false;
        }

        private void DoQuery()
        {
            if (InfoVisible == System.Windows.Visibility.Collapsed || InfoVisible == System.Windows.Visibility.Hidden)
            {
                _t.IsEnabled = false;
                return;
            }
            int cnt = 0;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT EstimatedChargeRemaining, EstimatedRunTime, BatteryStatus FROM Win32_Battery");
                string statustext = "";

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    int status = Convert.ToInt32(queryObj["BatteryStatus"]);

                    switch (status)
                    {
                        case 1:
                            statustext = "The battery is discharging.";
                            break;
                        case 2:
                            statustext = "The system has access to AC so no battery is being discharged. However, the battery is not necessarily charging.";
                            break;
                        case 3:
                            statustext = "Fully Charged";
                            break;
                        case 4:
                            statustext = "Low";
                            break;
                        case 5:
                            statustext = "Critical";
                            break;
                        case 6:
                            statustext = "Charging";
                            break;
                        case 7:
                            statustext = "Charging and High";
                            break;
                        case 8:
                            statustext = "Charging and Low";
                            break;
                        case 9:
                            statustext = "Charging and Critical";
                            break;
                        case 10:
                            statustext = "Undefined";
                            break;
                        case 11:
                            statustext = "Partially Charged";
                            break;
                    }
                    PercentRemain = Convert.ToDouble(queryObj["EstimatedChargeRemaining"]);
                    TimeRemain = String.Format("Status: {0}\r\nRemaining Time: {1} minutes",  statustext, queryObj["EstimatedRunTime"]);
                    ++cnt;
                }
                if (cnt == 0) InfoVisible = Visibility.Collapsed;
                searcher.Dispose();
            }
            catch (ManagementException) { InfoVisible = Visibility.Collapsed; }
        }

        private void _t_Tick(object sender, EventArgs e)
        {
            DoQuery();
        }

        public double PercentRemain
        {
            get { return (int)GetValue(PercentRemainProperty); }
            set { SetValue(PercentRemainProperty, value); }
        }

        public string TimeRemain
        {
            get { return (string)GetValue(TimeRemainProperty); }
            set { SetValue(TimeRemainProperty, value); }
        }

        public Visibility InfoVisible
        {
            get { return (Visibility)GetValue(InfoVisibleProperty); }
            set { SetValue(InfoVisibleProperty, value); }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _t.IsEnabled = true;
        }
    }

    [ValueConversion(typeof(double), typeof(SolidColorBrush))]
    internal class ValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double v = (double)value;
            if (v > 50) return new SolidColorBrush(Colors.Green);
            else if (v > 29 && v < 50) return new SolidColorBrush(Colors.Yellow);
            else return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
