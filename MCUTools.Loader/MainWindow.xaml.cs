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
        private string _appdir;

        public MainWindow()
        {
            InitializeComponent();
            _appdir = System.AppDomain.CurrentDomain.BaseDirectory;
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

        private static bool ISMcuRunning()
        {
            var proclist = Process.GetProcesses();
            var mcu = from i in proclist where i.ProcessName.ToLower() == "mcutools" select i;
            return mcu.Count() > 0;
        }

        private static void RunProcess(string path, string dir = null)
        {
            Process p = new Process();
            p.StartInfo.FileName = path;
            if (!string.IsNullOrEmpty(dir)) p.StartInfo.WorkingDirectory = dir;
            p.StartInfo.UseShellExecute = false;
            p.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ISMcuRunning())
            {
                MessageBox.Show("McuTools allready running. Launcher exits.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                if (File.Exists(_appdir + "\\mcutools.exe"))
                {
                    RunProcess(_appdir + "\\mcutools.exe", _appdir);
                    this.Close();
                }
                else
                {
                    if (string.IsNullOrEmpty(Settings.Default.NetworkPath))
                    {
                        MessageBox.Show("MCU Tools is installed in network mode. Please configure Network path", "Network setup", MessageBoxButton.OK, MessageBoxImage.Warning);
                        BtnOptions_Click(sender, e);
                    }
                }
            }
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
                    if (ISMcuRunning())
                    {
                        _quit = true;
                        throw new Exception("McuTools allready runing. Can't start another instance.\r\nApplication will now Exit");
                    }
                    else
                    {
                        RunProcess(path, Settings.Default.NetworkPath);
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
            this.Visibility = System.Windows.Visibility.Collapsed;
            SettingsWindow sw = new SettingsWindow();
            sw.Left = this.Left;
            sw.Top = this.Top;
            sw.ShowDialog();
            this.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
