using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace EHunter.Pixiv.Commands
{
    public class CopyUrlCommand : ICommand
    {
        public bool CanExecute(object? parameter) => parameter is Uri;
        public void Execute(object? parameter)
        {
            Uri uri = parameter as Uri ?? throw new ArgumentNullException(nameof(parameter));
            var dataPackage = new DataPackage();
            dataPackage.SetText(uri.ToString());
            Clipboard.SetContent(dataPackage);
        }

        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
