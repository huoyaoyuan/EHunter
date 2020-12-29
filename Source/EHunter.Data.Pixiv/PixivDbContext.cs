using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EHunter.Data.Pixiv
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

    public class PendingDownload
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArtworkId { get; set; }
        public DateTimeOffset Time { get; set; }
        [ConcurrencyCheck]
        public int PId { get; set; }
    }
}
