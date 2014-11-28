using MMediaTools.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace MMediaTools.Tools
{

    /// <summary>
    /// Interaction logic for PictureConverter.xaml
    /// </summary>
    public partial class PictureConverter : UserControl
    {
        private string[] _supported;
        private ObservableCollection<string> _Files;
        private ConvThread[] Threads;
        private DispatcherTimer _Timer;

        public ObservableCollection<string> Files
        {
            get { return _Files; }
            set { _Files = value; }
        }

        public static ConvertOptions ConvOptions { get; set; }

        public PictureConverter()
        {
            _Files = new ObservableCollection<string>();
            InitializeComponent();
            DataContext = this;
            _supported = new string[]
            {
                "*.jpg", "*.jpeg", "*.bmp", "*.png", ".tiff"
            };
            PictureConverter.ConvOptions = new ConvertOptions();
            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromSeconds(1);
            _Timer.Tick += new EventHandler(_Timer_Tick);
            _Timer.IsEnabled = false;
        }

        public string OpenDirectory(string InitialPath)
        {
            System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
            if (!string.IsNullOrEmpty(InitialPath)) fb.SelectedPath = InitialPath;
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK) return fb.SelectedPath;
            else return null;

        }

        public string[] OpenFiles(string InitialPath)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.CheckFileExists = true;
            if (!string.IsNullOrEmpty(InitialPath)) ofd.InitialDirectory = InitialPath;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) return ofd.FileNames;
            else return null;
        }

        private void InputAdd_Click(object sender, RoutedEventArgs e)
        {
            string[] f = OpenFiles(null);
            if (f != null)
            {
                foreach (var file in f)
                {
                    _Files.Add(file);
                }
            }
        }

        private void InputAddDir_Click(object sender, RoutedEventArgs e)
        {
            string dir = OpenDirectory(null);
            if (string.IsNullOrEmpty(dir)) return;
            List<string> files = new List<string>();
            foreach (var filter in _supported)
            {
                files.AddRange(Directory.GetFiles(dir, filter));
            }
            files.Sort();
            foreach (var file in files)
            {
                _Files.Add(file);
            }
        }

        private void InputAddDirr_Click(object sender, RoutedEventArgs e)
        {
            string dir = OpenDirectory(null);
            if (string.IsNullOrEmpty(dir)) return;
            List<string> files = new List<string>();
            foreach (var filter in _supported)
            {
                files.AddRange(Directory.GetFiles(dir, filter, SearchOption.AllDirectories));
            }
            files.Sort();
            foreach (var file in files)
            {
                _Files.Add(file);
            }
        }

        private void InputClear_Click(object sender, RoutedEventArgs e)
        {
            _Files.Clear();
        }

        private void InputRemove_Click(object sender, RoutedEventArgs e)
        {
            _Files.RemoveAt(LbInFiles.SelectedIndex);
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            string ipath = null;
            if (!string.IsNullOrEmpty(TbOutDir.Text)) ipath = TbOutDir.Text;
            string dir = OpenDirectory(ipath);
            TbOutDir.Text = dir;
        }

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            PictureConverter.ConvOptions.Height = (int)SlideResHeight.Value;
            PictureConverter.ConvOptions.Percent = (int)SlideResPercent.Value;
            PictureConverter.ConvOptions.Quality = (int)SlideFormatQuality.Value;
            PictureConverter.ConvOptions.Width = (int)SlideResWidth.Value;
            PictureConverter.ConvOptions.TargetDir = TbOutDir.Text;

            if ((bool)RbResFitbox.IsChecked) PictureConverter.ConvOptions.SizeMode = SizeingMode.BoxFit;
            else if ((bool)RbResPercent.IsChecked) PictureConverter.ConvOptions.SizeMode = SizeingMode.Percent;
            else PictureConverter.ConvOptions.SizeMode = SizeingMode.NoChange;

            if ((bool)RbFormJpg.IsChecked) PictureConverter.ConvOptions.OutputFormat = ImageFormat.Jpeg;
            else if ((bool)RbFormPng.IsChecked) PictureConverter.ConvOptions.OutputFormat = ImageFormat.Png;
            else if ((bool)RbFormBmp.IsChecked) PictureConverter.ConvOptions.OutputFormat = ImageFormat.Bmp;
            else if ((bool)RbFormTiff.IsChecked) PictureConverter.ConvOptions.OutputFormat = ImageFormat.Tiff;

            if ((bool)RbColorNo.IsChecked) PictureConverter.ConvOptions.PixelFormat = PixFormat.Nochange;
            else if ((bool)RbColor8bit.IsChecked) PictureConverter.ConvOptions.PixelFormat = PixFormat.Nochange;
            else if ((bool)RbColor24bit.IsChecked) PictureConverter.ConvOptions.PixelFormat = PixFormat.Bit24;
            else if ((bool)RbColor32bit.IsChecked) PictureConverter.ConvOptions.PixelFormat = PixFormat.Bit32;

            BtnConvert.IsEnabled = false;
            BtnCancel.IsEnabled = true;
            MainMenu.IsEnabled = false;
            Tabs.IsEnabled = false;

            SetupThreads();

            foreach (var thread in Threads) thread.StartThread();
            _Timer.IsEnabled = true;

            _Timer.IsEnabled = true;
        }


        public void SetupThreads()
        {
            Progress.Minimum = 0;
            Progress.Maximum = _Files.Count;
            if (Threads != null)
            {
                Threads = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            Threads = new ConvThread[Environment.ProcessorCount];
            for (int i = 0; i < Threads.Length; i++) Threads[i] = new ConvThread();
            int tc = 0;

            foreach (var file in _Files)
            {
                if (tc > Environment.ProcessorCount - 1) tc = 0;
                Threads[tc].AddFile(file);
                ++tc;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Stop Conversion?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                _Timer.IsEnabled = false;
                foreach (var thread in Threads) thread.StopThread();
            }
            BtnConvert.IsEnabled = true;
            BtnCancel.IsEnabled = false;
            MainMenu.IsEnabled = true;
            Tabs.IsEnabled = true;
        }

        private void _Timer_Tick(object sender, EventArgs e)
        {
            int val = 0;
            int compcount = 0;
            foreach (var thread in Threads)
            {
                if (thread.IsFinished) ++compcount;
                val += thread.Counter;
            }
            Progress.Value = val;
            if (compcount == Threads.Length)
            {
                _Timer.IsEnabled = false;
                MessageBox.Show("Conversion finished");
                BtnConvert.IsEnabled = true;
                BtnCancel.IsEnabled = false;
                MainMenu.IsEnabled = true;
                Tabs.IsEnabled = true;
            }
        }
    }
}
