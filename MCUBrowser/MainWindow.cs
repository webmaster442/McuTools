/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : MainWindow.cs
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
 *    Application window. This does not act as a main-parent window. 
 *    It's reusable. The application will exit when all windows are closed.
 *    Completely styled with a custom WindowChrome. Check the XAML file
 *    in Generic.xaml.
 *    
 *    
 ********************************************************************************/

#region Using
using System;
using System.Linq;
using System.Windows;
using Awesomium.Core;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using TabbedWPFSample.Properties;
using Awesomium.Windows.Controls;
#endregion

namespace TabbedWPFSample
{
    class MainWindow : Window
    {
        #region Fields
        private string[] initialUrls;
        private TabViewCollection tabViews;
        #endregion


        #region Ctors
        static MainWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( MainWindow ), new FrameworkPropertyMetadata( typeof( MainWindow ) ) );

            // We implement some elementary commands.
            // The shortcuts specified, are the same as in Chrome.
            OpenInTab = new RoutedUICommand(
                Properties.Resources.OpenInNewTab,
                "OpenInTab",
                typeof( MainWindow ),
                new InputGestureCollection( new KeyGesture[] { new KeyGesture( Key.Enter, ModifierKeys.Control ) } ) );
            OpenInWindow = new RoutedUICommand(
                Properties.Resources.OpenInNewWindow,
                "OpenInWindow",
                typeof( MainWindow ),
                new InputGestureCollection( new KeyGesture[] { new KeyGesture( Key.Enter, ModifierKeys.Shift ) } ) );
            NewTab = new RoutedUICommand(
                Properties.Resources.NewTab,
                "NewTab",
                typeof( MainWindow ),
                new InputGestureCollection( new KeyGesture[] { new KeyGesture( Key.T, ModifierKeys.Control ) } ) );
            CloseTab = new RoutedUICommand(
                Properties.Resources.CloseTab,
                "CloseTab",
                typeof( MainWindow ),
                new InputGestureCollection( new KeyGesture[] { new KeyGesture( Key.W, ModifierKeys.Control ) } ) );
            ShowDownloads = new RoutedUICommand(
                Properties.Resources.Downloads,
                "ShowDownloads",
                typeof( MainWindow ) );
            ShowSettings = new RoutedUICommand(
                Properties.Resources.Settings,
                "ShowSettings",
                typeof( MainWindow ) );
            OpenSource = new RoutedUICommand(
                Properties.Resources.ShowSource,
                "OpenSource",
                typeof( MainWindow ) );
        }

        public MainWindow( string[] args )
        {
            // Keep this. We will use it when we load.
            initialUrls = args;

            // Initialize collections.
            tabViews = new TabViewCollection();
            this.SetValue( MainWindow.ViewsPropertyKey, tabViews );
            this.SetValue( MainWindow.DownloadsPropertyKey, WebCore.Downloads );

            // Assign event handlers.
            this.Loaded += OnLoaded;

            // Assign command handlers.
            this.CommandBindings.Add( new CommandBinding( MainWindow.OpenInTab, OnOpenTab, CanOpen ) );
            this.CommandBindings.Add( new CommandBinding( MainWindow.OpenInWindow, OnOpenWindow, CanOpen ) );
            this.CommandBindings.Add( new CommandBinding( MainWindow.OpenSource, OnOpenSource, CanOpenSource ) );
            this.CommandBindings.Add( new CommandBinding( MainWindow.CloseTab, OnCloseTab ) );
            this.CommandBindings.Add( new CommandBinding( MainWindow.NewTab, OnNewTab ) );
            this.CommandBindings.Add( new CommandBinding( MainWindow.ShowDownloads, OnShowDownloads ) );
            this.CommandBindings.Add( new CommandBinding( MainWindow.ShowSettings, OnShowSettings ) );
            this.CommandBindings.Add( new CommandBinding( ApplicationCommands.Close, OnClose ) );

            // Perform lazy initialization of the WebCore.
            this.InitializeCore();
        }
        #endregion


        #region Overrides
        protected override void OnClosing( CancelEventArgs e )
        {
            // Hide during cleanup.
            this.Hide();

            // Close the views and perform cleanup
            // for every tab.
            foreach ( TabView view in tabViews )
                view.Close();

            tabViews.Clear();

            // We shutdown the core in
            // Application.OnExit.

            base.OnClosing( e );
        }
        #endregion

        #region Methods

        #region InitializeCore
        private void InitializeCore()
        {
            // We may be a new window in the same process.
            if ( !WebCore.IsInitialized )
            {
                // Setup WebCore with plugins enabled.            
                WebConfig config = new WebConfig
                {
                    HomeURL = Settings.Default.HomeURL,
                    LogPath = String.Format( "{0}{1}debug.log", My.Application.UserAppDataPath, System.IO.Path.DirectorySeparatorChar ),
                    // Let's gather some extra info for this sample.
                    LogLevel = LogLevel.Verbose,
                    ReduceMemoryUsageOnNavigation = true
                };

                WebCore.Initialize( config );
            }            

            WebCore.DownloadBegin += OnDownloadBegin;
        }
        #endregion

        #region OpenURL
        public void OpenURL( Uri url, bool isSource = false )
        {           
            tabViews.Add( new TabView( this, url, isSource ) );            
        }

        public void OpenURL( IntPtr nativeView )
        {
            tabViews.Add( new TabView( this, nativeView ) );
            // Make sure that the new tab is shown before this method returns.
            // This will force the tab's template to be applied, which will in turn
            // create and load the WebControl that wraps the new child view,
            // before the IWebView.ShowCreatedWebView handler returns.
            // (see also: TabView.OnApplyTemplate)
            Application.Current.DoEvents();
        }
        #endregion

        #endregion

        #region Properties

        #region Static
        public static RoutedUICommand OpenInTab { get; private set; }
        public static RoutedUICommand OpenInWindow { get; private set; }
        public static RoutedUICommand OpenSource { get; private set; }
        public static RoutedUICommand NewTab { get; private set; }
        public static RoutedUICommand CloseTab { get; private set; }
        public static RoutedUICommand ShowDownloads { get; private set; }
        public static RoutedUICommand ShowSettings { get; private set; }
        #endregion


        #region Views
        // Collection TabViews populating our WebTabControl.
        public IEnumerable<TabView> Views
        {
            get { return (IEnumerable<TabView>)this.GetValue( MainWindow.ViewsProperty ); }
        }

        private static readonly DependencyPropertyKey ViewsPropertyKey =
            DependencyProperty.RegisterReadOnly( "Views",
            typeof( IEnumerable<TabView> ), typeof( MainWindow ),
            new FrameworkPropertyMetadata( null ) );

        public static readonly DependencyProperty ViewsProperty =
            ViewsPropertyKey.DependencyProperty;
        #endregion

        #region Downloads
        // Note that this the core DownloadItem. Not the WPF subclass.
        // Otherwise we would not be able to set this to WebCore.Downloads.
        public IEnumerable<Awesomium.Core.DownloadItem> Downloads
        {
            get { return (IEnumerable<Awesomium.Core.DownloadItem>)this.GetValue( MainWindow.DownloadsProperty ); }
        }

        private static readonly DependencyPropertyKey DownloadsPropertyKey =
            DependencyProperty.RegisterReadOnly( "Downloads",
            typeof( IEnumerable<Awesomium.Core.DownloadItem> ), typeof( MainWindow ),
            new FrameworkPropertyMetadata( null ) );

        public static readonly DependencyProperty DownloadsProperty =
            DownloadsPropertyKey.DependencyProperty;
        #endregion

        #region DownloadsVisible
        // Shows/hides the DownloadsControl.
        public bool DownloadsVisible
        {
            get { return (bool)this.GetValue( MainWindow.DownloadsVisibleProperty ); }
            protected set { this.SetValue( MainWindow.DownloadsVisiblePropertyKey, value ); }
        }

        private static readonly DependencyPropertyKey DownloadsVisiblePropertyKey =
            DependencyProperty.RegisterReadOnly( "DownloadsVisible",
            typeof( bool ), typeof( MainWindow ),
            new FrameworkPropertyMetadata( false ) );

        public static readonly DependencyProperty DownloadsVisibleProperty =
            DownloadsVisiblePropertyKey.DependencyProperty;
        #endregion

        #region SelectedView
        // The currently selected tabView.
        public TabView SelectedView
        {
            get { return (TabView)this.GetValue( MainWindow.SelectedViewProperty ); }
            internal set { this.SetValue( MainWindow.SelectedViewPropertyKey, value ); }
        }

        private static readonly DependencyPropertyKey SelectedViewPropertyKey =
            DependencyProperty.RegisterReadOnly( "SelectedView",
            typeof( TabView ), typeof( MainWindow ),
            new FrameworkPropertyMetadata( null ) );

        public static readonly DependencyProperty SelectedViewProperty =
            SelectedViewPropertyKey.DependencyProperty;
        #endregion

        #endregion

        #region Event Handlers
        private void OnLoaded( object sender, RoutedEventArgs e )
        {
            // Just like any respectable browser, we are ready to respond
            // to command line arguments passed if our browser is set as
            // the default browser or when a user attempts to open an html
            // file or shortcut with our application.
            bool openUrl = false;
            if ( ( initialUrls != null ) && ( initialUrls.Length > 0 ) )
            {
                foreach ( String url in initialUrls )
                {
                    Uri absoluteUri;
                    Uri.TryCreate( url, UriKind.Absolute, out absoluteUri );

                    if ( absoluteUri != null )
                    {
                        this.OpenURL( absoluteUri );
                        openUrl = true;
                    }
                }

                initialUrls = null;
            }

            if ( !openUrl )
                this.OpenURL( Settings.Default.HomeURL );
        }

        private void OnOpenTab( object sender, ExecutedRoutedEventArgs e )
        {
            // If this is called from a menu item, the target URL is specified as a parameter.
            // If the user simply hit the shortcut, we need to get the target URL (if any) from the currently selected tab.
            // The same applies for the rest of link related commands below.
            string target = (string)e.Parameter ?? ( SelectedView != null ? SelectedView.Browser.LatestContextData.LinkURL.AbsoluteUri : String.Empty );

            if ( !String.IsNullOrWhiteSpace( target ) )
                this.OpenURL( target.ToUri() );
        }

        private void OnOpenWindow( object sender, ExecutedRoutedEventArgs e )
        {
            string target = (string)e.Parameter ?? ( SelectedView != null ? SelectedView.Browser.LatestContextData.LinkURL.AbsoluteUri : String.Empty );

            if ( !String.IsNullOrWhiteSpace( target ) )
            {
                // Open link in a new window.
                MainWindow win = new MainWindow( new string[] { target } );
                win.Show();
            }
        }

        private void CanOpen( object sender, CanExecuteRoutedEventArgs e )
        {
            string target = (string)e.Parameter ?? ( SelectedView != null ? SelectedView.Browser.LatestContextData.LinkURL.AbsoluteUri : String.Empty );
            e.CanExecute = !String.IsNullOrWhiteSpace( target );
        }

        private void OnOpenSource( object sender, ExecutedRoutedEventArgs e )
        {
            Uri target = (Uri)e.Parameter ?? ( SelectedView != null ? SelectedView.Browser.Source : null );

            if ( target != null )
                this.OpenURL( target, true );
        }

        private void CanOpenSource( object sender, CanExecuteRoutedEventArgs e )
        {
            Uri target = (Uri)e.Parameter ?? ( SelectedView != null ? SelectedView.Browser.Source : null );
            e.CanExecute = target != null;
        }

        private void OnNewTab( object sender, ExecutedRoutedEventArgs e )
        {
            this.OpenURL( Settings.Default.HomeURL );
        }

        private void OnCloseTab( object sender, ExecutedRoutedEventArgs e )
        {
            if ( ( e.Parameter != null ) && ( e.Parameter is TabView ) )
            {
                if ( tabViews.Count == 1 )
                    // This is the last tab we are attempting to close.
                    // Close the window. If this is the last window, the application exits.
                    this.Close();
                else
                {
                    TabView view = (TabView)e.Parameter;
                    // Remove the tab.
                    tabViews.Remove( view );
                    // Close the view and the WebControl.
                    view.Close();

                    GC.Collect();
                }
            }
        }

        private void OnDownloadBegin( object sender, DownloadBeginEventArgs e )
        {
            this.DownloadsVisible = true;
        }

        private void OnShowDownloads( object sender, ExecutedRoutedEventArgs e )
        {
            this.DownloadsVisible = true;
        }

        private void OnShowSettings( object sender, ExecutedRoutedEventArgs e )
        {
            // TODO: Show here a settings dialog.
            // If the changes made require a restart (such as changes to
            // the WebCore configuration), set My.Application.Restart
            // to true and close the application.
            return;
        }

        private void OnClose( object sender, ExecutedRoutedEventArgs e )
        {
            // The command we handle here is ApplicationCommands.Close. We need to maintain
            // the re-usability of this command, so we define a parameter for the downloads bar.
            if ( ( e.Parameter != null ) && ( String.Compare( e.Parameter.ToString(), "Downloads", true ) == 0 ) )
                this.DownloadsVisible = false;
        }
        #endregion
    }
}
