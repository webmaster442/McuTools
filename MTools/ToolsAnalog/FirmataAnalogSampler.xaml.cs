using McuTools.Interfaces.WPF;
using MTools.Controls;
using Sharpduino;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MTools.ToolsAnalog
{
    /// <summary>
    /// Interaction logic for FirmataAnalogSampler.xaml
    /// </summary>
    public partial class FirmataAnalogSampler : UserControl, IDisposable
    {
        private Arduino _arduino;
        private DispatcherTimer _timer;
        private bool _loaded;
        private IEnumerable<AnalogSampler> _controls;
        private Polyline[] _lines;
        private int _samplecount;

        public FirmataAnalogSampler()
        {
            InitializeComponent();
            _lines = new Polyline[4];
            Color[] _cols = new Color[] { Colors.Red, Colors.Blue, Colors.Green, Colors.Orange };
            for (int i = 0; i < 4; i++)
            {
                _lines[i] = new Polyline();
                _lines[i].Stroke = new SolidColorBrush(_cols[i]);
                _lines[i].StrokeThickness = 2;
                GraphArea.Children.Add(_lines[i]);
            }
        }

        private void BtnOpenClose_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            bool error = false;
            if (BtnOpenClose.IsChecked == true)
            {
                error = false;
                try
                {
                    _arduino = new Arduino(SerialPorts.SelectedItem.ToString());
                    if (BoardUno.IsChecked == true) foreach (var c in _controls) c.MaxChanels = 5;
                    else foreach (var c in _controls) c.MaxChanels = 15;
                    _timer.Interval = TimeSpan.FromMilliseconds(1000 / SamplesSec.Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Communication Error. Please Reset arduino.\r\nError description: " + ex.Message, "Communication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    error = true;
                    _timer.IsEnabled = false;
                    BtnOpenClose.IsChecked = false;
                    BtnOpenClose.Content = "Open";
                }
                if (!error)
                {
                    BtnOpenClose.Content = "Close";
                    _timer.IsEnabled = true;
                }
                else BtnOpenClose.IsChecked = false;
            }
            else
            {
                BtnOpenClose.Content = "Open";
                _timer.IsEnabled = false;
                try
                {
                    if (_arduino != null)
                    {
                        _arduino.Dispose();
                        _arduino = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Communication Error. Please Reset arduino.\r\nError description: " + ex.Message, "Communication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    BtnOpenClose.IsChecked = false;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            SerialPorts.Items.Clear();
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            foreach (var p in ports) SerialPorts.Items.Add(p);
            SerialPorts.SelectedIndex = 0;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
            Button_Click(this, e);
            _controls = WpfHelpers.FindChildren<AnalogSampler>(this);
            _timer = new DispatcherTimer();
            _timer.IsEnabled = false;
            _timer.Tick += _timer_Tick;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            _timer.IsEnabled = false;
            if (_arduino != null)
            {
                _arduino.Dispose();
                _arduino = null;
            }
        }

        protected virtual void Dispose(bool native)
        {
            _timer.IsEnabled = false;
            if (_arduino != null)
            {
                _arduino.Dispose();
                _arduino = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            double canvasHeight = GraphArea.ActualHeight;
            double canvasWidth = GraphArea.ActualWidth;
            double xScale = canvasWidth / 200;
            double yScale = canvasHeight / 1023;
            int i = 0;
            short sample = 0;

            foreach (var c in _controls)
            {
                if (c.AcceptsData)
                {
                    try { sample = (short)_arduino.ReadAnalog(c.SelectedChanel); }
                    catch (IOException) { }

                    c.AddItem(sample);
                    if (Tabs.SelectedIndex == 1)
                    {
                        if (_lines[i].Points.Count < 200) _lines[i].Points.Add(new Point(_lines[i].Points.Count * xScale, canvasHeight - sample * yScale));
                        else _lines[i].Points.Clear();
                        string item = c.SampleToString(sample);
                        switch (i)
                        {
                            case 0:
                                L0.Text = item;
                                break;
                            case 1:
                                L1.Text = item;
                                break;
                            case 2:
                                L2.Text = item;
                                break;
                            case 3:
                                L3.Text = item;
                                break;
                        }
                    }
                }
                i++;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.SelectedIndex == 1)
            {
                for (int i = 0; i < 4; i++) _lines[i].Points.Clear();
                L1.Text = "";
                L2.Text = "";
                L3.Text = "";
                L0.Text = "";
            }
        }
    }
}
