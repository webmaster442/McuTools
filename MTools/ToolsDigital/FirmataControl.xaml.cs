using McuTools.Interfaces.WPF;
using MTools.Controls;
using Sharpduino;
using Sharpduino.Constants;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MTools.ToolsDigital
{
    /// <summary>
    /// Interaction logic for FirmataControl.xaml
    /// </summary>
    public partial class FirmataControl : UserControl, IDisposable
    {
        private bool _loaded;
        private Arduino _arduino;
        private DispatcherTimer _timer;
        private IEnumerable<PinControl> _pins;

        public FirmataControl()
        {
            _loaded = false;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            BtnRescanPorts_Click(sender, e);
            _timer = new DispatcherTimer();
            _timer.IsEnabled = false;
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += _timer_Tick;
            _loaded = true;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_arduino == null) return;
            if (_pins == null) return;
            foreach (var c in _pins)
            {
                switch (c.PinMode)
                {
                    case PinModes.Analog:
                        switch (Models.SelectedIndex)
                        {
                            case 0:
                                c.Value = (int)_arduino.ReadAnalog(c.PinIndex - 14);
                                break;
                            case 1:
                                c.Value = (int)_arduino.ReadAnalog(c.PinIndex - 54);
                                break;
                        }
                        break;
                    case PinModes.Input:
                        c.Value = _arduino.ReadDigital(c.PinIndex);
                        break;
                }
            }
        }

        private void BtnRescanPorts_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            SerialPorts.Items.Clear();
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            foreach (var p in ports) SerialPorts.Items.Add(p);
            SerialPorts.SelectedIndex = 0;
        }

        private void DoLock(bool Lock = true)
        {
            for (int i = 0; i < Models.Items.Count; i++)
            {
                TabItem itm = (TabItem)Models.Items[i];
                if (i != Models.SelectedIndex) itm.IsEnabled = !Lock;
            }
        }

        private void SetPinModes()
        {
            _pins = WpfHelpers.FindChildren<PinControl>((TabItem)Models.SelectedItem);
            foreach (var c in _pins)
            {
                c.PinMode = PinModes.Input;
                Thread.Sleep(15);
                c.PinMode = PinModes.Output;
            }
        }

        private void BtnPortOpenClose_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            bool error = false;
            if (BtnPortOpenClose.IsChecked == true)
            {
                error = false;
                try
                {
                    _arduino = new Arduino(SerialPorts.SelectedItem.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Communication Error. Please Reset arduino.\r\nError description: " + ex.Message, "Communication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    error = true;
                }
                if (!error)
                {
                    BtnPortOpenClose.Content = "Close";
                    DoLock();
                    SetPinModes();
                    _timer.IsEnabled = true;
                }
                else BtnPortOpenClose.IsChecked = false;
            }
            else
            {
                error = false;
                BtnPortOpenClose.Content = "Open";
                DoLock(false);
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
                }
            }
        }

        private void PinControl_PinModeChanged(object sender, PinModeChangedArgs e)
        {
            PinControl s = (PinControl)sender;
            try
            {
                if (!_loaded || _arduino == null) return;
                _arduino.SetPinMode(s.PinIndex, e.Pincap);
                s.Value = 0;
            }
            catch (Exception)
            {
                MessageBox.Show("Error setting/geting pin state.\r\nMaybe incorrect modell selected", "I/O Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PinControl_PinChanged(object sender, PinChangedArgs e)
        {
            PinControl s = (PinControl)sender;
            try
            {
                int pin = s.PinIndex;
                if (!_loaded || _arduino == null) return;
                switch (e.Pincap)
                {
                    case PinModes.Output:
                        if (e.Value >= 1) _arduino.SetDO(pin, true);
                        else _arduino.SetDO(pin, false);
                        break;
                    case PinModes.PWM:
                        _arduino.SetPWM(pin, e.Value);
                        break;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error setting/geting pin state.\r\nMaybe incorrect modell selected", "I/O Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
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
    }
}
