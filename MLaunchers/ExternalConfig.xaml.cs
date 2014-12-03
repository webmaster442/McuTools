using McuTools.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace MLaunchers
{
    /// <summary>
    /// Interaction logic for ExternalConfig.xaml
    /// </summary>
    public partial class ExternalConfig : UserControl
    {
        public ExternalConfig()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FfsArduino.SelectedPath = ConfigReader.Configuration.ArduinoPath;
            FfsEagle.SelectedPath = ConfigReader.Configuration.EaglePath;
            FfsLibreOffice.SelectedPath = ConfigReader.Configuration.LibreOfficePath;
            FfsLtSpice.SelectedPath = ConfigReader.Configuration.LtSpicePath;
            FfsProcessing.SelectedPath = ConfigReader.Configuration.ProcessingPath;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

            ConfigReader.Configuration.ArduinoPath = FfsArduino.SelectedPath;
            ConfigReader.Configuration.EaglePath = FfsEagle.SelectedPath;
            ConfigReader.Configuration.LibreOfficePath = FfsLibreOffice.SelectedPath;
            ConfigReader.Configuration.LtSpicePath = FfsLtSpice.SelectedPath;
            ConfigReader.Configuration.ProcessingPath = FfsProcessing.SelectedPath;

            if (!Folders.IsDirectoryWritable(Folders.Application))
            {
                var response = MessageBox.Show("Application folder is not writeable.\r\nWould you like to save the config to another location?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (response == MessageBoxResult.Yes)
                {
                    System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                    sfd.Filter = "Launcher config|launcher.xml";
                    sfd.FileName = "launcher.xml";

                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        LauncherConfig.Save(ConfigReader.Configuration, sfd.FileName);
                        MessageBox.Show("Configuration saved. Copy it to the application folder to aply changes.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                LauncherConfig.Save(ConfigReader.Configuration, Folders.Application + "\\launchers.xml");
                MessageBox.Show("Configuration saved.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            
        }
    }
}
