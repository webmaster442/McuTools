/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : App.xaml.cs
 *    Version   : 1.7.0.0 
 *    Date      : 3/5/2013
 *    Author    : Perikles C. Stephanidis (perikles@awesomium.com)
 *    Copyright : ©2013 Awesomium Technologies LLC
 *    
 *    This code is provided "AS IS" and for demonstration purposes only,
 *    without warranty of any kind.
 *     
 *-------------------------------------------------------------------------------
 *
 *    Notes     :
 *
 *    Application's entry point. Also handles Shutdown and attempts to launch
 *    a second instance of our application.
 *    
 *    
 ********************************************************************************/

using System;
using Awesomium.Core;
using System.Windows;
using System.Reflection;
using System.Diagnostics;
using TabbedWPFSample.Properties;

namespace TabbedWPFSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    partial class App : Application
    {
        // TODO: Implement Inspector

        /// <summary>
        /// We need this because our main window no longer has a 
        /// XAML file to be set as a StartupUri.
        /// We achieve this by simply changing the "Build Action"
        /// of App.xaml, from ApplicationDefinition to Page.
        /// </summary>
        [STAThread()]
        static void Main( string[] args )
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();

            // Changes to settings may require a restart.
            if ( My.Application.Restart )
                Process.Start( Assembly.GetEntryAssembly().CodeBase );
        }

        protected override void OnStartup( StartupEventArgs e )
        {
            // Force single instance application.
            WPFSingleInstance.Make( SecondInstance );

            this.MainWindow = new MainWindow( e.Args )
            {
                Width = 1024,
                Height = 768,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            this.MainWindow.Show();
        }

        protected override void OnExit( ExitEventArgs e )
        {
            // Make sure we shutdown the core last.
            if ( WebCore.IsInitialized )
                WebCore.Shutdown();

            base.OnExit( e );
        }

        private static void SecondInstance( object obj )
        {
            // When the user is trying to launch a new instance of the application,
            // we simply open a new window in this application.
            // No more than one WebCore can be started per process.
            MainWindow win = new MainWindow( new string[] { Settings.Default.HomeURL.AbsoluteUri } );
            win.Show();
        }
    }
}
