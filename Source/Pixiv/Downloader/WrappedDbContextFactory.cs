using EHunter.Data;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Pixiv.Downloader.Manual
{
    internal class WrappedDbContextFactory<TContext> : IDbContextFactoryResolver<TContext>
        where TContext : DbContext
    {
        private readonly IDbContextFactory<TContext>? _service;

        public WrappedDbContextFactory(IDbContextFactory<TContext>? service) => _service = service;

        public IDbContextFactory<TContext>? Resolve() => _service;
    }
}
