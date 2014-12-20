using System.ComponentModel;

namespace McuShell.Kernel
{
    /// <summary>
    /// File Size Constants
    /// </summary>
    public enum FileSize : long
    {
        /// <summary>
        /// 1 PetaByte
        /// </summary>
        [Description("PiB")]
        PetaByte = TerraByte * 1024L,
        /// <summary>
        /// 1 TerraByte
        /// </summary>
        [Description("TiB")]
        TerraByte = GigaByte * 1024L,
        /// <summary>
        /// 1 GigaByte
        /// </summary>
        [Description("GiB")]
        GigaByte = MegaByte * 1024L,
        /// <summary>
        /// 1 MegaByte
        /// </summary>
        [Description("MiB")]
        MegaByte = KiloByte * 1024L,
        /// <summary>
        /// 1 KiloByte
        /// </summary>
        [Description("KiB")]
        KiloByte = 1024,
        /// <summary>
        /// 1 Byte
        /// </summary>
        [Description("Byte")]
        Byte = 1
    }

    /// <summary>
    /// File Type Indication flag
    /// </summary>
    public enum FileType
    {
        Document, Audio, Video, Executable, Image, Unknown, Archive
    }

    /// <summary>
    /// Known File Extensions
    /// </summary>
    public static class KnownFileExtensions
    {
        /// <summary>
        /// Video Extensions
        /// </summary>
        public static string[] Video
        {
            get { return ".avi;.mpg;.mpeg;.mp4;.m4v;.qt;.mov;.asf;.wmv;.mkv;.vob".Split(';'); }
        }

        /// <summary>
        /// Audio Extensions
        /// </summary>
        public static string[] Audio
        {
            get { return ".mp3;.wav;.wma;.ape;.flac;.wv;.m4a;.m4b;.aac;.ac3;.dts;.mid;.rmi".Split(';'); }
        }

        /// <summary>
        /// Document Extensions
        /// </summary>
        public static string[] Document
        {
            get { return ".ppt;.xls;.doc;.mdb;.pptx;.xlsx;.docx;.odf;.odt;.txt;.csv;.pdf;.md".Split(';'); }
        }

        /// <summary>
        /// Image Extensions
        /// </summary>
        public static string[] Image
        {
            get { return ".jpg;.jpeg;.psd;.png;.bmp;.tiff;.emf;.wmf".Split(';'); }
        }

        /// <summary>
        /// Executables
        /// </summary>
        public static string[] Execute
        {
            get { return ".exe;.bat;.cmd;.ps".Split(';'); }
        }

        /// <summary>
        /// Archives
        /// </summary>
        public static string[] Archive
        {
            get { return ".zip;.rar;.7z;.ace;.tar;.bz2;.gz;.cab;.arj;.lha".Split(';'); }
        }
    }
}
