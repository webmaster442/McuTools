using McuTools.Interfaces;
using System;
using System.IO;

namespace McuTools.Browser
{
    internal class BrowserData
    {
        public string Path { get; set; }
        public bool IsEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(Path)) return false;
                return File.Exists(Path); 
            }
        }

        public BrowserData() { }
        public BrowserData(string Path)
        {
            this.Path = Path;
        }
    }


    internal static class BrowserSelect
    {
        static BrowserSelect()
        {
            string pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            Firefox = new BrowserData(Path.Combine(pf, @"\Mozilla Firefox\firefox.exe"));
            ChromeGlobal = new BrowserData(Path.Combine(pf, @"\Google\Chrome\Application\chrome.exe"));
            ChromeLocal = new BrowserData(Path.Combine(appdata, @"\Google\Chrome\Application\chrome.exe"));
            Iexplore = new BrowserData(Path.Combine(pf , @"\Internet Explorer\iexplore.exe"));
            McuBrowser = new BrowserData(Path.Combine(Folders.Application, @"\MCUBrowser.exe"));
        }

        public static BrowserData Firefox { get; private set; }
        public static BrowserData ChromeGlobal { get; private set; }
        public static BrowserData ChromeLocal { get; private set; }
        public static BrowserData Iexplore { get; private set; }
        public static BrowserData McuBrowser { get; private set; }
    }
}
