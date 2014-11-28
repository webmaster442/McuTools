using Sharpduino.Constants;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MTools.Controls
{
    public class PinChangedArgs: EventArgs
    {
        public PinChangedArgs() : base() { }
        public PinModes Pincap { get; set; }
        public int Value { get; set; }
    }

    public class PinModeChangedArgs : EventArgs
    {
        public PinModeChangedArgs() : base() { }
        public PinModes Pincap { get; set; }
    }

    /// <summary>
    /// Interaction logic for PinControl.xaml
    /// </summary>
    public partial class PinControl : UserControl
    {
        private PinModes _pincap;
        private bool _loaded;

        public static DependencyProperty IsPwmCapeableProperty = DependencyProperty.Register("IsPwmCapeable", typeof(bool), typeof(PinControl), new PropertyMetadata(false));
        public static DependencyProperty IsAnalogCapeableProperty = DependencyProperty.Register("IsAnalogCapeable", typeof(bool), typeof(PinControl), new PropertyMetadata(false));
        public static DependencyProperty AnalogMaxVoltsProperty = DependencyProperty.Register("AnalogMaxVolts", typeof(float), typeof(PinControl), new PropertyMetadata(5.0f));
        public static DependencyProperty AnalogMaxValueProperty = DependencyProperty.Register("AnalogMaxValue", typeof(int), typeof(PinControl), new PropertyMetadata(1023));
        public static DependencyProperty PinHeaderProperty = DependencyProperty.Register("PinHeader", typeof(string), typeof(PinControl));
        public static DependencyProperty PinIndexProperty = DependencyProperty.Register("PinIndex", typeof(int), typeof(PinControl));


        public delegate void PinChangedDelegate(object sender, PinChangedArgs e);
        public delegate void PinModeChangedDelegate(object sender, PinModeChangedArgs e);

        public event PinChangedDelegate PinChanged;
        public event PinModeChangedDelegate PinModeChanged;

        public PinModes PinMode
        {
            get { return _pincap; }
            set
            {
                _pincap = value;
                if (PinModeChanged != null) PinModeChanged(this, new PinModeChangedArgs() { Pincap = _pincap });
                switch (_pincap)
                {
                    case PinModes.Analog:
                        PinModeSelect.SelectedIndex = 0;
                        break;
                    case PinModes.Input:
                        PinModeSelect.SelectedIndex = 1;
                        break;
                    case PinModes.Output:
                        PinModeSelect.SelectedIndex = 2;
                        break;
                    case PinModes.PWM:
                        PinModeSelect.SelectedIndex = 3;
                        break;
                }
                Value = 0;
            }
        }

        public PinControl()
        {
            InitializeComponent();
            _pincap = PinModes.Output;
        }

        public bool IsPwmCapeable
        {
            get { return (bool)GetValue(IsPwmCapeableProperty); }
            set { SetValue(IsPwmCapeableProperty, value); }
        }

        public string PinHeader
        {
            get { return (string)GetValue(PinHeaderProperty); }
            set { SetValue(PinHeaderProperty, value); }
        }

        public bool IsAnalogCapeable
        {
            get { return (bool)GetValue(IsAnalogCapeableProperty); }
            set { SetValue(IsAnalogCapeableProperty, value); }
        }

        /// <summary>
        /// Analog maximum volts, default is 5
        /// </summary>
        public float AnalogMaxVolts
        {
            get { return (float)GetValue(AnalogMaxVoltsProperty); }
            set { SetValue(AnalogMaxVoltsProperty, value); }
        }

        /// <summary>
        /// Analog max value, default is 1024
        /// </summary>
        public int AnalogMaxValue
        {
            get { return (int)GetValue(AnalogMaxValueProperty); }
            set { SetValue(AnalogMaxValueProperty, value); }
        }

        public int PinIndex
        {
            get { return (int)GetValue(PinIndexProperty); }
            set { SetValue(PinIndexProperty, value); }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            if (OutTogle.IsChecked == true) OutTogle.Content = "1";
            else OutTogle.Content = "0";
            if (PinChanged == null) return;
            PinChanged(this, new PinChangedArgs() { Pincap = _pincap, Value = this.Value });
        }

        public int Value
        {
            get
            {
                if (!_loaded) return -1;
                switch (_pincap)
                {
                    case PinModes.Output:
                        if (OutTogle.IsChecked == true) return 1;
                        else return 0;
                    case PinModes.PWM:
                        return (int)PwmSlider.Value;
                    default:
                        return -1;
                }
            }
            set
            {
                if (!_loaded) return;
                switch (_pincap)
                {
                    case PinModes.Analog:
                        AnalogValue.Text = value.ToString();
                        double volts = (AnalogMaxVolts / AnalogMaxValue) * value;
                        AnalogVolts.Text = Math.Round(volts, 3).ToString();
                        break;
                    case PinModes.Input:
                        DigitalValue.Text = value.ToString();
                        if (value >= 1) DigitalDisplay.Fill = new SolidColorBrush(Colors.Green);
                        else DigitalDisplay.Fill = new SolidColorBrush(Colors.Red);
                        break;
                    case PinModes.Output:
                        if (value >= 1)
                        {
                            OutTogle.Content = "1";
                            OutTogle.IsChecked = true;
                        }
                        else
                        {
                            OutTogle.Content = "0";
                            OutTogle.IsChecked = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_loaded) return;
            if (PinModeChanged == null) return;
            switch (PinModeSelect.SelectedIndex)
            {
                case 0:
                    PinModeChanged(this, new PinModeChangedArgs() { Pincap = PinModes.Analog });
                    _pincap = PinModes.Analog;
                    break;
                case 1:
                    PinModeChanged(this, new PinModeChangedArgs() { Pincap = PinModes.Input });
                    _pincap = PinModes.Input;
                    break;
                case 2:
                    PinModeChanged(this, new PinModeChangedArgs() { Pincap = PinModes.Output });
                    _pincap = PinModes.Output;
                    break;
                case 3:
                    PinModeChanged(this, new PinModeChangedArgs() { Pincap = PinModes.PWM });
                    _pincap = PinModes.PWM;
                    break;
            }
        }

        private void PinC_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            if (!IsAnalogCapeable) PinModeSelect.SelectedIndex = 1;
            _loaded = true;
        }

        private void PwmSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            if (PinChanged == null) return;
            PinChanged(this, new PinChangedArgs() { Pincap = _pincap, Value = this.Value });
        }
    }
}
