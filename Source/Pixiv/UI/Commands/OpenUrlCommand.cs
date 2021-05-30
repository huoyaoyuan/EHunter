using System;
using System.Windows.Input;
using Windows.System;

namespace EHunter.Pixiv.Commands
{
    public class OpenUrlCommand : ICommand
    {
        public bool CanExecute(object? parameter) => parameter is Uri;
        public void Execute(object? parameter)
        {
            Uri uri = parameter as Uri ?? throw new ArgumentNullException(nameof(parameter));
            _ = Launcher.LaunchUriAsync(uri);
        }

        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
