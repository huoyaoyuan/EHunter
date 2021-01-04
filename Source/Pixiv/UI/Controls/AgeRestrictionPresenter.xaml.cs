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

        // TODO: https://github.com/microsoft/microsoft-ui-xaml/issues/3339
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

        public int IntAge
        {
            get => (int)Age;
            set => Age = (AgeRestriction)value;
        }
    }
}
