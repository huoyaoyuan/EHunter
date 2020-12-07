using Microsoft.EntityFrameworkCore;

namespace EHunter.Data
{
    public class EHunterDbContext : DbContext
    {
        public EHunterDbContext(DbContextOptions<EHunterDbContext> options)
            : base(options)
        {
        }
    }
}
