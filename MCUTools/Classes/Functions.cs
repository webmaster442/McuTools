using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using McuTools.Interfaces;

namespace McuTools
{
    internal static class Functions
    {
        public static void DeleteBrowserCache(bool silent = false)
        {
            if (!silent)
            {
                var dialog = MessageBox.Show("Delete Browser cache?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog == MessageBoxResult.No) return;
            }
            var local = Folders.Local;
            if (Directory.Exists(local + "\\Cache"))
            {
                try { Directory.Delete(local + "\\Cache", true); }
                catch (IOException) { MessageBox.Show("Cache delete failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                finally 
                { 
                    if (!silent) MessageBox.Show("Cache deleted succesfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public static long CalculateCacheSize(string dirparam = null)
        {
            long size = 0;

            if (dirparam == null) dirparam = System.AppDomain.CurrentDomain.BaseDirectory + "\\Cache";

            if (!Directory.Exists(dirparam)) return -1;

            try
            {
                foreach (string file in Directory.GetFiles(dirparam))
                {
                    if (File.Exists(file))
                    {
                        FileInfo finfo = new FileInfo(file);
                        size += finfo.Length;
                    }
                    foreach (string dir in Directory.GetDirectories(dirparam)) size += CalculateCacheSize(dir);
                }
            }
            catch (Exception) { size = -1; }
            return size;
        }

        public static void SetupProfile()
        {
            string profiledir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\McuTools";
            if (!Directory.Exists(profiledir)) Directory.CreateDirectory(profiledir);
            System.Runtime.ProfileOptimization.SetProfileRoot(profiledir);
            System.Runtime.ProfileOptimization.StartProfile("McuTools");
        }
    }
}
