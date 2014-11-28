using MCalculator.Maths;
using System.Text;
using System.Windows;

namespace MCalculator.UserInterface
{
    /// <summary>
    /// Interaction logic for EditSetDlg.xaml
    /// </summary>
    internal partial class EditSetDlg : Window
    {
        public EditSetDlg()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public Set Items
        {
            get
            {
                Set set = new Set(Values.LineCount);
                string[] lines = Values.Text.Split('\n');
                double value = 0;
                foreach (var line in lines)
                {
                    if (double.TryParse(line, out value)) set.Add(value);
                }
                return set;
            }
            set
            {
                StringBuilder sb = new StringBuilder();
                foreach (var val in value)
                {
                    sb.AppendFormat("{0}\r\n", val);
                }
                Values.Text = sb.ToString();
            }
        }
    }
}
