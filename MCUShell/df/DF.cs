using McuShell.Kernel;
using System;
using System.Collections.Generic;
using System.IO;

namespace df
{
    class DF
    {
        static void Main(string[] args)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            Console.WriteLine("{0,-10}\t{1,-20}\t{2,-20}\t{3}", "Letter:", "Total capacity:", "Free space:", "% used:");
            Console.WriteLine("--------------------------------------------------------------------------------");
            List<string> notready = new List<string>();
            foreach (var drive in drives)
            {
                try
                {
                    string free = Kernel.GetFileSize(drive.TotalFreeSpace);
                    string total = Kernel.GetFileSize(drive.TotalSize);
                    double percent = (double)(drive.TotalSize - drive.TotalFreeSpace) / drive.TotalSize;
                    percent *= 100;
                    Console.WriteLine("{0,-10}\t{1,-20}\t{2,-20}\t{3:0.000}", drive.Name, total, free, percent);
                }
                catch (IOException)
                {
                    notready.Add(drive.Name);
                }
            }
            if (notready.Count > 0)
            {
                Console.WriteLine("\r\nNot available drives:");
                Console.WriteLine("--------------------------------------------------------------------------------");
                notready.WriteToConsole();
            }
            Kernel.DebugWait();
        }
    }
}
