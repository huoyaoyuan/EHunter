using System;
using System.Composition;
using System.Threading.Tasks;
using EHunter.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

#pragma warning disable EF1001

namespace EHunter.Data
{
    [Export(typeof(EHunterDbContextResolver<>)), Export(typeof(IDbContextFactoryResolver<>)), Shared]
    public sealed class EHunterDbContextResolver<TContext> :
        IDbContextFactoryResolver<TContext>,
        IDisposable
        where TContext : DbContext
    {
        private DbContextPool<TContext>? _pool;
        private PooledDbContextFactory<TContext>? _factory;
        private readonly IDisposable _databaseSettingDisposable;
        private readonly object _memberLock = new();

        [ImportingConstructor]
        public EHunterDbContextResolver(IDatabaseSetting setting)
        {
            _databaseSettingDisposable = setting.ConnectionString.Subscribe(
                cs => InitializeTask = CreateFactoryAsync(cs));
        }

        private Task CreateFactoryAsync(string connectionString) => Task.Run(async () =>
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return;

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
                return;
            }

            lock (_memberLock)
            {
                _pool = pool;
                _factory = factory;
            }
        });

        public Task InitializeTask { get; private set; } = null!;

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
}
