using MApplicationLaunchers.Properties;
using System.Windows;

namespace MApplicationLaunchers
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.EaglePath = Eagle.FilePath;
            Settings.Default.ArduinoPath = Arduino.FilePath;
            Settings.Default.LtSpicePath = LTSpice.FilePath;
            Settings.Default.ProcessingPath = Processing.FilePath;
            Settings.Default.Save();
            MessageBox.Show("Settings saved!");
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Eagle.FilePath = Settings.Default.EaglePath;
            Arduino.FilePath = Settings.Default.ArduinoPath;
            LTSpice.FilePath = Settings.Default.LtSpicePath;
            Processing.FilePath = Settings.Default.ProcessingPath;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
