using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using MCUTools.Loader.Properties;
using System.IO;

namespace MCUTools.Loader
{
    /// <summary>
    /// Interaction logic for DoWork.xaml
    /// </summary>
    public partial class DoWork : UserControl
    {
        private WebClient _wc;

        public DoWork()
        {
            InitializeComponent();
            _wc = new WebClient();
            _wc.DownloadProgressChanged += _wc_DownloadProgressChanged;
        }

        private bool ConfigureWebClient()
        {
            bool test = true;
            string error = null;
            int counter = 0;
            while (test)
            {
                try
                {
                    _wc.OpenRead("http://www.example.com/");
                    if (!string.IsNullOrEmpty(error)) MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    test = false;
                    break;
                }
                catch (WebException wx)
                {
                    if (wx is InvalidOperationException && wx.Response == null)
                    {
                        MessageBox.Show("Internet acces error:\r\n" + wx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    switch (((HttpWebResponse)wx.Response).StatusCode)
                    {
                        case HttpStatusCode.ProxyAuthenticationRequired:
                            NetworkCredential nc = new NetworkCredential(Settings.Default.ProxyUser, Settings.Default.ProxyPass);
                            _wc.Proxy.Credentials = nc;
                            break;
                    }
                }
                ++counter;
                if (counter > 10) return false;
            }
            return !test;
        }

        private void _wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            PbCurrent.Value = e.ProgressPercentage;
        }

        public async Task<RepositoryItem[]> TaskDownloadRepoFile()
        {
            ConfigureWebClient();
            PbCurrent.IsIndeterminate = false;
            await _wc.DownloadFileTaskAsync(Settings.Default.RepositoryUrl, "repository.csv");
            PbCurrent.IsIndeterminate = true;
            return InstallFunctions.ParseRepoFile("repository.csv");
        }
    }
}
