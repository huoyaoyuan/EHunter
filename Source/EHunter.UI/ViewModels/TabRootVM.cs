using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.UI.ViewModels
{
    public sealed class TabRootVM : ObservableObject, IEquatable<TabRootVM>
    {
        private string _header = "New Tab";
        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        #region IEqutable Support
        public bool Equals(TabRootVM? other) => ReferenceEquals(this, other);
        public override bool Equals(object? obj) => Equals(obj as TabRootVM);
        public override int GetHashCode() => base.GetHashCode();
        #endregion
    }
}
