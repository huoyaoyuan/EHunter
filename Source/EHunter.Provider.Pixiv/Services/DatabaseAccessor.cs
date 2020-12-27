using System;
using System.Threading.Tasks;
using EHunter.Data;
using EHunter.Data.Pixiv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#nullable enable

namespace EHunter.Provider.Pixiv.Services
{
    public class DatabaseAccessor
    {
        public DatabaseAccessor(
            IDbContextFactory<EHunterDbContext>? eHunterFactory = null,
            IDbContextFactory<PixivDbContext>? pixivFactory = null,
            ILogger<DatabaseAccessor>? logger = null)
        {
            FactoryTask = Task.Run(async () =>
            {
                if (eHunterFactory is null || pixivFactory is null)
                {
                    logger?.LogInformation("No db context configured.");
                    return null;
                }

                try
                {
                    using var eHunterContext = eHunterFactory.CreateDbContext();
                    using var pixivContext = pixivFactory.CreateDbContext();

                    logger?.LogTrace($"Connecting to database with connection string: {pixivContext.Database.GetConnectionString()}");

                    await eHunterContext.Database.MigrateAsync().ConfigureAwait(false);
                    await pixivContext.Database.MigrateAsync().ConfigureAwait(false);

                    logger?.LogInformation($"Database migrated succesfully.");
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, $"Database migration failed. Stopping access to database.");
                    return null;
                }

                return pixivFactory;
            });
        }

        public Task<IDbContextFactory<PixivDbContext>?> FactoryTask { get; }
    }
}
