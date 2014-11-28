using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MCalculator.Classes
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    internal partial class ProgressControl : UserControl
    {
        private DispatcherTimer t;
        private DateTime time;

        public event RoutedEventHandler TerminateButtonClicked;

        public ProgressControl()
        {
            InitializeComponent();
            t = new DispatcherTimer();
            t.Interval = new TimeSpan(0, 0, 1);
            time = new DateTime();
            t.Tick += new EventHandler(t_Tick);
        }

        void t_Tick(object sender, EventArgs e)
        {
            time = time.AddSeconds(1);
            string s = time.ToLongTimeString();
            TimeString.Content = s;
        }

        public bool TimerEnabled
        {
            get { return t.IsEnabled; }
            set
            {
                time = new DateTime();
                if (value) t.Start();
                else t.Stop();
            }
        }

        private void BtnTerminate_Click(object sender, RoutedEventArgs e)
        {
            if (TerminateButtonClicked != null) TerminateButtonClicked(sender, e);
        }
    }
}
