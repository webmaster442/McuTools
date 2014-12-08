using McuShell.Kernel;
using System;
using System.IO;

namespace df
{
    class Program
    {
        static void Main(string[] args)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                try
                {
                    string free = Kernel.GetFileSize(drive.TotalFreeSpace);
                    string total = Kernel.GetFileSize(drive.TotalSize);
                    double percent = (drive.TotalSize / (double)drive.TotalFreeSpace) * 100.0;
                    Console.WriteLine("{0}\t{1}\t{2}\t{3:0.000}", drive.Name, total, free, percent);
                }
                catch (IOException)
                {
                    Console.WriteLine("Drive {0}: I/O Error", drive);
                }
            }
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
