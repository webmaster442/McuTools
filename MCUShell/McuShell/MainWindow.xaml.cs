using AurelienRibon.Ui.Terminal;
using ConsoleControlAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace McuShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessInterface _cmd;
        private List<string> _history;
        private bool _loaded;

        public MainWindow()
        {
            InitializeComponent();
            _loaded = false;
            _history = new List<string>(15);
            Terminal.Prompt = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
            _cmd = new ProcessInterface();
            _cmd.OnProcessOutput += cmd_OnProcessOutput;
            _cmd.OnProcessError += cmd_OnProcessOutput;
            _cmd.OnProcessExit += cmd_OnProcessExit;
            _cmd.StartProcess("cmd.exe", "/q /k shellloader.cmd");
        }

        private void ExecuteCommand(string command)
        {
            Dispatcher.Invoke(() =>
            {
                _cmd.WriteInput(command);
                if (_history.Count > 14) _history.RemoveAt(0);
                if (_history.Contains(command)) _history.Remove(command);
                _history.Add(command);
            });
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
                if (Terminal.Text.Length > 16 * 1024)
                {
                    string remain = Terminal.Text.Substring(16 * 1024, Terminal.Text.Length - (16 * 1024));
                    Terminal.Clear();
                    Terminal.Text = remain + "\r\n";
                }
                Terminal.Text += args.Content;
                Terminal.InsertNewPrompt();
            }
        }

        private void Terminal_CommandEntered(object sender, Terminal.CommandEventArgs e)
        {
            ExecuteCommand(e.Command);
        }

        private void Terminal_AbortRequested(object sender, System.EventArgs e)
        {
            _cmd.SendAbort();
        }

        private void HandleCommand_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            MenuItem s = (MenuItem)sender;
            Dispatcher.Invoke(() =>
                {
                    string command = (string)s.ToolTip;
                    ExecuteCommand(command);
                });
        }

        private void MenDrives_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            MenDrives.Items.Clear();
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                MenuItem men = new MenuItem();
                men.Header = drive.Name;
                men.ToolTip = drive.Name.Replace('\\', ' ');
                men.Click += HandleCommand_Click;
                MenDrives.Items.Add(men);
            }
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            MenHistory.Items.Clear();
            foreach (var item in _history)
            {
                MenuItem men = new MenuItem();
                men.Header = item;
                men.ToolTip = item;
                men.Click += HandleCommand_Click;
                MenHistory.Items.Add(men);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
