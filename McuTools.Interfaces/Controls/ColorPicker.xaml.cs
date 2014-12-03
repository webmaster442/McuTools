// Based on code from Page Brooks, Website: http://www.pagebrooks.com, RSS Feed: http://feeds.pagebrooks.com/pagebrooks
// updated and improved by Nokola (http://www.nokola.com):
// 1. added mouse capture capability
// 2. fixed bugs around edge cases (e.g. 0000000 and FFFFFFF colors)
// 3. added Alpha picker
// 4. Added ability to type in hex color
// 5. improved speed and layout

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace McuTools.Interfaces.Controls
{
    public partial class ColorPicker : UserControl
    {
        float m_selectedHue;
        double m_sampleX;
        double m_sampleY;
        private Color m_selectedColor = Colors.White;
        public event RoutedEventHandler ColorSelected;
        private UIElement _mouseCapture = null;

        public static DependencyProperty ColorpPoperty = DependencyProperty.Register("Color", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.White));

        public Color Color
        {
            get { return (Color)GetValue(ColorpPoperty); }
            set
            {
                m_selectedColor = value;
                UpdateOnColorChanged(m_selectedColor.A, m_selectedColor.R, m_selectedColor.G, m_selectedColor.B);
                HexValue.Text = ColorSpace.GetHexCode(value);
                SetValue(ColorpPoperty, value);
            }
        }

        //static WriteableBitmap _checkerboard = WriteableBitmapHelper.GetCheckerboard(20, 20);

        public ColorPicker()
        {
            InitializeComponent();
            rectHueMonitor.MouseLeftButtonDown += new MouseButtonEventHandler(rectHueMonitor_MouseLeftButtonDown);
            rectHueMonitor.MouseLeftButtonUp += new MouseButtonEventHandler(rectHueMonitor_MouseLeftButtonUp);
            rectHueMonitor.LostMouseCapture += new MouseEventHandler(rectHueMonitor_LostMouseCapture);
            rectHueMonitor.MouseMove += new MouseEventHandler(rectHueMonitor_MouseMove);

            rectSampleMonitor.MouseLeftButtonDown += new MouseButtonEventHandler(rectSampleMonitor_MouseLeftButtonDown);
            rectSampleMonitor.MouseLeftButtonUp += new MouseButtonEventHandler(rectSampleMonitor_MouseLeftButtonUp);
            rectSampleMonitor.LostMouseCapture += new MouseEventHandler(rectSampleMonitor_LostMouseCapture);
            rectSampleMonitor.MouseMove += new MouseEventHandler(rectSampleMonitor_MouseMove);

            ctlAlphaSelect.AlphaChanged += new AlphaSelectControl.AlphaChangedHandler(ctlAlphaSelect_AlphaChanged);
            m_selectedHue = 0;
            m_sampleX = 0;
            m_sampleY = 0;
            //imgCheckerBoard.Source = _checkerboard;
            this.LayoutUpdated += new EventHandler(ColorPicker_LayoutUpdated);
        }

        bool _firstTime = true;
        void ColorPicker_LayoutUpdated(object sender, EventArgs e)
        {
            if (_firstTime)
            {
                _firstTime = false;
                UpdateOnColorChanged(m_selectedColor.A, m_selectedColor.R, m_selectedColor.G, m_selectedColor.B);
            }
        }

        void rectSampleMonitor_LostMouseCapture(object sender, MouseEventArgs e)
        {
            _mouseCapture = null;
        }

        void rectHueMonitor_LostMouseCapture(object sender, MouseEventArgs e)
        {
            _mouseCapture = null;
        }

        void rectHueMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            rectHueMonitor.CaptureMouse();
            _mouseCapture = rectHueMonitor;
            int yPos = (int)e.GetPosition((UIElement)sender).Y;
            UpdateSelection(yPos);
        }

        void rectHueMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            rectHueMonitor.ReleaseMouseCapture();
        }

        void rectHueMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseCapture != rectHueMonitor) return;
            int yPos = (int)e.GetPosition((UIElement)sender).Y;
            if (yPos < 0) yPos = 0;
            if (yPos >= rectHueMonitor.ActualHeight) yPos = (int)rectHueMonitor.ActualHeight - 1;
            UpdateSelection(yPos);
        }

        void rectSampleMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            rectSampleMonitor.CaptureMouse();
            _mouseCapture = rectSampleMonitor;
            Point pos = e.GetPosition((UIElement)sender);
            m_sampleX = (int)pos.X;
            m_sampleY = (int)pos.Y;
            UpdateSample(m_sampleX, m_sampleY);
        }

        void rectSampleMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            rectSampleMonitor.ReleaseMouseCapture();
        }

        void rectSampleMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseCapture != rectSampleMonitor) return;
            Point pos = e.GetPosition((UIElement)sender);
            m_sampleX = (int)pos.X;
            m_sampleY = (int)pos.Y;
            if (m_sampleY < 0) m_sampleY = 0;
            if (m_sampleY > rectSampleMonitor.ActualHeight) m_sampleY = (int)rectSampleMonitor.ActualHeight;
            if (m_sampleX < 0) m_sampleX = 0;
            if (m_sampleX > rectSampleMonitor.ActualWidth) m_sampleX = (int)rectSampleMonitor.ActualWidth;
            UpdateSample(m_sampleX, m_sampleY);
        }

        private void UpdateSample(double xPos, double yPos)
        {

            SampleSelector.Margin = new Thickness(xPos - (SampleSelector.Height / 2), yPos - (SampleSelector.Height / 2), 0, 0);

            float yComponent = 1 - (float)(yPos / rectSample.ActualHeight);
            float xComponent = (float)(xPos / rectSample.ActualWidth);

            byte a = m_selectedColor.A;
            m_selectedColor = ColorSpace.ConvertHsvToRgb((float)m_selectedHue, xComponent, yComponent);
            m_selectedColor.A = a;
            ((SolidColorBrush)SelectedColor.Fill).Color = m_selectedColor;
            HexValue.Text = ColorSpace.GetHexCode(m_selectedColor);

            ctlAlphaSelect.DisplayColor = m_selectedColor;
            if (ColorSelected != null)
                ColorSelected(this, new RoutedEventArgs());

            SetValue(ColorpPoperty, m_selectedColor);
        }

        private void UpdateSelection(int yPos)
        {
            int huePos = (int)(yPos / rectHueMonitor.ActualHeight * 255);
            int gradientStops = 6;
            Color c = ColorSpace.GetColorFromPosition(huePos * gradientStops);
            rectSample.Fill = new SolidColorBrush(c);
            HueSelector.Margin = new Thickness(0, yPos - (HueSelector.ActualHeight / 2), 0, 0);
            m_selectedHue = (float)(yPos / rectHueMonitor.ActualHeight) * 360;
            UpdateSample(m_sampleX, m_sampleY);
        }

        private void ctlAlphaSelect_AlphaChanged(byte newAlpha)
        {
            m_selectedColor.A = newAlpha;
            HexValue.Text = ColorSpace.GetHexCode(m_selectedColor);
            ((SolidColorBrush)SelectedColor.Fill).Color = m_selectedColor;

            if (ColorSelected != null)
                ColorSelected(this, new RoutedEventArgs());

            SetValue(ColorpPoperty, m_selectedColor);
        }

        private void HexValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = HexValue.Text;
            if (text == ColorSpace.GetHexCode(m_selectedColor)) return;
            byte a, r, g, b;
            if (!GetArgb(text, out a, out r, out g, out b)) return; // invalid color

            UpdateOnColorChanged(a, r, g, b);
        }

        private void UpdateOnColorChanged(byte a, byte r, byte g, byte b)
        {
            m_selectedColor.A = a;
            m_selectedColor.R = r;
            m_selectedColor.G = g;
            m_selectedColor.B = b;

            double h, s, v;
            ColorSpace.ConvertRgbToHsv(r / 255.0, g / 255.0, b / 255.0, out h, out s, out v);

            // update selected color
            SelectedColor.Fill = new SolidColorBrush(m_selectedColor);

            // update Saturation and Value locator
            double xPos = s * rectSample.ActualWidth;
            double yPos = (1 - v) * rectSample.ActualHeight;
            m_sampleX = xPos;
            m_sampleY = yPos;
            SampleSelector.Margin = new Thickness(xPos - (SampleSelector.Height / 2), yPos - (SampleSelector.Height / 2), 0, 0);

            m_selectedHue = (float)h;
            h /= 360;
            const int gradientStops = 6;
            rectSample.Fill = new SolidColorBrush(ColorSpace.GetColorFromPosition(((int)(h * 255)) * gradientStops));

            // Update Hue locator
            HueSelector.Margin = new Thickness(0, (h * rectHueMonitor.ActualHeight) - (HueSelector.ActualHeight / 2), 0, 0);

            // update alpha selector
            ctlAlphaSelect.DisplayColor = m_selectedColor;

            if (ColorSelected != null)
                ColorSelected(this, new RoutedEventArgs());
        }

        private static bool GetArgb(string hexColor, out byte a, out byte r, out byte g, out byte b)
        {
            a = r = b = g = 0;
            if (hexColor.Length != 9) return false;
            string strA = hexColor.Substring(1, 2);
            string strR = hexColor.Substring(3, 2);
            string strG = hexColor.Substring(5, 2);
            string strB = hexColor.Substring(7, 2);

            if (!Byte.TryParse(strA, System.Globalization.NumberStyles.HexNumber, null, out a)) return false;
            if (!Byte.TryParse(strR, System.Globalization.NumberStyles.HexNumber, null, out r)) return false;
            if (!Byte.TryParse(strG, System.Globalization.NumberStyles.HexNumber, null, out g)) return false;
            if (!Byte.TryParse(strB, System.Globalization.NumberStyles.HexNumber, null, out b)) return false;
            return true;
        }
    }
}
