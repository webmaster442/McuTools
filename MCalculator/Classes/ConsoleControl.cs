using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MCalculator.Classes
{

    internal interface IConsole
    {
        void Clear();
        void Write(string Text);
        void WriteLine(string Text);
        void WriteError(Exception ex);

        void BufferedWrite(string Text);
        void WriteBuffer();

        void WriteImage(Uri location);

        void LoadRtf(string path);
        void AppendRtf(string path);
        void SaveRtf(string path);
        void SaveTxt(string path);

        Color DefaultFontColor { get; set; }
        Color ErrorFontColor { get; set; }
        Color OkFontColor { get; set; }
        Color WarningFontColor { get; set; }
        Color BackgroundColor { get; set; }
        Color CurrentForeground { get; set; }
    }

    internal class ConsoleControl : RichTextBox, IConsole
    {

        #region Thread safe Dependency properties
        public static readonly DependencyProperty DefaultFontColorProperty = DependencyProperty.Register("DefaultFontColor", typeof(Color), typeof(ConsoleControl));
        public static readonly DependencyProperty ErrorFontColorProperty = DependencyProperty.Register("ErrorFontColor", typeof(Color), typeof(ConsoleControl));
        public static readonly DependencyProperty OkFontColorProperty = DependencyProperty.Register("OkFontColor", typeof(Color), typeof(ConsoleControl));
        public static readonly DependencyProperty WarningFontColorProperty = DependencyProperty.Register("WarningFontColor", typeof(Color), typeof(ConsoleControl));
        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(ConsoleControl));
        public static readonly DependencyProperty CurrentForegroundProperty = DependencyProperty.Register("CurrentForeground", typeof(Color), typeof(ConsoleControl));

        public Color DefaultFontColor
        {
            get
            {
                try
                {
                    return (Color)this.Dispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate { return GetValue(DefaultFontColorProperty); }, DefaultFontColorProperty);
                }
                catch
                {
                    return (Color)DefaultFontColorProperty.DefaultMetadata.DefaultValue;
                }
            }
            set
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { SetValue(DefaultFontColorProperty, value); }, value);
            }
        }

        public Color ErrorFontColor
        {
            get
            {
                try
                {
                    return (Color)this.Dispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate { return GetValue(ErrorFontColorProperty); }, ErrorFontColorProperty);
                }
                catch
                {
                    return (Color)ErrorFontColorProperty.DefaultMetadata.DefaultValue;
                }
            }
            set
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { SetValue(ErrorFontColorProperty, value); }, value);
            }
        }

        public Color OkFontColor
        {
            get
            {
                try
                {
                    return (Color)this.Dispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate { return GetValue(OkFontColorProperty); }, OkFontColorProperty);
                }
                catch
                {
                    return (Color)OkFontColorProperty.DefaultMetadata.DefaultValue;
                }
            }
            set
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { SetValue(OkFontColorProperty, value); }, value);
            }
        }

        public Color WarningFontColor
        {
            get
            {
                try
                {
                    return (Color)this.Dispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate { return GetValue(WarningFontColorProperty); }, WarningFontColorProperty);
                }
                catch
                {
                    return (Color)WarningFontColorProperty.DefaultMetadata.DefaultValue;
                }
            }
            set
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { SetValue(WarningFontColorProperty, value); }, value);
            }
        }

        public Color BackgroundColor
        {
            get
            {
                try
                {
                    return (Color)this.Dispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate { return GetValue(BackgroundColorProperty); }, BackgroundColorProperty);
                }
                catch
                {
                    return (Color)BackgroundColorProperty.DefaultMetadata.DefaultValue;
                }
            }
            set
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { SetValue(BackgroundColorProperty, value); }, value);
            }
        }

        public Color CurrentForeground
        {
            get
            {
                try
                {
                    return (Color)this.Dispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate { return GetValue(CurrentForegroundProperty); }, CurrentForegroundProperty);
                }
                catch
                {
                    return (Color)CurrentForegroundProperty.DefaultMetadata.DefaultValue;
                }
            }
            set
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { SetValue(CurrentForegroundProperty, value); }, value);
            }
        }
        #endregion

        delegate void NullDelegate();
        delegate void StringDelegate(string str);
        delegate void UriDelegate(Uri uri);
        delegate void BitmapSourceDelegae(WriteableBitmap src);
        StringBuilder buffer;
        int _buffcnt;


        public ConsoleControl()
            : base()
        {
            Init();
        }

        private void Init()
        {
            this.IsReadOnly = true;
            this.AcceptsTab = true;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            buffer = new StringBuilder();
            _buffcnt = 0;
        }

        private void AddRtf(string file, bool apend)
        {
            FileStream fs = File.OpenRead(file);
            TextRange tr;
            if (apend) tr = new TextRange(Document.ContentEnd, Document.ContentEnd);
            else tr = new TextRange(Document.ContentStart, Document.ContentEnd);
            tr.Load(fs, DataFormats.Rtf);
            fs.Close();
        }

        private void Save(string target, string format)
        {
            FileStream fs = File.Create(target);
            TextRange tr = new TextRange(Document.ContentStart, Document.ContentEnd);
            tr.Save(fs, format);
            fs.Close();
        }

        public void Clear()
        {
            if (Dispatcher.CheckAccess()) this.Document.Blocks.Clear();
            else Dispatcher.Invoke(new NullDelegate(Clear), null);
        }

        public void Write(string Text)
        {
            if (Dispatcher.CheckAccess())
            {
                TextRange tr = new TextRange(Document.ContentEnd, Document.ContentEnd);
                tr.Text = Text;
                tr.ApplyPropertyValue(Paragraph.MarginProperty, new Thickness(0));
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(CurrentForeground));
                this.ScrollToEnd();
            }
            else Dispatcher.Invoke(new StringDelegate(Write), Text);
        }

        public void WriteError(Exception ex)
        {
            CurrentForeground = ErrorFontColor;
            this.WriteLine("Error: " + ex.Message);
            CurrentForeground = DefaultFontColor;
        }

        public void BufferedWrite(string Text)
        {
            if (Dispatcher.CheckAccess())
            {
                buffer.Append(Text);
                ++_buffcnt;
                if (_buffcnt > 25)
                {
                    this.CurrentForeground = this.OkFontColor;
                    this.Write(buffer.ToString());
                    this.CurrentForeground = this.DefaultFontColor;
                    buffer.Clear();
                    _buffcnt = 0;
                }
            }
            else Dispatcher.Invoke(new StringDelegate(BufferedWrite), Text);
        }

        public void WriteBuffer()
        {
            if (_buffcnt == 0) return;
            if (Dispatcher.CheckAccess())
            {
                this.CurrentForeground = this.OkFontColor;
                this.Write(buffer.ToString());
                this.CurrentForeground = this.DefaultFontColor;
                buffer.Clear();
                _buffcnt = 0;
            }
            else Dispatcher.Invoke(new NullDelegate(WriteBuffer), null);
        }

        public void WriteLine(string Text)
        {
            if (Dispatcher.CheckAccess()) this.Write(Text + "\n");
            else Dispatcher.Invoke(new StringDelegate(WriteLine), Text);
        }

        public void WriteImage(Uri location)
        {
            if (Dispatcher.CheckAccess())
            {
                Paragraph para = new Paragraph();
                Image Img = new Image();
                BitmapImage i = new BitmapImage(location);
                Img.Width = i.PixelWidth;
                Img.Height = i.PixelHeight;
                Img.Source = i;
                para.Margin = new Thickness(0);
                para.Inlines.Add(Img);
                para.Inlines.Add("\n");
                Document.Blocks.Add(para);
                this.ScrollToEnd();
            }
            else Dispatcher.Invoke(new UriDelegate(WriteImage), location);
        }

        public void LoadRtf(string path)
        {
            if (Dispatcher.CheckAccess()) AddRtf(path, false);
            else Dispatcher.Invoke(new StringDelegate(LoadRtf), path);
        }

        public void AppendRtf(string path)
        {
            if (Dispatcher.CheckAccess()) AddRtf(path, true);
            else Dispatcher.Invoke(new StringDelegate(AppendRtf), path);
        }

        public void SaveRtf(string path)
        {
            if (Dispatcher.CheckAccess()) Save(path, DataFormats.Rtf);
            else Dispatcher.Invoke(new StringDelegate(SaveRtf), path);
        }
        public void SaveTxt(string path)
        {
            if (Dispatcher.CheckAccess()) Save(path, DataFormats.Text);
            else Dispatcher.Invoke(new StringDelegate(SaveTxt), path);
        }
    }
}
