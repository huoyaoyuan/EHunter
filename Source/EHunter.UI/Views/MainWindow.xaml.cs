using System;
using EHunter.UI.Views.EHentai;
using EHunter.UI.Views.Pixiv;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.UI.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly ProviderMenuItem[] _providers =
        {
            new ProviderMenuItem("Pixiv", "https://www.pixiv.net/favicon.ico", typeof(PixivRootView)),
            new ProviderMenuItem("EHentai", "https://exhentai.org/favicon.ico", typeof(EHentaiRootView)),
        };

        public MainWindow() => InitializeComponent();

        private SettingsView? _settingsView;

        private void NavigationView_SelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            sender.Content = args.IsSettingsSelected
                ? (_settingsView ??= new SettingsView())
                : ((ProviderMenuItem)args.SelectedItem).Content;
        }
    }

    public class ProviderMenuItem
    {
#pragma warning disable CA1054
        public ProviderMenuItem(string name, string iconUri, Type contentType)
#pragma warning restore CA1054
        {
            Name = name;
            IconUri = new Uri(iconUri);
            _contentType = contentType;
        }

        public string Name { get; }

        public Uri IconUri { get; }

        private readonly Type _contentType;
        private FrameworkElement? _content;
        public FrameworkElement Content => _content ??= (FrameworkElement)Activator.CreateInstance(_contentType)!;
    }
}
