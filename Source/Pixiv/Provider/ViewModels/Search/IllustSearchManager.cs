using System.Composition;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Messages;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Search
{
    [Export, Shared]
    public sealed partial class IllustSearchManager : TabsViewModel<IllustSearchVM>, IRecipient<NavigateToTagMessage>
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

                // TODO: activate this in view
                vm.OnActivated();
            }

            SelectedItem = vm;
        }

        void IRecipient<NavigateToTagMessage>.Receive(NavigateToTagMessage message) => GoToTag(message.Tag);
    }
}
