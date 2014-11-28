using McuTools.Interfaces;
using System;
using System.Windows.Media.Imaging;

namespace MTools
{
    public class ArduinoABC : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/index.html?file=page-arduinoabc"; }
        }

        public override string Description
        {
            get { return "Arduino Basic Connections"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/arduinoabc.png", UriKind.Relative)); }
        }
    }

    public class ArduinoPinouts : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/index.html?file=page-arduinopinreference"; }
        }

        public override string Description
        {
            get { return "Arduino Pinouts"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/arduino.png", UriKind.Relative)); }
        }
    }

    public class DigitalSchematics : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/index.html?file=page-digitalsch"; }
        }

        public override string Description
        {
            get { return "Digital schematics"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/schematics.png", UriKind.Relative)); }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }
    }

    public class dev7xpinout : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/index.html?file=page-pinout74"; }
        }

        public override string Description
        {
            get { return "74xx IC pinouts"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/7-128.png", UriKind.Relative)); }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }
    }

    public class dev4xpinout : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/index.html?file=page-pinout4x"; }
        }

        public override string Description
        {
            get { return "4xxx IC pinouts"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/4-128.png", UriKind.Relative)); }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }
    }

    public class dev7xlist : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/index.html?file=page-digitalic74"; }
        }

        public override string Description
        {
            get { return "74xx IC list"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/7-128.png", UriKind.Relative)); }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }
    }

    public class dev4xlist : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/index.html?file=page-digitalic4x"; }
        }

        public override string Description
        {
            get { return "4xxx IC list"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/4-128.png", UriKind.Relative)); }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }
    }

    public class devpinouts : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/index.html?file=page-devboard"; }
        }

        public override string Description
        {
            get { return "Development board pinouts"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/integrated_circuit.png", UriKind.Relative)); }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }
    }

    public class TutVideos : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/index.html?file=page-videos"; }
        }

        public override string Description
        {
            get { return "Tutorial Videos"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/film-128.png", UriKind.Relative)); }
        }
    }

    public class VariousIC : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/index.html?file=page-pinoutetc"; }
        }

        public override string Description
        {
            get { return "Various IC Pinouts"; }
        }
    }
}
