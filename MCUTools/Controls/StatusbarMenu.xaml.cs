using McuTools.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace McuTools.Controls
{
    /// <summary>
    /// Interaction logic for StatusbarMenu.xaml
    /// </summary>
    public partial class StatusbarMenu : UserControl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("user32")]
        private static extern void LockWorkStation();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xF170;

        public StatusbarMenu()
        {
            InitializeComponent();
        }

        private void Power_Click(object sender, RoutedEventArgs e)
        {
            MenuItem send = (MenuItem)sender;
            var res = MessageBox.Show(string.Format("Confirm action: {0}\r\nAre you sure?", send.Header.ToString()), "Action confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.No) return;
            switch (send.Header.ToString())
            {
                case "Sleep":
                    System.Windows.Forms.Application.SetSuspendState(System.Windows.Forms.PowerState.Suspend, true, false);
                    break;
                case "Hibernate":
                    System.Windows.Forms.Application.SetSuspendState(System.Windows.Forms.PowerState.Hibernate, true, true);
                    break;
                case "Restart":
                    Process.Start("shutdown", "/r /t 0");
                    break;
                case "Shutdown":
                    Process.Start("shutdown", "/s /t 0");
                    break;
                case "Lock":
                    LockWorkStation();
                    break;
                case "Turn Display off":
                    Thread.Sleep(500);
                    SendMessage(App.MainWindowHandle, WM_SYSCOMMAND, new IntPtr(SC_MONITORPOWER), new IntPtr(2));
                    break;
            }
        }

        private void MyComputer_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "/n /root, ::{20D04FE0-3AEA-1069-A2D8-08002B30309D}");
        }

        private void DeviceMan_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("devmgmt.msc");
        }

        private void cpanel_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("control.exe");
        }

        private void Drives_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            Drives.Items.Clear();
            string[] drives = Environment.GetLogicalDrives();
            DriveInfo di;
            string label;
            foreach (var drive in drives)
            {
                di = new DriveInfo(drive);
                if (!di.IsReady) continue;
                try { label = di.VolumeLabel; }
                catch (IOException) { label = ""; }
                MenuItem m = new MenuItem();
                Image img = new Image();
                img.Width = 16;
                img.Height = 16;
                m.Header = string.Format("{0} - {1}", di.Name, label);
                m.Click += m_Click;
                switch (di.DriveType)
                {
                    case DriveType.CDRom:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/images/statusbar/cd-32.png", UriKind.Absolute));
                        break;
                    case DriveType.Fixed:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/images/statusbar/hdd-32.png", UriKind.Absolute));
                        break;
                    case DriveType.Network:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/images/statusbar/cloud_storage-32.png", UriKind.Absolute));
                        break;
                    case DriveType.Removable:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/images/statusbar/usb_on-32.png", UriKind.Absolute));
                        break;
                    default:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/images/statusbar/hdd-32.png", UriKind.Absolute));
                        break;
                }
                m.Icon = img;
                Drives.Items.Add(m);
            }
        }

        private void m_Click(object sender, RoutedEventArgs e)
        {
            MenuItem s = (MenuItem)sender;
            string[] p = s.Header.ToString().Split('-');
            if (p.Length < 1) return;
            Process.Start("explorer.exe", "/e /root,"+p[0].Trim());
        }

        private void Console_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
