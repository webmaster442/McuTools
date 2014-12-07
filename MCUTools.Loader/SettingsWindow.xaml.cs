using MCUTools.Loader.Properties;
using System.Windows;

namespace MCUTools.Loader
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            McuPath.Text = Settings.Default.NetworkPath;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.NetworkPath = McuPath.Text;
            Settings.Default.Save();
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
