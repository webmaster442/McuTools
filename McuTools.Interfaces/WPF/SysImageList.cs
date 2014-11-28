using McuTools.Interfaces.NativeCode;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace McuTools.Interfaces.WPF
{
    #region SysImageList
    /// <summary>
    /// Summary description for SysImageList.
    /// </summary>
    internal class SysImageList : IDisposable
    {
        #region Member Variables
        private IntPtr hIml = IntPtr.Zero;
        private IImageList iImageList = null;
        private SysImageListSize size = SysImageListSize.smallIcons;
        private bool disposed = false;
        #endregion

        #region Implementation

        #region Properties
        /// <summary>
        /// Gets the hImageList handle
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return this.hIml;
            }
        }
        /// <summary>
        /// Gets/sets the size of System Image List to retrieve.
        /// </summary>
        public SysImageListSize ImageListSize
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                create();
            }

        }

        /// <summary>
        /// Returns the size of the Image List Icons.
        /// </summary>
        public System.Drawing.Size Size
        {
            get
            {
                int cx = 0;
                int cy = 0;
                if (iImageList == null)
                {
                    NativeMethods.ImageList_GetIconSize(
                        hIml,
                        ref cx,
                        ref cy);
                }
                else
                {
                    iImageList.GetIconSize(ref cx, ref cy);
                }
                System.Drawing.Size sz = new System.Drawing.Size(
                    cx, cy);
                return sz;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a GDI+ copy of the icon from the ImageList
        /// at the specified index.
        /// </summary>
        /// <param name="index">The index to get the icon for</param>
        /// <returns>The specified icon</returns>
        public Icon Icon(int index)
        {
            Icon icon = null;

            IntPtr hIcon = IntPtr.Zero;
            if (iImageList == null)
            {
                hIcon = NativeMethods.ImageList_GetIcon(
                    hIml,
                    index,
                    (int)ImageListDrawItemConstants.ILD_TRANSPARENT);

            }
            else
            {
                iImageList.GetIcon(
                    index,
                    (int)ImageListDrawItemConstants.ILD_TRANSPARENT,
                    ref hIcon);
            }

            if (hIcon != IntPtr.Zero)
            {
                icon = System.Drawing.Icon.FromHandle(hIcon);
            }
            return icon;
        }

        /// <summary>
        /// Return the index of the icon for the specified file, always using 
        /// the cached version where possible.
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(string fileName)
        {
            return IconIndex(fileName, false);
        }

        /// <summary>
        /// Returns the index of the icon for the specified file
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <param name="forceLoadFromDisk">If True, then hit the disk to get the icon,
        /// otherwise only hit the disk if no cached icon is available.</param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(
            string fileName,
            bool forceLoadFromDisk)
        {
            return IconIndex(
                fileName,
                forceLoadFromDisk,
                ShellIconStateConstants.ShellIconStateNormal);
        }

        /// <summary>
        /// Returns the index of the icon for the specified file
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <param name="forceLoadFromDisk">If True, then hit the disk to get the icon,
        /// otherwise only hit the disk if no cached icon is available.</param>
        /// <param name="iconState">Flags specifying the state of the icon
        /// returned.</param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(
            string fileName,
            bool forceLoadFromDisk,
            ShellIconStateConstants iconState
            )
        {
            SHGetFileInfoConstants dwFlags = SHGetFileInfoConstants.SHGFI_SYSICONINDEX;
            int dwAttr = 0;
            if (size == SysImageListSize.smallIcons)
            {
                dwFlags |= SHGetFileInfoConstants.SHGFI_SMALLICON;
            }

            // We can choose whether to access the disk or not. If you don't
            // hit the disk, you may get the wrong icon if the icon is
            // not cached. Also only works for files.
            if (!forceLoadFromDisk)
            {
                dwFlags |= SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES;
                dwAttr = NativeConsts.FILE_ATTRIBUTE_NORMAL;
            }
            else
            {
                dwAttr = 0;
            }

            // sFileSpec can be any file. You can specify a
            // file that does not exist and still get the
            // icon, for example sFileSpec = "C:\PANTS.DOC"
            SHFILEINFO shfi = new SHFILEINFO();
            uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
            IntPtr retVal = NativeMethods.SHGetFileInfo(
                fileName, dwAttr, ref shfi, shfiSize,
                ((uint)(dwFlags) | (uint)iconState));

            if (retVal.Equals(IntPtr.Zero))
            {
                System.Diagnostics.Debug.Assert((!retVal.Equals(IntPtr.Zero)), "Failed to get icon index");
                return 0;
            }
            else
            {
                return shfi.iIcon;
            }
        }

        /// <summary>
        /// Draws an image
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        public void DrawImage(
            IntPtr hdc,
            int index,
            int x,
            int y
            )
        {
            DrawImage(hdc, index, x, y, ImageListDrawItemConstants.ILD_TRANSPARENT);
        }

        /// <summary>
        /// Draws an image using the specified flags
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="flags">Drawing flags</param>
        public void DrawImage(
            IntPtr hdc,
            int index,
            int x,
            int y,
            ImageListDrawItemConstants flags
            )
        {
            if (iImageList == null)
            {
                int ret = NativeMethods.ImageList_Draw(
                    hIml,
                    index,
                    hdc,
                    x,
                    y,
                    (int)flags);
            }
            else
            {
                IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
                pimldp.hdcDst = hdc;
                pimldp.cbSize = Marshal.SizeOf(pimldp.GetType());
                pimldp.i = index;
                pimldp.x = x;
                pimldp.y = y;
                pimldp.rgbFg = -1;
                pimldp.fStyle = (int)flags;
                iImageList.Draw(ref pimldp);
            }

        }

        /// <summary>
        /// Draws an image using the specified flags and specifies
        /// the size to clip to (or to stretch to if ILD_SCALE
        /// is provided).
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="flags">Drawing flags</param>
        /// <param name="cx">Width to draw</param>
        /// <param name="cy">Height to draw</param>
        public void DrawImage(
            IntPtr hdc,
            int index,
            int x,
            int y,
            ImageListDrawItemConstants flags,
            int cx,
            int cy
            )
        {
            IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
            pimldp.hdcDst = hdc;
            pimldp.cbSize = Marshal.SizeOf(pimldp.GetType());
            pimldp.i = index;
            pimldp.x = x;
            pimldp.y = y;
            pimldp.cx = cx;
            pimldp.cy = cy;
            pimldp.fStyle = (int)flags;
            if (iImageList == null)
            {
                pimldp.himl = hIml;
                int ret = NativeMethods.ImageList_DrawIndirect(ref pimldp);
            }
            else
            {

                iImageList.Draw(ref pimldp);
            }
        }

        /// <summary>
        /// Draws an image using the specified flags and state on XP systems.
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="flags">Drawing flags</param>
        /// <param name="cx">Width to draw</param>
        /// <param name="cy">Height to draw</param>
        /// <param name="foreColor">Fore colour to blend with when using the 
        /// ILD_SELECTED or ILD_BLEND25 flags</param>
        /// <param name="stateFlags">State flags</param>
        /// <param name="glowOrShadowColor">If stateFlags include ILS_GLOW, then
        /// the colour to use for the glow effect.  Otherwise if stateFlags includes 
        /// ILS_SHADOW, then the colour to use for the shadow.</param>
        /// <param name="saturateColorOrAlpha">If stateFlags includes ILS_ALPHA,
        /// then the alpha component is applied to the icon. Otherwise if 
        /// ILS_SATURATE is included, then the (R,G,B) components are used
        /// to saturate the image.</param>
        public void DrawImage(
            IntPtr hdc,
            int index,
            int x,
            int y,
            ImageListDrawItemConstants flags,
            int cx,
            int cy,
            System.Drawing.Color foreColor,
            ImageListDrawStateConstants stateFlags,
            System.Drawing.Color saturateColorOrAlpha,
            System.Drawing.Color glowOrShadowColor
            )
        {
            IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
            pimldp.hdcDst = hdc;
            pimldp.cbSize = Marshal.SizeOf(pimldp.GetType());
            pimldp.i = index;
            pimldp.x = x;
            pimldp.y = y;
            pimldp.cx = cx;
            pimldp.cy = cy;
            pimldp.rgbFg = Color.FromArgb(0,
                foreColor.R, foreColor.G, foreColor.B).ToArgb();
            Console.WriteLine("{0}", pimldp.rgbFg);
            pimldp.fStyle = (int)flags;
            pimldp.fState = (int)stateFlags;
            if ((stateFlags & ImageListDrawStateConstants.ILS_ALPHA) ==
                ImageListDrawStateConstants.ILS_ALPHA)
            {
                // Set the alpha:
                pimldp.Frame = (int)saturateColorOrAlpha.A;
            }
            else if ((stateFlags & ImageListDrawStateConstants.ILS_SATURATE) ==
                ImageListDrawStateConstants.ILS_SATURATE)
            {
                // discard alpha channel:
                saturateColorOrAlpha = Color.FromArgb(0,
                    saturateColorOrAlpha.R,
                    saturateColorOrAlpha.G,
                    saturateColorOrAlpha.B);
                // set the saturate color
                pimldp.Frame = saturateColorOrAlpha.ToArgb();
            }
            glowOrShadowColor = Color.FromArgb(0,
                glowOrShadowColor.R,
                glowOrShadowColor.G,
                glowOrShadowColor.B);
            pimldp.crEffect = glowOrShadowColor.ToArgb();
            if (iImageList == null)
            {
                pimldp.himl = hIml;
                int ret = NativeMethods.ImageList_DrawIndirect(ref pimldp);
            }
            else
            {

                iImageList.Draw(ref pimldp);
            }
        }

        /// <summary>
        /// Determines if the system is running Windows XP
        /// or above
        /// </summary>
        /// <returns>True if system is running XP or above, False otherwise</returns>
        private bool isXpOrAbove()
        {
            bool ret = false;
            if (Environment.OSVersion.Version.Major > 5)
            {
                ret = true;
            }
            else if ((Environment.OSVersion.Version.Major == 5) &&
                (Environment.OSVersion.Version.Minor >= 1))
            {
                ret = true;
            }
            return ret;
            //return false;
        }

        /// <summary>
        /// Creates the SystemImageList
        /// </summary>
        private void create()
        {
            // forget last image list if any:
            hIml = IntPtr.Zero;

            if (isXpOrAbove())
            {
                // Get the System IImageList object from the Shell:
                Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
                int ret = NativeMethods.SHGetImageList(
                    (int)size,
                    ref iidImageList,
                    ref iImageList
                    );
                // the image list handle is the IUnknown pointer, but 
                // using Marshal.GetIUnknownForObject doesn't return
                // the right value.  It really doesn't hurt to make
                // a second call to get the handle:
                NativeMethods.SHGetImageListHandle((int)size, ref iidImageList, ref hIml);
            }
            else
            {
                // Prepare flags:
                SHGetFileInfoConstants dwFlags = SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES | SHGetFileInfoConstants.SHGFI_SYSICONINDEX;
                if (size == SysImageListSize.smallIcons)
                {
                    dwFlags |= SHGetFileInfoConstants.SHGFI_SMALLICON;
                }
                // Get image list
                SHFILEINFO shfi = new SHFILEINFO();
                uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());

                // Call SHGetFileInfo to get the image list handle
                // using an arbitrary file:
                hIml = NativeMethods.SHGetFileInfo(
                    ".txt",
                    NativeConsts.FILE_ATTRIBUTE_NORMAL,
                    ref shfi,
                    shfiSize,
                    (uint)dwFlags);
                System.Diagnostics.Debug.Assert((hIml != IntPtr.Zero), "Failed to create Image List");
            }
        }
        #endregion

        #region Constructor, Dispose, Destructor
        /// <summary>
        /// Creates a Small Icons SystemImageList 
        /// </summary>
        public SysImageList()
        {
            create();
        }
        /// <summary>
        /// Creates a SystemImageList with the specified size
        /// </summary>
        /// <param name="size">Size of System ImageList</param>
        public SysImageList(SysImageListSize size)
        {
            this.size = size;
            create();
        }

        /// <summary>
        /// Clears up any resources associated with the SystemImageList
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Clears up any resources associated with the SystemImageList
        /// when disposing is true.
        /// </summary>
        /// <param name="disposing">Whether the object is being disposed</param>
        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (iImageList != null)
                    {
                        Marshal.ReleaseComObject(iImageList);
                    }
                    iImageList = null;
                }
            }
            disposed = true;
        }
        /// <summary>
        /// Finalise for SysImageList
        /// </summary>
        ~SysImageList()
        {
            Dispose(false);
        }

    }
        #endregion

        #endregion

    #endregion

    #region SysImageListHelper
    /// <summary>
    /// Helper Methods for Connecting SysImageList to Common Controls
    /// </summary>
    internal class SysImageListHelper
    {
        #region UnmanagedMethods
        private const int LVM_FIRST = 0x1000;
        private const int LVM_SETIMAGELIST = (LVM_FIRST + 3);

        private const int LVSIL_NORMAL = 0;
        private const int LVSIL_SMALL = 1;
        private const int LVSIL_STATE = 2;

        private const int TV_FIRST = 0x1100;
        private const int TVM_SETIMAGELIST = (TV_FIRST + 9);

        private const int TVSIL_NORMAL = 0;
        private const int TVSIL_STATE = 2;

        #endregion

        /// <summary>
        /// Associates a SysImageList with a ListView control
        /// </summary>
        /// <param name="listView">ListView control to associate ImageList with</param>
        /// <param name="sysImageList">System Image List to associate</param>
        /// <param name="forStateImages">Whether to add ImageList as StateImageList</param>
        public static void SetListViewImageList(
            ListView listView,
            SysImageList sysImageList,
            bool forStateImages
            )
        {
            IntPtr wParam = (IntPtr)LVSIL_NORMAL;
            if (sysImageList.ImageListSize == SysImageListSize.smallIcons)
            {
                wParam = (IntPtr)LVSIL_SMALL;
            }
            if (forStateImages)
            {
                wParam = (IntPtr)LVSIL_STATE;
            }
            NativeMethods.SendMessage(
                listView.Handle,
                LVM_SETIMAGELIST,
                wParam,
                sysImageList.Handle);
        }

        /// <summary>
        /// Associates a SysImageList with a TreeView control
        /// </summary>
        /// <param name="treeView">TreeView control to associated ImageList with</param>
        /// <param name="sysImageList">System Image List to associate</param>
        /// <param name="forStateImages">Whether to add ImageList as StateImageList</param>
        public static void SetTreeViewImageList(
            TreeView treeView,
            SysImageList sysImageList,
            bool forStateImages
            )
        {
            IntPtr wParam = (IntPtr)TVSIL_NORMAL;
            if (forStateImages)
            {
                wParam = (IntPtr)TVSIL_STATE;
            }
            NativeMethods.SendMessage(
                treeView.Handle,
                TVM_SETIMAGELIST,
                wParam,
                sysImageList.Handle);
        }
    }
    #endregion

}
