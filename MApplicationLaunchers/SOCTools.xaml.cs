using McuTools.Interfaces.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MApplicationLaunchers
{
    /// <summary>
    /// Interaction logic for SOCTools.xaml
    /// </summary>
    public partial class SOCTools : UserControl
    {
        private string _toolsdir;
        private string _windir;
        private bool _loaded;

        public SOCTools()
        {
            InitializeComponent();
            _loaded = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _toolsdir = AppDomain.CurrentDomain.BaseDirectory + "SOC\\";
            _windir = Environment.ExpandEnvironmentVariables("%windir%");
            _loaded = true;
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            ImageButton s = (ImageButton)sender;
            switch (s.ImageText)
            {
                case "Ext2Explore":
                    Helpers.TryRunTool(_toolsdir + "ext2explore.exe", true);
                    break;
                case "KiTTY":
                    Helpers.TryRunTool(_toolsdir + "kitty_portable.exe");
                    break;
                case "TFTPD32":
                    if (Environment.Is64BitOperatingSystem) Helpers.TryRunTool(_toolsdir + "tftpd64.exe");
                    else Helpers.TryRunTool(_toolsdir + "tftpd32.exe");
                    break;
                case "Universal USB Installer":
                    Helpers.TryRunTool(_toolsdir + "Universal-USB-Installer.exe", true);
                    break;
                case "Win32DiskImager":
                    Helpers.TryRunTool(_toolsdir + "Win32DiskImager.exe", true);
                    break;
                case "WinSCP":
                    Helpers.TryRunTool(_toolsdir + "WinSCP.exe");
                    break;
                case "Remote Desktop Connection":
                    Helpers.TryRunTool(_windir + @"\system32\mstsc.exe");
                    break;
                case "Command Line":
                    Helpers.TryRunTool(_windir + @"\system32\cmd.exe");
                    break;
                case "PowerShell":
                    Helpers.TryRunTool(_windir + @"\system32\WindowsPowerShell\v1.0\powershell.exe");
                    break;
                case "7Zip":
                    Helpers.TryRunTool(_toolsdir + @"\7z\7zFM.exe");
                    break;
                case "Notepad++":
                    Helpers.TryRunTool(_toolsdir + @"\npp\notepad++.exe");
                    break;
            }
        }
    }
}
