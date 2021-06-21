using System.Composition;
using System.Linq;
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
        internal readonly PixivVMFactory Factory;

        [ImportingConstructor]
        public IllustSearchManager(ICustomResolver<PixivClient> clientResolver,
            PixivVMFactory factory)
        {
            ClientResolver = clientResolver;
            Factory = factory;
        }

        protected override IllustSearchVM CreateNewTab() => new(this);

        public void GoToTag(Tag tag)
        {
            var vm = Tabs.FirstOrDefault(x => x.Tag?.Name == tag.Name);
            if (vm is null)
            {
                Tabs.Add(vm = new(this, tag));
            }

            SelectedItem = vm;
        }
    }
}
