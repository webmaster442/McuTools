using MApplicationLaunchers.Properties;
using McuTools.Interfaces;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MApplicationLaunchers
{
    public class SettingsDialog: ExternalTool
    {

        private SettingsWindow _sw;

        public override string Description
        {
            get { return "Launcher Settings"; }
        }

        public override ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MApplicationLaunchers.Tool;component/images/settings2-128.png", UriKind.Relative)); }
        }

        public override void RunTool()
        {
            _sw = new SettingsWindow();
            _sw.ShowDialog();
        }

        public override bool IsVisible
        {
            get { return true; }
        }
    }

    public class ApplicationEagle: ExternalTool
    {
        public override string Description
        {
            get { return "Launch Cadsoft Eagle"; }
        }

        public override ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MApplicationLaunchers.Tool;component/images/eagle.png", UriKind.Relative)); }
        }

        public override void RunTool()
        {
            Helpers.TryRunTool(Settings.Default.EaglePath);
        }

        public override bool IsVisible
        {
            get { return Helpers.VisibilityCheck("EaglePath", Helpers.ReferencePaths.Eagle); }
        }
    }

    public class ApplicationArduino : ExternalTool
    {
        public override string Description
        {
            get { return "Launch Arduino"; }
        }

        public override ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MApplicationLaunchers.Tool;component/images/arduino.png", UriKind.Relative)); }
        }

        public override void RunTool()
        {
            ContextMenu cm = new ContextMenu();
            cm.PlacementTarget = (UIElement)Mouse.DirectlyOver;

            string[] arduinos = Helpers.ReferencePaths.Arduino;
            if (arduinos != null)
            {
                foreach (var a in arduinos)
                {
                    MenuItem m = new MenuItem();
                    m.Header = Path.GetFileName(Path.GetDirectoryName(a));
                    m.FontSize = 14;
                    m.Click += arduino_Click;
                    m.ToolTip = a;
                    cm.Items.Add(m);
                }
            }
            if (!string.IsNullOrEmpty(Settings.Default.ArduinoPath))
            {
                MenuItem a = new MenuItem();
                a.ToolTip = Settings.Default.ArduinoPath;
                a.FontSize = 14;
                a.Header = "External Arduino installation";
                a.Click += arduino_Click;
                cm.Items.Add(a);
            }
            cm.IsOpen = true;
        }

        private void arduino_Click(object sender, RoutedEventArgs e)
        {
            MenuItem s = (MenuItem)sender;
            Helpers.TryRunTool(s.ToolTip.ToString());
        }

        public override bool IsVisible
        {
            get 
            {
                return !string.IsNullOrEmpty(Settings.Default.ArduinoPath) || Helpers.ReferencePaths.Arduino != null;
            }
        }
    }


    public class ApplicationArduinoExt : ExternalTool
    {
        public override string Description
        {
            get { return "Install Arduino Extensions"; }
        }

        public override ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MApplicationLaunchers.Tool;component/images/arduino.png", UriKind.Relative)); }
        }

        public override void RunTool()
        {
            Helpers.TryRunTool(AppDomain.CurrentDomain.BaseDirectory+"ArduinoExtensions.exe");
        }

        public override bool IsVisible
        {
            get { return Helpers.VisibilityCheckPath(AppDomain.CurrentDomain.BaseDirectory + "ArduinoExtensions.exe"); }
        }
    }

    public class ApplicationEagleExt : ExternalTool
    {
        public override string Description
        {
            get { return "Install Eagle Libraries"; }
        }

        public override ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MApplicationLaunchers.Tool;component/images/eagle.png", UriKind.Relative)); }
        }

        public override void RunTool()
        {
            Helpers.TryRunTool(AppDomain.CurrentDomain.BaseDirectory + "EagleLibInstaller.exe");
        }

        public override bool IsVisible
        {
            get { return Helpers.VisibilityCheckPath(AppDomain.CurrentDomain.BaseDirectory + "EagleLibInstaller.exe"); }
        }
    }

    public class ApplicationLtSpice : ExternalTool
    {

        public override string Description
        {
            get { return "Launch LT Spice"; }
        }

        public override ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MApplicationLaunchers.Tool;component/images/ltspice.png", UriKind.Relative)); }
        }

        public override void RunTool()
        {
            Helpers.TryRunTool(Settings.Default.LtSpicePath);
        }

        public override bool IsVisible
        {
            get { return Helpers.VisibilityCheck("LtSpicePath", Helpers.ReferencePaths.LTSpice); }
        }

    }

    public class ApplicationProcessing : ExternalTool
    {

        public override string Description
        {
            get { return "Launch Processing"; }
        }

        public override ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MApplicationLaunchers.Tool;component/images/processing.png", UriKind.Relative)); }
        }

        public override void RunTool()
        {
            Helpers.TryRunTool(Settings.Default.ProcessingPath);
        }

        public override bool IsVisible
        {
            get { return Helpers.VisibilityCheck("ProcessingPath"); }
        }

    }

    public class SOCTool : PopupTool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            if (System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "SOC\\")) return new SOCTools();
            else
            {
                McuTools.Interfaces.WPF.WpfHelpers.ExceptionDialog("Component not installed required by this tool. Please run installer again");
                return null;
            }
        }

        public override string Description
        {
            get { return "SOC Tools"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MApplicationLaunchers.Tool;component/images/soc.png", UriKind.Relative)); }
        }
    }
}
