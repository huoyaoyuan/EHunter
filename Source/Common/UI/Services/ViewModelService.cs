using System.Diagnostics.CodeAnalysis;
using EHunter.ComponentModel;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using Windows.Win32;
using WinRT.Interop;

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
                IntPtr hwnd = PInvoke.GetActiveWindow();
                InitializeWithWindow.Initialize(picker, hwnd);
            }

            var folder = await picker.PickSingleFolderAsync();
            return folder?.Path;
        }
    }
}
