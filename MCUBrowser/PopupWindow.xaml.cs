/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : PopupWindow.xaml.cs
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
 *    A popup window used to present pages loaded in response to a JavaScript
 *    window.open with specified specs.
 *    
 *    
 ********************************************************************************/

using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace TabbedWPFSample
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        public PopupWindow( IntPtr nativeView )
        {
            InitializeComponent();

            this.NativeView = nativeView;

            // Make sure the template of the tab is loaded.
            // This will consequently load the template of the
            // hosted WebControl and make sure that the child native
            // view is properly wrapped, before the ShowCreatedWebView 
            // handler returns (see TabView.OnApplyTemplate).
            tabView.ApplyTemplate();
        }

        protected override void OnClosed( EventArgs e )
        {
            base.OnClosed( e );
            // This will also dispose the WebControl.
            tabView.Close();
        }

        // The TabView's NativeView property and consequently the WebControl's
        // NativeView property, are bound to this.
        public IntPtr NativeView
        {
            get { return (IntPtr)this.GetValue( PopupWindow.NativeViewProperty ); }
            internal set { this.SetValue( PopupWindow.NativeViewPropertyKey, value ); }
        }

        private static readonly DependencyPropertyKey NativeViewPropertyKey = 
            DependencyProperty.RegisterReadOnly( "NativeView",
            typeof( IntPtr ), typeof( PopupWindow ),
            new FrameworkPropertyMetadata( IntPtr.Zero ) );

        public static readonly DependencyProperty NativeViewProperty =
            NativeViewPropertyKey.DependencyProperty;
    }
}
