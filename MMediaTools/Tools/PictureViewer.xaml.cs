using McuTools.Interfaces;
using MMediaTools.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MMediaTools.Tools
{
    /// <summary>
    /// Interaction logic for PictureViewer.xaml
    /// </summary>
    public partial class PictureViewer : UserControl
    {
        private string[] pictureext = { ".jpg", ".jpeg", ".png", ".bmp", ".tif", ".tiff" };
        private string[] videoext = { ".avi", ".wmv", ".mp4" };
        private PictureDisplay _display;
        private List<Item> _items;

        public PictureViewer()
        {
            InitializeComponent();
            _items = new List<Item>(50);
            _display = new PictureDisplay();
        }

        private int IsValidExtension(string path)
        {
            string p = path.ToLower();
            foreach (var ext in pictureext)
            {
                if (p.EndsWith(ext)) return 0;
            }
            foreach (var ext in videoext)
            {
                if (p.EndsWith(ext)) return 1;
            }
            return -1;
        }

        private void FileFolderSelector_SelectedPathChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PathBox.SelectedPath)) return;
            _items.Clear();
            Images.ItemsSource = null;
            string[] files = Directory.GetFiles(PathBox.SelectedPath, "*.*");
            string[] dirs = Directory.GetDirectories(PathBox.SelectedPath);
            int type;

            foreach (var dir in dirs)
            {
                _items.Add(new Item(dir, 2));
            }

            foreach (var file in files)
            {
                type = IsValidExtension(file);
                if (type > -1)
                {
                    _items.Add(new Item(file, type));
                }
            }
            Images.ItemsSource = _items;
            _display.Items = _items;
        }

        private void Images_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Images.SelectedIndex < 0) return;

            if (_items[Images.SelectedIndex].Type == 2) PathBox.SelectedPath = _items[Images.SelectedIndex].Path;
            else
            {
                _display.SwitchImage(Images.SelectedIndex);
                ToolBase.Host.OpenUserControlAsPopup(_display, "Picture viewer");
            }
        }
    }

    public class Item
    {
        public string Path { get; private set; }
        public string DisplayName { get; private set; }
        public BitmapImage Thumbnail { get; private set; }
        public int Type { get; private set; }


        public Item(string path, int type = 0)
        {
            try
            {
                this.Path = path;
                this.DisplayName = System.IO.Path.GetFileName(path);
                this.Type = type;

                if (type == 0)
                {
                    this.Thumbnail = new BitmapImage();
                    this.Thumbnail.BeginInit();
                    this.Thumbnail.UriSource = new Uri(path);
                    this.Thumbnail.DecodePixelWidth = 200;
                    this.Thumbnail.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    this.Thumbnail.EndInit();
                }
                else if (type == 1)
                {
                    //video
                }
                else if (type == 2)
                {
                    //directory
                }
            }
            catch (IOException) { }
        }
    }
}
