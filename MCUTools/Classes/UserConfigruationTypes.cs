using McuTools.Interfaces.WPF;
using McuTools.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace McuTools.Classes
{
    [Serializable]
    public class Config
    {
        public UsageInfo[] Stats;
        public SubConfigDict<string, string> SubConfigs;
    }

    public class UserConfiguration
    {
        private UsageStats _stats;
        private SubConfigDict<string, string> _subconfigs;

        public UserConfiguration()
        {
            _stats = new UsageStats();
        }

        public UsageStats UsageStats
        {
            get { return _stats; }
        }

        public string GetSubConfig(string key)
        {
            return _subconfigs[key];
        }

        public void SetSubConfig(string key, string value)
        {
            if (_subconfigs.ContainsKey(key)) _subconfigs[key] = value;
            else _subconfigs.Add(key, value);
        }

        public void DeleteSubConfig(string key)
        {
            _subconfigs.Remove(key);
        }

        public void Save()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Config));
                StringWriter textWriter = new StringWriter();
                Config conf = new Config();
                conf.Stats = this._stats.Pack();
                conf.SubConfigs = this._subconfigs;
                ser.Serialize(textWriter, conf);
                Settings.Default.UserConfigXML = textWriter.ToString();
                Settings.Default.Save();
                conf = null;

            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }

        public void Load()
        {
            if (string.IsNullOrEmpty(Settings.Default.UserConfigXML)) return;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Config));
                StringReader stringReader = new StringReader(Settings.Default.UserConfigXML);
                Config loaded = (Config)ser.Deserialize(stringReader);
                this._stats.Unpack(loaded.Stats);
                this._subconfigs = loaded.SubConfigs;
                loaded = null;
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }
    }
}
