using McuShell.Kernel;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace wget
{
    class wget
    {
        class Settings
        {
            [ParameterArgument(ShortName = "u", LongName = "url", Required = true, Description = "URL to download")]
            public string url { get; set; }

            [ParameterArgument(ShortName = "f", LongName = "file", Required = false, Description = "Target file")]
            public string target { get; set; }
        }

        private static string GetFileName(string uri)
        {
            string[] parts = uri.Split('/');
            return parts[parts.Length - 1];
        }

        private static DateTime start;

        static void Main(string[] args)
        {
            Settings settings = new Settings();
            CommandParser.CommandDescription = "Downloads files from the internet";
            CommandParser.Parse(settings, args);

            WebClient webclient = new WebClient();
            webclient.DownloadFileCompleted += webclient_DownloadFileCompleted;
            webclient.DownloadProgressChanged += webclient_DownloadProgressChanged;
            try
            {
                if (string.IsNullOrEmpty(settings.target)) settings.target = GetFileName(settings.url);
                if (File.Exists(settings.target)) File.Move(settings.target, settings.target + ".old");
                Console.WriteLine("Downloading: ");
                start = DateTime.Now;
                webclient.DownloadFileAsync(new Uri(settings.url), settings.target);
            }
            catch (Exception ex)
            {
                CommandParser.Error(ex);
            }
#if DEBUG
            Console.ReadKey();
#endif
        }

        private static void webclient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Write("\rProgress: {0}% ", e.ProgressPercentage);
        }

        private static void webclient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Console.WriteLine("\r\nDownload complete");
            Console.WriteLine("Download time: {0}", DateTime.Now - start);
        }

    }
}
