/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : WebTabItem.cs
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
 *    Re-styled tab item (header of a tab).
 *    
 *    
 ********************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;

namespace TabbedWPFSample
{
    class WebTabItem : TabItem
    {
        static WebTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( WebTabItem ), new FrameworkPropertyMetadata( typeof( WebTabItem ) ) );
        }
    }
}
