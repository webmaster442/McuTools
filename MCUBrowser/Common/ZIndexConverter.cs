/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : ZIndexConverter.cs
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
 *    Utility converter that handles the z-order of newly added tabs.
 *    
 *    
 ********************************************************************************/

using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;

namespace TabbedWPFSample
{
    class ZIndexConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( ( value != null ) && ( value is UIElement ) )
            {
                UIElement element = (UIElement)value;
                ItemsControl parent = element.FindAncestor<ItemsControl>();

                if ( parent != null )
                {
                    return -1 * parent.Items.IndexOf( element );
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return Binding.DoNothing;
        }
    }
}
