using McuTools.Interfaces.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace McuTools.Classes
{

    [Serializable]
    public class ExtProgram
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    [Serializable]
    public class ExternalProgs: List<ExtProgram>
    {
        public ExternalProgs() : base() { }

        public List<ExtProgram> FilterPrograms(string filter)
        {
            if (filter == "**fav**")
            {
                var q = from i in this where App._Config.UsageStats.ContainsKey(i.Name) orderby App._Config.UsageStats[i.Name] descending select i;
                return q.ToList();
            }
            if (string.IsNullOrEmpty(filter)) return (from i in this orderby i.Name ascending select i).ToList();
            return (from i in this where i.Name.ToLower().Contains(filter.ToLower()) orderby i.Name ascending select i).ToList();
        }

        public ExtProgram GetProgram(string Name)
        {
            var q2 = (from i in this where String.Compare(Name, i.Name, true) == 0 select i).FirstOrDefault();
            return q2;
        }

        public void Add(string ProgName, string ProgPath)
        {
            ExtProgram prog = new ExtProgram()
            {
                Name = ProgName,
                Path = ProgPath,
            };
            this.Add(prog);
        }

        public void Remove(string Name)
        {
            var q2 = (from i in this where String.Compare(Name, i.Name, true) == 0 select i).FirstOrDefault();
            this.Remove(q2);
        }
    }
}
