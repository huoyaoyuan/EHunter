using System;
using System.Windows.Input;
using EHunter.Provider.Pixiv.Messages;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.Messaging;

#nullable enable

namespace EHunter.Provider.Pixiv.Commands
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
