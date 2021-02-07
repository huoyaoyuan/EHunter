using EHunter.DependencyInjection;

namespace EHunter.Pixiv.Downloader.Manual
{
    internal class WrappedResolver<T> : ICustomResolver<T>
            where T : class
    {
        private readonly T _service;

        public WrappedResolver(T service) => _service = service;

        public T Resolve() => _service;
    }
}
