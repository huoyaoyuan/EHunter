using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

[assembly: InternalsVisibleTo("EHunter.Data.Test")]

namespace EHunter.Data
{
    internal class EHunterDbContextFactory : IDesignTimeDbContextFactory<EHunterDbContext>
    {
        public EHunterDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EHunterDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true");
            return new EHunterDbContext(optionsBuilder.Options);
        }
    }
}
