using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.ComponentModel;
using EHunter.EHentai.Api.Models;

namespace EHunter.EHentai.ViewModels.Galleries
{
    public class GalleryDetailVM : ObservableObject
    {
        public GalleryDetailVM(EHentaiVMFactory factory, Gallery gallery)
        {
            async IAsyncEnumerable<ImagePage> GetPagesAsync(
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                int p = 0;
                int totalGalleryPages;
                do
                {
                    var page = await gallery.GetPageAtAsync(p, cancellationToken).ConfigureAwait(true);
                    totalGalleryPages = page.TotalPagesGet;
                    p++;

                    foreach (var image in page.Images)
                        yield return image;
                }
                while (p < totalGalleryPages);
            }

            Images = factory.CreateAsyncCollection(GetPagesAsync());
        }

        public IBindableCollection<ImagePagePreviewVM> Images { get; }
    }
}
