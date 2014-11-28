using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace McuTools
{
    /// <summary>
    /// Interaction logic for AppConfigManager.xaml
    /// </summary>
    public partial class AppConfigManager : UserControl
    {
        private enum States
        {
            None, Edit, New
        }

        private States _state;
        private int _editindex;

        public AppConfigManager()
        {
            InitializeComponent();
            _state = States.None;
        }

        private void DoList()
        {
            LbPrograms.Items.Clear();
            foreach (var prog in App._Config.ExternalProgs.FilterPrograms(null))
            {
                LbPrograms.Items.Add(prog.Name);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DoList();
        }

        public void HandleDrop(string path)
        {
            BtnAdd_Click(this, null);
            TbFile.SelectedPath = path;
            TbName.Text = Path.GetFileName(path);
        }

        private void LbPrograms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnRemove.IsEnabled = (LbPrograms.SelectedItem != null);
            if (LbPrograms.SelectedItem == null)
            {
                Display.Visibility = System.Windows.Visibility.Collapsed;
                Informations.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            int index = LbPrograms.SelectedIndex;
            DispName.Text =  App._Config.ExternalProgs[index].Name;
            DispFilePath.Text = App._Config.ExternalProgs[index].Path;
            AddEdit.Visibility = System.Windows.Visibility.Collapsed;
            Informations.Visibility = System.Windows.Visibility.Collapsed;
            Display.Visibility = System.Windows.Visibility.Visible;
        }

        private void LbPrograms_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LbPrograms_SelectionChanged(this, null);
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (LbPrograms.SelectedItem != null)
            {
                int index = LbPrograms.SelectedIndex;
                var res = MessageBox.Show(string.Format("Do you want to delete the program {0} ?", LbPrograms.SelectedItem.ToString()), "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    LbPrograms.Items.RemoveAt(index);
                    App._Config.ExternalProgs.RemoveAt(index);
                }
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _state = States.New;
            Informations.Visibility = System.Windows.Visibility.Collapsed;
            Display.Visibility = System.Windows.Visibility.Collapsed;
            AddEdit.Visibility = System.Windows.Visibility.Visible;
            TbFile.SelectedPath = "";
            TbName.Text = "";
            BtnSave.IsEnabled = false;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnSave.IsEnabled = true;
        }

        private void TbFile_SelectedPathChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbName.Text)) TbName.Text = Path.GetFileName(TbFile.SelectedPath); 
            BtnSave.IsEnabled = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Display.Visibility = System.Windows.Visibility.Collapsed;
            AddEdit.Visibility = System.Windows.Visibility.Collapsed;
            Informations.Visibility = System.Windows.Visibility.Visible;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(TbFile.SelectedPath))
            {
                MessageBox.Show("Program file not found: " + TbFile.SelectedPath, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(TbName.Text))
            {
                MessageBox.Show("Program name can't be null or empty", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            switch (_state)
            {
                case States.New:
                    App._Config.ExternalProgs.Add(TbName.Text, TbFile.SelectedPath);
                    break;
                case States.Edit:
                    App._Config.ExternalProgs[_editindex].Name = TbName.Text;
                    App._Config.ExternalProgs[_editindex].Path = TbFile.SelectedPath;
                    break;
            }
            DoList();
            Informations.Visibility = System.Windows.Visibility.Visible;
            AddEdit.Visibility = System.Windows.Visibility.Collapsed;
            Display.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            _editindex = LbPrograms.SelectedIndex;
            if (_editindex > -1)
            {
                Display.Visibility = System.Windows.Visibility.Collapsed;
                TbName.Text = App._Config.ExternalProgs[_editindex].Name;
                TbFile.SelectedPath = App._Config.ExternalProgs[_editindex].Path;
                AddEdit.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
