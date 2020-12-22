using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EHunter.Data
{
    internal class EHunterDbContextFactory : IDesignTimeDbContextFactory<EHunterDbContext>
    {
        public EHunterDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EHunterDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true",
                sql => sql.UseHierarchyId());
            return new EHunterDbContext(optionsBuilder.Options);
        }
    }
}
