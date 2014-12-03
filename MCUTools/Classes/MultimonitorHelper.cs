using McuTools.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace McuTools
{

    public class Monitor
    {
        public string Device { get; set; }
        public bool Primary { get; set; }
        public Size WorkArea { get; set; }
        public Size WorkAreaStartPosition { get; set; }

        public Monitor()
        {
            Device = null;
            Primary = false;
            WorkArea = new Size(0, 0);
            WorkAreaStartPosition = new Size(0, 0);
        }

        public static Monitor CreateFromScreen(System.Windows.Forms.Screen screen)
        {
            Monitor mon = new Monitor();
            mon.WorkArea = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);
            mon.Primary = screen.Primary;
            mon.Device = screen.DeviceName;
            mon.WorkAreaStartPosition = new Size(screen.WorkingArea.Left, screen.WorkingArea.Top);
            return mon;
        }
    }

    class MultimonitorHelper
    {
        private List<Monitor> _monitors;

        public MultimonitorHelper()
        {
            _monitors = new List<Monitor>(System.Windows.Forms.Screen.AllScreens.Length);
            
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                _monitors.Add(Monitor.CreateFromScreen(screen));
            }
        }

        private Monitor GetPrimary()
        {
            var q = from m in _monitors where m.Primary == true select m;
            return q.FirstOrDefault();
        }

        private Monitor GetByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return GetPrimary();
            else
            {
                var q = from m in _monitors where m.Device == name select m;
                return q.FirstOrDefault();
            }
        }

        public static Monitor WindowToMonitor(Window w)
        {
            var screen = System.Windows.Forms.Screen.FromRectangle(new System.Drawing.Rectangle((int)w.Left, (int)w.Top, (int)w.Width, (int)w.Height));
            return Monitor.CreateFromScreen(screen);
        }

        public void AplySettings(Window win)
        {
            double left = Settings.Default.WindowLeft;
            double top = Settings.Default.WindowTop;
            double width = Settings.Default.WindowWidth;
            double height = Settings.Default.WindowHeight;

            Monitor current = GetByName(Settings.Default.WindowDevice);

            if (width > current.WorkArea.Width) win.Width = current.WorkArea.Width;
            if (height > current.WorkArea.Height) win.Height = current.WorkArea.Height;
            if (left < (20 - width) || left > current.WorkArea.Width) left = 0;
            if (top < (20 - height) || top > current.WorkArea.Height) top = 0;

            win.Left = current.WorkAreaStartPosition.Width + left;
            win.Top = current.WorkAreaStartPosition.Height + top;

        }

        public void SaveSettings(Window win)
        {
            Monitor current = WindowToMonitor(win);
            Settings.Default.WindowDevice = current.Device;
            Settings.Default.WindowWidth = win.Width;
            Settings.Default.WindowHeight = win.Height;
            Settings.Default.WindowLeft = win.Left - current.WorkAreaStartPosition.Width;
            Settings.Default.WindowTop = win.Top - current.WorkAreaStartPosition.Height;
        }

        public int Count
        {
            get { return _monitors.Count; }
        }
    }
}
