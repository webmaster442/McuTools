using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MApplicationLaunchers
{
    /// <summary>
    /// Interaction logic for FileSelector.xaml
    /// </summary>
    public partial class FileSelector : UserControl
    {

        public static DependencyProperty LabelTextProperty = DependencyProperty.Register("LabelText", typeof(string), typeof(FileSelector));
        public static DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(FileSelector));
        public static DependencyProperty DownloadLinkPoroperty = DependencyProperty.Register("DownloadLink", typeof(Uri), typeof(FileSelector));

        System.Windows.Forms.OpenFileDialog ofd;

        public Uri DownloadLink
        {
            get { return (Uri)GetValue(DownloadLinkPoroperty); }
            set { SetValue(DownloadLinkPoroperty, value); }
        }

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public FileSelector()
        {
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            ofd = new System.Windows.Forms.OpenFileDialog();
            if (!string.IsNullOrEmpty(FilePath)) ofd.FileName = FilePath;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = ofd.FileName;
            }
        }

        private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
        {
            Hyperlink s = (Hyperlink)sender;
            Process.Start(s.NavigateUri.ToString());
        }
    }
}
