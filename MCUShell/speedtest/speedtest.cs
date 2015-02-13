using McuShell.Kernel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace speedtest
{
    class speedtest
    {
        class settings
        {
            [ParameterArgument(ShortName = "d", LongName = "drive", Required = false, Description = "NTP server to use")]
            public string drive { get; set; }
        }

        private static double WriteTest(long size, string drive, string name)
        {
            using (var stream = File.Create(Path.Combine(drive, name)))
            {
                byte[] buffer = new byte[8192];
                DateTime start = DateTime.Now;
                long written = 0;
                int cnt = buffer.Length;
                do
                {
                    if ((size - written) < 8192) cnt = (int)(size - written);
                    stream.Write(buffer, 0, cnt);
                }
                while (written < size);
                DateTime end = DateTime.Now;
                return size / (end - start).TotalSeconds;
            }
        }

        private static double ReadTest(string drive, string path)
        {
            using (var stream = File.OpenRead(Path.Combine(drive, path)))
            {
                byte[] buffer = new byte[8192];
                DateTime start = DateTime.Now;
                long read = 0;
                int cnt = 0;
                do
                {
                    cnt = stream.Read(buffer, 0, buffer.Length);
                    read += cnt;
                }
                while (cnt > 0);
                DateTime end = DateTime.Now;
                return read / (end - start).TotalSeconds;
            }
        }

        private const int Tests = 10;

        static void Main(string[] args)
        {
            settings s = new settings();
            CommandParser.CommandDescription = "Tests a drive read and write speed.";
            CommandParser.Parse(s, args);

            try
            {
                Dictionary<string, long> testfiles = new Dictionary<string, long>();
                double[] readtests, writetests;
                readtests = new double[Tests];
                writetests = new double[Tests];
                int i = 0;
                Random r = new Random();

                for (i = 0; i < Tests; i++)
                {
                    testfiles.Add(r.Next().ToString(), (i + 1 * 256 * 8192)); //2mb * file number
                }
                Console.WriteLine("Testing File Write....");
                i = 0;

                foreach (var testfile in testfiles)
                {
                    writetests[i] = WriteTest(testfile.Value, s.drive, testfile.Key);
                }

                Console.WriteLine("Testing File Read...");

                foreach (var testfile in testfiles)
                {
                    readtests[i] = ReadTest(s.drive, testfile.Key);
                }

                Console.WriteLine("---------------------------------------------------------------");
                Console.WriteLine("Test Results:");
                Console.WriteLine("---------------------------------------------------------------");
                for (i = 0; i < Tests; i++)
                {
                    Console.WriteLine("Write #{0}:\t{1} MiB/s", i + 1, writetests[i] / (long)FileSize.MegaByte);
                }
                Console.WriteLine("Average Write speed: {0} MiB/s", writetests.Average() / (long)FileSize.MegaByte);
                Console.WriteLine();

                for (i = 0; i < Tests; i++)
                {
                    Console.WriteLine("Read #{0}:\t{1} MiB/s", i + 1, readtests[i] / (long)FileSize.MegaByte);
                }
                Console.WriteLine("Average Read speed: {0} MiB/s", readtests.Average() / (long)FileSize.MegaByte);

                foreach (var file in testfiles)
                {
                    File.Delete(Path.Combine(s.drive, file.Key));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error");
                Console.WriteLine("{0}\r\nSource: {1}", ex.Message, ex.Source);
            }
            Kernel.DebugWait();
        }
    }
}
