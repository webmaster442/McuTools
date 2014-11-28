using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace McuTools.Interfaces.Controls
{
    /// <summary>
    /// Interaction logic for FileFolderSelector.xaml
    /// </summary>
    public partial class FileFolderSelector : UserControl
    {
        public FileFolderSelector()
        {
            InitializeComponent();
        }

        public static DependencyProperty DialogTypePropery = DependencyProperty.Register("DialogType", typeof(DialogType), typeof(FileFolderSelector), new PropertyMetadata(DialogType.OpenFile));
        public static DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(FileFolderSelector), new PropertyMetadata("All Files | *.*"));
        public static DependencyProperty SelectedPathProperty = DependencyProperty.Register("SelectedPath", typeof(string), typeof(FileFolderSelector));

        public DialogType DialogType
        {
            get { return (DialogType)GetValue(DialogTypePropery); }
            set { SetValue(DialogTypePropery, value); }
        }

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public string SelectedPath
        {
            get { return (string)GetValue(SelectedPathProperty); }
            set { SetValue(SelectedPathProperty, value); }
        }

        public event RoutedEventHandler SelectedPathChanged;

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            bool selectedpathok = false;
            if (!string.IsNullOrEmpty(SelectedPath))
            {
                selectedpathok = File.Exists(SelectedPath);
                if (!selectedpathok) selectedpathok = Directory.Exists(SelectedPath);
            }

            switch (this.DialogType)
            {
                case Controls.DialogType.OpenFile:
                    System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                    ofd.Filter = this.Filter;
                    ofd.FilterIndex = 0;
                    ofd.Multiselect = false;
                    ofd.Title = "Open file...";
                    if (selectedpathok) ofd.FileName = SelectedPath;
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) this.SelectedPath = ofd.FileName;
                    break;
                case Controls.DialogType.SaveFile:
                    System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                    sfd.Filter = this.Filter;
                    sfd.FilterIndex = 0;
                    sfd.SupportMultiDottedExtensions = true;
                    sfd.Title = "Save file...";
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK) this.SelectedPath = sfd.FileName;
                    break;
                case Controls.DialogType.DirectorySelect:
                    System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
                    fb.RootFolder = Environment.SpecialFolder.Desktop;
                    if (selectedpathok) fb.SelectedPath = this.SelectedPath;
                    fb.Description = "Select folder ...";
                    if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK) this.SelectedPath = fb.SelectedPath;
                    break;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedPathChanged != null) SelectedPathChanged(this, new RoutedEventArgs());
        }
    }

    public enum DialogType
    {
        OpenFile, DirectorySelect, SaveFile
    }
}
