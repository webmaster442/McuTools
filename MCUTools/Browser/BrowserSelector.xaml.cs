using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace McuTools.Browser
{
    /// <summary>
    /// Interaction logic for BrowserSelector.xaml
    /// </summary>
    public partial class BrowserSelector : UserControl
    {
        public BrowserSelector()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler OpenActionComplete;

        public string LinkToOpen
        {
            get;
            set;
        }

        private void Browser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LinkToOpen)) return;
            Button s = (Button)sender;
            switch (s.Name)
            {
                case "McuBrowser":
                    Process.Start(BrowserSelect.McuBrowser.Path, LinkToOpen);
                    break;
                case "Firefox":
                    Process.Start(BrowserSelect.Firefox.Path, LinkToOpen);
                    break;
                case "Iexplore":
                    Process.Start(BrowserSelect.Iexplore.Path, LinkToOpen);
                    break;
                case "CopyLink":
                    Clipboard.SetText(LinkToOpen);
                    break;
                case "Chrome":
                    if (BrowserSelect.ChromeLocal.IsEnabled) Process.Start(BrowserSelect.ChromeLocal.Path, LinkToOpen);
                    else Process.Start(BrowserSelect.ChromeGlobal.Path, LinkToOpen);
                    break;
            }
            if (OpenActionComplete != null) OpenActionComplete(this, new RoutedEventArgs());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!BrowserSelect.Firefox.IsEnabled) Firefox.Visibility = System.Windows.Visibility.Collapsed;
            if (!BrowserSelect.ChromeLocal.IsEnabled && !BrowserSelect.ChromeGlobal.IsEnabled) Chrome.Visibility = System.Windows.Visibility.Collapsed;
            if (!BrowserSelect.Iexplore.IsEnabled) Iexplore.Visibility = System.Windows.Visibility.Collapsed;
            if (!BrowserSelect.McuBrowser.IsEnabled) McuBrowser.Visibility = System.Windows.Visibility.Collapsed;
            LinkDisplay.Text = LinkToOpen;
        }
    }
}
