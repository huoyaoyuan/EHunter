using System;
using EHunter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EHunter.Pixiv.Data
{
    public class PixivDbContext : DbContext
    {
        public DbSet<PendingDownload> PixivPendingDownloads => Set<PendingDownload>();

        public PixivDbContext(DbContextOptions<PixivDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public IDbContextTransaction UseTransactionWith(EHunterDbContext eHunterContext)
        {
            if (Database.GetConnectionString() != eHunterContext.Database.GetConnectionString())
            {
                throw new InvalidOperationException("Cross dbcontext transaction must be used with save database.");
            }

            Database.SetDbConnection(eHunterContext.Database.GetDbConnection());
            var transaction = eHunterContext.Database.BeginTransaction();
            Database.UseTransaction(transaction.GetDbTransaction());
            return transaction;
        }
    }
}
