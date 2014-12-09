using MCUTools.Loader.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace MCUTools.Loader
{
    /// <summary>
    /// Interaction logic for Installer.xaml
    /// </summary>
    public partial class Installer : Window
    {
        private ObservableCollection<RepositoryItem> _items;

        public Installer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _items = new ObservableCollection<RepositoryItem>();
            PackageList.ItemsSource = _items;
            TbProxyUser.Text = Settings.Default.ProxyUser;
            TbProxyPasswd.Password = Settings.Default.ProxyPass;
            TbRepoUrl.Text = Settings.Default.RepositoryUrl;
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Worker.Visibility = System.Windows.Visibility.Visible;
                var result = await Worker.TaskDownloadRepoFile();
                foreach (var r in result) _items.Add(r);
                Worker.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.RepositoryUrl = TbRepoUrl.Text;
            Settings.Default.ProxyUser = TbProxyUser.Text;
            Settings.Default.ProxyPass = TbProxyPasswd.Password;
            Settings.Default.Save();
        }
    }
}
