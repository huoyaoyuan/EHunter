﻿using EHunter.Provider.Pixiv.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Provider.Pixiv.Views.User
{
    public sealed partial class UserTab : UserControl
    {
        public UserTab() => InitializeComponent();

        public static readonly DependencyProperty VMProperty
            = DependencyProperty.Register(nameof(VM), typeof(UserVM), typeof(UserTab),
                new PropertyMetadata(null));
        public UserVM VM
        {
            get => (UserVM)GetValue(VMProperty);
            set => SetValue(VMProperty, value);
        }
    }
}
