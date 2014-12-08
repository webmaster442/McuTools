using McuTools.Interfaces;
using MTools.ToolsDigital;
using System;
using System.Windows.Media.Imaging;

namespace MTools
{
    public class ADCC : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new ADCCalculator();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override string Description
        {
            get { return "ADC Calculator"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/adc.png", UriKind.Relative)); }
        }
    }

    public class Ascii : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new AsciiTable();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override string Description
        {
            get { return "ASCII Table"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/ascii.png", UriKind.Relative)); }
        }
    }

    public class Segment14 : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new FourteenSegmentCalculator();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override string Description
        {
            get { return "Fourteen segment display calculator"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/fourteen.png", UriKind.Relative)); }
        }
    }

    public class LcdEdit : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new LcdCharCalculator();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override string Description
        {
            get { return "LCD Character Editor"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/lcd.png", UriKind.Relative)); }
        }
    }

    public class logicmin : Tool
    {

        public override System.Windows.Controls.UserControl GetControl()
        {
            return new LogicMinimalizer();
        }

        public override string Description
        {
            get { return "Logic function minimalizer"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/logicminimizer.png", UriKind.Relative)); }
        }
    }

    public class Numberc : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new NumberConverter();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override string Description
        {
            get { return "Number System Converter"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/numbersystem.png", UriKind.Relative)); }
        }
    }

    public class Segment7 : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new SevenSegmentCalculator();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override string Description
        {
            get { return "Seven Segment Calculator"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/sevensegment.png", UriKind.Relative)); }
        }
    }

    public class Terminal : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new SerialTerminal();
        }

        public override string Description
        {
            get { return "Serial Terminal"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/rs232terminal.png", UriKind.Relative)); }
        }
    }

    public class Boole : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new BooleAlgebra();
        }

        public override string Description
        {
            get { return "Boole Algebra"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/boolealgebra.png", UriKind.Relative)); }
        }
    }

    public class Firmata : Tool
    {

        public override System.Windows.Controls.UserControl GetControl()
        {
            return new FirmataControl();
        }

        public override string Description
        {
            get { return "Firmata Control"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/firmata.png", UriKind.Relative)); }
        }
    }

    public class PortValueCalc : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new PortEditor();
        }

        public override string Description
        {
            get { return "Port Value Calculator"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/integrated_circuit.png", UriKind.Relative)); }
        }
    }

    public class RgbLedColorCalc : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new ColorCalc();
        }

        public override string Description
        {
            get { return "PWM RGB Led driver Color editor"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/color.png", UriKind.Relative)); }
        }
    }

    public class HexEditor : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new Controls.HexEditor();
        }

        public override string Description
        {
            get { return "Hex Editor"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/hexedit.png", UriKind.Relative)); }
        }
    }

    public class CounterDes : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new SyncCounterDesigner();
        }

        public override string Description
        {
            get { return "Syc. Counter Designer"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/plus2-128.png", UriKind.Relative)); }
        }
    }

    public class LedMatrix : Tool
    {

        public override System.Windows.Controls.UserControl GetControl()
        {
            return new LEDMatrixEditor();
        }

        public override string Description
        {
            get { return "LED Matrix Editor"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/ledmatrix.png", UriKind.Relative)); }
        }
    }

    public class ArduinoSampleLoader : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new ArduinoExampleBrowser();
        }

        public override string Description
        {
            get { return "Arduino Sample Browser"; }
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

    public class Freqdiv : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new FreqDivCalc();
        }

        public override string Description
        {
            get { return "Frequency Division calculator"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/f_lowercase-100.png", UriKind.Relative)); }
        }
    }
}
