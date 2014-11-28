using McuTools.Interfaces.WPF;
using MTools.classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MTools.ToolOther
{
    /// <summary>
    /// Interaction logic for PortScanner.xaml
    /// </summary>
    public partial class PortScanner : UserControl
    {
        private AdapterList _adapters;
        private IPPortScanner _scanner;
        private Progress<int> Indicator;
        private CancellationTokenSource cts;
        private ObservableCollection<PortDataItem> _porttable;
        private List<int> _completed;
        private bool _loaded;

        public PortScanner()
        {
            InitializeComponent();
            Indicator = new Progress<int>(Report);
            _porttable = new ObservableCollection<PortDataItem>();
            _completed = new List<int>(255);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _scanner = new IPPortScanner();
            _scanner.Results = this.Results;
            PortDataItem.FillCollection(ref _porttable);
            PortTable.DataContext = _porttable;
            _scanner.Ports = _porttable;
            _loaded = true;
        }

        private void RangeFinder_Click(object sender, RoutedEventArgs e)
        {
            _adapters = new AdapterList();
            if (_adapters.ShowDialog() == true)
            {
                this.StartIP.IP = _adapters.Start;
                this.EndIP.IP = _adapters.End;
            }
        }

        private Task scann(IProgress<int> progress, CancellationToken ct, bool portscan)
        {
            _completed.Clear();
            return Task.Run(() => _scanner.Scann(progress, ct, portscan), ct);
        }

        private void Report(int value)
        {
            _completed.Add(value);
            PbProgress.Value = _completed.Count;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!this.StartIP.HasValidAdress || !this.EndIP.HasValidAdress) return;
                _scanner.IPStart = this.StartIP.IP;
                _scanner.IPEnd = this.EndIP.IP;
                PbProgress.Maximum = _scanner.GetCount();
                cts = new CancellationTokenSource();
                Tabs.SelectedIndex = 0;
                BtnStartStop.IsEnabled = false;
                await scann(Indicator, cts.Token, (bool)ChkScanPorts.IsChecked);
            }
            catch (Exception)
            {
                WpfHelpers.ExceptionDialog("Scann Canceled");
            }
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
                PbProgress.Value = 0;
                BtnStartStop.IsEnabled = true;
            }
        }
    }
}
