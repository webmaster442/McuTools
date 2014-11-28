using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Linq;

namespace McuTools.Classes
{
    internal class BookManager : IEnumerable<KeyValuePair<string, string>>
    {
        private Dictionary<string, string> _files;

        public BookManager()
        {
            _files = new Dictionary<string, string>(20);
            Load();
        }

        public void Load()
        {

            string _apprdir = System.AppDomain.CurrentDomain.BaseDirectory + "\\Books\\";
            try
            {
                string[] pdfs = Directory.GetFiles(_apprdir, "*.pdf", SearchOption.AllDirectories);
                foreach (var pdf in pdfs)
                {
                    _files.Add(pdf, Path.GetFileName(pdf));
                }
            }
            catch (IOException) { }
        }

        public int Count
        {
            get { return _files.Count; }
        }

        public string this[string key]
        {
            get { return _files[key]; }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        private static string GetProgID(string Extension)
        {
            string toReturn = string.Empty;
            if (Registry.ClassesRoot.OpenSubKey(Extension, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadPermissions) != null)
            {
                if (Registry.ClassesRoot.OpenSubKey(Extension, RegistryKeyPermissionCheck.ReadSubTree,  RegistryRights.ReadPermissions).GetValue("") != null)
                {
                    toReturn = Registry.ClassesRoot.OpenSubKey(Extension, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadPermissions).GetValue("").ToString();
                }
            }
            return toReturn;
        }

        public List<string> Filter(string search)
        {
            if (search == "**fav**")
            {
                var q2 = from i in _files where App._Config.UsageStats.ContainsKey(i.Value) orderby App._Config.UsageStats[i.Value] descending select i.Value;
                return q2.ToList();
            }
            if (string.IsNullOrEmpty(search)) return _files.Values.Select(i => i).ToList();
            var q = from i in _files where i.Value.ToLower().Contains(search.ToLower()) select i.Value;
            return q.ToList();
        }

        public string GetFilePath(string name)
        {
            var q = from f in _files where f.Value.ToLower() == name.ToLower() select f.Key;
            return q.FirstOrDefault();
        }

        public static bool PDFReaderInstalled()
        {
            bool extKeyExists = false;
            bool progIDkeyExists = false;
            string Extension = ".pdf";

            if (Registry.ClassesRoot.OpenSubKey(Extension) != null)
            {
                extKeyExists = true;
                string progid = GetProgID(".pdf");
                if (progid != null)
                {
                    if (Registry.ClassesRoot.OpenSubKey(progid) != null)
                    {
                        progIDkeyExists = true;
                    }
                }
            }

            if (extKeyExists && progIDkeyExists) return true;
            else return false;
        }
    }
}
