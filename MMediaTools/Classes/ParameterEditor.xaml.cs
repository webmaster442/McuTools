using System.Windows;
using System.Windows.Controls;

namespace MMediaTools.Classes
{
    /// <summary>
    /// Interaction logic for ParameterEditor.xaml
    /// </summary>
    public partial class ParameterEditor : UserControl
    {
        public ParameterEditor()
        {
            InitializeComponent();
        }

        private string[] _presets;

        public string Value
        {
            get { return TbText.Text; }
            set { TbText.Text = value; }
        }

        public string[] Presets
        {
            get
            {
                return _presets;
            }
            set
            {
                _presets = value;
                BtnPresets.IsEnabled = value != null;
                BtnBrowse.IsEnabled = value == null;
            }
        }

        public string Description
        {
            get { return TbDescript.Text; }
            set { TbDescript.Text = value; }
        }

        private void BtnPresets_Click(object sender, RoutedEventArgs e)
        {
            if (Presets != null)
            {
                if (Context.Items.Count == 0)
                {
                    foreach (var preset in Presets)
                    {
                        MenuItem m = new MenuItem();
                        m.Header = preset;
                        m.Click += m_Click;
                        Context.Items.Add(m);
                    }
                }
                Context.IsOpen = true;
            }
        }

        private void m_Click(object sender, RoutedEventArgs e)
        {
            MenuItem s = (MenuItem)sender;
            TbText.Text = s.Header.ToString();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TbText.Text = ofd.FileName;
            }
        }
    }
}
