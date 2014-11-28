/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : DownloadsControl.cs
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
 *    A control presenting a list of download operations.
 *    
 *    
 ********************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Awesomium.Core;

namespace TabbedWPFSample
{
    internal class DownloadsControl : Control
    {
        static DownloadsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( DownloadsControl ), new FrameworkPropertyMetadata( typeof( DownloadsControl ) ) );
        }

        // Note that this the core DownloadItem. Not the WPF subclass.
        // Otherwise we would not be able to set this to WebCore.Downloads.
        // The DownloadsControl's template takes care of recognizing the
        // WPF DownloadItem items.
        public IEnumerable<DownloadItem> Source
        {
            get { return (IEnumerable<DownloadItem>)this.GetValue( SourceProperty ); }
            set { SetValue( SourceProperty, value ); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register( "Source",
            typeof( IEnumerable<DownloadItem> ), typeof( DownloadsControl ),
            new FrameworkPropertyMetadata( null ) );
    }
}
