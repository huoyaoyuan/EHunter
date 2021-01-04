using System;
using System.Runtime.InteropServices;
using EHunter.UI.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.UI.Views
{
    public sealed partial class SettingsPage : Page
    {
        private readonly CommonSettingVM _setting = Ioc.Default.GetRequiredService<CommonSettingVM>();

        public SettingsPage() => InitializeComponent();

        private async void BrowseStorageRoot(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                FileTypeFilter =
                {
                    "*"
                }
            };

            // TODO: workaround from https://github.com/microsoft/microsoft-ui-xaml/issues/2716#issuecomment-727043010
            if (Window.Current == null)
            {
                IInitializeWithWindow initializeWithWindowWrapper = picker.As<IInitializeWithWindow>();
                IntPtr hwnd = GetActiveWindow();
                initializeWithWindowWrapper.Initialize(hwnd);
            }

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
                _setting.StorageRoot = folder.Path;
        }

        [ComImport, Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize([In] IntPtr hwnd);
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
#pragma warning disable CA5392
        private static extern IntPtr GetActiveWindow();
#pragma warning restore CA5392
    }
}
