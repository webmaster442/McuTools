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
}
