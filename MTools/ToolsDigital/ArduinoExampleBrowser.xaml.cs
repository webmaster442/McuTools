using McuTools.Interfaces.WPF;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MTools.ToolsDigital
{
    /// <summary>
    /// Interaction logic for ArduinoExampleBrowser.xaml
    /// </summary>
    public partial class ArduinoExampleBrowser : UserControl
    {
        public ArduinoExampleBrowser()
        {
            InitializeComponent();
        }

        private void ListDirectory(TreeView treeView, string path)
        {
            if (!File.Exists(path + "\\arduino.exe"))
            {
                WpfHelpers.ExceptionDialog("This is not a valid arduino directory. No Arduino.exe was found");
                return;
            }

            treeView.Items.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Items.Add(CreateDirectoryNode(rootDirectoryInfo));
            if (treeView.Items.Count > 0) (treeView.Items[0] as TreeViewItem).IsExpanded = true;
        }

        private bool ContainsFileOfInterest(string path)
        {
            string[] search = Directory.GetFiles(path, "*.ino", SearchOption.AllDirectories);
            if (search.Length > 0) return true;
            search = Directory.GetFiles(path, "*.pde", SearchOption.AllDirectories);
            return search.Length > 0;
        }

        private TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            if (ContainsFileOfInterest(directoryInfo.FullName))
            {
                var directoryNode = new TreeViewItem { Header = directoryInfo.Name };
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    TreeViewItem item = CreateDirectoryNode(directory);
                    if (item != null) directoryNode.Items.Add(item);
                }
                foreach (var file in directoryInfo.GetFiles("*.ino")) directoryNode.Items.Add(new TreeViewItem { Header = file.Name, ToolTip = file.FullName });
                foreach (var file in directoryInfo.GetFiles("*.pde")) directoryNode.Items.Add(new TreeViewItem { Header = file.Name, ToolTip = file.FullName });
                return directoryNode;
            }
            else return null;
        }

        private void ArduinoPath_SelectedPathChanged(object sender, RoutedEventArgs e)
        {
            ListDirectory(SampleTree, ArduinoPath.SelectedPath);
        }

        private void LoadText(string file)
        {
            using (var stream = File.OpenText(file))
            {
                string content = stream.ReadToEnd();
                Editor.Text = content;
            }
        }

        private void SampleTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SampleTree.SelectedItem == null) return;
            TreeViewItem selected = (TreeViewItem)SampleTree.SelectedItem;
            if (selected.ToolTip == null) return;
            string path = selected.ToolTip.ToString();
            FilePath.Text = path;
            LoadText(path);
        }

        private void SetNodeExpanded(bool expanded)
        {
            if (SampleTree.Items.Count > 0)
            {
                foreach (TreeViewItem item in SampleTree.Items)
                {
                    item.IsExpanded = expanded;
                }
            }
        }

        private void BtnExpand_Click(object sender, RoutedEventArgs e)
        {
            SetNodeExpanded(true);
        }

        private void BtnColapse_Click(object sender, RoutedEventArgs e)
        {
            SetNodeExpanded(false);
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Editor.SelectedText)) return;
            Clipboard.SetText(Editor.SelectedText);
        }
    }
}
