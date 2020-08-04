using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.UI.ViewModels
{
    internal class TabRootVM : ObservableObject
    {
        private string _header = "New Tab";
        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }
    }
}
