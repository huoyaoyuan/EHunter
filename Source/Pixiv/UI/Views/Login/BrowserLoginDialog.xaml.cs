using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Login
{
    public sealed partial class BrowserLoginDialog : ContentDialog
    {
        public BrowserLoginDialog() => InitializeComponent();

        public Uri? NavigateUrl { get; set; }

        public Uri? ResultUrl { get; set; }

        private async void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            try
            {
                await webView.EnsureCoreWebView2Async();
            }
            catch
            {
                // WebView2 not available
                Hide();
                return;
            }

            webView.Source = NavigateUrl;
        }

        private void WebView_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            var uri = new Uri(args.Uri);

            if (uri.Scheme == "pixiv")
            {
                ResultUrl = uri;
                args.Cancel = true;
                Hide();
            }
        }
    }
}
