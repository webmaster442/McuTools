using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCalculator.Classes
{
    internal class HelpGenerator
    {
        private string assemblyloc;
        private XDocument doc;

        public HelpGenerator()
        {
            assemblyloc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (File.Exists(assemblyloc + "\\MCalculator.Tool.XML")) doc = XDocument.Load(assemblyloc + "\\MCalculator.Tool.XML");
        }

        public string LookupDescription(string command)
        {
            if (string.IsNullOrEmpty(command)) return "";
            if (doc == null) return "";
            XElement[] inner;

            if (command.Contains("."))
            {
                var q = from method in doc.Descendants("member").AsParallel()
                        where
                            (method.Attribute("name").Value.StartsWith("M:") || method.Attribute("name").Value.StartsWith("F:") || method.Attribute("name").Value.StartsWith("P:"))
                            && method.Attribute("name").Value.Contains(command) orderby  method.Attribute("name").Value.Length ascending
                        select method;
                inner = q.ToArray();
            }
            else
            {
                var q = from type in doc.Descendants("member").AsParallel()
                        where
                           type.Attribute("name").Value.StartsWith("T:") && type.Attribute("name").Value.Contains(command)
                        select type;
                inner = q.ToArray();
            }
            StringBuilder sb = new StringBuilder();
            foreach (var i in inner)
            {
                sb.Append(i.FirstAttribute.Value.Replace("M:", "").Replace("F:", "").Replace("P:", "P:").Replace("MCalculator.Maths.", ""));
                sb.Append(":\n");
                sb.Append(i.Element("summary").Value.Trim());
                sb.Append("\n");
                sb.Append("Parameters:\n");
                foreach (var param in i.Elements("param"))
                {
                    sb.Append(param.Attribute("name").Value);
                    sb.Append("\t");
                    sb.Append(param.Value);
                    sb.Append("\n\n");
                }
            }
            return sb.ToString();
        }
    }
}
