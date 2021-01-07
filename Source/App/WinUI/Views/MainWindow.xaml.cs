using System;
using System.Collections.Generic;
using EHunter.Pixiv.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.UI.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly List<ProviderMenuItem> _providers = new()
        {
            new ProviderMenuItem("Pixiv", "ms-appx:///EHunter.Pixiv.UI/Assets/pixiv.png", typeof(PixivSwitchPage)),
        };

        public MainWindow()
        {
            Title = "EHunter";
            InitializeComponent();
        }

        private int _previousSelectedIndex = -1;

        private void NavigationView_SelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            var targetType = args.IsSettingsSelected
                ? typeof(SettingsPage)
                : ((ProviderMenuItem)args.SelectedItem).ContentType;

            int newIndex = args.IsSettingsSelected
                ? int.MaxValue
                : _providers.IndexOf((ProviderMenuItem)args.SelectedItem);

            var direction = newIndex > _previousSelectedIndex
                ? SlideNavigationTransitionEffect.FromRight
                : SlideNavigationTransitionEffect.FromLeft;

            _ = _frame.Navigate(targetType,
                null,
                new SlideNavigationTransitionInfo { Effect = direction });

            _previousSelectedIndex = newIndex;
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
            ContentType = contentType;
        }

        public string Name { get; }

        public Uri IconUri { get; }

        public Type ContentType { get; }
    }
}
