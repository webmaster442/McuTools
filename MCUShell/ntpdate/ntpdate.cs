using McuShell.Kernel;
using System;

namespace ntpdate
{
    class ntpdate
    {
        class settings
        {
            [ParameterArgument(ShortName="s", LongName="server", Required=false, Description="NTP server to use")]
            public string ntpserver { get; set; }
        }

        static bool QueryDate(string server)
        {
            try
            {
                Console.WriteLine("Trying: {0}...", server);
                DateTime local = DateTime.Now;
                DateTime ntpdate = NtpClient.GetNetworkTime(server);
                Console.WriteLine(Kernel.FormatDateTime(ntpdate, "NTP Date & Time"));
                TimeSpan ts = local - ntpdate;
                Console.WriteLine("Local clock drift: {0} ms", ts.TotalMilliseconds);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting: {0}", server);
                Console.WriteLine("Reason: {0}", ex.Message);
                return false;
            }
        }


        static void Main(string[] args)
        {
            settings s = new settings();
            CommandParser.CommandDescription = "Gets the current date & time using the NTP protocol";
            CommandParser.Parse(s, args);

            string[] servers = 
            {
                "time.windows.com",
                "time.nist.gov",
                "pool.ntp.org"
            };

            if (!string.IsNullOrEmpty(s.ntpserver))
            {
                QueryDate(s.ntpserver);
                Kernel.DebugWait();
                return;
            }

            foreach (var server in servers)
            {
                if (QueryDate(server)) break;
            }
            Kernel.DebugWait();
        }
    }
}
