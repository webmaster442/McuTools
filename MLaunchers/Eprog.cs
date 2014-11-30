using McuTools.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MLaunchers
{
    public abstract class Eprog: ExternalTool
    {
        public abstract string Path { get; }

        public override void RunTool()
        {
            string location = Environment.ExpandEnvironmentVariables(Path);

            if (string.IsNullOrEmpty(location))
            {
                MessageBox.Show("Tool Path has not been set. Please use settings", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(location))
            {
                MessageBox.Show("File not found. Please use settings", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                Process P = new Process();
                P.StartInfo.FileName = location;
                if (AdministratorRequired) P.StartInfo.Verb = "runas";
                P.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Runing program:\r\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public override bool IsVisible
        {
            get { return !string.IsNullOrEmpty(Path); }
        }

        public virtual bool AdministratorRequired
        {
            get { return false; }
        }
    }
}
