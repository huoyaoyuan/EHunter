using EHunter.Controls;

namespace EHunter.Pixiv.Views
{
    internal class PixivPageLocator : ReflectionPageLocator
    {
        public PixivPageLocator() : base(typeof(PixivPageLocator).Assembly)
        {
        }
    }
}
