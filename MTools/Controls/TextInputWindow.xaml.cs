using System.Windows;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for TextInputWindow.xaml
    /// </summary>
    public partial class TextInputWindow : Window
    {
        public TextInputWindow()
        {
            InitializeComponent();
        }

        public static DependencyProperty InputTextPropery = DependencyProperty.Register("InputText", typeof(string), typeof(TextInputWindow));

        public string InputText
        {
            get { return (string)GetValue(InputTextPropery); }
            set { SetValue(InputTextPropery, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InputText)) this.DialogResult = true;
            else MessageBox.Show("Please enter a value");
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter) this.DialogResult = true;
        }
    }
}
