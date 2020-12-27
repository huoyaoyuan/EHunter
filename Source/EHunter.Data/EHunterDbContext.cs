using Microsoft.EntityFrameworkCore;

namespace EHunter.Data
{
    public class EHunterDbContext : DbContext
    {
        public EHunterDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => BuildModelCore(modelBuilder, false);

        protected static void BuildModelCore(ModelBuilder modelBuilder, bool inherited = true)
        {
        }
    }
}
