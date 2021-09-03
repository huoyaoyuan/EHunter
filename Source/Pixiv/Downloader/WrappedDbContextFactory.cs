using EHunter.Data;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Pixiv.Downloader.Manual
{
#pragma warning disable CA2252 // TODO: https://github.com/dotnet/roslyn-analyzers/issues/5366
    internal class WrappedDbContextFactory<TContext> : IDbContextFactoryResolver<TContext>
#pragma warning restore CA2252
        where TContext : DbContext
    {
        private readonly IDbContextFactory<TContext>? _service;

        public WrappedDbContextFactory(IDbContextFactory<TContext>? service) => _service = service;

        public IDbContextFactory<TContext>? Resolve() => _service;
    }
}
