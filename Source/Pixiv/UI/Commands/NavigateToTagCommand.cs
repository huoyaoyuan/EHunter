using System;
using System.Windows.Input;
using EHunter.Pixiv.Messages;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.Messaging;

#nullable enable

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
