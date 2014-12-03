using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace McuTools.Classes
{
    internal class TrayIcon: IDisposable
    {
        private NotifyIcon _notify;
        private bool _closed;
        private ContextMenuStrip _menu;

        public TrayIcon()
        {
            _notify = new NotifyIcon();
            _notify.Visible = false;
            _notify.Text = "MCU Tools";
            _notify.Icon = McuTools.Properties.Resources.TaskBar;
            _notify.DoubleClick += _notify_DoubleClick;
            _closed = false;

            _menu = new ContextMenuStrip();
            
            var exit = _menu.Items.Add("Exit Program");
            exit.Click += exit_Click;

            _notify.ContextMenuStrip = _menu;

        }

        private void exit_Click(object sender, EventArgs e)
        {
            App.Current.MainWindow.Close();
        }

        private void _notify_DoubleClick(object sender, EventArgs e)
        {
            if (_closed)
            {
                App.Current.MainWindow.Show();
                _notify.Visible = false;
                _closed = false;
            }
        }

        public void CloseToTray()
        {
            App.Current.MainWindow.Hide();
            _closed = true;
            _notify.Visible = true;
        }

        protected virtual void Dispose(bool native)
        {
            if (_menu != null)
            {
                _menu.Dispose();
                _menu = null;
            }
            if (_notify != null)
            {
                _notify.Dispose();
                _notify = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
