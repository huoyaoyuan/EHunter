using System;
using System.Threading.Tasks;
using EHunter.DependencyInjection;
using EHunter.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable EF1001

namespace EHunter.Data
{
    public sealed class EHunterDbContextResolver<TContext> :
        ICustomResolver<IDbContextFactory<TContext>?>,
        IDisposable
        where TContext : DbContext
    {
        private DbContextPool<TContext>? _pool;
        private PooledDbContextFactory<TContext>? _factory;
        private readonly IDisposable _databaseSettingDisposable;
        private readonly object _memberLock = new();

        public EHunterDbContextResolver(IDatabaseSetting setting)
        {
            InitializeTask = CreateFactoryAsync(setting.ConnectionString);
            _databaseSettingDisposable = setting.ConnectionStringChanged.Subscribe(
                cs => _ = CreateFactoryAsync(cs));
        }

        private async Task CreateFactoryAsync(string connectionString)
        {
            await Task.Yield();

            var options = new DbContextOptionsBuilder<TContext>()
                .UseSqlServer(connectionString)
                .Options;
            var pool = new DbContextPool<TContext>(options);
            var factory = new PooledDbContextFactory<TContext>(pool);

            try
            {
                using var context = factory.CreateDbContext();
                await context.Database.MigrateAsync().ConfigureAwait(false);
            }
            catch
            {
                await pool.DisposeAsync().ConfigureAwait(false);
            }

            lock (_memberLock)
            {
                _pool = pool;
                _factory = factory;
            }
        }

        public Task InitializeTask { get; }

        public IDbContextFactory<TContext>? Resolve() => _factory;

        public void Dispose()
        {
            lock (_memberLock)
            {
                _databaseSettingDisposable.Dispose();
                _pool?.Dispose();
            }
        }
    }

    public static class DataDependencyInjectionExtensions
    {
        public static IServiceCollection AddEHunterDbContext<TContext>(this IServiceCollection serviceCollection)
            where TContext : DbContext
            => serviceCollection
            .AddSingleton<EHunterDbContextResolver<TContext>>()
            .AddConversion<ICustomResolver<IDbContextFactory<TContext>?>, EHunterDbContextResolver<TContext>>();
    }
}
