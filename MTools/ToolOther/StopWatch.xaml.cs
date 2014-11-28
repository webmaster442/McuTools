using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MTools.ToolOther
{
    /// <summary>
    /// Interaction logic for StopWatch.xaml
    /// </summary>
    public partial class StopWatch : UserControl
    {
        private DispatcherTimer dt = new DispatcherTimer();
        private Stopwatch stopWatch = new Stopwatch();
        private string currentTime = string.Empty;
        private bool _loaded;

        public StopWatch()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _loaded = true;
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                ClockTextBlock.Text = currentTime;
            }
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            stopWatch.Start();
            dt.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (stopWatch.IsRunning)
                stopWatch.Stop();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TimeElapsedItems.Items.Add(currentTime);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            stopWatch.Reset();
            currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", 0, 0, 0, 0);
            ClockTextBlock.Text = currentTime;
            //stopWatch.Start();
        }
    }
}
