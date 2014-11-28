using MMediaTools.Tools;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MMediaTools.Classes
{
    /// <summary>
    /// Interaction logic for PictureDisplay.xaml
    /// </summary>
    public partial class PictureDisplay : UserControl
    {
        private int _index;
        private int _currenttab;
        private List<Item> _sources;

        public PictureDisplay()
        {
            InitializeComponent();
        }

        public List<Item> Items
        {
            get { return _sources; }
            set
            {
                _sources = value;
                Index = 0;
                if (_sources != null)
                {
                    if (Items[Index].Type == 0) Image1.Source = new BitmapImage(new Uri(Items[Index].Path));
                    Display.SelectedIndex = 0;
                }
            }
        }

        public int Index
        {
            get { return _index; }
            set
            {
                if (Items != null)
                {
                    if (value >= Items.Count - 1) _index = 0;
                    else if (value < 0) _index = Items.Count - 1; 
                    else _index = value;

                    Counter.Text = string.Format("{0} / {1}", Items.Count, _index);
                    Filename.Text = Items[_index].Path;
                }
                else _index = 0;
            }
        }

        private void BtnPrev_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            do
            {
               --Index;
            }
            while (Items[Index].Type == 2);
            SwitchImage();
        }

        private void BtnNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            do
            {
                ++Index;
            }
            while (Items[Index].Type == 2);
            SwitchImage();
        }

        private ImageBrush GetDominantColor(string path)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(path);
            img.DecodePixelWidth = 10;
            img.DecodePixelHeight = 10;
            img.EndInit();
            return new ImageBrush(img);
        }

        public void SwitchImage(int index = -1)
        {
            if (Items == null) return;
            if (index > -1) Index = index;
            switch (Display.SelectedIndex)
            {
                case 0:
                    Image2.Source = new BitmapImage(new Uri(Items[Index].Path));
                    Container2.Background = GetDominantColor(Items[Index].Path);
                    Display.SelectedIndex++;
                    break;
                case 1:
                    Image1.Source = new BitmapImage(new Uri(Items[Index].Path));
                    Container1.Background = GetDominantColor(Items[Index].Path);
                    Display.SelectedIndex--;
                    break;
            }
        }

        private void BtnFullView_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BtnFullView.IsChecked == true)
            {
                _currenttab = Display.SelectedIndex;
                FullImage.Source = new BitmapImage(new Uri(Items[Index].Path));
                Display.SelectedIndex = 2;
            }
            else
            {
                Display.SelectedIndex = _currenttab;
            }
        }
    }
}
