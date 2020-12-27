using Microsoft.EntityFrameworkCore;

namespace EHunter.Data.Pixiv
{
    public class PixivDbContext : EHunterDbContext
    {
        public PixivDbContext(DbContextOptions<PixivDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildModelCore(modelBuilder);
        }
    }
}
