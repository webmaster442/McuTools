using AurelienRibon.Ui.SyntaxHighlightBox;
using McuTools.Interfaces.WPF;
using MTools.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MTools.ToolOther
{
    /// <summary>
    /// Interaction logic for EditorPlane.xaml
    /// </summary>
    public partial class SketchEditor : UserControl
    {
        private Dictionary<string, string> _files;
        private string _dir;
        private int _selected;

        public SketchEditor()
        {
            InitializeComponent();
            FilePanel.ItemsSource = null;
            _files = new Dictionary<string, string>();
            _dir = "";
            New();
            Editor.CurrentHighlighter = HighlighterManager.Instance.Highlighters["Arduino"];
        }

        private string GetKey(int index)
        {
            return ((KeyValuePair<string, string>)FilePanel.Items[index]).Key;
        }

        private void SwitchDocuments()
        {
            string oldkey = GetKey(_selected);
            string newkey = GetKey(FilePanel.SelectedIndex);
            if (oldkey == newkey) return;
            _files[oldkey] = Editor.Text;
            Editor.Text = _files[newkey];
            _selected = FilePanel.SelectedIndex;
        }

        private void New()
        {
            DocumentName = string.Format("sketch_{0}_{1}_{2}.ino", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            _files.Add(DocumentName, "");
            FilePanel.SelectedIndex = 0;
            _selected = 0;
            FilePanel.ItemsSource = _files;
            Editor.Text = "";
        }

        private void BtnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TextInputWindow ti = new TextInputWindow();
            ti.Title = "Enter new file name:";
            if (ti.ShowDialog() == true)
            {
                _files.Add(ti.InputText + ".ino", "");
            }
            FilePanel.ItemsSource = null;
            FilePanel.ItemsSource = _files;
            FilePanel.SelectedIndex = FilePanel.Items.Count - 1;
            SwitchDocuments();
        }

        private void BtnDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (FilePanel.SelectedIndex < 0) return;
            string key = GetKey(FilePanel.SelectedIndex);
            if (File.Exists(_dir + "\\" + key))
            {
                try
                {
                    File.Delete(_dir + "\\" + key);
                }
                catch (IOException ex)
                {
                    WpfHelpers.ExceptionDialog("Error removing file: " + key, ex);
                    return;
                }
                _files.Remove(key);
            }
        }

        private void BtnRename_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (FilePanel.SelectedIndex < 0) return;
            TextInputWindow ti = new TextInputWindow();
            ti.Title = "Enter new file name:";
            if (ti.ShowDialog() == true)
            {
                string oldkey = GetKey(FilePanel.SelectedIndex);
                if (File.Exists(_dir + "\\" + oldkey))
                {
                    try
                    {
                        File.Move(_dir + "\\" + oldkey, _dir + "\\" + ti.InputText);
                    }
                    catch (IOException ex)
                    {
                        WpfHelpers.ExceptionDialog("Error renaming file: " + oldkey, ex);
                        return;
                    }
                }
                _files.Add(ti.InputText, _files[oldkey]);
                _files.Remove(oldkey);
            }
        }

        private void Load(string dir)
        {
            _dir = dir;
            DocumentName = Path.GetFileName(dir);
            _files.Clear();
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(_dir, "*.ino", SearchOption.TopDirectoryOnly));
            files.AddRange(Directory.GetFiles(_dir, "*.pde", SearchOption.TopDirectoryOnly));
            foreach (var file in files)
            {
                try
                {
                    var text = File.OpenText(file);
                    string content = text.ReadToEnd();
                    text.Close();
                    string key = System.IO.Path.GetFileName(file);
                    _files.Add(key, content);
                }
                catch (IOException ex)
                {
                    WpfHelpers.ExceptionDialog("Error Loading sketch: " + _dir, ex);
                    _files.Clear();
                    FilePanel.ItemsSource = null;
                    return;
                }
            }
            FilePanel.ItemsSource = null;
            FilePanel.ItemsSource = _files;
        }

        private void BtnLoad_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
            fb.SelectedPath = _dir;
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK) Load(fb.SelectedPath);
        }

        private void SaveDirectory(string directorypath)
        {
            _dir = directorypath;
            string newkey = GetKey(FilePanel.SelectedIndex);
            _files[newkey] = Editor.Text;

            if (Directory.GetFiles(_dir).Length > 0)
            {
                WpfHelpers.ExceptionDialog("Target directory is not empty, can't save sketch");
                return;
            }

            var sketchname = Path.GetFileName(_dir) + ".ino";

            if (!_files.ContainsKey(sketchname))
            {
                MainSelectorWindow ms = new MainSelectorWindow();
                ms.ListItems = (from i in _files.Keys select i).ToArray();
                if (ms.ShowDialog() == true)
                {
                    string selected = ms.SelectedFile;
                    if (string.IsNullOrEmpty(selected))
                    {
                        WpfHelpers.ExceptionDialog("Save can't be completeted, because no main file is selected");
                        return;
                    }
                    string content = _files[selected];
                    _files.Remove(selected);
                    _files.Add(sketchname + ".ino", content);
                }
                else
                {
                    WpfHelpers.ExceptionDialog("Save can't be completeted, because no main file is selected");
                    return;
                }
            }

            foreach (var file in _files)
            {
                try
                {
                    var text = File.CreateText(directorypath + "\\" + file.Key + ".new");
                    text.Write(file.Value);
                    text.Close();
                    File.Move(directorypath + "\\" + file.Key + ".new", directorypath + "\\" + file.Key);
                }
                catch (IOException ex)
                {
                    WpfHelpers.ExceptionDialog("Error saving sketch: " + directorypath, ex);
                    break;
                }
            }

            Load(_dir);
        }

        private void BtnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_dir)) BtnSaveAs_Click(sender, e);
            else SaveDirectory(_dir);
        }

        private void BtnSaveAs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
            fb.SelectedPath = _dir;
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveDirectory(fb.SelectedPath);
                MessageBox.Show("Save complete", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void FilePanel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SwitchDocuments();
        }

        public string DocumentName
        {
            get;
            set;
        }
    }
}
