using McuTools.Interfaces.WPF;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace MTools.ToolOther
{
    /// <summary>
    /// Interaction logic for ArduinoInstaller.xaml
    /// </summary>
    public partial class ArduinoInstaller : UserControl, IDisposable
    {
        private SyndicationFeed _feed;
        private string _tempfile;
        private WebClient _webClient;
        private bool _loaded;

        public ArduinoInstaller()
        {
            InitializeComponent();
            _webClient = new WebClient();
            _webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
            _webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
        }

        private void ListDownloads()
        {
            try
            {
                var reader = XmlReader.Create("http://code.google.com/feeds/p/arduino/downloads/basic");
                _feed = SyndicationFeed.Load(reader);
            }
            catch (Exception)
            {
                WpfHelpers.ExceptionDialog("Internet connection problem. Can't list downloads");
            }
        }

        private Task ListDownloadsTask()
        {
            return Task.Run(() => ListDownloads());
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            ArduinoVersion.ItemsSource = null;
            await ListDownloadsTask();
            if (_feed != null)
            {
                var items = (from i in _feed.Items where i.Title.Text.Contains("-windows.zip") select i.Title.Text).ToList();
                ArduinoVersion.ItemsSource = items;
            }
            _loaded = true;
        }
        
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _webClient.CancelAsync();
        }

        private void BtnInstall_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(IPath.SelectedPath))
            {
                WpfHelpers.ExceptionDialog("Selected installation path doesn't exist. Please choose another");
                return;
            }
            if (ArduinoVersion.SelectedIndex < 0)
            {
                WpfHelpers.ExceptionDialog("Arduino version not selected. Please select one from the list");
                return;
            }
            Stage1.Visibility = System.Windows.Visibility.Collapsed;
            Stage2.Visibility = System.Windows.Visibility.Visible;

            string selected = (string)ArduinoVersion.SelectedItem;

            var links = (from i in _feed.Items where i.Title.Text == selected select i.Links).First();
            var link = (from i in links where i.GetAbsoluteUri().AbsolutePath.Contains("/files/") select i).First().GetAbsoluteUri();
            _tempfile = Path.GetTempFileName();
            PbProgress.IsIndeterminate = false;
            _webClient.DownloadFileAsync(link, _tempfile);
            TbStatus.Text = "Downloading installer file ...";
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            PbProgress.Value = e.ProgressPercentage;
        }

        private async void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            PbProgress.IsIndeterminate = true;
            TbStatus.Text = "Installing Arduino ...";
            BtnCancel.IsEnabled = false;
            await InstallTask((bool)CbRemovePrevious.IsChecked, IPath.SelectedPath);
            MessageBox.Show("Installation complete");
            Stage2.Visibility = System.Windows.Visibility.Collapsed;
            Stage1.Visibility = System.Windows.Visibility.Visible;
        }

        private Task InstallTask(bool remove, string targetdir)
        {
            return Task.Run(() => Install(remove, targetdir));
        }

        private void Install(bool remove, string targetdir)
        {
            try
            {
                if (remove)
                {
                    //Arduino extensions uninstall
                    if (File.Exists(IPath.SelectedPath + "\\unins000.exe"))
                    {
                        Process P = Process.Start(IPath.SelectedPath + "\\unins000.exe", "/SILENT /NORESTART /SUPPRESSMSGBOXES");
                        P.WaitForExit();
                    }
                    string[] del = Directory.GetFiles(IPath.SelectedPath, "*.*", SearchOption.AllDirectories);
                    foreach (var f in del) File.Delete(f);
                    string[] dir = Directory.GetDirectories(IPath.SelectedPath, "*.*", SearchOption.AllDirectories);
                    foreach (var d in dir) Directory.Delete(d);
                }
                ZipFile.ExtractToDirectory(_tempfile, targetdir);
                File.Delete(_tempfile);
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }

        protected virtual void Dispose(bool native)
        {
            if (_webClient != null)
            {
                _webClient.Dispose();
                _webClient = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
