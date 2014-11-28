using MApplicationLaunchers.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MApplicationLaunchers
{
    public static class Helpers
    {
        public static void TryRunTool(string path, bool runadministrator = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Tool Path has not been set. Please use settings", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(path))
            {
                MessageBox.Show("File not found. Please use settings", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                Process P = new Process();
                P.StartInfo.FileName = path;
                if (runadministrator) P.StartInfo.Verb = "runas";
                P.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Runing program:\r\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool VisibilityCheckPath(string Path)
        {
            if (string.IsNullOrEmpty(Path)) return false;
            if (!File.Exists(Path)) return false;
            return true;
        }

        public static bool VisibilityCheck(string SettingName, string referencepath = null)
        {
            var path = (string)Settings.Default[SettingName];
            if (string.IsNullOrEmpty(referencepath))
            {
                if (string.IsNullOrEmpty(path)) return false;
                if (!File.Exists(path)) return false;
                return true;
            }
            else
            {
                if (string.IsNullOrEmpty(path) && File.Exists(referencepath))
                {
                    Settings.Default[SettingName] = referencepath;
                    Settings.Default.Save();
                    return true;
                }
                else if (File.Exists(path)) return true;
                return false;
            }
        }

        public static class ReferencePaths
        {
            private static string ProgramFiles
            {
                get
                {
                    if (Environment.Is64BitOperatingSystem) return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                    else return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                }
            }

            public static string[] Arduino
            {
                get
                {
                    try
                    {
                        string searchpath = AppDomain.CurrentDomain.BaseDirectory + "\\arduino";
                        string[] find = Directory.GetFiles(searchpath, "arduino.exe", SearchOption.AllDirectories);
                        if (find.Length > 0) return find;
                        else return null;
                    }
                    catch (IOException) { return null; }
                }
            }

            public static string Eagle
            {
                get
                {
                    string[] dirs = Directory.GetDirectories(ProgramFiles, "eagle*");
                    if (dirs.Length > 0) return dirs[0] + "\\bin\\eagle.exe";
                    else return null;
                }
            }

            public static string LTSpice
            {
                get { return ProgramFiles + @"\LTC\LTspiceIV\scad3.exe"; }
            }
        }
    }
}
