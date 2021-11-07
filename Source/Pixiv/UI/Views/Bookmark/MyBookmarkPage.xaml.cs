using EHunter.Controls;
using EHunter.Pixiv.ViewModels.Bookmark;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Bookmark
{
    public abstract class MyBookmarkPageBase : PageFor<MyBookmarkVM>
    {
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyBookmarkPage : MyBookmarkPageBase
    {
        public MyBookmarkPage() => InitializeComponent();
    }
}
