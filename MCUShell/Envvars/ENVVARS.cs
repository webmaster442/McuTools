using McuShell.Kernel;
using System;
using System.Collections;

namespace envvars
{
    class ENVVARS
    {

        class options
        {
            [ParameterArgument(ShortName = "f", LongName = "filter", Required = false, Description = "Filter string. Only show variable names which contains the filter")]
            public string filter { get; set; }
        }

        static void Main(string[] args)
        {
            options opt = new options();
            CommandParser.CommandDescription = "List Command line variables";
            CommandParser.Parse(opt, args);
            int cnt = 0;

            var envvars = Environment.GetEnvironmentVariables();

            foreach (DictionaryEntry env in envvars)
            {
                string key = env.Key.ToString();
                if (!string.IsNullOrEmpty(opt.filter))
                {
                    if (!key.ToLower().Contains(opt.filter.ToLower())) continue;
                }
                Console.WriteLine("%{0}%\r\n\t{1}", env.Key, env.Value);
                Console.WriteLine();
                ++cnt;
            }
            Console.WriteLine("Total: {0}, Filter: {1}", cnt, opt.filter);
            Kernel.DebugWait();
        }
    }
}
