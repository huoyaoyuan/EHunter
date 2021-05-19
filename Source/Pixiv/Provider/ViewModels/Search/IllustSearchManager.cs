using System.Composition;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Search
{
    [Export, Shared]
    public partial class IllustSearchManager : TabsViewModel<IllustSearchVM>
    {
        internal readonly ICustomResolver<PixivClient> ClientResolver;
        internal readonly IllustVMFactory IllustVMFactory;

        [ImportingConstructor]
        public IllustSearchManager(ICustomResolver<PixivClient> clientResolver,
            IllustVMFactory illustVMFactory)
        {
            ClientResolver = clientResolver;
            IllustVMFactory = illustVMFactory;
        }

        protected override IllustSearchVM CreateNewTab() => new(this);

        public void GoToTag(Tag tag)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].Tag?.Name == tag.Name)
                {
                    SelectedIndex = i;
                    return;
                }
            }

            Tabs.Add(new(this, tag));
            SelectedIndex = Tabs.Count - 1;
        }
    }
}
