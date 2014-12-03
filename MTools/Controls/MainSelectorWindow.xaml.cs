using System.Windows;

namespace MTools.ToolOther
{
    /// <summary>
    /// Interaction logic for MainSelector.xaml
    /// </summary>
    public partial class MainSelectorWindow : Window
    {
        public MainSelectorWindow()
        {
            InitializeComponent();
        }

        public static DependencyProperty ListItemsProperty = DependencyProperty.Register("ListItems", typeof(string[]), typeof(MainSelectorWindow));

        public string[] ListItems
        {
            get { return (string[])GetValue(ListItemsProperty); }
            set { SetValue(ListItemsProperty, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string SelectedFile
        {
            get
            {
                if (list.SelectedIndex > -1) return ListItems[list.SelectedIndex];
                else return null;
            }
        }
    }
}
