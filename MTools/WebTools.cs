using McuTools.Interfaces;
using System;
using System.Windows.Media.Imaging;

namespace MTools
{
    public class ArduinoDoc : WebTool
    {
        public override string URL
        {
            get { return "http://arduino.cc/en/Reference/HomePage"; }
        }

        public override string Description
        {
            get { return "Arduino Reference"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/arduino.png", UriKind.Relative)); }
        }
    }

    public class CppReference : WebTool
    {
        public override string URL
        {
            get { return "http://en.cppreference.com/w/"; }
        }

        public override string Description
        {
            get { return "C/C++ reference"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/cppreference.png", UriKind.Relative)); }
        }
    }

    public class Tindle : WebTool
    {
        public override string URL
        {
            get { return "https://www.tindie.com/"; }
        }

        public override string Description
        {
            get { return "Tindie"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/tindie.png", UriKind.Relative)); }
        }
    }

    public class MAPS : WebTool
    {
        public override string URL
        {
            get { return "http://www.microchip.com/maps/main.aspx"; }
        }

        public override string Description
        {
            get { return "Microchip Advanced Part Selector"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/microchip.png", UriKind.Relative)); }
        }
    }

    public class ALS : WebTool
    {
        public override string URL
        {
            get { return "http://learn.adafruit.com/"; }
        }

        public override string Description
        {
            get { return "Adafruit Learning System"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/adafruit.png", UriKind.Relative)); }
        }
    }

    public class Pydoc : WebTool
    {

        public override string URL
        {
            get { return "http://docs.python.org/2/"; }
        }

        public override string Description
        {
            get { return "Python documentation"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/python.png", UriKind.Relative)); }
        }
    }

    public class APPS : WebTool
    {
        public override string URL
        {
            get { return "http://www.atmel.com/products/selector_overview.aspx"; }
        }

        public override string Description
        {
            get { return "Atmel Parametric Product Selector"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/atmel.png", UriKind.Relative)); }
        }
    }

    public class Webbench : WebTool
    {
        public override string URL
        {
            get { return "http://webench.ti.com/webench5/power/webench5.cgi?app=powerarchitect&lang_chosen=en_US"; }
        }

        public override string Description
        {
            get { return "WEBENCH® Power Architect"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/texasinstruments.png", UriKind.Relative)); }
        }
    }

    public class Circuitlab : WebTool
    {
        public override string URL
        {
            get { return "https://www.circuitlab.com/editor/"; }
        }

        public override string Description
        {
            get { return "Circuitlab"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/circuitlab.png", UriKind.Relative)); }
        }
    }

    public class pastebin : WebTool
    {
        public override string URL
        {
            get { return "http://pastebin.com/"; }
        }

        public override string Description
        {
            get { return "Pastebin"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/pastebin.png", UriKind.Relative)); }
        }
    }

    public class WaveDrom : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/App/Wavedrom/editor.html"; }
        }

        public override string Description
        {
            get { return "Timing diagram editor"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/wavedrom.png", UriKind.Relative)); }
        }
    }

    public class game2048 : WebTool
    {
        public override string URL
        {
            get { return "asset://mcutools/App/2048/index.html"; }
        }

        public override string Description
        {
            get { return "2048"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/2048.png", UriKind.Relative)); }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Other; }
        }
    }
}
