using IronPython;
using IronPython.Hosting;
using IronPython.Runtime.Types;
using MCalculator.Classes;
using MCalculator.Maths;
using McuTools.Interfaces;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace MCalculator
{
    internal class PythonCore: IDisposable
    {
        private ScriptEngine _engine;
        private ScriptScope _scope;
        private EventRaisingStreamWriter _outp;
        private NullStream _history;
        private IConsole _terminal;
        private PythonSystax _snytax;

        public PythonCore(IConsole term)
        {
            Dictionary<String, Object> options = new Dictionary<string, object>();
            options["DivisionOptions"] = PythonDivisionOptions.New;
            _history = new NullStream();
            _outp = new EventRaisingStreamWriter(_history);
            _terminal = term;
            _outp.StringWritten += new EventHandler<MyEvtArgs<string>>(_outp_StringWritten);
            _engine = Python.CreateEngine(options);
            _engine.Runtime.IO.SetOutput(_history, _outp);
            _engine.Runtime.IO.SetErrorOutput(_history, _outp);
            _scope = _engine.CreateScope();
            _snytax = new PythonSystax();
            _snytax.Engine = _engine;
            TrigMode = TrigMode.Deg;
        }

        public PythonSystax SyntaxProvider
        {
            get { return _snytax; }
        }

        public void AttachTypeToRuntime(Type Class)
        {
            _scope.SetVariable(Class.Name, DynamicHelpers.GetPythonTypeFromType(Class));
        }

        private void _outp_StringWritten(object sender, MyEvtArgs<string> e)
        {
            _terminal.BufferedWrite(e.Value);
        }

        private string FormatDouble(double input)
        {
            string gchar = CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator;
            string fchar = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            if (double.IsNaN(input) || double.IsInfinity(input)) return input.ToString(CultureInfo.CurrentCulture);
            StringBuilder sb = new StringBuilder();
            bool passed = false;
            int j = 1;
            int i;
            char[] ar;
            string text = input.ToString();
            if (text.Contains(fchar))
            {
                for (i = text.Length - 1; i >= 0; i--)
                {
                    if (!passed && text[i] != fchar[0]) sb.Append(text[i]);
                    else if (text[i] == fchar[0])
                    {
                        sb.Append(text[i]);
                        passed = true;
                    }
                    if (passed && text[i] != fchar[0])
                    {
                        sb.Append(text[i]);
                        if (j % 3 == 0) sb.Append(gchar);
                        j++;
                    }
                }
                ar = sb.ToString().ToCharArray();
                Array.Reverse(ar);
                return new string(ar).Trim();
            }
            else
            {
                for (i = text.Length - 1; i >= 0; i--)
                {
                    sb.Append(text[i]);
                    if (j % 3 == 0) sb.Append(gchar);
                    j++;
                }
                ar = sb.ToString().ToCharArray();
                Array.Reverse(ar);
                return new string(ar).Trim();
            }
        }

        private string FormatComplex(Complex c)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Algebric: {0} + {1}*i\tAbs: {2}\tPhase: {3} rad\r\n", c.Real, c.Imaginary, c.Magnitude, c.Phase);
            sb.AppendFormat("Trigonometric: {0} * ({1} + i*{2})", c.Magnitude, Math.Cos(c.Phase), Math.Sin(c.Phase));
            return sb.ToString();
        }

        private string DisplayString(object o)
        {
            Type t = o.GetType();
            switch (t.Name)
            {
                case "Byte":
                case "SByte":
                case "Int16":
                case "Int32":
                case "Int64":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                case "Double":
                case "Single":
                    return FormatDouble(Convert.ToDouble(o));
                case "Complex":
                    return FormatComplex((Complex)o);
                default:
                    if (t.IsArray)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Array of " + t.Name + " {\n");
                        foreach (object x in (object[])o)
                        {
                            sb.Append(x.ToString());
                            sb.Append("\n");
                        }
                        sb.Append("}");
                        return sb.ToString();
                    }
                    else if (o is IEnumerable)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Collection {\n");
                        IEnumerable coll = (IEnumerable)o;
                        foreach (var i in coll)
                        {
                            sb.Append(i.ToString());
                            sb.Append("\n");
                        }
                        sb.Append("}");
                        return sb.ToString();
                    }
                    else if ((o is IronPython.Runtime.PythonFunction) || (o is IronPython.Runtime.Types.BuiltinFunction))
                    {
                        return "This is a function.";
                    }
                    else return o.ToString();
            }
        }

        public string[] VariablesNames
        {
            get
            {
                return _scope.GetVariableNames().ToArray();
            }
        }

        public dynamic GetVariable(string name)
        {
            return _scope.GetVariable(name);
        }

        public void ClearVariables()
        {
            foreach (var v in _scope.GetVariableNames())
            {
                _scope.RemoveVariable(v);
            }
        }
       
        public void DeleteVariable(string Name)
        {
            _scope.RemoveVariable(Name);
        }

        public void Run(string input)
        {
            try
            {
                Trigonometry.Mode = TrigMode;
                ScriptSource source = _engine.CreateScriptSourceFromString(input, SourceCodeKind.AutoDetect);
                object result = source.Execute(_scope);
                _scope.SetVariable("ans", result);
                if (result != null)
                {
                    _terminal.CurrentForeground = _terminal.OkFontColor;
                    _terminal.WriteLine(DisplayString(result));
                    _terminal.CurrentForeground = _terminal.DefaultFontColor;
                }
            }
            catch (Exception ex)
            {
                _terminal.WriteError(ex);
            }
        }

        public IConsole Terminal
        {
            get { return _terminal; }
        }

        public TrigMode TrigMode
        {
            get;
            set;
        }

        public static Dictionary<string, string> GetClassMembers(Type t)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            List<string> members = new List<string>();
            var methods = (from i in t.GetMethods() where i.IsPublic && i.IsStatic select i.Name).ToArray();
            //var props = (from i in t.GetProperties() select i.Name).ToArray();
            var consts = (from i in t.GetFields(BindingFlags.Public | BindingFlags.Static) where i.IsLiteral && !i.IsInitOnly select i.Name).ToArray();
            members.AddRange(methods);
            //members.AddRange(props);
            members.AddRange(consts);
            foreach (var m in members)
            {
                if (ret.ContainsKey(m)) continue;
                ret.Add(m, string.Format("{0}.{1}", t.Name, m));
            }
            return ret;
        }

        protected virtual void Dispose(bool native)
        {
            if (_outp != null)
            {
                _outp.Dispose();
                _outp = null;
            }
            if (_history != null)
            {
                _history.Dispose();
                _history = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
