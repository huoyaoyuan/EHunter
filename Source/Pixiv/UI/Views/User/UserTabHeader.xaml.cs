﻿using EHunter.Pixiv.ViewModels.User;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.User
{
    [DependencyProperty("ViewModel", typeof(JumpToUserVM), IsNullable = true)]
    public sealed partial class UserTabHeader : UserControl
    {
        public UserTabHeader() => InitializeComponent();
    }
}
