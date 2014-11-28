using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace McuTools.Interfaces.Controls
{
    /// <summary>
    /// Interaction logic for ShaderTabPopoutWin.xaml
    /// </summary>
    public partial class ShaderTabPopoutWin : Window
    {
        private WindowChrome _wchrome;
        private UIElement _tabcontent;

        public static DependencyProperty GlassTitlePorperty = DependencyProperty.Register("GlassTitle", typeof(string), typeof(ShaderTabPopoutWin));

        public string GlassTitle
        {
            get { return (string)GetValue(GlassTitlePorperty); }
            set { SetValue(GlassTitlePorperty, value); }
        }

        public UIElement TabContent
        {
            get { return _tabcontent; }
            set
            {
                _tabcontent = value;
                if (value == null) return;
                Child.Children.Add(_tabcontent);
            }
        }

        public ShaderTabPopoutWin()
        {
            InitializeComponent();
            _wchrome = new WindowChrome();
            _wchrome.GlassFrameThickness = new Thickness(7, 40, 7, 7);
            _wchrome.CaptionHeight = 0;
            _wchrome.UseAeroCaptionButtons = true;
            _wchrome.ResizeBorderThickness = new Thickness(4);
            _wchrome.NonClientFrameEdges = NonClientFrameEdges.Left | NonClientFrameEdges.Right | NonClientFrameEdges.Bottom;
            WindowChrome.SetWindowChrome(this, _wchrome);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
            if ((p.X > this.Width - _wchrome.GlassFrameThickness.Right) || (p.X < _wchrome.GlassFrameThickness.Left) || (p.Y < _wchrome.GlassFrameThickness.Top) || (p.Y > this.Height - _wchrome.GlassFrameThickness.Bottom))
            {
                this.DragMove();
                e.Handled = true;
            }
        }

        private void WinClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TabContent != null)
            {
                if (TabContent is IDisposable) (TabContent as IDisposable).Dispose();
                TabContent = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void WinStayTop_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
            WinStayTop.Header = string.Format("Always top: {0}", this.Topmost);
        }
    }
}
