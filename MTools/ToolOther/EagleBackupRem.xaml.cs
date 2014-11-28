using McuTools.Interfaces.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MTools.ToolOther
{
    /// <summary>
    /// Interaction logic for EagleBackupRem.xaml
    /// </summary>
    public partial class EagleBackupRem : UserControl
    {

        private List<string> _files;

        public EagleBackupRem()
        {
            InitializeComponent();
        }

        private async void SearchFiles()
        {
            LbResults.Visibility = System.Windows.Visibility.Collapsed;
            Indicator.Visibility = System.Windows.Visibility.Visible;
            Controls.IsEnabled = false;
            _files = null;

            LbResults.ItemsSource = null;
            _files = await AsyncFileSearch(DirectorySel.SelectedPath, (bool)CbSubdirs.IsChecked, (bool)CbSch.IsChecked, (bool)CbBrd.IsChecked);
            LbResults.ItemsSource = _files;

            Indicator.Visibility = System.Windows.Visibility.Collapsed;
            LbResults.Visibility = System.Windows.Visibility.Visible;

            Controls.IsEnabled = LbResults.Items.Count > 0;
        }

        private Task<List<string>> AsyncFileSearch(string path, bool subdirs, bool sch, bool brd)
        {
            return Task.Run<List<string>>(() => FindFiles(path, subdirs, sch, brd));
        }

        private List<string> FindFiles(string path, bool subdirs, bool sch, bool brd)
        {
            List<string> files = new List<string>();

            SearchOption opt = SearchOption.TopDirectoryOnly;
            if (subdirs) opt = SearchOption.AllDirectories;
            if (sch) files.AddRange(Directory.GetFiles(path, "*.s#*", opt));
            if (brd) files.AddRange(Directory.GetFiles(path, "*.b#*", opt));
            return files.OrderBy(t => t).ToList();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchFiles();
        }

        private void BtnClean_Click(object sender, RoutedEventArgs e)
        {
            _files.Clear();
            LbResults.ItemsSource = null;
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < LbResults.SelectedItems.Count; i++)
            {
                _files.Remove(LbResults.SelectedItems[i].ToString());
            }
            LbResults.ItemsSource = null;
            LbResults.ItemsSource = _files;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var q = MessageBox.Show("Delete selected files? The operation can't be undone when complete.", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (q == MessageBoxResult.No) return;
                for (int i = 0; i < LbResults.SelectedItems.Count; i++)
                {
                    string item = LbResults.SelectedItems[i].ToString();
                    _files.Remove(item);
                    File.Delete(item);
                }
                LbResults.ItemsSource = null;
                LbResults.ItemsSource = _files;
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }

        }

        private void BtnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var q = MessageBox.Show("Delete listed files? The operation can't be undone when complete.", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (q == MessageBoxResult.No) return;
                foreach (var file in _files)
                {
                    File.Delete(file);
                }
                _files.Clear();
                LbResults.ItemsSource = null;
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }

    }
}
