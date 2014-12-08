using System;

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
    }
}
