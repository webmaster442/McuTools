using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace MCalculator.Classes
{
    /// <summary>
    /// Internal Calculator related functions
    /// </summary>
    public static class Calc
    {
        internal static PythonCore Core;

        private static string ShortName(string LongName)
        {
            string[] parts0, parts1, parts2;
            StringBuilder ret = new StringBuilder(); ;
            if (LongName.Contains("("))
            {
                parts0 = LongName.Split('(');
                parts1 = parts0[0].Split('.');
                ret.Append(parts1[parts1.Length - 2] + "." + parts1[parts1.Length - 1] + "(");
                if (parts0.Length > 1)
                {
                    parts1 = parts0[1].Split(',');
                    for (int i = 0; i < parts1.Length; i++)
                    {
                        parts2 = parts1[i].Split('.');
                        ret.Append(parts2[parts2.Length - 2] + "." + parts2[parts2.Length - 1]);
                        if (i != parts1.Length - 1) ret.Append(",");
                    }
                }
                return ret.ToString();
            }
            else
            {
                parts0 = LongName.Split('.');
                return parts0[parts0.Length - 2] + "." + parts0[parts0.Length - 1];
            }

        }

        /// <summary>
        /// Clears the terminal screen
        /// </summary>
        public static void Clear()
        {
            if (Core != null) Core.Terminal.Clear();
        }

        /// <summary>
        /// Changes calulator mode
        /// </summary>
        /// <param name="mode">can be one of the following: DEG, RAD, GRAD</param>
        public static void SetMode(string mode)
        {
            var m = mode.ToLower().Trim();
            switch (m)
            {
                case "deg":
                    Core.TrigMode = Maths.TrigMode.Deg;
                    break;
                case "rad":
                    Core.TrigMode = Maths.TrigMode.Rad;
                    break;
                case "grad":
                    Core.TrigMode = Maths.TrigMode.Grad;
                    break;
                default:
                    throw new Exception("Unknown calculator mode");
            }
        }
    }
}
