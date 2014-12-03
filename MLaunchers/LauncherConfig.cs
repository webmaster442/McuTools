using McuTools.Interfaces;
using McuTools.Interfaces.WPF;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace MLaunchers
{
    [Serializable]
    public class LauncherConfig
    {
        public string EaglePath { get; set; }
        public string LibreOfficePath { get; set; }
        public string LtSpicePath { get; set; }
        public string ArduinoPath { get; set; }
        public string ProcessingPath { get; set; }

        public static LauncherConfig Load(string path)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(LauncherConfig));
                using (var file = File.OpenRead(path))
                {
                    return (LauncherConfig)xs.Deserialize(file);
                }
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
                return null;
            }
        }

        public static void Save(LauncherConfig cf, string path)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(LauncherConfig));
                using (var file = File.Create(path))
                {
                    xs.Serialize(file, cf);
                }
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }
    }

    public static class ConfigReader
    {
        public static LauncherConfig Configuration { get; private set; }

        static ConfigReader()
        {
            if (Configuration == null)
            {
                if (File.Exists(Path.Combine(Folders.Application, "launchers.xml"))) Configuration = LauncherConfig.Load(Path.Combine(Folders.Application, "launchers.xml"));
                else Configuration = new LauncherConfig();
            }
        }
    }
}
