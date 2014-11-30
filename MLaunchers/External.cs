using System;
using System.Windows.Media.Imaging;
using McuTools.Interfaces;

namespace MLaunchers
{
    public class ExternalCfg: PopupTool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new ExternalConfig();
        }

        public override string Description
        {
            get { return "External App configuration"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.External; }
        }
    }

    public class Eagle : Eprog
    {
        public override string Path
        {
            get { return ConfigReader.Configuration.EaglePath; }
        }

        public override string Description
        {
            get { return "Eagle"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/eagle.png", UriKind.Relative)); }
        }
    }

    public class Arduino: Eprog
    {
        public override string Path
        {
            get { return ConfigReader.Configuration.ArduinoPath; }
        }

        public override string Description
        {
            get { return "Arduino"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/arduino.png", UriKind.Relative)); }
        }
    }

    public class LTSpice : Eprog
    {
        public override string Path
        {
            get { return ConfigReader.Configuration.LtSpicePath; }
        }

        public override string Description
        {
            get { return "LT Spice"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/ltspice.png", UriKind.Relative)); }
        }
    }

    public class Processing: Eprog
    {

        public override string Path
        {
            get { return ConfigReader.Configuration.ProcessingPath; }
        }

        public override string Description
        {
            get { return "Processing"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/processing.png", UriKind.Relative)); }
        }
    }
}
