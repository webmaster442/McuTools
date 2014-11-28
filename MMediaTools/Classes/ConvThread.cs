using MMediaTools.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MMediaTools.Classes
{
    class ConvThread
    {
        private Thread _thr;
        private List<string> _sourcefiles;
        private List<string> _error;
        private bool _isfinished;
        private int _cntr;

        public ConvThread()
        {
            _sourcefiles = new List<string>();
            _error = new List<string>();
        }

        public void AddFile(string file)
        {
            _sourcefiles.Add(file);
        }

        public int Count
        {
            get { return _sourcefiles.Count; }
        }

        public int Counter
        {
            get { return _cntr; }
        }

        public void StartThread()
        {
            _isfinished = false;
            _error.Clear();
            _cntr = 0;
            if (_sourcefiles.Count < 1) return;
            _thr = new Thread(ThreadFunction);
            _thr.Start();
        }

        public void StopThread()
        {
            if (_thr != null)
            {
                if (_thr.IsAlive) _thr.Abort();
                _thr = null;
            }
        }

        public void PauseResrume()
        {
            if (_thr.IsAlive)
            {
                if (_thr.ThreadState == ThreadState.Running) _thr.Suspend();
                else if (_thr.ThreadState == ThreadState.Suspended) _thr.Resume();
            }
        }

        public bool IsFinished
        {
            get { return _isfinished; }
        }

        private string CreateOutName(string input)
        {
            string dir = Path.GetDirectoryName(input);
            string outp = input.Replace(dir, PictureConverter.ConvOptions.TargetDir);
            switch (PictureConverter.ConvOptions.OutputFormat)
            {
                case ImageFormat.Bmp:
                    outp = Path.ChangeExtension(outp, ".bmp");
                    break;
                case ImageFormat.Gif:
                    outp = Path.ChangeExtension(outp, ".gif");
                    break;
                case ImageFormat.Jpeg:
                    outp = Path.ChangeExtension(outp, ".jpg");
                    break;
                case ImageFormat.Png:
                    outp = Path.ChangeExtension(outp, ".png");
                    break;
                case ImageFormat.Tiff:
                    outp = Path.ChangeExtension(outp, ".tiff");
                    break;
            }
            return outp;
        }

        private FormatConvertedBitmap ColorConv(TransformedBitmap input)
        {
            FormatConvertedBitmap ret = new FormatConvertedBitmap();
            ret.BeginInit();
            ret.Source = input;
            switch (PictureConverter.ConvOptions.PixelFormat)
            {
                case PixFormat.Bit24:
                    ret.DestinationFormat = PixelFormats.Rgb24;
                    break;
                case PixFormat.Bit32:
                    ret.DestinationFormat = PixelFormats.Bgra32;
                    break;
                case PixFormat.Indexed:
                    ret.DestinationFormat = PixelFormats.Indexed8;
                    break;
            }
            ret.EndInit();
            return ret;
        }

        private void ThreadFunction()
        {
            BitmapImage img;
            TransformedBitmap transformed;
            ScaleTransform trasform;
            BitmapEncoder enc;
            FileStream trgstream;
            string target;

            try
            {
                if (!Directory.Exists(PictureConverter.ConvOptions.TargetDir)) Directory.CreateDirectory(PictureConverter.ConvOptions.TargetDir);
            }
            catch (Exception) { return; }


            for (int i = 0; i < _sourcefiles.Count; i++)
            {
                try
                {
                    img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(_sourcefiles[i]);
                    img.EndInit();
                    img.Freeze();

                    switch (PictureConverter.ConvOptions.SizeMode)
                    {
                        case SizeingMode.Percent:
                            trasform = new ScaleTransform(PictureConverter.ConvOptions.Percent, PictureConverter.ConvOptions.Percent);
                            break;
                        case SizeingMode.BoxFit:
                            double zoomratio = Math.Min((double)PictureConverter.ConvOptions.Height / img.PixelHeight, (double)PictureConverter.ConvOptions.Width / img.PixelWidth);
                            trasform = new ScaleTransform(zoomratio, zoomratio);
                            break;
                        case SizeingMode.NoChange:
                        default:
                            trasform = new ScaleTransform(1, 1);
                            break;
                    }

                    transformed = new TransformedBitmap(img, trasform);

                    target = CreateOutName(_sourcefiles[i]);
                    if (File.Exists(target)) File.Delete(target);
                    trgstream = File.Create(target);

                    switch (PictureConverter.ConvOptions.OutputFormat)
                    {
                        case ImageFormat.Jpeg:
                            enc = new JpegBitmapEncoder();
                            (enc as JpegBitmapEncoder).QualityLevel = PictureConverter.ConvOptions.Quality;
                            enc.Frames.Add(BitmapFrame.Create(transformed));
                            enc.Save(trgstream);
                            break;
                        case ImageFormat.Bmp:
                            enc = new BmpBitmapEncoder();
                            if (PictureConverter.ConvOptions.PixelFormat != PixFormat.Nochange)
                                enc.Frames.Add(BitmapFrame.Create(ColorConv(transformed)));
                            else enc.Frames.Add(BitmapFrame.Create(transformed));
                            enc.Save(trgstream);
                            break;
                        case ImageFormat.Png:
                            enc = new PngBitmapEncoder();
                            if (PictureConverter.ConvOptions.PixelFormat != PixFormat.Nochange)
                                enc.Frames.Add(BitmapFrame.Create(ColorConv(transformed)));
                            else enc.Frames.Add(BitmapFrame.Create(transformed));
                            enc.Save(trgstream);
                            break;
                        case ImageFormat.Gif:
                            enc = new GifBitmapEncoder();
                            enc.Frames.Add(BitmapFrame.Create(transformed));
                            enc.Save(trgstream);
                            break;
                        case ImageFormat.Tiff:
                            enc = new TiffBitmapEncoder();
                            if (PictureConverter.ConvOptions.PixelFormat != PixFormat.Nochange)
                                enc.Frames.Add(BitmapFrame.Create(ColorConv(transformed)));
                            else enc.Frames.Add(BitmapFrame.Create(transformed));
                            enc.Save(trgstream);
                            break;
                        case ImageFormat.Wmp:
                            enc = new WmpBitmapEncoder();
                            (enc as WmpBitmapEncoder).QualityLevel = (byte)PictureConverter.ConvOptions.Quality;
                            enc.Frames.Add(BitmapFrame.Create(transformed));
                            enc.Save(trgstream);
                            break;
                    }
                    trgstream.Close();
                }
                catch (Exception)
                {
                    _error.Add(_sourcefiles[i]);
                }
                ++_cntr;
            }
            _isfinished = true;
        }
    }
}
