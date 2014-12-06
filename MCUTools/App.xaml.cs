using Awesomium.Core;
using McuTools.Browser;
using McuTools.Classes;
using McuTools.Interfaces;
using McuTools.Interfaces.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace McuTools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        internal static List<Tool> _Tools;
        internal static List<ExternalTool> _ExtTools;
        internal static List<WebTool> _WebTools;
        internal static List<PopupTool> _Popups;
        //internal static JumplistHelper _Jumplist;
        internal static UserConfiguration _Config;
        internal static IntPtr MainWindowHandle;

        private const string Unique = "MCUTools";
       
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
            //_Jumplist = new JumplistHelper();
        }

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                Functions.SetupProfile();

                long cache = Functions.CalculateCacheSize();

                if (cache > 20 * 1024 * 1024) Functions.DeleteBrowserCache(true);

                var application = new App();

                WebCore.Initialize(new WebConfig()
                {
                    HomeURL = new Uri("asset://mcutools/Index.html"),
                    LogLevel = LogLevel.Verbose,
                });

                application.InitializeComponent();
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            string filename = string.Format("crashlog_{0}_{1}_{2}_{3}_{4}_{5}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            using (var f = File.CreateText(Path.Combine(Folders.Local, filename)))
            {
                f.WriteLine("---------------------------------------------");
                f.WriteLine("Application Crash: {0}", DateTime.Now);
                f.WriteLine("---------------------------------------------");
                f.WriteLine("Message: {0}\r\nSource: {1}", ex.Message, ex.Source);
                f.WriteLine("Exception type: {0}", ex.GetType().FullName);
                f.WriteLine("---------------------------------------------");
                f.WriteLine("Target Site:");
                f.WriteLine(ex.TargetSite);
                f.WriteLine("---------------------------------------------");
                f.WriteLine("Stack Trace:");
                f.WriteLine(ex.StackTrace);
                f.WriteLine("---------------------------------------------");
            }
            Process P = new Process();
            P.StartInfo.FileName = Path.Combine(Folders.Local, filename);
            P.Start();
            MessageBox.Show("Application crashed\r\nDetails saved to crashlog.txt");
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }
    }
}
