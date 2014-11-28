using Awesomium.Core;
using Awesomium.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace McuTools.Browser
{
    internal static class StacicBrowser
    {

        // This static handler, will handle the ShowCreatedWebView event for both the 
        // WebControl of our main application window, as well as for any other windows
        // hosting WebControls.
        internal static void OnShowNewView(object sender, ShowCreatedWebViewEventArgs e)
        {
            WebControl webControl = sender as WebControl;

            if (webControl == null)
                return;

            if (!webControl.IsLive)
                return;

            // Create an instance of our application's child window, that will
            // host the new view instance, either we wrap the created child view,
            // or we let the WebControl create a new underlying web-view.
            ChildWindow newWindow = new ChildWindow();

            // Treat popups differently. If IsPopup is true, the event is always
            // the result of 'window.open' (IsWindowOpen is also true, so no need to check it).
            // Our application does not recognize user defined, non-standard specs. 
            // Therefore child views opened with non-standard specs, will not be presented as 
            // popups but as regular new windows (still wrapping the child view however -- se below).
            if (e.IsPopup && !e.IsUserSpecsOnly)
            {
                // JSWindowOpenSpecs.InitialPosition indicates screen coordinates.
                Int32Rect screenRect = e.Specs.InitialPosition.GetInt32Rect();

                // Set the created native view as the underlying view of the
                // WebControl. This will maintain the relationship between
                // the parent view and the child, usually required when the new view
                // is the result of 'window.open' (JS can access the parent window through
                // 'window.opener'; the parent window can manipulate the child through the 'window'
                // object returned from the 'window.open' call).
                newWindow.NativeView = e.NewViewInstance;
                // Do not show in the taskbar.
                newWindow.ShowInTaskbar = false;
                // Set a border-style to indicate a popup.
                newWindow.WindowStyle = WindowStyle.ToolWindow;
                // Set resizing mode depending on the indicated specs.
                newWindow.ResizeMode = e.Specs.Resizable ? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize;

                // If the caller has not indicated a valid size for the new popup window,
                // let it be opened with the default size specified at design time.
                if ((screenRect.Width > 0) && (screenRect.Height > 0))
                {
                    // Assign the indicated size.
                    newWindow.Width = screenRect.Width;
                    newWindow.Height = screenRect.Height;
                }

                // Show the window.
                newWindow.Show();

                // If the caller has not indicated a valid position for the new popup window,
                // let it be opened in the default position specified at design time.
                if ((screenRect.Y > 0) && (screenRect.X > 0))
                {
                    // Move it to the indicated coordinates.
                    newWindow.Top = screenRect.Y;
                    newWindow.Left = screenRect.X;
                }
            }
            else if (e.IsWindowOpen || e.IsPost)
            {
                // No specs or only non-standard specs were specified, but the event is still 
                // the result of 'window.open' or of an HTML form with tagret="_blank" and method="post".
                // We will open a normal window but we will still wrap the new native child view, 
                // maintaining its relationship with the parent window.
                newWindow.NativeView = e.NewViewInstance;
                // Show the window.
                newWindow.Show();
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
                newWindow.Source = e.TargetURL;
                // Show the window.
                newWindow.Show();
            }
        }
    }
}
