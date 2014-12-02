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
            LauncherConfig.Save(ConfigReader.Configuration, ConfigReader.AppDir + "\\launchers.xml");
        }
    }
}
