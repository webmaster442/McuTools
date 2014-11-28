///////////////////////////////////////////////////////////////////////////////
// CapDevice v1.1
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
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace CatenaLogic.Windows.Presentation.WebcamPlayer
{
    public class CapDevice : DependencyObject, IDisposable
    {
        #region Win32
        static readonly Guid FilterGraph = new Guid(0xE436EBB3, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        static readonly Guid SampleGrabber = new Guid(0xC1F400A0, 0x3F08, 0x11D3, 0x9F, 0x0B, 0x00, 0x60, 0x08, 0x03, 0x9E, 0x37);

        public static readonly Guid SystemDeviceEnum = new Guid(0x62BE5D10, 0x60EB, 0x11D0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        public static readonly Guid VideoInputDevice = new Guid(0x860BB310, 0x5D01, 0x11D0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        [ComVisible(false)]
        internal class MediaTypes
        {
            public static readonly Guid Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid Interleaved = new Guid(0x73766169, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid Audio = new Guid(0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid Text = new Guid(0x73747874, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid Stream = new Guid(0xE436EB83, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
        }

        [ComVisible(false)]
        internal class MediaSubTypes
        {
            public static readonly Guid YUYV = new Guid(0x56595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid IYUV = new Guid(0x56555949, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid DVSD = new Guid(0x44535644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

            public static readonly Guid RGB1 = new Guid(0xE436EB78, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

            public static readonly Guid RGB4 = new Guid(0xE436EB79, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

            public static readonly Guid RGB8 = new Guid(0xE436EB7A, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

            public static readonly Guid RGB565 = new Guid(0xE436EB7B, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

            public static readonly Guid RGB555 = new Guid(0xE436EB7C, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

            public static readonly Guid RGB24 = new Guid(0xE436Eb7D, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

            public static readonly Guid RGB32 = new Guid(0xE436EB7E, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

            public static readonly Guid Avi = new Guid(0xE436EB88, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

            public static readonly Guid Asf = new Guid(0x3DB80F90, 0x9412, 0x11D1, 0xAD, 0xED, 0x00, 0x00, 0xF8, 0x75, 0x4B, 0x99);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
        #endregion

        #region Variables
        private ManualResetEvent _stopSignal = null;
        private Thread _worker = null;
        private IGraphBuilder _graph = null;
        private ISampleGrabber _grabber = null;
        private IBaseFilter _sourceObject = null;
        private IBaseFilter _grabberObject = null;
        private IMediaControl _control = null;
        private CapGrabber _capGrabber = null;
        private IntPtr _map = IntPtr.Zero;
        private IntPtr _section = IntPtr.Zero;

        private System.Diagnostics.Stopwatch _timer = System.Diagnostics.Stopwatch.StartNew();
        private double _frames = 0.0;
        private string _monikerString = "";
        #endregion

        #region Constructor & destructor
        /// <summary>
        /// Initializes the default capture device
        /// </summary>
        public CapDevice()
            : this("")
        { }

        /// <summary>
        /// Initializes a specific capture device
        /// </summary>
        /// <param name="moniker">Moniker string that represents a specific device</param>
        public CapDevice(string moniker)
        {
            // Store moniker string
            MonikerString = moniker;

            // Check if this code is invoked by an application or as a user control
            if (Application.Current != null)
            {
                // Application, subscribe to exit event so we can shut down
                Application.Current.Exit += new ExitEventHandler(CurrentApplication_Exit);
            }
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            // Stop
            Stop();
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is invoked when a new bitmap is ready
        /// </summary>
        public event EventHandler NewBitmapReady;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the device monikers
        /// </summary>
        public static FilterInfo[] DeviceMonikers
        {
            get
            {
                List<FilterInfo> filters = new List<FilterInfo>();
                IMoniker[] ms = new IMoniker[1];
                ICreateDevEnum enumD = Activator.CreateInstance(Type.GetTypeFromCLSID(SystemDeviceEnum)) as ICreateDevEnum;
                IEnumMoniker moniker;
                Guid g = VideoInputDevice;
                if (enumD.CreateClassEnumerator(ref g, out moniker, 0) == 0)
                {
                    while (true)
                    {
                        int r = moniker.Next(1, ms, IntPtr.Zero);
                        if (r != 0 || ms[0] == null)
                            break;
                        filters.Add(new FilterInfo(ms[0]));
                        Marshal.ReleaseComObject(ms[0]);
                        ms[0] = null;
                    }
                }

                return filters.ToArray();
            }
        }

        /// <summary>
        /// Gets the available devices
        /// </summary>
        public static CapDevice[] Devices
        {
            get
            {
                // Declare variables
                List<CapDevice> devices = new List<CapDevice>();

                // Loop all monikers
                foreach (FilterInfo moniker in DeviceMonikers)
                {
                    devices.Add(new CapDevice(moniker.MonikerString));
                }

                // Return result
                return devices.ToArray();
            }
        }

        /// <summary>
        /// Wrapper for the BitmapSource dependency property
        /// </summary>
        public InteropBitmap BitmapSource
        {
            get { return (InteropBitmap)GetValue(BitmapSourceProperty); }
            private set { SetValue(BitmapSourcePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey BitmapSourcePropertyKey =
            DependencyProperty.RegisterReadOnly("BitmapSource", typeof(InteropBitmap), typeof(CapDevice), new UIPropertyMetadata(default(InteropBitmap)));

        public static readonly DependencyProperty BitmapSourceProperty = BitmapSourcePropertyKey.DependencyProperty;

        /// <summary>
        /// Wrapper for the Name dependency property
        /// </summary>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(CapDevice), new UIPropertyMetadata(""));

        /// <summary>
        /// Wrapper for the MonikerString dependency property
        /// </summary>
        public string MonikerString
        {
            get { return (string)GetValue(MonikerStringProperty); }
            set { SetValue(MonikerStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonikerString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonikerStringProperty =
            DependencyProperty.Register("MonikerString", typeof(string), typeof(CapDevice), new UIPropertyMetadata("", new PropertyChangedCallback(MonikerString_Changed)));

        /// <summary>
        /// Wrapper for the Framerate dependency property
        /// </summary>
        public float Framerate
        {
            get { return (float)GetValue(FramerateProperty); }
            set { SetValue(FramerateProperty, value); }
        }

        public static readonly DependencyProperty FramerateProperty =
            DependencyProperty.Register("Framerate", typeof(float), typeof(CapDevice), new UIPropertyMetadata(default(float)));

        /// <summary>
        /// Gets whether the capture device is currently running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                // Check if we have a worker thread
                if (_worker == null) return false;

                // Check if we can join the thread
                if (_worker.Join(0) == false) return true;

                // Release
                Release();

                // Not running
                return false;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Invoked when the application exits
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void CurrentApplication_Exit(object sender, ExitEventArgs e)
        {
            // Dispose
            Dispose();
        }

        /// <summary>
        /// Invoked when a new frame arrived
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void capGrabber_NewFrameArrived(object sender, EventArgs e)
        {
            // Make sure to be thread safe
            if (Dispatcher != null)
            {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
                {
                    if (BitmapSource != null)
                    {
                        BitmapSource.Invalidate();
                        UpdateFramerate();
                    }
                }, null);
            }
        }

        /// <summary>
        /// Invoked when the MonikerString dependency property has changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private static void MonikerString_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Get typed sender
            CapDevice typedSender = sender as CapDevice;
            if (typedSender != null)
            {
                // Always stop the device
                typedSender.Stop();

                // Get the new value
                string newMonikerString = e.NewValue as string;

                // Check if we have a valid moniker string
                if (!string.IsNullOrEmpty(newMonikerString))
                {
                    // Initialize device
                    typedSender.InitializeDeviceForMoniker(newMonikerString);

                    // Start
                    typedSender.Start();
                }
            }
        }

        /// <summary>
        /// Invoked when a property of the CapGrabber object has changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">PropertyChangedEventArgs</param>
        private void capGrabber_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.DataBind, (SendOrPostCallback)delegate
            {
                try
                {
                    if ((_capGrabber.Width != default(int)) && (_capGrabber.Height != default(int)))
                    {
                        // Get the pixel count
                        uint pcount = (uint)(_capGrabber.Width * _capGrabber.Height * PixelFormats.Bgr32.BitsPerPixel / 8);

                        // Create a file mapping
                        _section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, pcount, null);
                        _map = MapViewOfFile(_section, 0xF001F, 0, 0, pcount);

                        // Get the bitmap
                        BitmapSource = Imaging.CreateBitmapSourceFromMemorySection(_section, _capGrabber.Width,
                            _capGrabber.Height, PixelFormats.Bgr32, _capGrabber.Width * PixelFormats.Bgr32.BitsPerPixel / 8, 0) as InteropBitmap;
                        _capGrabber.Map = _map;

                        // Invoke event
                        if (NewBitmapReady != null)
                        {
                            NewBitmapReady(this, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trace
                    Trace.TraceError(ex.Message);
                }
            }, null);
        }

        /// <summary>
        /// Updates the framerate
        /// </summary>
        private void UpdateFramerate()
        {
            // Increase the frames
            _frames++;

            // Check the timer
            if (_timer.ElapsedMilliseconds >= 1000)
            {
                // Set the framerate
                Framerate = (float)Math.Round(_frames * 1000 / _timer.ElapsedMilliseconds);

                // Reset the timer again so we can count the framerate again
                _timer.Reset();
                _timer.Start();
                _frames = 0;
            }
        }

        /// <summary>
        /// Initialize the device for a specific moniker
        /// </summary>
        /// <param name="moniker">Moniker to initialize the device for</param>
        private void InitializeDeviceForMoniker(string moniker)
        {
            // Store moniker (since dependency properties are not thread-safe, store it locally as well)
            _monikerString = moniker;

            // Find the name
            foreach (FilterInfo filterInfo in DeviceMonikers)
            {
                if (filterInfo.MonikerString == moniker)
                {
                    Name = filterInfo.Name;
                    break;
                }
            }
        }

        /// <summary>;
        /// Starts grabbing images from the capture device
        /// </summary>
        public void Start()
        {
            // First check if we have a valid moniker string
            if (string.IsNullOrEmpty(_monikerString)) return;

            // Check if we are already running
            if (IsRunning)
            {
                // Yes, stop it first
                Stop();
            }

            // Create new grabber
            _capGrabber = new CapGrabber();
            _capGrabber.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(capGrabber_PropertyChanged);
            _capGrabber.NewFrameArrived += new EventHandler(capGrabber_NewFrameArrived);

            // Create manual reset event
            _stopSignal = new ManualResetEvent(false);

            // Start the thread
            _worker = new Thread(RunWorker);
            _worker.Start();
        }

        /// <summary>
        /// Stops grabbing images from the capture device
        /// </summary>
        public void Stop()
        {
            try
            {
                // Check if the capture device is even running
                if (IsRunning)
                {
                    // Yes, stop via the event
                    _stopSignal.Set();

                    // Abort the thread
                    _worker.Abort();
                    if (_worker != null)
                    {
                        // Join
                        _worker.Join();

                        // Release
                        Release();
                    }
                }
            }
            catch (Exception ex)
            {
                // Trace
                Trace.TraceError(ex.Message);

                // Release
                Release();
            }
        }

        /// <summary>
        /// Releases the capture device
        /// </summary>
        private void Release()
        {
            // Stop the thread
            _worker = null;

            // Clear the event
            if (_stopSignal != null)
            {
                _stopSignal.Close();
                _stopSignal = null;
            }

            if (_graph != null)
            {
                _graph.Abort();
                _graph.Disconnect(_sourceObject.GetPin(PinDirection.Output, 0));
                _graph.Disconnect(_grabberObject.GetPin(PinDirection.Input, 0));
                _graph.RemoveFilter(_sourceObject);
                _graph.RemoveFilter(_grabberObject);
            }

            // Clean up
            _graph = null;
            _sourceObject = null;
            _grabberObject = null;
            _grabber = null;
            _capGrabber = null;
            _control = null;
        }

        /// <summary>
        /// Worker thread that captures the images
        /// </summary>
        private void RunWorker()
        {
            try
            {
                // Create the main graph
                _graph = Activator.CreateInstance(Type.GetTypeFromCLSID(FilterGraph)) as IGraphBuilder;

                // Create the webcam source
                _sourceObject = FilterInfo.CreateFilter(_monikerString);

                // Create the grabber
                _grabber = Activator.CreateInstance(Type.GetTypeFromCLSID(SampleGrabber)) as ISampleGrabber;
                _grabberObject = _grabber as IBaseFilter;

                // Add the source and grabber to the main graph
                _graph.AddFilter(_sourceObject, "source");
                _graph.AddFilter(_grabberObject, "grabber");

                using (AMMediaType mediaType = new AMMediaType())
                {
                    mediaType.MajorType = MediaTypes.Video;
                    mediaType.SubType = MediaSubTypes.RGB32;
                    _grabber.SetMediaType(mediaType);

                    if (_graph.Connect(_sourceObject.GetPin(PinDirection.Output, 0), _grabberObject.GetPin(PinDirection.Input, 0)) >= 0)
                    {
                        if (_grabber.GetConnectedMediaType(mediaType) == 0)
                        {
                            // During startup, this code can be too fast, so try at least 3 times
                            int retryCount = 0;
                            bool succeeded = false;
                            while ((retryCount < 3) && !succeeded)
                            {
                                // Tried again
                                retryCount++;

                                try
                                {
                                    // Retrieve the grabber information
                                    VideoInfoHeader header = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));
                                    _capGrabber.Width = header.BmiHeader.Width;
                                    _capGrabber.Height = header.BmiHeader.Height;

                                    // Succeeded
                                    succeeded = true;
                                }
                                catch (Exception)
                                {
                                    // Trace
                                    Trace.TraceInformation("Failed to retrieve the grabber information, tried {0} time(s)", retryCount);

                                    // Sleep
                                    Thread.Sleep(50);
                                }
                            }
                        }
                    }
                    _graph.Render(_grabberObject.GetPin(PinDirection.Output, 0));
                    _grabber.SetBufferSamples(false);
                    _grabber.SetOneShot(false);
                    _grabber.SetCallback(_capGrabber, 1);

                    // Get the video window
                    IVideoWindow wnd = (IVideoWindow)_graph;
                    wnd.put_AutoShow(false);
                    wnd = null;

                    // Create the control and run
                    _control = (IMediaControl)_graph;
                    _control.Run();

                    // Wait for the stop signal
                    while (!_stopSignal.WaitOne(0, true))
                    {
                        Thread.Sleep(10);
                    }

                    // Stop when ready
                    _control.StopWhenReady();
                }
            }
            catch (Exception ex)
            {
                // Trace
                Trace.WriteLine(ex);
            }
            finally
            {
                // Clean up
                Release();
            }
        }
        #endregion
    }
}
