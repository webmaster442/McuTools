/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : TabViewCollection.cs
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
 *    Collection of TabView elements.
 *    
 *    
 ********************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace TabbedWPFSample
{
    class TabViewCollection : ObservableCollection<TabView>
    {
        public void RaiseResetCollection()
        {
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }
    }
}
