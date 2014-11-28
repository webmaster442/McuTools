using CatenaLogic.Windows.Presentation.WebcamPlayer;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MMediaTools.Tools
{
    /// <summary>
    /// Interaction logic for UsbVideo.xaml
    /// </summary>
    public partial class UsbVideo : UserControl, IDisposable
    {
        private bool _loaded;

        public UsbVideo()
        {
            _loaded = false;
            InitializeComponent();
            foreach (var filter in CapDevice.DeviceMonikers)
            {
                Devices.Items.Add(filter.Name);
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton s = (RadioButton)sender;
            switch (s.Content as string)
            {
                case "None":
                    PostProcEff.PreProcess = 0;
                    break;
                case "B and W":
                    PostProcEff.PreProcess = 1;
                    break;
                case "Invert":
                    PostProcEff.PreProcess = 2;
                    break;
                case "Inv. B and W":
                    PostProcEff.PreProcess = 3;
                    break;
                case "Color overlay":
                    PostProcEff.PreProcess = 4;
                    break;
            }
        }

        private void FlipX_Click(object sender, RoutedEventArgs e)
        {
            ScaleTrans.ScaleX *= -1;
        }

        private void FlipY_Click(object sender, RoutedEventArgs e)
        {
            ScaleTrans.ScaleY *= -1;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var res = (from i in CapDevice.DeviceMonikers where i.Name == Devices.SelectedItem.ToString() select i.MonikerString).FirstOrDefault();
            WPlayer.Device = new CapDevice(res);
            WPlayer.Device.Start();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }

        public void Dispose()
        {
            if (WPlayer != null)
            {
                if (WPlayer.Device != null)
                {
                    WPlayer.Device.Stop();
                    WPlayer.Device.Dispose();
                    WPlayer.Dispose();
                }
                WPlayer.Device = null;
                WPlayer = null;
                GC.SuppressFinalize(this);
            }
        }

        private void Sharpening_ValueChanged_1(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            if (WPlayer.Device != null)
            {
                if (WPlayer.Device.IsRunning) PostProcEff.InputSize = new Size(WPlayer.CurrentBitmap.PixelWidth, WPlayer.CurrentBitmap.PixelHeight);
            }
        }

        private void Pos_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            PostProcEff.CenterPoint = new Point(PosX.Value, PosY.Value);
        }
    }
}
