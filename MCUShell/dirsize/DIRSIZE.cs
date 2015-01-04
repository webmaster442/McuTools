using McuShell.Kernel;
using System;
using System.IO;

namespace dirsize
{
    class DIRSIZE
    {
        static long Files = 0;
        static long Dirs = 0;

        static long Size(string path)
        {
            long ret = 0;
            try
            {
                foreach (var file in Directory.EnumerateFiles(path))
                {
                    FileInfo fi = new FileInfo(file);
                    ret += fi.Length;
                    ++Files;
                }
                foreach (var directory in Directory.EnumerateDirectories(path))
                {
                    ret += Size(directory);
                    ++Dirs;
                }
                return ret;
            }
            catch (Exception ex)
            {
                CommandParser.Error(ex);
                return 0;
            }
        }

        class settings
        {
            [ParameterArgument(ShortName="d", LongName="directory", Required=true, Description="Source directory")]
            public string Directory { get; set; }
        }

        static void Main(string[] args)
        {
            settings s = new settings();
            CommandParser.CommandDescription = "Calculates the size of a direcotry";
            CommandParser.Parse(s, args);

            Console.WriteLine("Calculating size...");
            long size = Size(s.Directory);
            Console.WriteLine("Directory size:\t\t{0}", Kernel.GetFileSize(size));
            Console.WriteLine("Subdirectories:\t\t{0}", Dirs);
            Console.WriteLine("Files:\t\t\t{0}", Files);
            Kernel.DebugWait();
        }
    }
}
