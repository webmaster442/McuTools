///////////////////////////////////////////////////////////////////////////////
// CapGrabber v1.1
//
// This software is released into the public domain.  You are free to use it
// in any way you like, except that you may not sell this source code.
//
// This software is provided "as is" with no expressed or implied warranty.
// I accept no liability for any damage or loss of business that this software
// may cause.
// 
// This source code is originally written by Tamir Khason (see http://blogs.microsoft.co.il/blogs/tamir
// or http://www.codeplex.com/wpfcap).
// 
// Modifications are made by Geert van Horrik (CatenaLogic, see http://blog.catenalogic.com) 
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.ComponentModel;

namespace CatenaLogic.Windows.Presentation.WebcamPlayer
{
    internal class CapGrabber : ISampleGrabberCB, INotifyPropertyChanged
    {
        #region Win32 imports
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        #endregion

        #region Variables
        private int _height = default(int);
        private int _width = default(int);
        #endregion 

        #region Constructor & destructor
        public CapGrabber()
        {
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler NewFrameArrived;
        #endregion

        #region Properties
        public IntPtr Map { get; set; }

        /// <summary>
        /// Gets or sets the width of the grabber
        /// </summary>
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        /// <summary>
        /// Gets or sets the height of the grabber
        /// </summary>
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }
        #endregion

        #region Methods
        public int SampleCB(double sampleTime, IntPtr sample)
        {
            return 0;
        }

        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
            if (Map != IntPtr.Zero)
            {
                CopyMemory(Map, buffer, bufferLen);
                OnNewFrameArrived();
            }
            return 0;
        }

        void OnNewFrameArrived()
        {
            if (NewFrameArrived != null)
            {
                NewFrameArrived(this, null);
            }
        }

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
