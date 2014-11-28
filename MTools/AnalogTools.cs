using McuTools.Interfaces;
using MTools.ToolsAnalog;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MTools
{
    public class ImpedanceCalc : Tool
    {
        public override UserControl GetControl()
        {
            return new ImpedanceCalculator();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override string Description
        {
            get { return "Impedance Calculator"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/rlc.png", UriKind.Relative)); }
        }
    }

    public class LedResist : Tool
    {
        public override UserControl GetControl()
        {
            return new LedResistor();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override string Description
        {
            get { return "Led Current Limiter calculator"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/led.png", UriKind.Relative)); }
        }
    }

    public class Lm317 : Tool
    {
        public override UserControl GetControl()
        {
            return new Lm317VoltageRegulator();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override string Description
        {
            get { return "Lm317 Calculator"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/lm317.png", UriKind.Relative)); }
        }
    }

    public class n555 : Tool
    {
        public override UserControl GetControl()
        {
            return new ne555Calc();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override string Description
        {
            get { return "555 Timer tool"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/555.png", UriKind.Relative)); }
        }
    }

    public class Opamp : Tool
    {
        public override UserControl GetControl()
        {
            return new OpAmpCalculator();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override string Description
        {
            get { return "OpAmp Calculator"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/opamp.png", UriKind.Relative)); }
        }
    }

    public class ResistColor : Tool
    {
        public override UserControl GetControl()
        {
            return new ResistorColorCode();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override string Description
        {
            get { return "Resistor Color Code Decoder"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/resistor.png", UriKind.Relative)); }
        }
    }

    public class ResistList : Tool
    {

        public override UserControl GetControl()
        {
            return new ResistorList();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override string Description
        {
            get { return "Standard Resistor values"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/resistor.png", UriKind.Relative)); }
        }
    }

    public class TotalResist : Tool
    {

        public override UserControl GetControl()
        {
            return new TotalResistance();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override string Description
        {
            get { return "Total Resistance Calculator"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/resistor.png", UriKind.Relative)); }
        }
    }

    public class ToneGen : Tool
    {
        public override UserControl GetControl()
        {
            return new ToolsAnalog.ToneGenerator();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override string Description
        {
            get { return "Tone Generator"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/functiongen.png", UriKind.Relative)); }
        }
    }

    public class Analogsampler : Tool
    {

        public override UserControl GetControl()
        {
            return new FirmataAnalogSampler();
        }

        public override string Description
        {
            get { return "Firmata Analog Sampler"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/firmatatanalog.png", UriKind.Relative)); }
        }
    }

    public class VoltageCurrentDiv : Tool
    {
        public override UserControl GetControl()
        {
            return new VoltageCurrentDivider();
        }

        public override string Description
        {
            get { return "Voltage & Current Divider"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/dividers.png", UriKind.Relative)); }
        }
    }

    public class FilterCalculator : Tool
    {
        public override UserControl GetControl()
        {
            return new FilterDesigner();
        }

        public override string Description
        {
            get { return "Filter Calculator"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/empty_filter-128.png", UriKind.Relative)); }
        }
    }

    public class OhmCalc : Tool
    {
        public override UserControl GetControl()
        {
            return new OhmCalculator();
        }

        public override string Description
        {
            get { return "Ohm's Law Calculator"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MTools.Tool;component/icons/ohmslaw.png", UriKind.Relative)); }
        }
    }

    public class ResSolver : Tool
    {
        public override UserControl GetControl()
        {
            return new ResistorSolver();
        }

        public override string Description
        {
            get { return "Resistor Value Solver"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Analog; }
        }
    }

}
