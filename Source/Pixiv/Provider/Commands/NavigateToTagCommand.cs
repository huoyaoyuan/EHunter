using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.Pixiv.Messages;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Commands
{
    public sealed class NavigateToTagCommand : ICommand
    {
        public bool CanExecute(object? parameter) => parameter is Tag;
        public void Execute(object? parameter)
            => WeakReferenceMessenger.Default.Send(new NavigateToTagMessage((Tag?)parameter
                ?? throw new ArgumentNullException(nameof(parameter))));

        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
