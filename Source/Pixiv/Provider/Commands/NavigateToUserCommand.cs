using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.Pixiv.Messages;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Commands
{
    public sealed class NavigateToUserCommand : ICommand
    {
        public bool CanExecute(object? parameter) => parameter is UserInfo;
        public void Execute(object? parameter)
            => WeakReferenceMessenger.Default.Send(new NavigateToUserMessage((UserInfo?)parameter
                ?? throw new ArgumentNullException(nameof(parameter))));

        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
