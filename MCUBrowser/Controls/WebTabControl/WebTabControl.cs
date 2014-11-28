/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : WebTabControl.cs
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
 *    Re-styled TabControl that handles the size of its tabs.
 *    
 *    
 ********************************************************************************/

#region Using
using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using Microsoft.Windows.Shell;
#endregion

namespace TabbedWPFSample
{
    class WebTabControl : TabControl
    {
        private bool collectionChanging;

        #region Ctor
        static WebTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( WebTabControl ), new FrameworkPropertyMetadata( typeof( WebTabControl ) ) );
        }
        #endregion

        #region Overrides
        protected override void OnItemsChanged( NotifyCollectionChangedEventArgs e )
        {
            collectionChanging = true;
            base.OnItemsChanged( e );
            collectionChanging = false;

            if ( ( e.NewItems != null ) && ( e.NewItems.Count > 0 ) )
            {
                TabView view = (TabView)e.NewItems[ 0 ];
                this.SelectedItem = view;

                if ( !view.IsLoaded )
                {
                    RoutedEventHandler loaded = null;
                    loaded = ( sender, ea ) =>
                    {
                        view.Loaded -= loaded;
                        this.InvalidateMeasure();
                    };

                    view.Loaded += loaded;
                }
            }

            this.InvalidateMeasure();
        }

        protected override Size MeasureOverride( Size constraint )
        {
            Size size = base.MeasureOverride( constraint );

            if ( constraint.Width > 0 )
            {
                double totalWidth = this.Items.Count * 180;
                double rightPadding = SystemParameters2.Current.IsGlassEnabled ? 170 : 70;
                double finalWidth = 180;

                if ( totalWidth > ( constraint.Width - rightPadding ) )
                    finalWidth = ( constraint.Width - rightPadding ) / this.Items.Count;

                foreach ( TabView item in this.Items )
                {
                    WebTabItem container = this.ItemContainerGenerator.ContainerFromItem( item ) as WebTabItem;

                    if ( ( container != null ) && ( container.ActualWidth > 0 ) )
                        container.Width = finalWidth;
                }
            }

            return size;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WebTabItem();
        }

        protected override bool IsItemItsOwnContainerOverride( object item )
        {
            return item is WebTabItem;
        }

        protected override void OnSelectionChanged( SelectionChangedEventArgs e )
        {
            base.OnSelectionChanged( e );

            // Prevent the TabControl from staying with no selected item.
            // This can occur is we try to un-select the selected tab from code,
            // or by clicking it in the tabs menu.
            if ( !collectionChanging && ( this.Items.Count > 0 ) && ( e.AddedItems.Count == 0 ) && ( this.SelectedIndex < 0 ) )
                ( (TabView)this.Items[ 0 ] ).IsSelected = true;

        }
        #endregion
    }
}
