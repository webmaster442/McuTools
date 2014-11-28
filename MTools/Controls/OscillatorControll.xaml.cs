using MTools.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for OscillatorControll.xaml
    /// </summary>
    public partial class OscillatorControll : UserControl, IOscillator
    {
        private bool _running;
        private WaveType _type;

        public OscillatorControll()
        {
            InitializeComponent();
            _running = false;
            _type = WaveType.Sinus;
        }

        public WaveType Wavetype
        {
            get { return _type; }
        }

        public double OscFrequency
        {
            get { return Frequency.Value; }
        }

        public double OscAmplitude
        {
            get { return (Amplitude.Value / 100 ) * 32760; }
        }

        public bool Running
        {
            get { return _running; }
            set
            {
                foreach (FrameworkElement control in Layout.Children) control.IsEnabled = !value;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            string s = rb.Content.ToString();

            switch (s)
            {
                case "Off":
                    _type = WaveType.None;
                    break;
                case "Sinus":
                    _type = WaveType.Sinus;
                    break;
                case "Square":
                    _type = WaveType.Square;
                    break;
                case "Saw":
                    _type = WaveType.Saw;
                    break;
                case "Noise":
                    _type = WaveType.Noise;
                    break;
            }

        }
    }
}
