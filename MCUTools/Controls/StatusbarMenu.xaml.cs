using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace McuTools.Controls
{
    /// <summary>
    /// Interaction logic for StatusbarMenu.xaml
    /// </summary>
    public partial class StatusBarMenu : UserControl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("user32")]
        private static extern void LockWorkStation();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xF170;

        public StatusBarMenu()
        {
            InitializeComponent();
            ListCloudProviders();
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

        private void ListCloudProviders()
        {
            CloudMenu.Items.Clear();
            MenuItem clouditem = new MenuItem();
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string dbPath = System.IO.Path.Combine(appDataPath, "Dropbox\\host.db"); string[] lines = System.IO.File.ReadAllLines(dbPath);
                byte[] dbBase64Text = Convert.FromBase64String(lines[1]);
                clouditem.Header = "Dropbox";
                clouditem.ToolTip = System.Text.ASCIIEncoding.ASCII.GetString(dbBase64Text);
                Image img = new Image();
                img.Width = 16;
                img.Height = 16;
                img.Source = new BitmapImage(new Uri("pack://application:,,,/images/statusbar/dropbox_copyrighted-32.png", UriKind.Absolute));
                clouditem.Click += clouditem_Click;
                clouditem.Icon = img;
                CloudMenu.Items.Add(clouditem);
            }
            catch (Exception) { }
            try
            {
                clouditem = new MenuItem();
                string dbFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google\\Drive\\sync_config.db");
                File.Copy(dbFilePath, "temp.db", true);
                string text = File.ReadAllText("temp.db", Encoding.ASCII);
                // The "29" refers to the end position of the keyword plus a few extra chars
                string trim = text.Substring(text.IndexOf("local_sync_root_pathvalue") + 29);
                // The "30" is the ASCII code for the record separator
                clouditem.Header = "Google drive";
                clouditem.ToolTip = trim.Substring(0, trim.IndexOf(char.ConvertFromUtf32(30)));
                Image img = new Image();
                img.Width = 16;
                img.Height = 16;
                img.Source = new BitmapImage(new Uri("pack://application:,,,/images/statusbar/google_drive_copyrighted-32.png", UriKind.Absolute));
                clouditem.Click += clouditem_Click;
                clouditem.Icon = img;
                CloudMenu.Items.Add(clouditem);

            }
            catch (Exception) { }
            try
            {
                clouditem = new MenuItem();
                clouditem.Header = "One Drive";
                clouditem.ToolTip = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\SkyDrive", "UserFolder", null).ToString();
                Image img = new Image();
                img.Width = 16;
                img.Height = 16;
                img.Source = new BitmapImage(new Uri("pack://application:,,,/images/statusbar/skydrive_copyrighted-32.png", UriKind.Absolute));
                clouditem.Click += clouditem_Click;
                clouditem.Icon = img;
                CloudMenu.Items.Add(clouditem);

            }
            catch (Exception) { }
        }

        private void Drives_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            Local.Items.Clear();
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
                Local.Items.Add(m);
            }
        }

        private void clouditem_Click(object sender, RoutedEventArgs e)
        {
            string path = (sender as MenuItem).ToolTip.ToString();
            Process.Start("explorer.exe", "/e /root," + path);
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
