using EHunter.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EHunter.Data
{
    public sealed class EHunterDbContextResolver<TContext> :
        IDbContextFactoryResolver<TContext>,
        IDisposable
        where TContext : DbContext
    {
        private PooledDbContextFactory<TContext>? _factory;
        private readonly IDisposable _databaseSettingDisposable;

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
            var factory = new PooledDbContextFactory<TContext>(options);

            try
            {
                using var context = factory.CreateDbContext();
                await context.Database.MigrateAsync().ConfigureAwait(false);
            }
            catch
            {
                return;
            }

            _factory = factory;
        });

        public Task InitializeTask { get; private set; } = null!;

        public IDbContextFactory<TContext>? Resolve() => _factory;

        public void Dispose() => _databaseSettingDisposable.Dispose();
    }
}
