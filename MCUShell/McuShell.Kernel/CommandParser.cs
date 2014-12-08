﻿using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace McuShell.Kernel
{
    /// <summary>
    /// Command argument
    /// </summary>
    public abstract class CmdArgument : Attribute
    {
        /// <summary>
        /// argument short name
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// argument long name
        /// </summary>
        public string LongName { get; set; }
        /// <summary>
        /// Gets or sets wheather the argument is required or not
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// Command description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Creates a new instance of CmdArgument
        /// </summary>
        public CmdArgument()
        {
            Required = false;
            ShortName = "";
            LongName = "";
            Description = "";
        }
    }

    /// <summary>
    /// SWitch Argument
    /// </summary>
    public class SwitchArgument : CmdArgument
    {
        /// <summary>
        /// Creates a new instace of SwitchArgument
        /// </summary>
        public SwitchArgument() : base() { }
    }

    /// <summary>
    /// Parameter argument
    /// </summary>
    public class ParameterArgument : CmdArgument
    {
        /// <summary>
        /// Creates a new instace of ParameterArgument
        /// </summary>
        public ParameterArgument() : base() { }
    }

    /// <summary>
    /// Enum argument
    /// </summary>
    public class EnumArgument : CmdArgument
    {
        /// <summary>
        /// Creates a new instace of EnumArgument
        /// </summary>
        public EnumArgument() : base() { }
    }

    /// <summary>
    /// Command Parser
    /// </summary>
    public static class CommandParser
    {
        private static string[] _split;
        private static object _settings;

        /// <summary>
        /// Gets or sets the command description
        /// </summary>
        public static string CommandDescription { get; set; }

        private static bool IsPresent(CmdArgument atr, bool param = false)
        {
            for (int i = 0; i < _split.Length; i++)
            {
                var item = _split[i];
                if (param)
                {
                    if ((item == "-" + atr.ShortName) || (item == "--" + atr.LongName))
                    {
                        if (i + 1 <= _split.Length - 1) return (!_split[i + 1].StartsWith("-") && !_split[i + 1].StartsWith("--"));
                        else return false;
                    }
                }
                else if ((item == "-" + atr.ShortName) || (item == "--" + atr.LongName)) return true;
            }
            return false;
        }

        private static string GetPararm(CmdArgument atr)
        {
            for (int i = 0; i < _split.Length; i++)
            {
                var item = _split[i];
                if ((item == "-" + atr.ShortName) || (item == "--" + atr.LongName))
                {
                    if (i + 1 <= _split.Length - 1) return _split[i + 1];
                    else return null;
                }
            }
            return null;
        }

        private static bool HelpRequested()
        {
            foreach (var item in _split)
            {
                if (item == "--help" || item == "-h") return true;
            }
            return false;
        }

        private static string ParametersHelp(object SettingsObject)
        {
            StringBuilder result = new StringBuilder();
            Type t = SettingsObject.GetType();
            if (!t.IsClass) throw new ArgumentException("SettingsObject must be class");
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite) continue;
                var attributes = property.GetCustomAttributes(true);
                foreach (var attribute in attributes)
                {
                    if (attribute is EnumArgument)
                    {
                        CmdArgument cmd = (CmdArgument)attribute;
                        result.AppendFormat("\t-{0} or {1}, required: {2}\r\n", cmd.ShortName, cmd.LongName, cmd.Required);
                        string[] values = Enum.GetNames(property.PropertyType);
                        result.AppendFormat("\t{0}\r\n", cmd.Description);
                        result.AppendFormat("\tPossible values: {0}\r\n\r\n", String.Join(",", values.Select(p => p.ToString()).ToArray()));
                    }
                    if (attribute is CmdArgument)
                    {
                        CmdArgument cmd = (CmdArgument)attribute;
                        result.AppendFormat("\t-{0} or {1}, required: {2}\r\n", cmd.ShortName, cmd.LongName, cmd.Required);
                        result.AppendFormat("\t{0}\r\n\r\n", cmd.Description);
                    }
                }
            }
            return result.ToString();
        }

        private static void Error(string s, params object[] args)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(s, args);
            Console.ForegroundColor = current;
            Help();
        }

        /// <summary>
        /// Prints an exception message to the output
        /// </summary>
        /// <param name="ex"></param>
        public static void Error(Exception ex)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error:");
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = current;
        }

        private static void Help(bool clear = false)
        {
            if (clear) Console.Clear();
            Console.WriteLine(CommandDescription);
            Console.WriteLine("\r\nArguments & Switches:");
            Console.WriteLine(ParametersHelp(_settings));
#if DEBUG
            Console.WriteLine();
            Console.ReadKey();
#endif
            Environment.Exit(1);
        }

        /// <summary>
        /// Parse argument
        /// </summary>
        /// <param name="SettingsObject">A class containing the settings</param>
        /// <param name="CommandLine">Application command line to parse</param>
        public static void Parse(object SettingsObject, string[] CommandLine)
        {
            _split = CommandLine;
            _settings = SettingsObject;

            if (HelpRequested()) Help(true);

            Type t = SettingsObject.GetType();
            if (!t.IsClass) throw new ArgumentException("SettingsObject must be class");
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite) continue;
                var attributes = property.GetCustomAttributes(true);
                foreach (var attribute in attributes)
                {
                    if (attribute is SwitchArgument)
                    {
                        SwitchArgument sw = (SwitchArgument)attribute;
                        if (sw.Required && !IsPresent(sw)) Error("Required switch parameter missing: -{0} or --{1}", sw.ShortName, sw.LongName);
                        if (property.PropertyType != typeof(bool)) throw new ArgumentException("SwitchAttrib needs a bool property");
                        property.SetValue(SettingsObject, IsPresent(sw));
                    }
                    else if (attribute is ParameterArgument)
                    {
                        ParameterArgument par = (ParameterArgument)attribute;
                        if (par.Required && !IsPresent(par, true)) Error("Required switch parameter missing: -{0} or --{1}", par.ShortName, par.LongName);
                        string val = GetPararm(par);
                        try
                        {
                            if (property.PropertyType == typeof(string)) property.SetValue(SettingsObject, val);
                            else if (property.PropertyType == typeof(double)) property.SetValue(SettingsObject, Convert.ToDouble(val));
                            else if (property.PropertyType == typeof(float)) property.SetValue(SettingsObject, Convert.ToSingle(val));
                            else if (property.PropertyType == typeof(long)) property.SetValue(SettingsObject, Convert.ToInt64(val));
                            else if (property.PropertyType == typeof(int)) property.SetValue(SettingsObject, Convert.ToInt32(val));
                            else if (property.PropertyType == typeof(uint)) property.SetValue(SettingsObject, Convert.ToUInt32(val));
                            else if (property.PropertyType == typeof(ulong)) property.SetValue(SettingsObject, Convert.ToUInt64(val));
                            else if (property.PropertyType == typeof(byte)) property.SetValue(SettingsObject, Convert.ToByte(val));
                            else if (property.PropertyType == typeof(sbyte)) property.SetValue(SettingsObject, Convert.ToSByte(val));
                            else throw new ArgumentException("Property type not supported by parser");
                        }
                        catch (Exception) { Error("Error parsing argument: {0} or {1}, value: {2}", par.ShortName, par.LongName, val); }
                    }
                    else if (attribute is EnumArgument)
                    {
                        EnumArgument en = (EnumArgument)attribute;
                        if (en.Required && !IsPresent(en, true)) Error("Required switch parameter missing: -{0} or --{1}", en.ShortName, en.LongName);
                        string val = GetPararm(en);
                        try
                        {
                            object enumresult = Enum.Parse(property.PropertyType, val);
                            property.SetValue(SettingsObject, enumresult);
                        }
                        catch (Exception) { Error("Error parsing argument: {0} or {1}, value: {2}", en.ShortName, en.LongName, val); }
                    }
                }
            }
        }
    }
}
