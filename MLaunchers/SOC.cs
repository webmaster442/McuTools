using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MLaunchers
{
    public class Kitty: Eprog
    {
        public override string Path
        {
            get { return ConfigReader.AppDir+"\\SOC\\Kitty.exe"; }
        }

        public override string Description
        {
            get { return "Kitty SSH"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/kitty.png", UriKind.Relative)); }
        }
    }
}
