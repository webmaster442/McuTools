/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : TabView.cs
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
 *    Represents the contents of a tab in an application window. This control
 *    contains the WebControl and an independent bar with the address-box,
 *    navigation buttons etc. It is lookless and the only needed part (the
 *    WebControl) is exposed as template part so that the control can be easily
 *    styled. The default style is defined in Themes/TabView.xaml.
 *    
 *    
 ********************************************************************************/

#region Using
using System;
using System.Linq;
using System.Windows;
using Awesomium.Core;
using System.Windows.Media;
using System.Windows.Controls;
using Awesomium.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Automation.Peers;
#endregion

namespace TabbedWPFSample
{
    [TemplatePart( Name = TabView.ElementBrowser, Type = typeof( WebControl ) )]
    class TabView : Control
    {
        #region Constants
        public const String ElementBrowser = "PART_Browser";

        private const String JS_FAVICON = "(function(){ " +
            "links = document.getElementsByTagName('link'); " +
            "wHref = window.location.protocol + '//' + window.location.hostname + '/favicon.ico'; " +
            "for(i=0; i<links.length; i++){s=links[i].rel; if(s.indexOf('icon') != -1){ wHref = links[i].href }; }; " +
            "return wHref; })();";
        #endregion

        #region Fields
        private MainWindow parentWindow;
        #endregion


        #region Ctors
        static TabView()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( TabView ), new FrameworkPropertyMetadata( typeof( TabView ) ) );
        }

        public TabView()
        {
            // Parameterless public constructor allows the TabView
            // to be used in XAML (for popup windows).
        }

        internal TabView( MainWindow parent, Uri url, bool isSourceView )
        {
            parentWindow = parent;
            this.IsSourceView = isSourceView;
            this.Source = url;
        }

        internal TabView( MainWindow parent, IntPtr nativeView )
        {
            parentWindow = parent;
            this.NativeView = nativeView;
        }
        #endregion


        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.SetValue( TabView.BrowserPropertyKey, GetTemplateChild( ElementBrowser ) );

            // Verify we have the control from the template.
            if ( Browser == null )
                return;

            // Monitor some important events.
            Browser.ShowCreatedWebView += OnShowNewView;
            Browser.LoadingFrame += OnBeginLoading;
            Browser.DocumentReady += OnDomReady;
            Browser.ConsoleMessage += OnConsoleMessage;
            Browser.WindowClose += OnWindowClose;
            // Make sure the control's template is applied.
            // This will immediately create the underlying web-view
            // instance, or, if this control wraps a created child view,
            // initialize the view and start listening to events.
            // This is important if this tab hosts a WebControl
            // that wraps a created child view: This ensures that
            // the created native view will be wrapped before
            // the ShowCreatedWebView handler (below) returns.
            Browser.ApplyTemplate();
        }

        protected override void OnGotFocus( RoutedEventArgs e )
        {
            base.OnGotFocus( e );

            // Always make sure that when the container of a WebControl
            // gets focus, it moves focus to the WebControl.
            if ( Browser != null )
                Browser.Focus();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Closes the view and performs cleanup. Must be called by container.
        /// </summary>
        public void Close()
        {
            this.CommandBindings.Clear();

            if ( Browser != null )
            {
                using ( Browser )
                {
                    Browser.ShowCreatedWebView -= OnShowNewView;
                    Browser.LoadingFrame -= OnBeginLoading;
                    Browser.DocumentReady -= OnDomReady;
                    Browser.ConsoleMessage -= OnConsoleMessage;
                    Browser.WindowClose -= OnWindowClose;
                }
            }

            // Release references.
            this.ClearValue( TabView.BrowserPropertyKey );
            this.ClearValue( TabView.FaviconPropertyKey );
            this.ClearValue( TabView.IsSelectedProperty );
            parentWindow = null;
        }

        private void UpdateFavicon()
        {
            // Execute some simple javascript that will search for a favicon.
            string val = Browser.ExecuteJavascriptWithResult( JS_FAVICON );

            // Check for any errors.
            if ( Browser.GetLastError() != Error.None )
                return;

            // Check if we got a valid response.
            if ( String.IsNullOrEmpty( val ) || !Uri.IsWellFormedUriString( val, UriKind.Absolute ) )
                return;

            BitmapDecoder decoder = BitmapDecoder.Create( val.ToUri(), BitmapCreateOptions.None, BitmapCacheOption.None );

            if ( decoder.IsDownloading )
                decoder.DownloadCompleted += ( s, e ) => UpdateFavicon( s as BitmapDecoder );
            else
                UpdateFavicon( decoder );
        }

        private void UpdateFavicon( BitmapDecoder decoder )
        {
            if ( ( decoder == null ) || ( decoder.Frames.Count == 0 ) )
                return;

            this.SetValue( 
                TabView.FaviconPropertyKey, 
                decoder.Frames.FirstOrDefault( f => f.PixelWidth == 16 || f.PixelHeight == 16 ) ?? 
                decoder.Frames[ 0 ] );
        }
        #endregion

        #region Properties

        #region ParentWindow
        // Gets the application's main window. This is passed
        // to us when the TabView is hosted by the main window,
        // but we may need to acquire it if the TabView is hosted
        // in a popup window.
        public MainWindow ParentWindow
        {
            get
            {
                if ( parentWindow == null )
                    parentWindow = this.FindAncestor<MainWindow>();

                return parentWindow;
            }
        }
        #endregion

        #region Browser
        // The WebConrol in the TabView's template.
        public WebControl Browser
        {
            get { return (WebControl)this.GetValue( TabView.BrowserProperty ); }
        }

        private static readonly DependencyPropertyKey BrowserPropertyKey =
            DependencyProperty.RegisterReadOnly( "Browser",
            typeof( WebControl ), typeof( TabView ),
            new FrameworkPropertyMetadata( null ) );

        public static readonly DependencyProperty BrowserProperty =
            BrowserPropertyKey.DependencyProperty;
        #endregion

        #region Source
        // The WebControl's Source property is bound to this.
        public Uri Source
        {
            get { return this.GetValue( SourceProperty ) as Uri; }
            set { SetValue( SourceProperty, value ); }
        }

        public static readonly DependencyProperty SourceProperty = 
            DependencyProperty.Register( "Source",
            typeof( Uri ), typeof( TabView ),
            new UIPropertyMetadata( null ) );
        #endregion

        #region NativeView
        // The WebControl's NativeView property is bound to this.
        // Used when the TabView hosts a created child view.
        public IntPtr NativeView
        {
            get { return (IntPtr)this.GetValue( NativeViewProperty ); }
            set { SetValue( NativeViewProperty, value ); }
        }

        public static readonly DependencyProperty NativeViewProperty = 
            DependencyProperty.Register( "NativeView",
            typeof( IntPtr ), typeof( TabView ),
            new UIPropertyMetadata( IntPtr.Zero ) );
        #endregion

        #region Favicon
        // The tab's image is bound to this property. We acquire the favicon
        // by executing JavaScript on every page loaded.
        public ImageSource Favicon
        {
            get { return (ImageSource)this.GetValue( TabView.FaviconProperty ); }
        }

        private static readonly DependencyPropertyKey FaviconPropertyKey =
            DependencyProperty.RegisterReadOnly( "Favicon",
            typeof( ImageSource ), typeof( TabView ),
            new FrameworkPropertyMetadata( null ) );

        public static readonly DependencyProperty FaviconProperty =
            FaviconPropertyKey.DependencyProperty;
        #endregion

        #region IsSelected
        // This property is bound to the IsSelected property of the
        // WebTabItem that hosts us. It allows us to inform the parent
        // window of the currently selected tab, at any given time.
        // As the binding is a TwoWay binding, this also allows us
        // to update the selected tab from code.
        public bool IsSelected
        {
            get { return (bool)this.GetValue( IsSelectedProperty ); }
            set { SetValue( IsSelectedProperty, value ); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register( "IsSelected",
            typeof( bool ), typeof( TabView ),
            new FrameworkPropertyMetadata( false, IsSelectedChanged ) );

        private static void IsSelectedChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            TabView owner = (TabView)d;
            bool value = (bool)e.NewValue;

            if ( value && ( owner.ParentWindow != null ) )
                owner.ParentWindow.SelectedView = owner;
        }
        #endregion

        #region IsRegularTab
        // Indicates if this TabView is part of tab or of a popup window.
        // When in a popup window, we do not display the toolbox and address-box.
        public bool IsRegularTab
        {
            get { return (bool)this.GetValue( IsRegularTabProperty ); }
            set { SetValue( IsRegularTabProperty, value ); }
        }

        public static readonly DependencyProperty IsRegularTabProperty = 
            DependencyProperty.Register( "IsRegularTab",
            typeof( bool ), typeof( TabView ),
            new UIPropertyMetadata( true ) );
        #endregion

        #region IsSourceView
        // Indicates if this tab presents a source view.
        public bool IsSourceView
        {
            get { return (bool)GetValue( IsSourceViewProperty ); }
            internal set { SetValue( IsSourceViewPropertyKey, value ); }
        }

        private static readonly DependencyPropertyKey IsSourceViewPropertyKey = 
            DependencyProperty.RegisterReadOnly( "IsSourceView",
            typeof( bool ), typeof( TabView ),
            new FrameworkPropertyMetadata( false ) );

        public static readonly DependencyProperty IsSourceViewProperty =
            IsSourceViewPropertyKey.DependencyProperty;
        #endregion

        #endregion

        #region Event Handlers
        // ShowCreatedWebView handler. This event must always be handled.
        private void OnShowNewView( object sender, ShowCreatedWebViewEventArgs e )
        {
            WebControl view = sender as WebControl;

            if ( view == null )
                return;

            if ( !view.IsLive )
                return;

            // Treat popups differently. If IsPopup is true, the event is always
            // the result of 'window.open' (IsWindowOpen is also true, so no need to check it).
            // Our application does not recognize user defined, non-standard specs. 
            // Therefore child views opened with non-standard specs, will not be presented as 
            // popups but as regular new windows (still wrapping the child view however -- se below).
            if ( e.IsPopup && !e.IsUserSpecsOnly )
            {
                // JSWindowOpenSpecs.InitialPosition indicates screen coordinates.
                Int32Rect screenRect = e.Specs.InitialPosition.GetInt32Rect();

                // Set the created native view as the underlying view of the
                // WebControl. This will maintain the relationship between
                // the parent view and the child, usually required when the new view
                // is the result of 'window.open' (JS can access the parent window through
                // 'window.opener'; the parent window can manipulate the child through the 'window'
                // object returned from the 'window.open' call).
                PopupWindow newWindow = new PopupWindow( e.NewViewInstance )
                {
                    /* Set a border-style to indicate a popup.*/
                    WindowStyle = WindowStyle.ToolWindow,
                    /* Set resizing mode depending on the indicated specs.*/
                    ResizeMode = e.Specs.Resizable ? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize
                };

                // If the caller has not indicated a valid size for the new popup window,
                // let it be opened with the default size specified at design time.
                if ( ( screenRect.Width > 0 ) && ( screenRect.Height > 0 ) )
                {
                    // Assign the indicated size.
                    newWindow.Width = screenRect.Width;
                    newWindow.Height = screenRect.Height;
                }

                // Show the window.
                newWindow.Show();

                // If the caller has not indicated a valid position for the new popup window,
                // let it be opened in the default position specified at design time.
                if ( ( screenRect.Y > 0 ) && ( screenRect.X > 0 ) )
                {
                    // Move it to the indicated coordinates.
                    newWindow.Top = screenRect.Y;
                    newWindow.Left = screenRect.X;
                }
            }
            else if ( e.IsWindowOpen || e.IsPost )
            {
                // No specs or only non-standard specs were specified, but the event is still 
                // the result of 'window.open' or of an HTML form with tagret="_blank" and method="post".
                // We will open a normal window but we will still wrap the new native child view, 
                // maintaining its relationship with the parent window.
                ParentWindow.OpenURL( e.NewViewInstance );
            }
            else
            {
                // The event is not the result of 'window.open' or of an HTML form with tagret="_blank" 
                // and method="post"., therefore it's most probably the result of a link with target='_blank'. 
                // We will not be wrapping the created view; we let the WebControl hosted in ChildWindow 
                // create its own underlying view. Setting Cancel to true tells the core to destroy the 
                // created child view.
                //
                // Why don't we always wrap the native view passed to ShowCreatedWebView?
                //
                // - In order to maintain the relationship with their parent view,
                // child views execute and render under the same process (awesomium_process)
                // as their parent view. If for any reason this child process crashes,
                // all views related to it will be affected. When maintaining a parent-child 
                // relationship is not important, we prefer taking advantage of the isolated process 
                // architecture of Awesomium and let each view be rendered in a separate process.
                e.Cancel = true;
                // Note that we only explicitly navigate to the target URL, when a new view is 
                // about to be created, not when we wrap the created child view. This is because 
                // navigation to the target URL (if any), is already queued on created child views. 
                // We must not interrupt this navigation as we would still be breaking the parent-child
                // relationship.
                ParentWindow.OpenURL( e.TargetURL );
            }
        }

        private void OnWindowClose( object sender, WindowCloseEventArgs e )
        {
            if ( IsRegularTab )
            {
                // Ask the user and invoke the CloseTab command.
                if ( MessageBox.Show( ParentWindow,
                    "The page is asking to close this tab. Do you confirm?",
                    Browser.Source.AbsoluteUri,
                    MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
                    MainWindow.CloseTab.Execute( this, this );
            }
            else
            {
                // It's a popup. Don't ask. Find the parent window
                // and close it.
                Window parentWindow = this.FindAncestor<Window>();

                if ( parentWindow != null )
                    parentWindow.Close();
            }
        }

        private void OnConsoleMessage( object sender, ConsoleMessageEventArgs e )
        {
            // Display JavaScript console messages.
            System.Diagnostics.Debug.Print( String.Format( "{0} - Line: {1}", e.Message, e.LineNumber ) );
        }

        private void OnBeginLoading( object sender, LoadingFrameEventArgs e )
        {
            // By now we have already navigated to the address.
            // Clear the old favicon. The default style, will assign
            // a default (globe) icon to the tab when null is set for
            // FaviconProperty.
            if ( e.IsMainFrame )
                this.ClearValue( TabView.FaviconPropertyKey );
        }

        private void OnDomReady( object sender, EventArgs e )
        {
            // DOM is ready. We can start looking for a favicon.
            UpdateFavicon();
        }
        #endregion
    }
}
