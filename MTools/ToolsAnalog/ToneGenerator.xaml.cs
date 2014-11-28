using McuTools.Interfaces;
using MTools.classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTools.ToolsAnalog
{
    /// <summary>
    /// Interaction logic for ToneGenerator.xaml
    /// </summary>
    [Loadable]
    public partial class ToneGenerator : UserControl, IDisposable
    {
        private Mixer _audiomixer;
        private bool _loaded;

        public ToneGenerator()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _audiomixer = new Mixer();
            _audiomixer.Oscillator.Add(Oscillator1);
            _audiomixer.Oscillator.Add(Oscillator2);
            _audiomixer.Oscillator.Add(Oscillator3);
            _audiomixer.Oscillator.Add(Oscillator4);
            _audiomixer.DrawArea = Visual;
            _loaded = true;
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            _audiomixer.Play();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _audiomixer.Stop();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Wav | *.wav";
            sfd.FilterIndex = 0;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _audiomixer.SavetoFile(sfd.FileName);
            }
        }

        protected virtual void Dispose(bool native)
        {
            if (_audiomixer != null)
            {
                _audiomixer.Stop();
                _audiomixer.Dispose();
                _audiomixer = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
