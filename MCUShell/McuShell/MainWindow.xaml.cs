using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AurelienRibon.Ui.Terminal;
using ConsoleControlAPI;

namespace McuShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProcessInterface cmd;

        public MainWindow()
        {
            InitializeComponent();
            Terminal.Prompt = "";
            cmd = new ProcessInterface();
            cmd.OnProcessOutput += cmd_OnProcessOutput;
            cmd.OnProcessError += cmd_OnProcessOutput;
            cmd.OnProcessExit += cmd_OnProcessExit;
            cmd.StartProcess("cmd.exe", "/q");
            cmd.WriteInput("chcp 65001");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cmd.WriteInput("cls");
        }

        private void cmd_OnProcessExit(object sender, ProcessEventArgs args)
        {
            Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Shell Exited. Program now Closes.", "CMD.exe exited", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                });
        }

        private void cmd_OnProcessOutput(object sender, ProcessEventArgs args)
        {
            if (args.Content == "\f") //cls
            {
                Terminal.Clear();
                Terminal.InsertNewPrompt();
            }
            else
            {
                Terminal.Text += args.Content;
                Terminal.InsertNewPrompt();
            }
        }

        private void Terminal_CommandEntered(object sender, Terminal.CommandEventArgs e)
        {
            Dispatcher.Invoke(() =>
                {
                    cmd.WriteInput(e.Command.Raw);
                });
        }

        private void HandleCommand_Click(object sender, RoutedEventArgs e)
        {
            MenuItem s = (MenuItem)sender;
            Dispatcher.Invoke(() =>
                {
                    string command = (string)s.ToolTip;
                    cmd.WriteInput(command);
                });
        }
    }
}
