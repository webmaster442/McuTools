using System.ComponentModel;

namespace McuShell.Kernel
{
    /// <summary>
    /// File Size Constants
    /// </summary>
    public enum FileSize: long
    {
        /// <summary>
        /// 1 PetaByte
        /// </summary>
        [Description("PiB")]
        PetaByte = 1125899906842624,
        /// <summary>
        /// 1 TerraByte
        /// </summary>
        [Description("TiB")]
        TerraByte = 1073741824,
        /// <summary>
        /// 1 GigaByte
        /// </summary>
        [Description("GiB")]
        GigaByte = 1073741824,
        /// <summary>
        /// 1 MegaByte
        /// </summary>
        [Description("MiB")]
        MegaByte = 1048576,
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
}
