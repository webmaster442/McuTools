using MTools.classes;
using System.Windows.Controls;
using System.IO;
using System;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for AnalogSampler.xaml
    /// </summary>
    public partial class AnalogSampler : UserControl
    {
        private int _chmax;
        private StatSampling _sampler;
         
        public AnalogSampler()
        {
            InitializeComponent();
            _sampler = new StatSampling();
            this.DataContext = _sampler;
        }

        public void AddItem(short item)
        {
            string itm = _sampler.Add(item);
            LbDisplay.Items.Add(itm);
            var selected = LbDisplay.Items[LbDisplay.Items.Count - 1];
            LbDisplay.ScrollIntoView(selected);
        }

        public string SampleToString(short item)
        {
            return _sampler.MapToVolts(item);
        }

        public int MaxChanels
        {
            get { return _chmax; }
            set
            {
                _chmax = value;
                ChanelSelector.Items.Clear();
                for (int i = 0; i < _chmax; i++)
                {
                    ChanelSelector.Items.Add(i);
                }
                ChanelSelector.SelectedIndex = 0;
            }
        }

        public bool AcceptsData
        {
            get { return (bool)BtnStartStop.IsChecked; }
        }

        public int SelectedChanel
        {
            get { return ChanelSelector.SelectedIndex; }
        }

        private void BtnStartStop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _sampler.VoltsPerItem = MaxVolts.Value / 1023.0;
            if (BtnStartStop.IsChecked == true) BtnStartStop.Content = "Stop";
            else BtnStartStop.Content = "Start";
        }

        private void BtnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _sampler.Clear();
            LbDisplay.Items.Clear();
        }

        private void BtnExport_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Excel CSV | *.csv";
            sfd.AddExtension = true;
            sfd.DefaultExt = "*.csv";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (TextWriter tx = File.CreateText(sfd.FileName))
                {
                    foreach (var i in _sampler) tx.WriteLine(i);
                }
            }
        }
    }
}
