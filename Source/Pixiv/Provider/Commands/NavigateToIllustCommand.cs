using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.Pixiv.Messages;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Commands
{
    public class NavigateToIllustCommand : ICommand
    {
        public bool CanExecute(object? parameter) => parameter is Illust;
        public void Execute(object? parameter)
            => WeakReferenceMessenger.Default.Send(new NavigateToIllustMessage((Illust?)parameter
                ?? throw new ArgumentNullException(nameof(parameter))));

        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
