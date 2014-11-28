using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMediaTools.Classes
{
    public enum ImageFormat
    {
        Jpeg, Png, Bmp, Gif, Tiff, Wmp
    }

    public enum SizeingMode
    {
        Percent, BoxFit, NoChange
    }

    public enum PixFormat
    {
        Nochange, Indexed, Bit24, Bit32
    }

    public class ConvertOptions
    {
        public ImageFormat OutputFormat
        {
            get;
            set;
        }

        public SizeingMode SizeMode
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public double Percent
        {
            get;
            set;
        }

        public int Quality
        {
            get;
            set;
        }

        public PixFormat PixelFormat
        {
            get;
            set;
        }

        public string TargetDir
        {
            get;
            set;
        }

    }
}
