using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace McuTools.Interfaces.NativeCode
{
    /// <summary>
    /// Available system image list sizes
    /// </summary>
    internal enum SysImageListSize : int
    {
        /// <summary>
        /// System Large Icon Size (typically 32x32)
        /// </summary>
        largeIcons = 0x0,
        /// <summary>
        /// System Small Icon Size (typically 16x16)
        /// </summary>
        smallIcons = 0x1,
        /// <summary>
        /// System Extra Large Icon Size (typically 48x48).
        /// Only available under XP; under other OS the
        /// Large Icon ImageList is returned.
        /// </summary>
        extraLargeIcons = 0x2,
        jumbo = 0x4
    }

    /// <summary>
    /// Flags controlling how the Image List item is 
    /// drawn
    /// </summary>
    [Flags]
    internal enum ImageListDrawItemConstants : int
    {
        /// <summary>
        /// Draw item normally.
        /// </summary>
        ILD_NORMAL = 0x0,
        /// <summary>
        /// Draw item transparently.
        /// </summary>
        ILD_TRANSPARENT = 0x1,
        /// <summary>
        /// Draw item blended with 25% of the specified foreground colour
        /// or the Highlight colour if no foreground colour specified.
        /// </summary>
        ILD_BLEND25 = 0x2,
        /// <summary>
        /// Draw item blended with 50% of the specified foreground colour
        /// or the Highlight colour if no foreground colour specified.
        /// </summary>
        ILD_SELECTED = 0x4,
        /// <summary>
        /// Draw the icon's mask
        /// </summary>
        ILD_MASK = 0x10,
        /// <summary>
        /// Draw the icon image without using the mask
        /// </summary>
        ILD_IMAGE = 0x20,
        /// <summary>
        /// Draw the icon using the ROP specified.
        /// </summary>
        ILD_ROP = 0x40,
        /// <summary>
        /// Preserves the alpha channel in dest. XP only.
        /// </summary>
        ILD_PRESERVEALPHA = 0x1000,
        /// <summary>
        /// Scale the image to cx, cy instead of clipping it.  XP only.
        /// </summary>
        ILD_SCALE = 0x2000,
        /// <summary>
        /// Scale the image to the current DPI of the display. XP only.
        /// </summary>
        ILD_DPISCALE = 0x4000
    }

    /// <summary>
    /// Enumeration containing XP ImageList Draw State options
    /// </summary>
    [Flags]
    internal enum ImageListDrawStateConstants : int
    {
        /// <summary>
        /// The image state is not modified. 
        /// </summary>
        ILS_NORMAL = (0x00000000),
        /// <summary>
        /// Adds a glow effect to the icon, which causes the icon to appear to glow 
        /// with a given color around the edges. (Note: does not appear to be
        /// implemented)
        /// </summary>
        ILS_GLOW = (0x00000001), //The color for the glow effect is passed to the IImageList::Draw method in the crEffect member of IMAGELISTDRAWPARAMS. 
        /// <summary>
        /// Adds a drop shadow effect to the icon. (Note: does not appear to be
        /// implemented)
        /// </summary>
        ILS_SHADOW = (0x00000002), //The color for the drop shadow effect is passed to the IImageList::Draw method in the crEffect member of IMAGELISTDRAWPARAMS. 
        /// <summary>
        /// Saturates the icon by increasing each color component 
        /// of the RGB triplet for each pixel in the icon. (Note: only ever appears
        /// to result in a completely unsaturated icon)
        /// </summary>
        ILS_SATURATE = (0x00000004), // The amount to increase is indicated by the frame member in the IMAGELISTDRAWPARAMS method. 
        /// <summary>
        /// Alpha blends the icon. Alpha blending controls the transparency 
        /// level of an icon, according to the value of its alpha channel. 
        /// (Note: does not appear to be implemented).
        /// </summary>
        ILS_ALPHA = (0x00000008) //The value of the alpha channel is indicated by the frame member in the IMAGELISTDRAWPARAMS method. The alpha channel can be from 0 to 255, with 0 being completely transparent, and 255 being completely opaque. 
    }

    /// <summary>
    /// Flags specifying the state of the icon to draw from the Shell
    /// </summary>
    [Flags]
    internal enum ShellIconStateConstants
    {
        /// <summary>
        /// Get icon in normal state
        /// </summary>
        ShellIconStateNormal = 0,
        /// <summary>
        /// Put a link overlay on icon 
        /// </summary>
        ShellIconStateLinkOverlay = 0x8000,
        /// <summary>
        /// show icon in selected state 
        /// </summary>
        ShellIconStateSelected = 0x10000,
        /// <summary>
        /// get open icon 
        /// </summary>
        ShellIconStateOpen = 0x2,
        /// <summary>
        /// apply the appropriate overlays
        /// </summary>
        ShellIconAddOverlays = 0x000000020,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        int left;
        int top;
        int right;
        int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        int x;
        int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGELISTDRAWPARAMS
    {
        public int cbSize;
        public IntPtr himl;
        public int i;
        public IntPtr hdcDst;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int xBitmap;        // x offest from the upperleft of bitmap
        public int yBitmap;        // y offset from the upperleft of bitmap
        public int rgbBk;
        public int rgbFg;
        public int fStyle;
        public int dwRop;
        public int fState;
        public int Frame;
        public int crEffect;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGEINFO
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int Unused1;
        public int Unused2;
        public RECT rcImage;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public int dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NativeConsts.MAX_PATH)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    [Flags]
    internal enum SHGetFileInfoConstants : int
    {
        SHGFI_ICON = 0x100,                // get icon 
        SHGFI_DISPLAYNAME = 0x200,         // get display name 
        SHGFI_TYPENAME = 0x400,            // get type name 
        SHGFI_ATTRIBUTES = 0x800,          // get attributes 
        SHGFI_ICONLOCATION = 0x1000,       // get icon location 
        SHGFI_EXETYPE = 0x2000,            // return exe type 
        SHGFI_SYSICONINDEX = 0x4000,       // get system icon index 
        SHGFI_LINKOVERLAY = 0x8000,        // put a link overlay on icon 
        SHGFI_SELECTED = 0x10000,          // show icon in selected state 
        SHGFI_ATTR_SPECIFIED = 0x20000,    // get only specified attributes 
        SHGFI_LARGEICON = 0x0,             // get large icon 
        SHGFI_SMALLICON = 0x1,             // get small icon 
        SHGFI_OPENICON = 0x2,              // get open icon 
        SHGFI_SHELLICONSIZE = 0x4,         // get shell size icon 
        //SHGFI_PIDL = 0x8,                  // pszPath is a pidl 
        SHGFI_USEFILEATTRIBUTES = 0x10,     // use passed dwFileAttribute 
        SHGFI_ADDOVERLAYS = 0x000000020,     // apply the appropriate overlays
        SHGFI_OVERLAYINDEX = 0x000000040     // Get the index of the overlay
    }

    [ComImportAttribute()]
    [GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    //helpstring("Image List"),
    interface IImageList
    {
        [PreserveSig]
        int Add(
            IntPtr hbmImage,
            IntPtr hbmMask,
            ref int pi);

        [PreserveSig]
        int ReplaceIcon(
            int i,
            IntPtr hicon,
            ref int pi);

        [PreserveSig]
        int SetOverlayImage(
            int iImage,
            int iOverlay);

        [PreserveSig]
        int Replace(
            int i,
            IntPtr hbmImage,
            IntPtr hbmMask);

        [PreserveSig]
        int AddMasked(
            IntPtr hbmImage,
            int crMask,
            ref int pi);

        [PreserveSig]
        int Draw(
            ref IMAGELISTDRAWPARAMS pimldp);

        [PreserveSig]
        int Remove(
            int i);

        [PreserveSig]
        int GetIcon(
            int i,
            int flags,
            ref IntPtr picon);

        [PreserveSig]
        int GetImageInfo(
            int i,
            ref IMAGEINFO pImageInfo);

        [PreserveSig]
        int Copy(
            int iDst,
            IImageList punkSrc,
            int iSrc,
            int uFlags);

        [PreserveSig]
        int Merge(
            int i1,
            IImageList punk2,
            int i2,
            int dx,
            int dy,
            ref Guid riid,
            ref IntPtr ppv);

        [PreserveSig]
        int Clone(
            ref Guid riid,
            ref IntPtr ppv);

        [PreserveSig]
        int GetImageRect(
            int i,
            ref RECT prc);

        [PreserveSig]
        int GetIconSize(
            ref int cx,
            ref int cy);

        [PreserveSig]
        int SetIconSize(
            int cx,
            int cy);

        [PreserveSig]
        int GetImageCount(
            ref int pi);

        [PreserveSig]
        int SetImageCount(
            int uNewCount);

        [PreserveSig]
        int SetBkColor(
            int clrBk,
            ref int pclr);

        [PreserveSig]
        int GetBkColor(
            ref int pclr);

        [PreserveSig]
        int BeginDrag(
            int iTrack,
            int dxHotspot,
            int dyHotspot);

        [PreserveSig]
        int EndDrag();

        [PreserveSig]
        int DragEnter(
            IntPtr hwndLock,
            int x,
            int y);

        [PreserveSig]
        int DragLeave(
            IntPtr hwndLock);

        [PreserveSig]
        int DragMove(
            int x,
            int y);

        [PreserveSig]
        int SetDragCursorImage(
            ref IImageList punk,
            int iDrag,
            int dxHotspot,
            int dyHotspot);

        [PreserveSig]
        int DragShowNolock(
            int fShow);

        [PreserveSig]
        int GetDragImage(
            ref POINT ppt,
            ref POINT pptHotspot,
            ref Guid riid,
            ref IntPtr ppv);

        [PreserveSig]
        int GetItemFlags(
            int i,
            ref int dwFlags);

        [PreserveSig]
        int GetOverlayImage(
            int iOverlay,
            ref int piIndex);
    };
}
