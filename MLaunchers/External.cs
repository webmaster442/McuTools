using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MLaunchers
{
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
}
