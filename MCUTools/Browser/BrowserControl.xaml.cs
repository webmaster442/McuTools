using Awesomium.Core;
using McuTools.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Awesomium.Windows.Controls;
using Awesomium.Core.Data;

namespace McuTools.Browser
{
    /// <summary>
    /// Interaction logic for BrowserControl.xaml
    /// </summary>
    public partial class BrowserControl : UserControl, IDisposable, IResourceInterceptor
    {
        private bool _loaded;
        private string _cache;
        private string[] _whitelist;

        public BrowserControl()
        {
            _loaded = false;
            InitializeComponent();

            if (WebCore.IsInitialized) WebCore.ResourceInterceptor = this;
            else
            {
                // We could simply write this like that:
                //WebCore.Started += ( s, e ) => WebCore.ResourceInterceptor = this;
                // See below why we don't do it.

                CoreStartEventHandler handler = null;
                handler = (s, e) =>
                {
                    // Though this example shuts down the core when this
                    // MainWindow closes, in a normal application scenario,
                    // there could be many instances of MainWindow. We don't
                    // want them all referenced by the WebCore singleton
                    // and kept from garbage collection, so the handler
                    // has to be removed.
                    WebCore.Started -= handler;
                    WebCore.ResourceInterceptor = this;
                };
                WebCore.Started += handler;
            }

            // Assign our global ShowCreatedWebView handler.
            webControl.ShowCreatedWebView += StacicBrowser.OnShowNewView;
            _whitelist = new string[] { "asset", "youtube.com", "code.google.com", "webmaster442.hu" };
        }


        ResourceResponse IResourceInterceptor.OnRequest(ResourceRequest request)
        {
            // Resume normal processing.
            return null;
        }

        bool IResourceInterceptor.OnFilterNavigation(NavigationRequest request)
        {
            // ResourceInterceptor is global. It applies to all views
            // maintained by the WebCore. We are only interested in 
            // requests targeting our WebControl.
            // CAUTION: IResourceInterceptor members are called on the I/O thread.
            // Cast to IWebView to perform thread-safe access to the Identifier property!
            if (request.ViewId != ((IWebView)webControl).Identifier) return false;

            var s = request.Url.ToString();
            if (string.IsNullOrEmpty(s)) return true;

            foreach (var filter in _whitelist)
            {
                if (s.Contains(filter)) return false;
            }

            Dispatcher.Invoke(delegate
            {
                (App.Current.MainWindow as MainWindow).OpenUrl(s);
            });
            return true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            try
            {
                webControl.WebSession.AddDataSource("mcutools", new DataPakSource("htmlassets.pak"));
            }
            catch (Exception) { }
            if (_cache != null)
            {
                webControl.Source = new Uri(_cache);
                _cache = null;
            }
            _loaded = true;
        }

        private void webControl_WindowClose(object sender, WindowCloseEventArgs e)
        {
            //if (!e.IsCalledFromFrame)
            //this.Close();
        }

        public void NavigateTo(string url)
        {
            if (!_loaded) _cache = url;
            else
            {
                Uri uri = new Uri(url);
                webControl.Source = uri;
            }
        }

        protected virtual void Dispose(bool native)
        {
            WebCore.ResourceInterceptor = null;
            // Destroy the WebControl and its underlying view.
            webControl.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void webControl_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            (App.Current.MainWindow as MainWindow).SetTitle(e.Title);
        }
    }
}
