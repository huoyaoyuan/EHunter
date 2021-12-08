using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace EHunter.Pixiv.Themes
{
    internal partial class IllustCollectionViewTheme
    {
        public IllustCollectionViewTheme() => InitializeComponent();

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: workaround for https://github.com/microsoft/microsoft-ui-xaml/issues/6350
            ((ButtonBase)sender).ReleasePointerCaptures();
        }
    }
}
