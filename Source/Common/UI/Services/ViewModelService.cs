using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EHunter.ComponentModel;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using WinRT;

namespace EHunter.Services
{
    public class ViewModelService : IViewModelService
    {
        [return: NotNullIfNotNull("asyncEnumerable")]
        public IBindableCollection<T>? CreateAsyncCollection<T>(IAsyncEnumerable<T>? asyncEnumerable)
            => asyncEnumerable is null ? null : new AsyncEnumerableCollection<T>(asyncEnumerable);

        public async Task<string?> BrowseFolderAsync()
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
            return folder?.Path;
        }

        [ComImport, Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize([In] IntPtr hwnd);
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr GetActiveWindow();
    }
}
