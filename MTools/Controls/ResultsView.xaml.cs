using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();
        }

        public static DependencyProperty ComputerAdressProperty = DependencyProperty.Register("ComputerAdress", typeof(string), typeof(ResultsView));
        public static DependencyProperty PingResultProperty = DependencyProperty.Register("PingResult", typeof(string), typeof(ResultsView));

        public string ComputerAdress
        {
            get { return (string)GetValue(ComputerAdressProperty); }
            set { SetValue(ComputerAdressProperty, value); }
        }

        public string PingResult
        {
            get { return (string)GetValue(PingResultProperty); }
            set { SetValue(PingResultProperty, value); }
        }

        private void MenCopyName_Click(object sender, RoutedEventArgs e)
        {
            string[] parts = ComputerAdress.Split('-');
            if (parts.Length > 1) Clipboard.SetText(parts[1].Trim());
        }

        private void MenCopyAdr_Click(object sender, RoutedEventArgs e)
        {
            string[] parts = ComputerAdress.Split('-');
            Clipboard.SetText(parts[0].Trim());
        }

        private void MenOpenWeb_Click(object sender, RoutedEventArgs e)
        {
            string[] parts = ComputerAdress.Split('-');
            Process p = new Process();
            p.StartInfo.FileName = "http://" + parts[0];
            p.StartInfo.UseShellExecute = true;
            p.Start();
        }

        private void MenOpenExpl_Click(object sender, RoutedEventArgs e)
        {
            string[] parts = ComputerAdress.Split('-');
            Process p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = "\\" + parts[0];
            p.StartInfo.UseShellExecute = true;
            p.Start();
        }

    }
}
