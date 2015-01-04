using McuShell.Kernel;
using System;
using System.IO;

namespace ls
{
    class LS
    {
        static uint PrintArray(string[] array, bool files = true, FileType types = FileType.All)
        {
            uint counter = 0;
            if (files)
            {
                foreach (var item in array)
                {
                    FileInfo fi = new FileInfo(item);
                    var currentcolor = Console.ForegroundColor;
                    var type = Kernel.GetFileType(item);

                    if (types != FileType.All)
                    {
                        if (types != type) continue;
                    }

                    switch (type)
                    {
                        case FileType.Archive:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case FileType.Audio:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case FileType.Document:
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            break;
                        case FileType.Executable:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case FileType.Image:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            break;
                        case FileType.Video:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        default:
                            Console.ForegroundColor = currentcolor;
                            break;
                    }

                    Console.WriteLine("{0,-30}\t{1,-10}\t{2,-20}\t{3,-10}", fi.Name, Kernel.GetFileSize(fi.Length), fi.LastWriteTime, type);

                    Console.ForegroundColor = currentcolor;
                    ++counter;
                }
            }
            else
            {
                var currentcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                foreach (var item in array)
                {
                    DirectoryInfo di = new DirectoryInfo(item);
                    Console.WriteLine("{0,-30}\t{1,-10}\t{2,-20}", di.Name, "<dir>", di.LastWriteTime);
                    ++counter;
                }
                Console.ForegroundColor = currentcolor;
            }
            return counter;
        }

        class Options
        {
            [EnumArgument(ShortName="f", LongName="filter", Required=false, Description="Sets filter type.")]
            public FileType FilterType { get; set; }
        }

        static void Main(string[] args)
        {
            CommandParser.CommandDescription = "List Directory files & subdirectories";
            Options o = new Options();
            o.FilterType = FileType.All;
            CommandParser.Parse(o, args);

            string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.*");
            string[] dirs = Directory.GetDirectories(Environment.CurrentDirectory, "*.*");


            uint dc = PrintArray(dirs, false);
            uint fc = PrintArray(files, true, o.FilterType);
            Console.WriteLine("Total Files: {0}\t Total Dirs: {1}\t Filter: {2}", fc, dc, o.FilterType);
            Kernel.DebugWait();
        }
    }
}
