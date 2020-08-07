using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.UI.ViewModels
{
    public sealed class TabRootVM : ObservableObject
    {
        private string _header = "New Tab";
        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }
    }
}
