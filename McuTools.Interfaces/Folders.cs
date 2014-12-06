using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace McuTools.Interfaces
{
    public static class Folders
    {
        public static string Application { get; private set; }
        public static string Local { get; private set; }

        static Folders()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            string assemblyname = Assembly.GetExecutingAssembly().GetName().Name;
            UriBuilder uri = new UriBuilder(codeBase);
            
            //network location fix
            if (!string.IsNullOrEmpty(uri.Host))
            {
                string path = string.Format("\\\\{0}\\{1}", uri.Host, Uri.UnescapeDataString(uri.Path).Replace(assemblyname+".DLL", "").Replace("/", "\\"));
                Application = path;
            }
            else
            {
                string path = Uri.UnescapeDataString(uri.Path);
                Application = Path.GetDirectoryName(path);
            }

            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(documents + "\\mcutools")) Directory.CreateDirectory(documents + "\\mcutools");
            Local = Path.Combine(documents, "mcutools");
        }

        public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                {
                }
                return true;
            }
            catch
            {
                if (throwIfFails) throw;
                else return false;
            }
        }
    }
}
