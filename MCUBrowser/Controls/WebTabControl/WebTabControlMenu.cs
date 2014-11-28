/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : WebTabControlMenu.cs
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
 *    Subclassed ItemsControl needed to handle TabViews as data providers.
 *    We can then visualize data as Menu items. This is used in the
 *    opened tabs menu.
 *    
 *    
 ********************************************************************************/

using System;
using System.Windows.Controls;

namespace TabbedWPFSample
{
    class WebTabControlMenu : ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride( object item )
        {
            // This is needed to avoid the:
            // System.Windows.Data Error: 26 : ItemTemplate and ItemTemplateSelector are ignored for items already of the ItemsControl's container type;
            // This error occurs because the items we try to add to the ItemsControl, are already UIElements, therefore, they do not
            // need a DataTemplate to be visualized. In such cases, the ItemsControl ignores the DataTemplate defined in ItemTemplate.
            // In our case, we feed this ItemsControl with TabViews. We do not want these to be removed from the visual tree and added in the popup
            // (which is what would happen without this override)! We want our ItemTemplate to be respected.
            return false;
        }
    }
}
