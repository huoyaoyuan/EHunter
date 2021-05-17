using Meowtrix.PixivApi.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    public sealed partial class AgeRestrictionPresenter : UserControl
    {
        public AgeRestrictionPresenter() => InitializeComponent();

        private AgeRestriction _age;
        public AgeRestriction Age
        {
            get => _age;
            set
            {
                _age = value;
                VisualStateManager.GoToState(this, value.ToString(), false);
            }
        }
    }
}
