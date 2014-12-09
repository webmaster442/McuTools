using MCUTools.Loader.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace MCUTools.Loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private bool _quit;

        public MainWindow()
        {
            InitializeComponent();
            _quit = false;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.IsEnabled = false;
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _timer.IsEnabled = false;
            this.Close();
        }

        private void BtnLaunch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Settings.Default.NetworkPath)) throw new Exception("Network Path is not set. Use settings to specify network path");
                string path = System.IO.Path.Combine(Settings.Default.NetworkPath, "MCUTools.exe");
                bool check = File.Exists(path);
                if (check)
                {
                    var proclist = Process.GetProcesses();
                    var mcu = from i in proclist where i.ProcessName.ToLower().Contains("mcutools") select i;
                    if (mcu.Count() > 0)
                    {
                        _quit = true;
                        throw new Exception("McuTools allready runing. Can't start another instance.\r\nApplication will now Exit");
                    }
                    else
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = path;
                        p.StartInfo.WorkingDirectory = Settings.Default.NetworkPath;
                        p.StartInfo.UseShellExecute = false;
                        p.Start();
                        _timer.IsEnabled = true;
                    }
                }
                else throw new Exception("Invalid Path. MCUTools.exe is not found");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An errorr occured:\r\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                if (_quit) this.Close();
            }
        }

        private void BtnOptions_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.ShowDialog();
        }

        private void BtnInstaller_Click(object sender, RoutedEventArgs e)
        {
            Installer ist = new Installer();
            ist.Show();
            this.Close();
        }
    }
}
