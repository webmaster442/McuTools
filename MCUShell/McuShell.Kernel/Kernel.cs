using System;
using System.Linq;
using System.Text;

namespace McuShell.Kernel
{
    /// <summary>
    /// Various Functions for Shell programs
    /// </summary>
    public static class Kernel
    {
        /// <summary>
        /// Return a long value as a human readable file size
        /// </summary>
        /// <param name="value">a long file size</param>
        /// <returns>A human readable file size string</returns>
        public static string GetFileSize(long value)
        {
            FileSize size = FileSize.Byte;
            double outp = 0.0;
            if (value > (long)FileSize.PetaByte)
            {
                outp = value / (double)FileSize.PetaByte;
                size = FileSize.PetaByte;
            }
            else if (value > (long)FileSize.TerraByte)
            {
                outp = value / (double)FileSize.TerraByte;
                size = FileSize.TerraByte;
            }
            else if (value > (long)FileSize.GigaByte)
            {
                outp = value / (double)FileSize.GigaByte;
                size = FileSize.GigaByte;
            }
            else if (value > (long)FileSize.MegaByte)
            {
                outp = value / (double)FileSize.MegaByte;
                size = FileSize.MegaByte;
            }
            else if (value > (long)FileSize.KiloByte)
            {
                outp = value / (double)FileSize.KiloByte;
                size = FileSize.KiloByte;
            }
            else
            {
                outp = value;
                size = FileSize.Byte;
            }
            return string.Format("{0:0.000} {1}", outp, size.GetDescription());
        }

        /// <summary>
        /// Gets a file type from the file name
        /// </summary>
        /// <param name="filename">Input filename</param>
        /// <returns>A file type based on the file extension</returns>
        public static FileType GetFileType(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return FileType.Unknown;
            string[] parts = filename.Split('.');
            if (parts.Length < 2) return FileType.Unknown;
            string extension = parts[parts.Length - 1];
            if (string.IsNullOrEmpty(extension)) return FileType.Unknown;

            bool query = false;

            query = (from i in KnownFileExtensions.Archive where i == extension select i).Count() > 0;
            if (query) return FileType.Archive;
            query = (from i in KnownFileExtensions.Audio where i == extension select i).Count() > 0;
            if (query) return FileType.Audio;
            query = (from i in KnownFileExtensions.Document where i == extension select i).Count() > 0;
            if (query) return FileType.Document;
            query = (from i in KnownFileExtensions.Execute where i == extension select i).Count() > 0;
            if (query) return FileType.Executable;
            query = (from i in KnownFileExtensions.Image where i == extension select i).Count() > 0;
            if (query) return FileType.Image;
            query = (from i in KnownFileExtensions.Video where i == extension select i).Count() > 0;
            if (query) return FileType.Video;

            return FileType.Unknown;
        }

        /// <summary>
        /// Formats a date & time to various formats
        /// </summary>
        /// <param name="date">date to format</param>
        /// <returns></returns>
        public static string FormatDateTime(DateTime date, string label)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}:\t{1}\r\n", label, date);
            sb.AppendFormat("UTC Date & time:\t{0}\r\n", date.ToUniversalTime());
            sb.AppendFormat("Unix Time:\t\t{0}\r\n", (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            return sb.ToString();
        }

        /// <summary>
        /// Debug mode Wait for exit Function
        /// </summary>
        public static void DebugWait()
        {
#if DEBUG
            try { Console.ReadKey();  }
            catch (InvalidOperationException) { }
#endif
        }
    }
}
