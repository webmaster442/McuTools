using McuTools.Interfaces;
using MMediaTools.Tools;
using System;
using System.Windows.Media.Imaging;

namespace MMediaTools
{
    public class ImgConv : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new PictureConverter();
        }

        public override string Description
        {
            get { return "Picture Converter"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Digital; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MMediaTools.Tool;component/icons/picture-128.png", UriKind.Relative)); }
        }
    }

    public class USBLoader : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new UsbVideo();
        }

        public override string Description
        {
            get { return "USB Video view"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Other; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MMediaTools.Tool;component/icons/webcam.png", UriKind.Relative)); }
        }
    }

    public class ImageView : Tool
    {
        public override System.Windows.Controls.UserControl GetControl()
        {
            return new PictureViewer();
        }

        public override string Description
        {
            get { return "Picture viewer"; }
        }

        public override ToolCategory Category
        {
            get { return ToolCategory.Other; }
        }
    }
}
