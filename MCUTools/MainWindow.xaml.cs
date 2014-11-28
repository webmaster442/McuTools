using GoogleAnalyticsTracker.Simple;
using McuTools.Browser;
using McuTools.Classes;
using McuTools.Interfaces;
using McuTools.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;

namespace McuTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IToolHost, IDisposable
    {

        private MultimonitorHelper _monitors;
        private WindowChrome _wchrome;
        private SmartHomeScreen _smarthome;
        private TrayIcon _tray;
        private string _trackbaseuri;

        public MainWindow()
        {
            
            InitializeComponent();
            App._Tools = new List<Tool>();
            App._ExtTools = new List<ExternalTool>();
            App._WebTools = new List<WebTool>();
            App._Popups = new List<PopupTool>();
            App._Config = new UserConfiguration();
            App._Config.Load();
            _tray = new TrayIcon();
            ToolBase.Host = this;

            _wchrome = new WindowChrome();
            _wchrome.GlassFrameThickness = new Thickness(7, 75, 7, 7);
            _wchrome.CaptionHeight = 0;
            _wchrome.UseAeroCaptionButtons = true;
            _wchrome.ResizeBorderThickness = new Thickness(4);
            _wchrome.NonClientFrameEdges = NonClientFrameEdges.Left | NonClientFrameEdges.Right | NonClientFrameEdges.Bottom;
            WindowChrome.SetWindowChrome(this, _wchrome);
            this.Visibility = System.Windows.Visibility.Collapsed;
            this._trackbaseuri = this.GetType().Assembly.GetName().Version.ToString();
            VersionInfo.Text = string.Format("MCUTools v. {0}", _trackbaseuri);
        }

        private async void ReportAnalytics(string title, string uri)
        {
            if (Settings.Default.Tracking != 1) return;
            try
            {
                using (SimpleTracker tracker = new SimpleTracker("UA-50763481-1", "mcutools.com"))
                {
                    await tracker.TrackPageViewAsync(title, _trackbaseuri + @"/" + uri);
                }
            }
            catch (Exception) { }
        }

        #region Interface

        public void SetTitle(string s)
        {
            Tabs.SetCurrentTitle(s);
        }

        public void OpenUrl(string url, string description = null)
        {
            if (!url.StartsWith("asset://"))
            {
                var selector = new BrowserSelector();
                selector.OpenActionComplete += delegate
                {
                    Tabs.ClosePopup();
                };
                selector.LinkToOpen = url;
                Tabs.OpenPopup(selector, "Browser selector");
            }
            else
            {
                Browser.BrowserControl web = new Browser.BrowserControl();
                ShowControlAsTab(web, description);
                web.NavigateTo(url);
            }
        }

        public void SetProgressValue(TaskbarItemProgressState state, double percent)
        {
            TaskbarItemInfo.ProgressState = state;
            TaskbarItemInfo.ProgressValue = percent;
        }

        public void OpenUserControlAsPopup(UserControl control, string Header)
        {
            Tabs.OpenPopup(control, Header);
        }

        public string ReadSubconfKey(string key)
        {
            return App._Config.GetSubConfig(key);
        }

        public void WriteSubconfKey(string key, string value)
        {
            App._Config.SetSubConfig(key, value);
        }

        public void DeleteSubconfKey(string key)
        {
            App._Config.DeleteSubConfig(key);
        }

        protected virtual void Dispose(bool native)
        {
            if (_tray != null)
            {
                _tray.Dispose();
                _tray = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Tool Loading
        
        private Assembly LoadModule(string path, bool network = false)
        {
            if (!network) return Assembly.LoadFile(path);
            else
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                return Assembly.Load(bytes);
            }
        }
        
        private void LoadTools()
        {
            string AppDir = AppDomain.CurrentDomain.BaseDirectory;
            bool isnetwork = false;
            DriveInfo di = new DriveInfo(AppDir);
            if (di.DriveType == DriveType.Network) isnetwork = true; 
            string[] tools = Directory.GetFiles(AppDir, "*.Tool.dll");
            object l = new object();
            SetProgressValue(TaskbarItemProgressState.Indeterminate, 100);
            foreach (var tool in tools)
            {
                try
                {
                    Assembly asm = LoadModule(tool, isnetwork);
                    Type[] types = asm.GetTypes();
                    object load;
                    foreach (var type in types)
                    {
                        if (!type.IsPublic || type.IsAbstract) continue;
                        if (type.IsClass)
                        {
                            object loadable = (from i in type.GetCustomAttributes(true) where i is Loadable select i).FirstOrDefault();
                            if (loadable == null) continue;
                            load = Activator.CreateInstance(type);
                            if (load is WebTool)
                            {
                                WebTool wt = (WebTool)load;
                                App._WebTools.Add(wt);
                            }
                            else if (load is Tool)
                            {
                                Tool t = (Tool)load;
                                App._Tools.Add(t);
                            }
                            else if (load is ExternalTool)
                            {
                                ExternalTool et = (ExternalTool)load;
                                App._ExtTools.Add(et);
                            }
                            else if (load is PopupTool)
                            {
                                PopupTool pt = (PopupTool)load;
                                App._Popups.Add(pt);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Loading modul: " + tool +
                                    "\r\nException:" + ex.Message + "\r\n" +
                                    "Trace: " + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            SetProgressValue(TaskbarItemProgressState.None, 0);
        }

        private void ShowControlAsTab(UserControl c, string TabHeader)
        {
            c.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            c.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            c.Background = SystemColors.WindowBrush;
            Tabs.AddControl(c, TabHeader);
        }

        public void RunTool(Tool t)
        {
            try
            {
                ShowControlAsTab(t.GetControl(), t.Description);
                ReportAnalytics(t.Description, t.TrackId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error runing tool: " + t.Description +
                                "\r\nException:" + ex.Message + "\r\n" +
                                "Trace: " + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void RunPopupTool(PopupTool t)
        {
            try
            {
                UserControl control = t.GetControl();
                if (control == null) return;
                Tabs.OpenPopup(control, t.Description);
                ReportAnalytics(t.Description, t.TrackId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error runing tool: " + t.Description +
                                "\r\nException:" + ex.Message + "\r\n" +
                                "Trace: " + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void RunETool(ExternalTool t)
        {
            try
            {
                t.RunTool();
                ReportAnalytics(t.Description, t.TrackId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error runing tool: " + t.Description +
                                "\r\nException:" + ex.Message + "\r\n" +
                                "Trace: " + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RunWebTool(WebTool t)
        {
            OpenUrl(t.URL, t.Description);
            ReportAnalytics(t.Description, t.TrackId);
        }

        public void RunTool(string toolname)
        {
            Tool t = (from tool in App._Tools where tool.Description == toolname select tool).FirstOrDefault();
            if (t == null)
            {
                ExternalTool et = (from tool in App._ExtTools where tool.Description == toolname select tool).FirstOrDefault();
                RunETool(et);
            }
            else RunTool(t);
        }

        #endregion

        #region User Interface

        private void ShowStartup()
        {
            _smarthome = new SmartHomeScreen();
            _smarthome.MainWin = this;
            ShowControlAsTab(_smarthome, "Smart tools list");
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Dwm.ExtendGlass(this, new GlassMargins(5, 60, 5, 5));
            LoadTools();
            ShowStartup();
            _monitors = new MultimonitorHelper();
            WindowInteropHelper helper = new WindowInteropHelper(this);
            App.MainWindowHandle = helper.Handle;
            _monitors.AplySettings(this);
            this.Topmost = Settings.Default.WindowTopmost;
            if (Settings.Default.Tracking == 2) FileOptionsStatData_Click(this, e);
            else this.Visibility = System.Windows.Visibility.Visible;
            ReportAnalytics("start", "mcutools/start");
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("asset://mcutools/About.html", "About");
        }

        private void FileNewTools_Click(object sender, RoutedEventArgs e)
        {
            ShowStartup();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _monitors.SaveSettings(this);
            Settings.Default.WindowTopmost = this.Topmost;
            Settings.Default.Save();
            App._Config.Save();
            Tabs.CloseAllTabs();
            ReportAnalytics("stop", "mcutools/stop");
            e.Cancel = false;
        }

        private void ChangeLog_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("asset://mcutools/Changelog.html", "Change Log");
        }

        private void Website_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("http://www.webmaster442.hu/programjaim/mcu-tools/", "Webmaster442 website");
        }

        private void FileOptionsInternet_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("inetcpl.cpl", ",4");
        }

        private void FileOptionsDeletecache_Click(object sender, RoutedEventArgs e)
        {
            Functions.DeleteBrowserCache();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
            if ((p.X > this.Width - _wchrome.GlassFrameThickness.Right) || (p.X < _wchrome.GlassFrameThickness.Left) || (p.Y < _wchrome.GlassFrameThickness.Top) || (p.Y > this.Height - _wchrome.GlassFrameThickness.Bottom))
            {
                this.DragMove();
                e.Handled = true;
            }
        }

        private void Tabs_PopUpClosed(object sender, RoutedEventArgs e)
        {
            if (Tabs.PopUpHeader == "Manage External Applications") _smarthome.RefreshView();
        }

        private void Allwaystop_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
        }

        private void Minimizetotray_Click(object sender, RoutedEventArgs e)
        {
            _tray.CloseToTray();
        }

        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            Allwaystop.Header = string.Format("Allways on top: {0}", this.Topmost.ToString().Replace("False", "off").Replace("True", "on"));
        }

        private void FileOptionsStatData_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Enable anonymus statistical data sharing?\r\nData will be used to develop MCUTools", "Statistical data sharing", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes) Settings.Default.Tracking = 1;
            else Settings.Default.Tracking = 0;
            Settings.Default.Save();
        }

        #endregion
    }
}
