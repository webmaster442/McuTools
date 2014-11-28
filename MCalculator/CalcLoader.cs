using McuTools.Interfaces;
using System;
using System.Windows.Media.Imaging;

namespace MCalculator
{
    public class CalcLoader: Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new Calculator2();
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Other; }
        }

        public override string Description
        {
            get { return "Calculator"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MCalculator.Tool;component/icon/plus_minus-128.png", UriKind.Relative)); }
        }
    }

    public class Unitconv : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new UnitConverter();
        }

        public override string Description
        {
            get { return "Unit Converter"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Other; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MCalculator.Tool;component/icon/length-128.png", UriKind.Relative)); }
        }
    }

    public class Functest : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new FunctionTester();
        }

        public override string Description
        {
            get { return "Logic function tester"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MCalculator.Tool;component/icon/circuit-128.png", UriKind.Relative)); }
        }
    }
}
