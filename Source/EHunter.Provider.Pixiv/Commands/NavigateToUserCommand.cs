using System;
using System.Windows.Input;
using EHunter.Provider.Pixiv.Messages;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.Messaging;

#nullable enable

namespace EHunter.Provider.Pixiv.Commands
{
    public sealed class NavigateToUserCommand : ICommand
    {
        public bool CanExecute(object? parameter) => parameter is UserInfo;
        public void Execute(object? parameter)
            => Messenger.Default.Send(new NavigateToUserMessage((UserInfo?)parameter
                ?? throw new ArgumentNullException(nameof(parameter))));

        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
