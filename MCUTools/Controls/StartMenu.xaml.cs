using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace McuTools.Controls
{
    /// <summary>
    /// Interaction logic for StartMenu.xaml
    /// </summary>
    public partial class StartMenu : UserControl
    {
        public StartMenu()
        {
            InitializeComponent();
            _menu = new List<StartItem>();
            _common = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\programs";
            _user = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\programs";
        }

        private List<StartItem> _menu;
        private string _user;
        private string _common;
        private string _current;

        private void StartMenu_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SM.SelectedIndex < 0) return;
            var item = _menu[SM.SelectedIndex];
            if (item.IsDir) SetItems(item.Path);
            else Process.Start(item.Path);
        }

        private void SetItems(string path = null)
        {
            SM.ItemsSource = null;
            _current = path;
            var dirs = new List<string>();
            if (path == null)
            {
                dirs.AddRange(Directory.GetDirectories(_user));
                dirs.AddRange(Directory.GetDirectories(_common));
                dirs.AddRange(Directory.GetFiles(_user));
                dirs.AddRange(Directory.GetFiles(_common));
                dirs.Sort();
            }
            else
            {
                dirs.AddRange(Directory.GetDirectories(path));
                dirs.AddRange(Directory.GetFiles(path));
                dirs.Sort();
            }

            _menu.Clear();
            foreach (var dir in dirs)
            {
                _menu.Add(new StartItem() { Path = dir });
            }
            SM.ItemsSource = _menu;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetItems();
        }

        private void BtnRoot_Click(object sender, RoutedEventArgs e)
        {
            SetItems();
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;
            if (_current == _user || _current == _common) return;

            string[] path = _current.Split('\\');
            string newpath = _current.Replace("\\" + path[path.Length - 1], "");

            if (newpath == _user || newpath == _common) SetItems();
            else SetItems(newpath);
        }
    }

    internal class StartItem
    {
        public string DisplayName
        {
            get { return System.IO.Path.GetFileName(Path); }
        }
        public string Path { get; set; }
        public bool IsDir
        {
            get { return Directory.Exists(Path); }
        }

        public ImageSource Image
        {
            get 
            {
                if (IsDir) return new BitmapImage(new Uri("/McuTools;component/images/folder.png", UriKind.Relative));
                return McuTools.Interfaces.WPF.WpfHelpers.GetExeIcon(Path);
            }
        }
    }
}
